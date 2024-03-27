using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwEditorCallbacks : AssetModificationProcessor, UnityEditor.Build.IPreprocessBuildWithReport
    {
        #region --- Construction ---

        static SwEditorCallbacks() // Static constructor, called only once, when the class gets loaded.
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnPreCompilation;
        }

        #endregion

        
        #region --- Public Methods ---

        public int callbackOrder
        {
            get { return 0; }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            UpdateRequiredDefineSymbols();
        }

        public static void UpdateRequiredDefineSymbols()
        {
            var currentStageDefineSymbols = LowestInstanceSubclassOfType<SwDefineSymbols>();
            currentStageDefineSymbols?.UpdateRequiredSymbols();
        }
        
        #endregion
        
        
        #region --- Private Methods ---

        [DidReloadScripts]
        private static void OnPostCompilation()
        {
            SwAccountUtils.TryToRestoreLoginToken();
            WelcomeMessageUtils.TryShowWelcomeMessage();
            SwSelfUpdateEventsHandler.OnPostCompilation();
            InjectPackageJsonDependenciesIfNeeded();
            UpdateRequiredDefineSymbols();
        }
        
        public static void InjectPackageJsonDependenciesIfNeeded()
        {
            var currentStageDependenciesInjector = LowestInstanceSubclassOfType<SwEditorDependenciesInjector>();
            currentStageDependenciesInjector?.InjectPackageJsonDependenciesIfNeeded();
        }

        /// Returns the most derived type of specific type, assuming the inheritance is linear and each base class has one subclass, for example: SwStage10Editor inherits from SwStageEditor, SwStage30Editor inherits from SwStage1Editor, and so on...
        private static Type LowestSubclassOfType<T>()
        {
            var baseType = typeof(T);
            var allSubclasses = TypeCache.GetTypesDerivedFrom<T>().ToArray();
            var lowestSubType = baseType;

            foreach (var subType in allSubclasses)
            {
                if (subType.IsSubclassOf(lowestSubType))
                {
                    lowestSubType = subType;
                }
            }
            
            return lowestSubType;
        }

        /// Returns an object instance of the most derived type of specific type (depends on <see cref="SwEditorCallbacks.LowestSubclassOfType()"/>)
        private static T LowestInstanceSubclassOfType<T>()
        {
            var lowestDerivedType = LowestSubclassOfType<T>();
            var subclassInstance = lowestDerivedType?.GetConstructor(new Type[] { })?.Invoke(new object[] { });

            return (T) subclassInstance;
        }

        #endregion


        #region --- Event Handler ---

        private static void OnPreCompilation()
        {
            SwSelfUpdateEventsHandler.OnPreCompilation();
        }

        #endregion
    }
}