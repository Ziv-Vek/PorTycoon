#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Linq;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwStage10AndroidSettingsValidator : SwStage10SettingsValidator
    {
        #region --- Properties ---

        private IEnumerable<Tuple<MissingParam, string>> ParamsToCheck
        {
            get
            {
                return new List<Tuple<MissingParam, string>>().Append(new Tuple<MissingParam, string>(MissingParam.GameId, SwEditorUtils.SwSettings.androidGameId)).Append(new Tuple<MissingParam, string>(MissingParam.FbAppId, SwEditorUtils.FacebookAppId)).Append(new Tuple<MissingParam, string>(MissingParam.FbClientToken, SwEditorUtils.FacebookClientToken));
            }
        }

        #endregion


        #region --- Private Methods ---

        internal override MissingParam GetMissingParam()
        {
            var missingParams = base.GetMissingParam();

            if (missingParams != MissingParam.No) return missingParams;

            return GetMissingParam(ParamsToCheck);
        }

        internal override List<MissingParam> GetMissingParams()
        {
            var missingParams = base.GetMissingParams();
            missingParams.AddRange(GetMissingParams(ParamsToCheck));

            return missingParams;
        }

        #endregion
    }
}

#endif