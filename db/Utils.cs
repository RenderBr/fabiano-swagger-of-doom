#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

#endregion

public static class Utils
{
    public const string BAD_WORDS =
        "(anus|ass|arse|arsehole|ass|asshat|assjabber|asspirate|assbag|assbandit|assbanger|assbite|assclown|asscock|asscracker|asses|assface|assfuck|assfucker|assgoblin|asshat|asshead|asshole|asshopper|assjacker|asslick|asslicker|assmonkey|assmunch|assmuncher|assnigger|asspirate|assshit|assshole)";

    public static class ConsoleCloseEventHandler
    {
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        public delegate bool EventHandler(CtrlType sig);

        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
    }

    public static uint NextUInt32(this Random rand)
    {
        return (uint)(rand.Next(1 << 30)) << 2 | (uint)(rand.Next(1 << 2));
    }

    public static string Sha1(string input)
    {
        using var sha1 = SHA1.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha1.ComputeHash(bytes);
        var sb = new StringBuilder(hashBytes.Length * 2);
        foreach (var b in hashBytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    public static string GenerateRandomString(int size, Random rand=null)
    {
        var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var builder = new StringBuilder();
        var random = rand ?? new Random();
        char ch;
        for (var i = 0; i < size; i++)
        {
            ch = chars[random.Next(0, chars.Length - 1)];
            builder.Append(ch);
        }
        return builder.ToString();
    }

    public static void Swap<T>(ref T a, ref T b)
    {
        var obj = a;
        a = b;
        b = obj;
    }

    public static int ToUnixTimestamp(this DateTime dt)
    {
        return (int)(dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    public static bool IsValidEmail(string email)
    {
        try
        {
            return new MailAddress(email).Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static byte[] GetPixels(this Image<Rgba32> img, byte transparency = 255)
    {
        int width = img.Width;
        int height = img.Height;
        byte[] argb = new byte[width * height * 4];

        int index = 0;
        img.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (int x = 0; x < width; x++)
                {
                    Rgba32 pixel = row[x];
                    // preserve original loop order: A, R, G, B
                    argb[index++] = transparency; // Alpha override
                    argb[index++] = pixel.R;
                    argb[index++] = pixel.G;
                    argb[index++] = pixel.B;
                }
            }
        });

        return argb;
    }

    public static T Convert<T>(this string value)
    {
        return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom((object)value);
    }

    public static T[] FromCSV<T>(this string csv)
    {
        return (csv.Split(',')).Select((value => value.Trim().Convert<T>())).ToArray();
    }

    public static int FromString(string x)
    {
        x = x.Trim();
        if (x.StartsWith("0x")) return int.Parse(x.Substring(2), NumberStyles.HexNumber);
        return int.Parse(x);
    }

    /// <summary>
    ///     Indicates whether a specified string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">value: The string to test.</param>
    /// <returns>
    ///     true if the value parameter is null or System.String.Empty, or if value consists exclusively of white-space
    ///     characters.
    /// </returns>
    public static bool IsNullOrWhiteSpace(this string value)
    {
        if (value == null)
        {
            return true;
        }

        int index = 0;
        while (index < value.Length)
        {
            if (char.IsWhiteSpace(value[index]))
            {
                index++;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public static string To4Hex(short x)
    {
        return "0x" + x.ToString("x4");
    }

    public static string To2Hex(short x)
    {
        return "0x" + x.ToString("x2");
    }

    public static string GetCommaSepString<T>(T[] arr)
    {
        StringBuilder ret = new StringBuilder();
        for (int i = 0; i < arr.Length; i++)
        {
            if (i != 0) ret.Append(", ");
            ret.Append(arr[i]);
        }

        return ret.ToString();
    }

    public static List<int> StringListToIntList(List<string> strList)
    {
        List<int> ret = new List<int>();
        foreach (string i in strList)
        {
            try
            {
                ret.Add(System.Convert.ToInt32(i));
            }
            catch
            {
            }
        }

        return ret;
    }

    public static int[] FromCommaSepString32(string x)
    {
        if (IsNullOrWhiteSpace(x)) return new int[0];
        return x.Split(',').Select(_ => FromString(_.Trim())).ToArray();
    }

    public static short[] FromCommaSepString16(string x)
    {
        if (IsNullOrWhiteSpace(x)) return new short[0];
        return x.Split(',').Select(_ => (short)FromString(_.Trim())).ToArray();
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        if (list is null || list.Count < 2)
            return;

        var n = list.Count;
        // use static RandomNumberGenerator for perf and no dispose cost
        var rng = RandomNumberGenerator.Create();

        while (n > 1)
        {
            n--;
            int k = RandomNumber(n, rng); // uniform [0, n)
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    private static int RandomNumber(int maxExclusive, RandomNumberGenerator rng)
    {
        // generates a uniformly distributed int between 0 and maxExclusive - 1
        Span<byte> bytes = stackalloc byte[4];
        uint scale;
        do
        {
            rng.GetBytes(bytes);
            scale = BitConverter.ToUInt32(bytes);
        } while (scale >= uint.MaxValue - (uint.MaxValue % (uint)maxExclusive));

        return (int)(scale % (uint)maxExclusive);
    }

    public static string ToSafeText(this string str)
    {
        Regex wordFilter = new Regex(BAD_WORDS);
        return wordFilter.Replace(str, "<3");
    }

    public static short[] PackFromEquips(this Char chr)
    {
        List<short> bpItems = FromCommaSepString16(chr._Equipment).ToList();
        bpItems.RemoveRange(0, 4);
        return bpItems.ToArray();
    }

    public static short[] EquipSlots(this Char chr)
    {
        List<short> eqpSlots = FromCommaSepString16(chr._Equipment).ToList();
        //eqpSlots.RemoveRange(4, 8);
        return eqpSlots.ToArray();
    }

    public static T GetEnumByName<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static string GetEnumName<T>(object value)
    {
        return Enum.GetName(typeof(T), value);
    }

    public static byte[] RandomBytes(int len)
    {
        var arr = new byte[len];
        var r = new Random();
        r.NextBytes(arr);
        return arr;
    }

    public static void ExecuteSync(this Task task)
    {
        task.Wait();
    }

    public static T ExecuteSync<T>(this Task<T> task)
    {
        task.Wait();
        return task.Result;
    }
}