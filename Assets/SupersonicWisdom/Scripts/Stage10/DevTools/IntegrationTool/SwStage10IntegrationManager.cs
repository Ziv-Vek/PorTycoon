#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    internal class SwStage10IntegrationManager
    {
        #region --- Constants ---

        private const string INTEGRATION_DATA_URL = "integrationDataUrl";
        private const string INTEGRATION_DATA_KEY = "integrationDataKey";
        private const string INTEGRATION_DATA_IV = "integrationDataIV";
        private const string INTEGRATION_MANAGER_PREVIOUS_DEEP_LINK_KEY = "swIntegrationManagerPreviousDeepLink";
        private const string PREVIOUS_RUN_STATUS_KEY = "swIntegrationManagerPreviousRunStatus";
        private const string INTEGRATION_COMPLETED_TOAST_MESSAGE = "Integration completed!";

        #endregion


        #region --- Members ---

        private readonly SwDevToolsCanvas _devToolsCanvas;
        private Dictionary<string, string> _deeplinkParams;

        private SwIntegrationToolConsoleTestCasesManager _integrationToolConsoleTestCasesManager;
        private SwIntegrationToolManualTestCasesManager _integrationToolManualTestCasesManager;

        #endregion


        #region --- Properties ---

        private Dictionary<string, ESwTestCaseStatus> PreviousRunStatus
        {
            get
            {
                var previousRunStatusString = SwInfra.KeyValueStore.GetString(PREVIOUS_RUN_STATUS_KEY, null);

                SerializedTestCaseStatus[] previousRunStatusObject = null;

                try
                {
                    previousRunStatusObject = previousRunStatusString != null ? JsonConvert.DeserializeObject<SerializedTestCaseStatus[]>(previousRunStatusString) : null;
                }
                catch (Exception)
                {
                    SwInfra.Logger.Log(EWisdomLogType.IntegrationTool, "No previous status");   
                }

                var dict = new Dictionary<string, ESwTestCaseStatus>();

                if (previousRunStatusObject != null)
                {
                    foreach (var testCase in previousRunStatusObject)
                    {
                        dict[testCase.id] = testCase.status;
                    }
                }

                return dict;
            }
        }

        private SwBaseIntegrationToolTestCase[] All
        {
            get
            {
                var allList = new List<SwBaseIntegrationToolTestCase>();

                if (_integrationToolManualTestCasesManager?.Status != null)
                {
                    allList.AddRange(_integrationToolManualTestCasesManager.Status);
                }
                
                if (_integrationToolConsoleTestCasesManager?.Status != null)
                {
                    allList.AddRange(_integrationToolConsoleTestCasesManager.Status);
                }

                return allList.ToArray();
            }
        }

        private SwBaseIntegrationToolTestCase[] SortedTestCases
        {
            get
            {
                var allTestCasesStatus = All.ToList();
                allTestCasesStatus.Sort(Comparer<ISwIntegrationToolTestCase>.Default);

                return allTestCasesStatus.ToArray();
            }
        }

        #endregion


        #region --- Construction ---

        public SwStage10IntegrationManager(SwDevToolsCanvas devToolsCanvas)
        {
            _devToolsCanvas = devToolsCanvas;
        }

        #endregion


        #region --- Private Methods ---

        public IEnumerator Init(Dictionary<string, string> deepLinkParams)
        {
            _deeplinkParams = deepLinkParams;
            yield return TryCreateTestCase(_deeplinkParams);
            TryModifyStatusByPreviousRun();
        }

        private IEnumerator TryCreateTestCase(Dictionary<string, string> deepLinkParams)
        {
            yield return ParseDeepLink(deepLinkParams, (integrationData) =>
            {
                // enable mock rate-us
                InitManagers(integrationData);
                _devToolsCanvas.IntegrationPanel.FinishButtonClicked += OnFinishButtonClicked;
            });
        }

        private static IEnumerator ParseDeepLink(Dictionary<string, string> deepLinkParams, Action<string> onDone)
        {
            if (deepLinkParams != null
                && deepLinkParams.ContainsKey(INTEGRATION_DATA_URL) 
                && deepLinkParams.ContainsKey(INTEGRATION_DATA_KEY) 
                && deepLinkParams.ContainsKey(INTEGRATION_DATA_IV))
            {
                var integrationLink = new IntegrationLink()
                {
                    url = deepLinkParams[INTEGRATION_DATA_URL],
                    key = deepLinkParams[INTEGRATION_DATA_KEY],
                    iv = deepLinkParams[INTEGRATION_DATA_IV]
                };

                yield return SwIntegrationManagerUtils.GetIntegrationData(integrationLink, (integrationJsonString) =>
                {
                    SwInfra.KeyValueStore.DeleteKey(PREVIOUS_RUN_STATUS_KEY);
                    SwInfra.KeyValueStore.SetString(INTEGRATION_MANAGER_PREVIOUS_DEEP_LINK_KEY, integrationJsonString);
                    SwInfra.KeyValueStore.Save();

                    onDone?.Invoke(integrationJsonString);
                });
            }
            else
            {
                onDone?.Invoke(SwInfra.KeyValueStore.GetString(INTEGRATION_MANAGER_PREVIOUS_DEEP_LINK_KEY));
            }
        }

        private void TryModifyStatusByPreviousRun()
        {
            var previousRunStatus = PreviousRunStatus;

            if (previousRunStatus != null)
            {
                foreach (var testCase in All)
                {
                    if (previousRunStatus.ContainsKey(testCase.ID))
                    {
                        testCase.Status = previousRunStatus[testCase.ID];
                    }
                }
            }
        }

        private void InitManagers(string integrationData)
        {
            var integrationObject = ParseRawData(integrationData);

            if (integrationObject == null) return;
            
            _devToolsCanvas.IntegrationPanel.Setup(integrationObject.title);
            _integrationToolManualTestCasesManager = new SwIntegrationToolManualTestCasesManager(_devToolsCanvas.SetIntegrationData, OnIntegrationToolTestCaseManagerStatusChanged);
            _integrationToolManualTestCasesManager.Init();
            _integrationToolConsoleTestCasesManager = new SwIntegrationToolConsoleTestCasesManager(integrationObject.testCases, _devToolsCanvas.SetIntegrationData, _devToolsCanvas.ShowToast, OnIntegrationToolTestCaseManagerStatusChanged);
            _integrationToolConsoleTestCasesManager.Init();
        }

        private SwIntegrationToolSuiteRawData ParseRawData(string integrationRawData)
        {
            return integrationRawData.SwIsNullOrEmpty() ? null : JsonConvert.DeserializeObject<SwIntegrationToolSuiteRawData>(integrationRawData);
        }

        private void OnIntegrationToolTestCaseManagerStatusChanged(ISwIntegrationToolTestCase testCaseData)
        {
            _devToolsCanvas.IntegrationPanel.ChangeTestCaseStateOrCreate(testCaseData.ID, testCaseData);

            if (testCaseData.Status == ESwTestCaseStatus.Failed)
            {
                _devToolsCanvas.ShowToast($"{testCaseData.Title} Failed!");
            }

            Save();

            if (IsIntegrationCompleted())
            {
                OnIntegrationCompleted();
            }
        }

        private void OnIntegrationCompleted()
        {
            _devToolsCanvas.ShowToast(INTEGRATION_COMPLETED_TOAST_MESSAGE);
        }

        private bool IsIntegrationCompleted()
        {
            return _integrationToolManualTestCasesManager.IsIntegrationCompleted() && _integrationToolConsoleTestCasesManager.IsIntegrationCompleted();
        }

        private void Save()
        {
            var statusDict = new SerializedTestCaseStatus[All.Length];

            for (var i=0; i< All.Length; i++)
            {
                statusDict[i] = new SerializedTestCaseStatus()
                {
                    id = All[i].ID,
                    status = All[i].Status,
                };
            }

            var allJsonString = JsonConvert.SerializeObject(statusDict);

            SwInfra.KeyValueStore.SetString(PREVIOUS_RUN_STATUS_KEY, allJsonString);
        }

        #endregion


        #region --- Event Handler ---

        private void OnFinishButtonClicked()
        {
            _integrationToolConsoleTestCasesManager.Finish();
            _integrationToolManualTestCasesManager.Finish();

            var allSortedTestCases = SortedTestCases;

            Print(allSortedTestCases);
        }

        private void Print(ISwIntegrationToolTestCase[] allSortedTestCases)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var testCase in allSortedTestCases)
            {
                sb.AppendLine($"{testCase.Status} - {testCase.ID} - {testCase.Title}");
            }
            
            SwInfra.Logger.Log(EWisdomLogType.IntegrationTool, sb.ToString());
        }

        #endregion
    }
}
#endif