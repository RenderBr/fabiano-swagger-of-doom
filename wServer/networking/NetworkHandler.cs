#region

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace wServer.networking
{
    internal class NetworkHandler : IDisposable
    {
        private readonly ILogger<NetworkHandler> _logger;
        private const int HEADER_SIZE = 5;
        private const int BUFFER_SIZE = 8192;

        private readonly Client parent;
        private readonly Socket socket;
        private readonly ConcurrentQueue<Packet> sendQueue = new();
        private readonly SemaphoreSlim sendLock = new(1, 1);
        private readonly CancellationTokenSource cts = new();
        public bool _isRunning = true;
        private bool _disposed;

        public NetworkHandler(Client parent, Socket socket)
        {
            _logger = Program.Services?.GetRequiredService<ILogger<NetworkHandler>>();
            this.parent = parent;
            this.socket = socket;
        }

        public void BeginHandling()
        {
            socket.NoDelay = true;
            _ = Task.Run(ReceiveLoop, cts.Token);
            _ = Task.Run(SendLoop, cts.Token);
        }

        public void Stop()
        {
            try
            {
                if (!parent.Reconnecting)
                {
                    _logger?.LogInformation("Stopping network handler for {0}", parent?.Account?.Name);
                    cts.Cancel(); // only cancel for *real* disconnects
                    socket?.Shutdown(SocketShutdown.Both);
                    socket?.Close();
                }
                else
                {
                    _logger?.LogInformation("Skipping socket cancel for reconnecting client {0}",
                        parent?.Account?.Name);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error while stopping network handler");
            }

            _isRunning = false;
            sendQueue.Clear();
        }


        private async Task ReceiveLoop()
        {
            var buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);
            try
            {
                while (_isRunning && !cts.IsCancellationRequested && socket.Connected)
                {
                    // Receive header (4 length bytes + 1 packet id)
                    int received = await ReceiveExactAsync(buffer.AsMemory(0, HEADER_SIZE)).ConfigureAwait(false);
                    if (received < HEADER_SIZE)
                    {
                        _logger?.LogWarning("Client disconnected mid-header.");
                        break;
                    }

                    // Policy or usage checks
                    if (IsPolicyRequest(buffer))
                    {
                        await SendPolicyResponse().ConfigureAwait(false);
                        break;
                    }

                    if (IsUsageRequest(buffer))
                    {
                        await SendUsageResponse().ConfigureAwait(false);
                        break;
                    }

                    var totalLen = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, 0));
                    var packetId = buffer[4];

                    int bodyLen = totalLen - HEADER_SIZE;
                    _logger?.LogDebug("Header => totalLen={bodyLen}, packetId={packetId}, available={available}",
                        bodyLen, packetId, socket.Available);

                    if (bodyLen is < 0 or > BUFFER_SIZE)
                    {
                        _logger?.LogError("Invalid packet length {bodyLen} from {endPoint}",
                            bodyLen, socket.RemoteEndPoint);
                        break;
                    }

                    Packet packet = null;
                    try
                    {
                        packet = Packet.Packets[(PacketID)packetId].CreateInstance();
                    }
                    catch
                    {
                        _logger?.LogError("Unknown packet ID {packetId:X2} from {endPoint}",
                            packetId, socket.RemoteEndPoint);
                        continue;
                    }

                    // Read body
                    received = await ReceiveExactAsync(buffer.AsMemory(HEADER_SIZE - 1, bodyLen)).ConfigureAwait(false);
                    if (received < bodyLen)
                    {
                        _logger?.LogWarning("Client disconnected mid-body.");
                        break;
                    }

                    try
                    {
                        packet.Read(parent, buffer, HEADER_SIZE - 1, bodyLen);

                        if (!Client.ExcludePacketsFromLogging.Contains((PacketID)packetId))
                        {
                            _logger?.LogInformation(
                                "Received {packetId} ({packetType}) [{bodyLen} bytes] from {endPoint}",
                                packetId, packet.GetType().Name, bodyLen, socket.RemoteEndPoint);
                        }

                        if (parent.IsReady())
                            parent.Manager.Network.AddPendingPacket(parent, packet);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error reading packet {packetId:X2} from {endPoint}",
                            packetId, socket.RemoteEndPoint);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Receive loop failed");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private async Task<int> ReceiveExactAsync(Memory<byte> buffer)
        {
            int total = 0;
            try
            {
                while (total < buffer.Length && socket.Connected && !cts.IsCancellationRequested)
                {
                    int read;
                    try
                    {
                        read = await socket.ReceiveAsync(buffer[total..], SocketFlags.None, cts.Token)
                            .ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger?.LogWarning("ReceiveExactAsync cancelled while waiting for {bytesNeeded} bytes",
                            buffer.Length - total);
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        _logger?.LogWarning("Socket disposed during ReceiveExactAsync.");
                        break;
                    }
                    catch (SocketException ex)
                    {
                        _logger?.LogError(ex, "SocketException in ReceiveExactAsync (Code {errorCode})",
                            ex.SocketErrorCode);
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Unexpected exception in ReceiveExactAsync");
                        break;
                    }

                    if (read == 0)
                    {
                        _logger?.LogWarning("Socket closed mid-read (had {total}/{bufferLength} bytes)",
                            total, buffer.Length);
                        break;
                    }

                    total += read;
                    _logger?.LogDebug("ReceiveExactAsync read {read} bytes ({total}/{bufferLength})",
                        read, total, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "ReceiveExactAsync outer exception");
            }

            return total;
        }


        private async Task SendLoop()
        {
            var buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);
            try
            {
                while (!cts.IsCancellationRequested && socket.Connected)
                {
                    if (socket is not { Connected: true })
                    {
                        return;
                    }

                    if (!sendQueue.TryDequeue(out var packet))
                    {
                        await Task.Delay(5, cts.Token).ConfigureAwait(false);
                        continue;
                    }

                    int len = 0;
                    try
                    {
                        len = packet.Write(parent, buffer, 0);
                        await sendLock.WaitAsync(cts.Token).ConfigureAwait(false);
                        await socket.SendAsync(buffer.AsMemory(0, len), SocketFlags.None, cts.Token)
                            .ConfigureAwait(false);
                        sendLock.Release();
                        if (!Client.ExcludePacketsFromLogging.Contains(packet.ID))
                        {
                            _logger?.LogDebug("Sent {packetType} [{len} bytes] to {endPoint}",
                                packet.GetType().Name, len, socket.RemoteEndPoint);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error sending packet ({packetType})",
                            packet?.GetType().Name ?? "null");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Send loop failed");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private bool IsUsageRequest(byte[] header)
        {
            return header[0] == 0x4d && header[1] == 0x61 &&
                   header[2] == 0x64 && header[3] == 0x65 && header[4] == 0xff;
        }

        private async Task SendUsageResponse()
        {
            var msg = $"{parent.Manager.MaxClients}:{parent.Manager.Clients.Count}";
            var data = Encoding.ASCII.GetBytes(msg);
            await socket.SendAsync(data, SocketFlags.None).ConfigureAwait(false);
            _logger?.LogInformation("Sent usage info to {endPoint}", socket.RemoteEndPoint);
        }

        private bool IsPolicyRequest(byte[] header)
        {
            return header[0] == 0x3c && header[1] == 0x70 &&
                   header[2] == 0x6f && header[3] == 0x6c && header[4] == 0x69;
        }

        private async Task SendPolicyResponse()
        {
            const string xml = """
                               <cross-domain-policy>
                                   <allow-access-from domain="*" to-ports="*" />
                               </cross-domain-policy>\r\n
                               """;
            var bytes = Encoding.UTF8.GetBytes(xml);
            await socket.SendAsync(bytes, SocketFlags.None).ConfigureAwait(false);
            _logger?.LogInformation("Sent policy file to {endPoint}", socket.RemoteEndPoint);
        }

        public void SendPacket(Packet packet)
        {
            if (!socket.Connected) return;
            sendQueue.Enqueue(packet);
        }

        public void SendPackets(IEnumerable<Packet> packets)
        {
            if (!socket.Connected) return;
            foreach (var pkt in packets)
                sendQueue.Enqueue(pkt);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _isRunning = false;

            try
            {
                if (!cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }
            }
            catch
            {
            }

            try
            {
                socket?.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }

            try
            {
                socket?.Dispose();
            }
            catch
            {
            }

            sendQueue.Clear();

            try
            {
                cts.Dispose();
            }
            catch
            {
            }

            sendLock.Dispose();
        }
    }
}