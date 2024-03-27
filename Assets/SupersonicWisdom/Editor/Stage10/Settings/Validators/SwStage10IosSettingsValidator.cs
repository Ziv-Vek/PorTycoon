#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Linq;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwStage10IosSettingsValidator : SwStage10SettingsValidator
    {
        #region --- Properties ---

        private IEnumerable<Tuple<MissingParam, string>> ParamsToCheck
        {
            get
            {
                return new List<Tuple<MissingParam, string>>()
                    .Append(new Tuple<MissingParam, string>(MissingParam.IosAppId, SwEditorUtils.SwSettings.IosAppId))
                    .Append(new Tuple<MissingParam, string>(MissingParam.FbAppId, SwEditorUtils.FacebookAppId))
                    .Append(new Tuple<MissingParam, string>(MissingParam.FbClientToken, SwEditorUtils.FacebookClientToken))
                    .Append(new Tuple<MissingParam, string>(MissingParam.GameId, SwEditorUtils.SwSettings.IosGameId));
            }
        }

        #endregion
        
        
        #region --- Private Methods ---

        internal override MissingParam GetMissingParam ()
        {
            var missingParams = base.GetMissingParam();

            if (missingParams != MissingParam.No) return missingParams;

            return SwSettingsValidator.GetMissingParam(ParamsToCheck);
        }
        
        internal override List<MissingParam> GetMissingParams ()
        {
            var missingParams = base.GetMissingParams();
            missingParams.AddRange(GetMissingParams(ParamsToCheck));

            return missingParams;
        }

        #endregion
    }
}

#endif