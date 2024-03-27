namespace SupersonicWisdomSDK
{
    public interface ISwIntegrationToolTestCaseViewData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ESwTestCaseStatus Status { get; set; }
    }
}