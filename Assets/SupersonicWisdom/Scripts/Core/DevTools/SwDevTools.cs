using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SupersonicWisdomSDK
{
    internal class SwDevTools
    {
        #region --- Constants ---

        private const string DEVTOOLS_PREFAB_PATH = "Core/DevTools/SwDevToolsCanvas";

        #endregion


        #region --- Members ---

        private readonly Dictionary<string, Action> _buttons;
        private readonly SwFilesCacheManager _filesCacheManager;

        protected SwDevToolsCanvas DevToolsCanvas;

        #endregion


        #region --- Properties ---

        private bool IsDevtoolEnabled { get; set; }

        #endregion


        #region --- Construction ---

        protected SwDevTools(SwFilesCacheManager filesCacheManager)
        {
            _filesCacheManager = filesCacheManager;

            var myButtons = new Dictionary<string, Action>
            {
                { "Clear SDK Cache", ClearSDKCache },
                { "Delete PlayerPrefs", DeletePlayerPrefs },
                { "Quit Application", QuitApplication },
            };

            _buttons = new Dictionary<string, Action>();
            _buttons.SwMerge(true, myButtons);
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator EnableDevtools(bool isEnabled)
        {
            IsDevtoolEnabled = isEnabled;

            if (IsDevtoolEnabled)
            {
                CreateDevTools();
                yield return SetUpDevToolsCanvas();
            }
            else if (DevToolsCanvas != null)
            {
                Object.Destroy(DevToolsCanvas.gameObject);
                DevToolsCanvas = null;
            }
        }

        #endregion


        #region --- Private Methods ---

        protected virtual IEnumerator SetUpDevToolsCanvas()
        {
            if (!IsDevtoolEnabled) yield break;

            DevToolsCanvas.Setup();
            DevToolsCanvas.SetUtilButtons(_buttons);
        }

        protected SwDevToolsCanvas Create()
        {
            var swDevToolsPrefab = (GameObject)Resources.Load(DEVTOOLS_PREFAB_PATH);

            if (swDevToolsPrefab == null)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Devtools, "Couldn't load DevTools prefab");
            }

            var devTools = Object.Instantiate(swDevToolsPrefab);

            return devTools.GetComponent<SwDevToolsCanvas>();
        }

        protected void AddUtilsButtons(Dictionary<string, Action> buttons)
        {
            _buttons.SwMerge(true, buttons);
            DevToolsCanvas.SetUtilButtons(_buttons);
        }

        private void ClearSDKCache()
        {
            var files = _filesCacheManager?.GetAllFilesFromCache() ?? new string[] { };

            foreach (var file in files)
            {
                _filesCacheManager?.DeleteFile(file);
            }
        }

        private void DeletePlayerPrefs()
        {
            SwInfra.KeyValueStore.DeleteAll();
        }

        private void QuitApplication()
        {
            Application.Quit();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }

        private void CreateDevTools()
        {
            DevToolsCanvas = Create();
        }

        #endregion
    }
}