using System;
using System.Globalization;

namespace SupersonicWisdomSDK
{
    internal class SwDateAndTimeUtils
    {
        #region --- Properties ---

        public long OneDayInSeconds
        {
            get
            {
                return ConvertHoursToSeconds(24);
            }
        }

        public long TwoDaysInSeconds
        {
            get
            {
                return ConvertHoursToSeconds(48);
            }
        }

        #endregion


        #region --- Public Methods ---

        public long GetTotalSeconds(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public long GetTotalMilliSeconds(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public long GetMilliseconds(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        /// <summary>
        ///     Use this method instead of dateTime.ToString() to avoid exceptions that occur in Unity 2020 and above.
        /// </summary>
        /// <param name="dateTime">The date instance you want to convert</param>
        /// <returns></returns>
        public string ToStringDate(DateTime dateTime)
        {
            return dateTime.SwToString();
        }

        /// <summary>
        ///     Use this method instead of dateTime.ToString() to avoid exceptions that occur in Unity 2020 and above.
        /// </summary>
        /// <param name="dateTime">The date instance you want to convert</param>
        /// <param name="format">A date string format you wish to apply</param>
        /// <returns></returns>
        public string ToStringDate(DateTime dateTime, string format)
        {
            return dateTime.SwToString(format);
        }

        #endregion


        #region --- Private Methods ---

        internal DateTime? TryParseDateTimeUtc(string dateString, string format)
        {
            var parsed = TryParseDate(dateString, format);

            return parsed.HasValue ? DateTime.SpecifyKind(parsed.Value, DateTimeKind.Utc) : null;
        }

        internal DateTime? TryParseDate(string dateString, string format)
        {
            if (string.IsNullOrEmpty(dateString)) return null;

            DateTime? parsed = null;

            try
            {
                parsed = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                // Ignore it because it's a legit scenario.
            }

            return parsed;
        }

        internal float ConvertMillisToSeconds(long millis)
        {
            return millis / 1000;
        }

        internal int ConvertHoursToSeconds(int hours)
        {
            return hours * 60 * 60;
        }

        #endregion
    }
}