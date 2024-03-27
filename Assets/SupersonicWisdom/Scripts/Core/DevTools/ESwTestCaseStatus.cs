namespace SupersonicWisdomSDK
{
    public enum ESwTestCaseStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
    }

    public enum ETestCaseType
    {
        scheduled_log_assertion,
        log_violation,
        user_acceptance,
    }
}