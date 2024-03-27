#if SW_STAGE_STAGE10_OR_ABOVE
namespace SupersonicWisdomSDK
{
    public interface ISwIntegrationToolTestCase : ISwIntegrationToolTestCaseViewData
    {
        #region --- Properties ---

        public string ID { get; }

        #endregion
    }
}
#endif