#if SW_STAGE_STAGE10_OR_ABOVE

namespace SupersonicWisdomSDK
{
    public abstract class SwIntegrationToolManualBaseTestCase : SwBaseIntegrationToolTestCase
    {
        protected abstract string GetID();
        
        protected SwIntegrationToolManualBaseTestCase()
        {
            ID = GetID();
        }
    }
}
#endif