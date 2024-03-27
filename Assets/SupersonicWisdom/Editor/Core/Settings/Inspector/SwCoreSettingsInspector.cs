using Facebook.Unity.Editor;
using Facebook.Unity.Settings;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwCoreSettingsInspector : UnityEditor.Editor, ISwRepaintDelegate
    {
        #region --- Members ---

        private static SwCoreSettingsTab[] _tabs = new SwCoreSettingsTab[]{ };
        internal static int? SelectedTabIndex;
        protected SerializedObject _soSettings;
        
        private SwSettings _settings;
        private int _currentTabIndex;
        private string _currentStage;

        #endregion


        #region --- Properties ---

        private int SpaceBetweenFields
        {
            get { return SwCoreSettingsTab.SPACE_BETWEEN_FIELDS; }
        }

        private SwCoreSettingsTab[] SettingsTabs
        {
            get { return Tabs(); }
        }

        #endregion


        #region --- Mono Override ---

        private void OnEnable ()
        {
            // Fix for unpredictable bug in unity editor
            // OnEnable can sometime be called automatically in play mode
            // https://answers.unity.com/answers/1854373/view.html
            if (target == null)
            {
                return;
            }

            _settings = target as SwSettings;
            _soSettings = new SerializedObject(_settings);

            if (FacebookSettings.NullableInstance != null)
            {
                return;
            }

            FacebookSettingsEditor.Edit();
            Selection.activeObject = SwEditorUtils.SwSettings;
        }

        private void OnValidate ()
        {
            CheckIfStageChanged();
        }

        #endregion


        #region --- Public Methods ---

        public override void OnInspectorGUI()
        {
            CheckIfStageChanged();
            BeforeDrawSelectedTab();

            _soSettings.Update();
            GUILayout.BeginVertical();
            GUILayout.Space(SpaceBetweenFields * 3);
            DrawSupersonicTitle();
            GUILayout.Space(SpaceBetweenFields * 3);
            var didGuiChange = GUI.changed;
            _currentTabIndex = SelectTab(SelectedTabIndex ?? _currentTabIndex);
            SelectedTabIndex = null;
            GUI.changed = didGuiChange;
            DrawSelectedTab(_currentTabIndex, this);
            GUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_settings);
            }

            AfterDrawSelectedTab();
        }

        public void SwRepaint ()
        {
            Repaint();
        }
        
        internal static void ShowTab(string tabName)
        {
            var tab = _tabs.FirstOrDefault(tab => tab.Name().Equals(tabName));
            
            if (tab == null)
            {
                return;
            }
            
            tab.SetCanShow(true);
            SelectedTabIndex = _tabs.ToList().IndexOf(tab);
        }
        
        internal static void HideTab(string tabName)
        {
            var tab = _tabs.FirstOrDefault(tab => tab.Name().Equals(tabName));
            
            if (tab == null)
            {
                return;
            }
            
            tab.SetCanShow(false);
            SelectedTabIndex = null;
        }

        #endregion


        #region --- Private Methods ---

        protected virtual void AfterDrawSelectedTab ()
        { }

        protected virtual void BeforeDrawSelectedTab ()
        { }

        protected virtual SwCoreSettingsTab[] StageTabs()
        {
            return new SwCoreSettingsTab[]
            {
                new SwGeneralCoreSettingsTab(_soSettings),
                new SwIosCoreSettingsTab(_soSettings),
                new SwAndroidCoreSettingsTab(_soSettings),
                new SwDebugCoreSettingsTab(_soSettings)
            };
        }

        private void CheckIfStageChanged ()
        {
            if (_currentStage?.Equals(SwStageUtils.CurrentStageName) ?? false) return;
            
            _currentStage = SwStageUtils.CurrentStageName;
            OnStageChanged();
        }
        
        private void DrawSelectedTab(int selectedTabIndex, ISwRepaintDelegate repaintDelegate)
        {
            EditorGUI.BeginChangeCheck();
            var selectedTab = SettingsTabs.Where(tab => tab.CanShow).ToList().SwSafelyGet(selectedTabIndex, null);
            
            if (selectedTab != null)
            {
                selectedTab.RepaintDelegate = repaintDelegate;
                selectedTab.DrawContent();
            }

            if (EditorGUI.EndChangeCheck())
            {
                _soSettings.ApplyModifiedProperties();
            }
        }

        private void DrawSupersonicTitle()
        {
            GUILayout.BeginHorizontal();
            SwEditorUtils.DrawGuiLayoutSwLogoLabel(32);
            GUILayout.BeginVertical();
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            var title = $" SupersonicWisdom SDK v.{SwConstants.SDK_VERSION} [{SwStageUtils.CurrentStageName}]";
#if UNITY_2019_1_OR_NEWER
            title = $"{title}{(EditorUtility.IsDirty(_settings) ? " *" : "")}";
#endif
            GUILayout.Label(title, EditorStyles.largeLabel);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void OnStageChanged()
        {
            ResetTabs();
        }

        private void ResetTabs()
        {
            _tabs = new SwCoreSettingsTab[] { };
        }

        private int SelectTab(int tabToSelect)
        {
            EditorGUI.BeginChangeCheck();
            var tabNames = SettingsTabs.Where(tab => tab.CanShow).Select(tab => tab.Name()).ToArray();
            var selectedTab = GUILayout.Toolbar(tabToSelect, tabNames);

            if (!EditorGUI.EndChangeCheck())
            {
                return selectedTab;
            }

            _soSettings.ApplyModifiedProperties();
            GUI.FocusControl(null);

            return selectedTab;
        }

        private SwCoreSettingsTab[] Tabs ()
        {
            if (_tabs.Length > 0) return _tabs;

            _tabs = StageTabs();

            return _tabs;
        }

        #endregion
    }
}