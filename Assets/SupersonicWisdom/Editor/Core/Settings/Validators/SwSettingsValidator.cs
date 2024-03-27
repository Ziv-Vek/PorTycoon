using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    internal abstract class SwSettingsValidator
    {
        #region --- Private Variables ---

        private readonly Dictionary<MissingParam, Action> _missingParamActions = new Dictionary<MissingParam, Action>
        {
            { MissingParam.FbClientToken, () => HandleMissingParamAction(SwErrors.EBuild.MissingFacebookTokenSettings, MissingParam.FbClientToken) },
            { MissingParam.AdmobAppId, () => HandleMissingParamAction(SwErrors.EBuild.MissingAdmobAppIdSettings, MissingParam.AdmobAppId) },
        };

        #endregion
        
        
        #region --- Public Methods ---

        public IapDuplicateParam CheckIapForDuplicates ()
        {
            return IsDuplicateIap(GetIapNoAdsId()) ? IapDuplicateParam.NoAdsIapDuplicate : IapDuplicateParam.No;
        }

        public void HandleDuplicateIap(IapDuplicateParam param)
        {
            SwEditorAlerts.AlertWarning(SwEditorConstants.UI.DUPLICATE_PRODUCT.Format(GetIapDuplicationDetails(param)), SwEditorConstants.UI.ButtonTitle.OK);
        }
        
        public void HandleMissingParam(MissingParam param)
        {
            if (_missingParamActions.TryGetValue(param, out Action action))
            {
                action();
            }
            else
            {
                SwEditorAlerts.AlertWarning(SwEditorConstants.UI.PARAM_IS_MISSING.Format(GetDetails(param)), SwEditorConstants.UI.ButtonTitle.OK);
            }
        }

        #endregion


        #region --- Private Methods ---

        /// <summary>
        ///     Validates the values for specific params.
        /// </summary>
        /// <returns>If nothing missed will be returned NO_ERR otherwise id of error</returns>
        protected static MissingParam GetMissingParam(IEnumerable<Tuple<MissingParam, string>> paramsToCheck)
        {
            var missingParams = GetMissingParamsList(paramsToCheck);
            var firstMissing = missingParams.Count > 0 ? missingParams.First() : MissingParam.No;

            return firstMissing;
        }

        protected static List<MissingParam> GetMissingParams(IEnumerable<Tuple<MissingParam, string>> paramsToCheck)
        {
            var missingParams = GetMissingParamsList(paramsToCheck);
            var updatedList = new List<MissingParam>();
            
            updatedList.AddRange(missingParams);

            return updatedList;
        }

        private static bool IsMissingParam(string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        private static bool IsDuplicateIap(string noAdsProductId)
        {
            return SwEditorUtils.SwSettings.IapProductDescriptors.Any(product => product.productID.Equals(noAdsProductId));
        }
        
        private static List<MissingParam> GetMissingParamsList (IEnumerable<Tuple<MissingParam, string>> paramsToCheck)
        {
            var missingParams = paramsToCheck
                .Where(e => IsMissingParam(e.Item2))
                .Select(e => e.Item1)
                .ToList();
            
            return missingParams;
        }

        protected virtual string GetIapNoAdsId ()
        {
            return "";
        }

        internal abstract MissingParam GetMissingParam ();
        
        internal abstract List<MissingParam> GetMissingParams ();

        private static string GetDetails(MissingParam param)
        {
            switch (param)
            {
                case MissingParam.IosAppId:
                    return "iOS App ID";

                case MissingParam.GameId:
                    return "Game ID";

                case MissingParam.IsAppKey:
                    return "IronSource App Key";

                case MissingParam.GaGameKey:
                    return "GameAnalytics Game Key";

                case MissingParam.GaSecretKey:
                    return "GameAnalytics Secret Key";

                case MissingParam.FbAppId:
                    return "Facebook App ID";
                
                case MissingParam.FbClientToken:
                    return "Facebook Client Token";
                
                case MissingParam.AdmobAppId:
                    return "AdMob App ID";
                
                default:
                    return "Some required data";
            }
        }

        private string GetIapDuplicationDetails(IapDuplicateParam param)
        {
            switch (param)
            {
                case IapDuplicateParam.NoAdsIapDuplicate:
                    return "\"No Ads\" Product ID";
                default:
                    return "some iap product";
            }
        }
        
        private static void HandleMissingParamAction(SwErrors.EBuild errorCode, MissingParam missingParam)
        {
            var details = GetDetails(missingParam);
            
            if (SwEditorAlerts.AlertError(SwEditorConstants.UI.PARAM_IS_MISSING.Format(details), (long)errorCode, SwEditorConstants.UI.ButtonTitle.GO_TO_SETTINGS, SwEditorConstants.UI.ButtonTitle.CLOSE))
            {
                SwMenu.AllowEditingSettings();
                SwEditorUtils.OpenSettings();
            }

            throw new BuildPlayerWindow.BuildMethodException($"{details} is missing.");
        }

        #endregion


        #region --- Enums ---

        public enum IapDuplicateParam
        {
            No = 0,
            NoAdsIapDuplicate = -1,
        }

        protected internal enum MissingParam
        {
            No = 0,
            IosAppId = -1,
            GameId = -2,
            IsAppKey = -3,
            GaGameKey = -4,
            GaSecretKey = -5,
            FbAppId = -6,
            AdmobAppId = -7,
            FbClientToken = -8,
        }

        #endregion
    }
}