using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwAbConfig
    {
        #region --- Inner classes ---

        internal static class Constants
        {
            #region --- Constants ---

            /// The default is eternal
            public const int DEFAULT_DURATION_DAYS = int.MaxValue;

            #endregion
        }

        #endregion
        

        #region --- Members ---

        [JsonProperty("group")]
        public string group;
        [JsonProperty("id")]
        public string id;
        [JsonProperty("key")]
        public string key;
        [JsonProperty("value")]
        public string value;

        /// Date format: yyyy-MM-dd
        [JsonProperty("expirationDate")]
        public string expirationDate;

        [JsonProperty("durationDays", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(Constants.DEFAULT_DURATION_DAYS)]
        public int durationDays;

        [JsonIgnore]
        [NonSerialized]
        private int? _passedDays;

        [JsonIgnore]
        private int PassedDays
        {
            get
            {
                _passedDays ??= CalculatePassedDays();

                return _passedDays.Value;
            }
        }

        private int CalculatePassedDays()
        {
            var today = SwUtils.DateAndTime.TryParseDate(TodayString, SwConstants.SORTABLE_DATE_STRING_FORMAT);

            if (!today.HasValue) return -1;

            var startDayString = SwInfra.KeyValueStore.GetString(StartDateStorageKey(id), null);
            var startDay = SwUtils.DateAndTime.TryParseDate(startDayString, SwConstants.SORTABLE_DATE_STRING_FORMAT);
            var passedDays = -1;

            if (startDay.HasValue)
            {
                passedDays = today.Value.Subtract(startDay.Value).Days + 1;
                SwInfra.Logger.Log(EWisdomLogType.Config, $"passed days of A/B {id}: {passedDays}");
            }

            return passedDays;
        }

        #endregion


        #region --- Properties ---

        [JsonIgnore]
        private readonly Dictionary<string, int?> _cachedCalculation = new Dictionary<string, int?>();

        [JsonIgnore]
        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(id); }
        }

        [JsonIgnore]
        public bool IsExpired
        {
            get { return RemainingDays() <= 0; }
        }

        [JsonIgnore]
        public bool IsEternal
        {
            get { return RemainingDays() == null; }
        }

        #endregion


        #region --- Public Methods ---

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public int? RemainingDays()
        {
            SwInfra.Logger.Log(EWisdomLogType.Config, $"ab: {this}");
            var remainingDays = CalculateRemainingDays();
            SwInfra.Logger.Log(EWisdomLogType.Config, $"remainingDays: {remainingDays?.ToString() ?? "eternal"}");

            if (remainingDays <= 0)
            {
                // If expired, delete this config.
                InvalidateValues();
            }

            SwInfra.Logger.Log(EWisdomLogType.Config, $"ab key = {key} | ab value = {value}");

            return remainingDays;
        }

        private void InvalidateValues()
        {
            value = group = key = id = string.Empty;
        }

        #endregion


        #region --- Private Methods ---

        private static string StartDateStorageKey(string id)
        {
            return "StartDateForAb_" + id;
        }

        private void TryCacheStartDate()
        {
            if (!IsValid) return;

            var startDay = SwInfra.KeyValueStore.GetString(StartDateStorageKey(id), null);

            if (!string.IsNullOrEmpty(startDay)) return;

            // Today's start date only, at midnight.
            startDay = TodayString;
            SwInfra.KeyValueStore.SetString(StartDateStorageKey(id), startDay).Save();
        }

        private static DateTime TodayAtMidnight
        {
            get { return DateTime.ParseExact(TodayString, SwConstants.SORTABLE_DATE_STRING_FORMAT, CultureInfo.InvariantCulture); }
        }

        private static string TodayString
        {
            get { return DateTime.Now.SwToString(SwConstants.SORTABLE_DATE_STRING_FORMAT); }
        }

        private int? CalculateRemainingDays()
        {
            var todayKey = TodayString;
            var alreadyCalculated = _cachedCalculation.ContainsKey(todayKey);

            if (alreadyCalculated) return _cachedCalculation.SwSafelyGet(todayKey, null);

            int? remainingDays = null;

            if (IsValid)
            {
                TryCacheStartDate();
                
                if (durationDays >= 0 && durationDays != Constants.DEFAULT_DURATION_DAYS)
                {
                    remainingDays = durationDays - PassedDays;
                }

                var terminationDate = SwUtils.DateAndTime.TryParseDate(expirationDate, SwConstants.SORTABLE_DATE_STRING_FORMAT);

                if (terminationDate != null)
                {
                    var diffDays = terminationDate.Value.Subtract(TodayAtMidnight).Days;

                    if ((remainingDays ?? Constants.DEFAULT_DURATION_DAYS) > diffDays)
                    {
                        remainingDays = diffDays;
                    }
                }
            }

            _cachedCalculation.Add(todayKey, remainingDays);

            return remainingDays;
        }

        #endregion
    }
}