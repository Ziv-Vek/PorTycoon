namespace SupersonicWisdomSDK
{
    public delegate void OnInitEnded ();

    public delegate void OnSessionStarted(string sessionId);

    public delegate void OnSessionEnded(string sessionId);
    
    public delegate string GetAdditionalDataJsonMethod();

    public delegate void OnWebResponse(string response);
    
    public delegate void OnConnectivityStatusChanged(string connectionStatus);
}