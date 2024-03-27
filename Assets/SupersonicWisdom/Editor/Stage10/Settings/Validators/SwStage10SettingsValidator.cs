#if SW_STAGE_STAGE10_OR_ABOVE

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal abstract class SwStage10SettingsValidator : SwSettingsValidator
    {
        #region --- Properties ---

        private bool IsAndroid
        {
            get { return this is SwStage10AndroidSettingsValidator; }
        }
        
        private IEnumerable<Tuple<MissingParam, string>> ParamsToCheck
        {
            get
            {
                var (gaGameKey, gaSecretKey) = SwStage10EditorUtils.GetGameAnalyticsKeys(IsAndroid ? RuntimePlatform.Android : RuntimePlatform.IPhonePlayer);

                return new List<Tuple<MissingParam, string>>().Append(new Tuple<MissingParam, string>(MissingParam.GaGameKey, gaGameKey)).Append(new Tuple<MissingParam, string>(MissingParam.GaSecretKey, gaSecretKey));
            }
        }

        #endregion


        #region --- Private Methods ---

        internal override MissingParam GetMissingParam ()
        {
            return SwSettingsValidator.GetMissingParam(ParamsToCheck);
        }

        internal override List<MissingParam> GetMissingParams ()
        {
            return SwSettingsValidator.GetMissingParams(ParamsToCheck);
        }

        #endregion
    }
}

#endif