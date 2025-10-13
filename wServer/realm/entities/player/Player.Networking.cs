#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using wServer.networking;

#endregion

namespace wServer.realm.entities.player
{
    partial class Player
    {
        private bool _worldBroadcast = true;

        // use thread-safe queue in case packets arrive from async tasks
        private readonly ConcurrentQueue<(Packet packet, Predicate<Player> condition)> _pendingPackets = new();

        /// <summary>
        /// Sends all queued packets to all matching players in the same world.
        /// Called usually once per tick (e.g. during Move or logic update).
        /// </summary>
        internal void Flush()
        {
            if (Owner == null || Owner.Players == null)
                return;

            while (_pendingPackets.TryDequeue(out var item))
            {
                foreach (var player in Owner.Players.Values)
                {
                    try
                    {
                        if (item.condition(player))
                            player.Client.SendPacket(item.packet);
                    }
                    catch
                    {
                        // don’t break broadcast because of one bad connection
                    }
                }
            }
        }

        /// <summary>
        /// Broadcasts a packet to all players (synchronously at Move).
        /// </summary>
        public void BroadcastSync(Packet packet)
            => BroadcastSync(packet, _ => true);

        public void BroadcastSync(Packet packet, Predicate<Player> condition)
        {
            if (_worldBroadcast)
            {
                // we can’t pass a Predicate anymore; just exclude 'this' player
                Owner?.BroadcastPacketSync(packet, this);
            }
            else
            {
                _pendingPackets.Enqueue((packet, condition));
            }
        }


        private void BroadcastSync(IEnumerable<Packet> packets)
        {
            foreach (var p in packets)
                BroadcastSync(p, _ => true);
        }

        private void BroadcastSync(IEnumerable<Packet> packets, Predicate<Player> condition)
        {
            foreach (var p in packets)
                BroadcastSync(p, condition);
        }
    }
}
