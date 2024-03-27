using System;

namespace SupersonicWisdomSDK
{
    internal interface ISwUserData
    {
        #region --- Public Methods ---

        void Load(ISwInitParams initParams);
        void LoadUuid ();

        #endregion
    }
}