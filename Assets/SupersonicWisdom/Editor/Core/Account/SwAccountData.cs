using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwAccountData : ScriptableObject
    {
        #region --- Members ---

        [HideInInspector] [SerializeField] private List<TitleDetails> titlesList = new List<TitleDetails>();
        [HideInInspector] [SerializeField] private string integrationGuideUrl;

        #endregion


        #region --- Properties ---

        public List<TitleDetails> TitleDetailsList
        {
            get { return titlesList; }
            set
            {
                titlesList = value;
                EditorUtility.SetDirty(this);
            }
        }
        
        public string IntegrationGuideUrl
        {
            get { return string.IsNullOrEmpty(integrationGuideUrl) ? SwEditorConstants.DEFAULT_STAGE_INTEGRATION_GUIDE_URL : integrationGuideUrl; }
            set
            {
                integrationGuideUrl = value;
                EditorUtility.SetDirty(this);
            }
        }

        #endregion
    }
}