#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwStage10RevenueData
    {
        #region --- Members ---

        public readonly bool IsTriggeredByDev;
        public readonly SwRevenueType Type;
        
        public string Iso;
        public double Amount;

        #endregion


        #region --- Properties ---

        public virtual string SourceId
        {
            get { return string.Empty; }
        }

        #endregion


        #region --- Construction ---

        public SwStage10RevenueData(double amount, SwRevenueType type, bool isTriggeredByDev, string iso = SwEconomyUtils.USD_CURRENCY_ISO)
        {
            Amount = amount;
            Type = type;
            IsTriggeredByDev = isTriggeredByDev;
            Iso = iso;
        }

        #endregion


        #region --- Public Methods ---

        public virtual Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { SwStage10RevenueCalculator.REVENUE_AMOUNT_EVENT_PARAM, Amount },
                { SwStage10RevenueCalculator.REVENUE_TYPE_EVENT_PARAM, Type },
                { SwStage10RevenueCalculator.IS_DEV_REVENUE_REPORT_EVENT_PARAM, IsTriggeredByDev },
            };
        }
        
        public void ConvertToUsd(Dictionary<string, float> conversionRates)
        {
            Amount = SwEconomyUtils.ConvertToUSD(Amount, Iso, conversionRates);
            Iso = SwEconomyUtils.USD_CURRENCY_ISO;
        }

        #endregion
    }
}
#endif