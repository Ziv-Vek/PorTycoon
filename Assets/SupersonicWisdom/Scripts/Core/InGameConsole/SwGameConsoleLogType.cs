namespace SupersonicWisdomSDK
{
    internal enum SwGameConsoleLogType
    {
        /// <summary>
        ///     Rewrite of Unity's LogTypes, to fit for the InGameConsole use
        /// </summary>
        /// <footer>
        ///     <a href="https://docs.unity3d.com/ScriptReference/LogType.html"></a>
        /// </footer>
        Warning = 5,
        Error = 10,
        Exception = 15
    }
}