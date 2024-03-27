#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Linq;

namespace SupersonicWisdomSDK
{
    public abstract class SwBaseIntegrationToolTestCasesManager<T> where T : SwBaseIntegrationToolTestCase
    {
        #region --- Events ---

        public event Action<T> TestCaseStatusChangedEvent;

        #endregion


        #region --- Members ---

        private readonly Action<Dictionary<string, ISwIntegrationToolTestCaseViewData>> _setViewData;
        private readonly Action<ISwIntegrationToolTestCase> _onIntegrationToolTestCaseManagerStatusChanged;
        private readonly object _objectToSplitWith;

        private Dictionary<string, T> _pendingTestCases;
        private Dictionary<string, T> _runningTestCases;
        private Dictionary<string, T> _failedTestCases;
        private Dictionary<string, T> _completedTestCases;

        #endregion


        #region --- Properties ---

        protected abstract bool IsValid { get; }

        private Dictionary<string, T> All
        {
            get
            {
                var allTestCases = new Dictionary<string, T>();
                allTestCases.SwMerge(false, _failedTestCases);
                allTestCases.SwMerge(false, _runningTestCases);
                allTestCases.SwMerge(false, _pendingTestCases);
                allTestCases.SwMerge(false, _completedTestCases);

                return allTestCases;
            }
        }

        internal T[] Status
        {
            get { return All.Values.ToArray(); }
        }

        private bool IsEnabled { get; set; }

        #endregion


        #region --- Construction ---
        
        protected SwBaseIntegrationToolTestCasesManager(Action<Dictionary<string, ISwIntegrationToolTestCaseViewData>> setViewData, Action<ISwIntegrationToolTestCase> onIntegrationToolTestCaseManagerStatusChanged)
        {
            _setViewData = setViewData;
            _onIntegrationToolTestCaseManagerStatusChanged = onIntegrationToolTestCaseManagerStatusChanged;
        }

        #endregion


        #region --- Public Methods ---

        public void Init()
        {
            if (!IsValid) return;

            InitLists();
            SplitGenericTestCasesToDictionaries();
            SetTestCaseData(_setViewData);

            TestCaseStatusChangedEvent += _onIntegrationToolTestCaseManagerStatusChanged;
            IsEnabled = true;
        }

        public bool IsIntegrationCompleted()
        {
            return _pendingTestCases.Count == 0 && _runningTestCases.Count == 0 && _failedTestCases.Count == 0;
        }

        #endregion


        #region --- Private Methods ---

        protected abstract void SplitGenericTestCasesToDictionaries();

        protected void OnTestCaseCreated(T currentBaseTestCase)
        {
            currentBaseTestCase.TestCaseStatusChangedEvent += OnTestCaseStatusChanged;

            SplitToLists(currentBaseTestCase);
        }

        internal void Finish()
        {
            foreach (var testCase in All)
            {
                testCase.Value.Finish();
            }
        }

        private void InitLists()
        {
            _failedTestCases = new Dictionary<string, T>();
            _pendingTestCases = new Dictionary<string, T>();
            _runningTestCases = new Dictionary<string, T>();
            _completedTestCases = new Dictionary<string, T>();
        }

        private void SetTestCaseData(Action<Dictionary<string, ISwIntegrationToolTestCaseViewData>> action)
        {
            var allTestCases = new Dictionary<string, ISwIntegrationToolTestCaseViewData>();

            foreach (var testCase in All)
            {
                allTestCases.SwAddOrReplace(testCase.Key, testCase.Value);
            }

            action?.Invoke(allTestCases);
        }

        private void SplitToLists(T currentBaseTestCase)
        {
            switch (currentBaseTestCase.Status)
            {
                case ESwTestCaseStatus.Pending:
                    _pendingTestCases.Add(currentBaseTestCase.ID, currentBaseTestCase);

                    break;

                case ESwTestCaseStatus.Completed:
                    _completedTestCases.Add(currentBaseTestCase.ID, currentBaseTestCase);

                    break;

                case ESwTestCaseStatus.Failed:
                    _failedTestCases.Add(currentBaseTestCase.ID, currentBaseTestCase);

                    break;
                case ESwTestCaseStatus.Running:
                    _runningTestCases.Add(currentBaseTestCase.ID, currentBaseTestCase);

                    break;
            }
        }

        private void SetTestCaseInProperDictionary(T testCase)
        {
            switch (testCase.Status)
            {
                case ESwTestCaseStatus.Pending:
                    _pendingTestCases.SwAddOrReplace(testCase.ID, testCase);

                    break;
                case ESwTestCaseStatus.Running:
                    _runningTestCases.SwAddOrReplace(testCase.ID, testCase);

                    break;
                case ESwTestCaseStatus.Completed:
                    _completedTestCases.SwAddOrReplace(testCase.ID, testCase);

                    break;
                case ESwTestCaseStatus.Failed:
                    _failedTestCases.SwAddOrReplace(testCase.ID, testCase);

                    break;
            }
        }

        private void MoveTestCaseToProperStateList(T testCase)
        {
            if (testCase == null) return;

            RemoveFromAllLists(testCase);
            SetTestCaseInProperDictionary(testCase);

            TestCaseStatusChangedEvent?.Invoke(testCase);
        }

        private void RemoveFromAllLists(SwBaseIntegrationToolTestCase toolTestCase)
        {
            _pendingTestCases.Remove(toolTestCase.ID);
            _runningTestCases.Remove(toolTestCase.ID);
            _failedTestCases.Remove(toolTestCase.ID);
            _completedTestCases.Remove(toolTestCase.ID);
        }

        #endregion


        #region --- Event Handler ---

        private void OnTestCaseStatusChanged(SwBaseIntegrationToolTestCase toolTestCase)
        {
            MoveTestCaseToProperStateList(toolTestCase as T);
        }

        #endregion
    }
}
#endif