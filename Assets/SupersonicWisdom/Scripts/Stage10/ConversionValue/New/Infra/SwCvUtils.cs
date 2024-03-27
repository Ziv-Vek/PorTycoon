#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal static class SwCvUtils
    {
        #region --- Public Methods ---

        public static int GetCv(SwCvConfig config, EPostbackWindow window, double revenue)
        {
            var cv = -1;
            var scheme = GetSchemeByWindow(config, window)?.cv;

            if (scheme != null)
            {
                cv = GetSortedArrayInsertionValue(scheme, (float)revenue);
            }

            return cv;
        }

        public static ECVCoarseValue GetCoarse(SwCvConfig config, EPostbackWindow window, double revenue)
        {
            var coarse = ECVCoarseValue.Low;
            var scheme = GetSchemeByWindow(config, window)?.coarse;

            if (scheme != null)
            {
                coarse = (ECVCoarseValue)GetSortedArrayInsertionValue(scheme, (float)revenue);
            }

            return coarse;
        }

        public static bool ShouldLock(SwCvConfig config, EPostbackWindow window, ECVCoarseValue newCoarse)
        {
            if (config == null)
            {
                return false;
            }

            int? lockFromCoarse = null;
            
            switch (window)
            {
                case EPostbackWindow.Zero:
                    lockFromCoarse = config.postbackZeroScheme?.lockWindow?.lockFromCoarse;
                    break;
                
                case EPostbackWindow.One:
                    lockFromCoarse = config.postbackOneScheme?.lockWindow?.lockFromCoarse;
                    break;
                
                case EPostbackWindow.Two:
                    lockFromCoarse = config.postbackTwoScheme?.lockWindow?.lockFromCoarse;
                    break;
            }
            
            return lockFromCoarse > BaseCvUpdater.LOCK_FROM_COARSE_DEFAULT_VALUE && lockFromCoarse <= (int)newCoarse;
        }

        public static Dictionary<EPostbackWindow, float> ToLockFromTimeDictionary(this SwCvConfig config)
        {
            var lockFromHoursDict = new Dictionary<EPostbackWindow, float>();

            var lockFromTimeZero = config?.postbackZeroScheme?.lockWindow?.lockFromTime;
            var lockFromTimeOne = config?.postbackOneScheme?.lockWindow?.lockFromTime;
            var lockFromTimeTwo = config?.postbackTwoScheme?.lockWindow?.lockFromTime;

            if (lockFromTimeZero > BaseCvUpdater.LOCK_FROM_TIME_DEFAULT_VALUE)
            {
                lockFromHoursDict[EPostbackWindow.Zero] = lockFromTimeZero.Value;
            }

            if (lockFromTimeOne > BaseCvUpdater.LOCK_FROM_TIME_DEFAULT_VALUE)
            {
                lockFromHoursDict[EPostbackWindow.One] = lockFromTimeOne.Value;
            }

            if (lockFromTimeTwo > BaseCvUpdater.LOCK_FROM_TIME_DEFAULT_VALUE)
            {
                lockFromHoursDict[EPostbackWindow.Two] = lockFromTimeTwo.Value;
            }

            return lockFromHoursDict;
        }
        
        #endregion


        #region --- Private Methods ---

        private static SwPostbackScheme GetSchemeByWindow(SwCvConfig config, EPostbackWindow window)
        {
            if (config == null)
            {
                return null;
            }

            switch (window)
            {
                case EPostbackWindow.Zero:
                    return config.postbackZeroScheme;
                case EPostbackWindow.One:
                    return config.postbackOneScheme;
                case EPostbackWindow.Two:
                    return config.postbackTwoScheme;
            }

            return null;
        }

        private static int GetSortedArrayInsertionValue<T>(T[] sortedArray, T value)
        {
            var index = Array.BinarySearch(sortedArray, value);

            if (index < 0)
            {
                index = ~index;
            }

            return index;
        }

        #endregion
    }
}
#endif