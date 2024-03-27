namespace SupersonicWisdomSDK
{
    using System.Collections.Generic;
    using UnityEngine;

    internal class UpdateableManager : MonoBehaviour
    {
        #region --- Members ---

        private List<ILateUpdateable> _lateUpdateables;

        #endregion


        #region --- Mono Override ---

        private void LateUpdate()
        {
            if (_lateUpdateables == null) return;

            foreach (var lateUpdateable in _lateUpdateables)
            {
                lateUpdateable.OnLateUpdate();
            }
        }

        #endregion


        #region --- Public Methods ---

        public void Add(ILateUpdateable lateUpdateable)
        {
            if (_lateUpdateables == null)
            {
                _lateUpdateables = new List<ILateUpdateable>();
            }

            _lateUpdateables.Add(lateUpdateable);
        }

        #endregion
    }
}