using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwSettingsValidatorFactory
    {
        #region --- Private Methods ---

        internal SwSettingsValidator getSettingsValidator(BuildTarget target)
        {
            var platformName = BuildTarget.iOS == target ? "Ios" : "Android";
            var stageName = SwStageUtils.CurrentStageName;
            var containerClassQualifiedName = $"SupersonicWisdomSDK.Editor.Sw{stageName}{platformName}SettingsValidator";
            var swValidatorType = Type.GetType(containerClassQualifiedName); // Will be accessible without our "asmdef" files

            if (swValidatorType == null)
            {
                // Bypasses asmdef restrictions - This should run in dev mode, only when our "asmdef" files are preventing this access
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(item => item.GetName().Name.Equals($"SupersonicWisdom.{stageName}.Editor"));

                if (assembly != null)
                {
                    swValidatorType = assembly.GetType(containerClassQualifiedName);
                }
            }

            var settingsValidator = (SwSettingsValidator)swValidatorType.GetConstructor(new Type[] { }).Invoke(new object[] { });

            return settingsValidator;
        }

        #endregion
    }
}