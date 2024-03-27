using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    public class SwIntegrationPanel : MonoBehaviour
    {
        #region --- Events ---

        public event Action FinishButtonClicked;

        #endregion


        #region --- Inspector ---

        [SerializeField] private GameObject _testCasePrefab;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Transform _scrollView;

        #endregion


        #region --- Members ---

        private Dictionary<string, SwDevtoolsTestCaseMono> _testCaseDict;
        private SwDevtoolsTestCaseMono[] _devtoolsTestCases;

        #endregion


        #region --- Public Methods ---

        public void Setup(string title)
        {
            _title.SetText(title);
        }

        public void AddTestCase(Dictionary<string, ISwIntegrationToolTestCaseViewData> dataDict)
        {
            if (dataDict == null) return;

            _testCaseDict ??= new Dictionary<string, SwDevtoolsTestCaseMono>();

            foreach (var pair in dataDict)
            {
                AddTestCase(pair.Key, pair.Value);
            }
        }

        public void ChangeTestCaseStateOrCreate(string id, ISwIntegrationToolTestCaseViewData viewData)
        {
            if (!_testCaseDict.ContainsKey(id))
            {
                AddTestCase(id, viewData);

                return;
            }

            var testCase = _testCaseDict[id];
            testCase.Status = viewData.Status;
            testCase.SetLayoutElementPriority(viewData.Status);
        }

        public void Finish()
        {
            FinishButtonClicked?.Invoke();
        }

        #endregion


        #region --- Private Methods ---

        private void AddTestCase(string key, ISwIntegrationToolTestCaseViewData value)
        {
            var testCaseObject = Instantiate(_testCasePrefab, _scrollView);
            var testCaseMono = testCaseObject.GetComponent<SwDevtoolsTestCaseMono>();

            if (testCaseMono != null)
            {
                testCaseMono.Setup(value);
                _testCaseDict.Add(key, testCaseMono);
            }
        }

        #endregion
    }
}