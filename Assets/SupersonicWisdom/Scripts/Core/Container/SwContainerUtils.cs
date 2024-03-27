using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SupersonicWisdomSDK
{
    internal static class SwContainerUtils
    {
        #region --- Public Methods ---

        public static ISwContainer CreateContainer(string stageName, Dictionary<string, object> initParamsDictionary)
        {
            var containerClassQualifiedName = $"SupersonicWisdomSDK.Sw{stageName}Container";
            var swContainerType = Type.GetType(containerClassQualifiedName); // Will be accessible without our "asmdef" files

            if (swContainerType == null)
            {
                // This will run in dev mode, only when our "asmdef" files are preventing this access
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(item => item.GetName().Name.Equals($"SupersonicWisdom.{stageName}"));

                if (assembly != null)
                {
                    swContainerType = assembly.GetType(containerClassQualifiedName);
                }
            }

            if (swContainerType == null)
            {
                throw new Exception("Missing mandatory class: " + containerClassQualifiedName);
            }

            var getInstanceMethod = swContainerType.GetMethod("GetInstance", BindingFlags.Static | BindingFlags.Public);

            if (getInstanceMethod == null)
            {
                throw new Exception("Missing mandatory GetInstance method inside class: " + containerClassQualifiedName);
            }

            return (ISwContainer) getInstanceMethod.Invoke(null, new object[] { initParamsDictionary ?? new Dictionary<string, object>() });
        }

        public static void InitContainerAsync(ISwContainer container, ISwAsyncCatchableRunnable customInitAsync)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(container.InitAsync, exception =>
            {
                if (exception != null)
                {
                    container.AfterInit(exception);
                }
                else
                {
                    SwInfra.CoroutineService.StartCoroutine(customInitAsync.Run(container.AfterInit));
                }
            });
        }

        public static T InstantiateSupersonicWisdom<T>(string resourcePath)
        {
            var swGameObjectResource = (GameObject)Resources.Load(resourcePath);
            var sceneGameObject = Object.Instantiate(swGameObjectResource);

            if (sceneGameObject == null)
            {
                throw new Exception("Failed to instantiate SupersonicWisdom game object");
            }

            return sceneGameObject.GetComponent<T>();
        }

        public static void SetupContainer(ISwContainer container, string gameObjectName)
        {
            var mono = container.GetMono();
            var gameObject = mono.gameObject;
            gameObject.name = gameObjectName;
            gameObject.SetActive(true);
        }

        #endregion
    }
}