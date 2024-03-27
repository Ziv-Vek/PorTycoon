using System.Threading;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal static class SwInfra
    {
        #region --- Members ---

        private static ISwLogger _logger;
        private static MonoBehaviour _mono;
        private static bool _isInitialized;
        private static ISwKeyValueStore _keyValueStore;
        private static SwCoroutineService _coroutineService;
        private static SwFilesCacheManager _fileCacheManager;

        #endregion


        #region --- Properties ---

        public static ISwKeyValueStore KeyValueStore
        {
            get { return _keyValueStore ??= new SwPlayerPrefsStore(); }
        }

        public static ISwLogger Logger
        {
            get { return _logger ??= new SwLoggerService(); }
        }

        public static SwFilesCacheManager FileCacheManager
        {
            get { return _fileCacheManager ??= new SwFilesCacheManager(); }
        }

        public static ISwMainThreadRunner MainThreadRunner { get; private set; }

        public static SwCoroutineService CoroutineService
        {
            get { return _coroutineService; }
        }

        public static Thread MainThread { get; } = Thread.CurrentThread;

        #endregion


        #region --- Public Methods ---

        public static void Initialize(ISwMainThreadRunner mainThreadRunner, MonoBehaviour mono)
        {
            MainThreadRunner = mainThreadRunner;
            _coroutineService = new SwCoroutineService(mono);
        }

        #endregion
    }
}