namespace SupersonicWisdomSDK.Editor
{
    internal class SwEditorError
    {
        #region --- Members ---

        public int ResponseCode;
        public string ErrorMessage;

        #endregion


        #region --- Properties ---

        public bool IsValid
        {
            get { return !string.IsNullOrWhiteSpace(ErrorMessage); }
        }

        public override string ToString()
        {
            return $"Error message: {ErrorMessage}\nResponse code: {ResponseCode}";
        }

        #endregion
    }
}