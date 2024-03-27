using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Object = UnityEngine.Object;

namespace SupersonicWisdomSDK
{
    public class SwReflectionUtils
    {
        #region --- Constants ---
        
        private const string TEST_EXCLUDE_CLASSES = "Test";
		private const string PRIVATE_MEMBER_PREFIX = "_";

        #endregion
        
        
        #region --- Public Methods ---

        public static IEnumerable<Type> GetAllTypes<T>()
        {
            var types = new List<Type>();
            var stages = SwSdkUtils.GetAllStages();

            if (stages == null) return types;

            foreach (var stage in stages)
            {
                types.AddRange(GetAllTypesByStage<T>(stage.ToString()));
            }

            return types;
        }

        public static List<Type> GetAllTypesByStage<T>(string stage = "")
        {
            var allTypes = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.GetName().Name.Contains(stage)))
            {
                allTypes.AddRange(assembly.GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)) && !myType.Name.Contains(TEST_EXCLUDE_CLASSES)));
            }

            return allTypes;
        }

        public static bool SetProperty(string fieldName, string fieldValue, Object instance)
        {
            if (instance == null) return false;

            var didUpdate = false;

            try
            {
                instance.GetType().GetProperty(fieldName).SetValue(instance, fieldValue);
                didUpdate = true;
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Utils, e);
            }

            return didUpdate;
        }

        public static T GetProperty<T>(string fieldName, T defaultValue, Object instance) where T : class
        {
            var value = defaultValue;

            if (instance == null) return value;

            try
            {
                value = instance.GetType().GetProperty(fieldName).GetValue(instance) as T;
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Utils, e);
            }

            return value ?? defaultValue;
        }
        
        public static Dictionary<string, object> ToDictionary(object obj)
        {
            var dictionary = new Dictionary<string, object>();

            var type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                var fieldName = field.Name.StartsWith(PRIVATE_MEMBER_PREFIX) ? field.Name[1..] : field.Name;
                var value = field.GetValue(obj);
                dictionary[fieldName] = value;
            }

            return dictionary;
        }

        #endregion
    }
}