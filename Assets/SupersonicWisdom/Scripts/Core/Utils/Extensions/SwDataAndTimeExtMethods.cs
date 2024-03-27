using System;
using System.Globalization;

namespace SupersonicWisdomSDK
{
    internal static class SwDataAndTimeExtMethods
    {
        /// <summary>
        ///     <para>Use this method instead of `dateTime.ToString()` to avoid exceptions that occur in Unity 2020 and above.</para>
        /// </summary>
        internal static string SwToString(this DateTime self)
        {
            // Formatting a date with CultureInfo.InvariantCulture as second parameter prevents exceptions in case the users' default culture is Arabic or Thai
            return self.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     <para>Use this method instead of `dateTime.ToString(...)` to avoid exceptions that occur in Unity 2020 and above.</para>
        /// </summary>
        internal static string SwToString(this DateTime self, string format)
        {
            // Formatting a date with CultureInfo.InvariantCulture as second parameter prevents exceptions in case the users' default culture is Arabic or Thai
            return self.ToString(format, CultureInfo.InvariantCulture);
        }

        private static long SwTimestampMilliseconds(this DateTime self)
        {
            var epochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long) (self - epochTime).TotalMilliseconds;
        }
        
        internal static long SwTicksMilliseconds(this DateTime self)
        {
            return self.Ticks / TimeSpan.TicksPerMillisecond;
        }

        internal static long SwTimestampSeconds(this DateTime self)
        {
            return self.SwTimestampMilliseconds() / 1000;
        }
    }
}