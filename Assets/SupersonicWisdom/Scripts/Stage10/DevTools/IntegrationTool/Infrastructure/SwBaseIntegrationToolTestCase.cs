#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Text;

namespace SupersonicWisdomSDK
{
    public abstract class SwBaseIntegrationToolTestCase : ISwIntegrationToolTestCase, IComparable
    {
        #region --- Events ---

        public event Action<SwBaseIntegrationToolTestCase> TestCaseStatusChangedEvent;

        #endregion


        #region --- Members ---

        private ESwTestCaseStatus _status;

        #endregion


        #region --- Properties ---

        public ESwTestCaseStatus Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;

                    TestCaseStatusChangedEvent?.Invoke(this);
                }
            }
        }

        public string ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsRegExp { get; set; }

        #endregion


        #region --- Public Methods ---

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{ID} - {Status} - {Title}");

            return sb.ToString();
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (!(obj is ISwIntegrationToolTestCase otherTestCase))
            {
                return -1;
            }

            if(otherTestCase.Status > this.Status)
            {
                return 1;
            }
                
            if(otherTestCase.Status == this.Status)
            {
                return 0;
            }

            return -1;
        }

        #endregion


        #region --- Private Methods ---

        protected abstract void OnDone();
        protected internal virtual void Finish() { }

        #endregion
    }
}
#endif