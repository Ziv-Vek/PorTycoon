using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SupersonicWisdomSDK
{
    internal class SwBlockingSimulator : MonoBehaviour
    {
        #region --- Inspector ---

        [SerializeField] private Button closeButton;

        [SerializeField] private GameObject container;
        [SerializeField] private Text message;
        [SerializeField] private Text title;

        #endregion


        #region --- Members ---

        private bool _closable;

        /// <summary>
        ///     An index that incremented every time the popup is hidden
        /// </summary>
        private long _simulatorSessionIndex;

        #endregion


        #region --- Properties ---

        /// <summary>
        ///     Whether to show X button or not
        /// </summary>
        public bool Closable
        {
            get { return _closable; }
            set
            {
                _closable = value;
                closeButton.gameObject.SetActive(value);
            }
        }

        #endregion


        #region --- Mono Override ---

        private void Awake ()
        {
            DontDestroyOnLoad(gameObject);

            // Verify container is inactive even though it should be by default
            container.gameObject.SetActive(false);
        }

        #endregion


        #region --- Public Methods ---

        /// <summary>
        ///     Increments _simulatorSessionIndex and by that causing prior WaitForClose to be resolved
        ///     Hides the container
        /// </summary>
        public void Hide ()
        {
            _simulatorSessionIndex++;
            container.gameObject.SetActive(false);
        }

        #endregion


        #region --- Private Methods ---

        internal void OnCloseClick ()
        {
            Hide();
        }

        /// <summary>
        ///     Show a popup with a title and message
        /// </summary>
        /// <param name="titleText"></param>
        /// <param name="messageText"></param>
        /// <returns>A method which returns an async function.The async function is resolved when the popup is closed</returns>
        internal SwAsyncMethod Show(string titleText, string messageText = "")
        {
            if (container.gameObject.activeSelf)
            {
                SwInfra.Logger.LogError(EWisdomLogType.BlockingApi, "Blocking Simulator is already visible. Check for possible overlap between blocking notifications");
            }

            var currentSessionIndex = _simulatorSessionIndex;
            title.text = titleText;

            if (!string.IsNullOrEmpty(messageText))
            {
                message.text = messageText;
            }

            container.gameObject.SetActive(true);

            // This local method will bed resolved once Hide method is applied for
            // the first time after the current method invocation (The `Show` method)
            IEnumerator WaitForClose ()
            {
                while (currentSessionIndex == _simulatorSessionIndex)
                {
                    yield return null;
                }
            }

            return WaitForClose;
        }

        #endregion
    }
}