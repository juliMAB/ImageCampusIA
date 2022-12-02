using System;
using UnityEngine;

namespace Diciembre
{
    public class Resource : MonoBehaviour
    {
        #region PUBLIC_FIELDS
        public Action<Resource> OnEmpty;
        public int resourceAmount;
        #endregion

        #region PUBLIC_METHODS
        public float TakeResource()
        {
            if (resourceAmount > 0)
            {
                if (resourceAmount - 1 <= 0)
                    DestroyResource();
                return 1;
            }
            else
            {
                DestroyResource();
                return 0;
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void DestroyResource() =>OnEmpty?.Invoke(this);
        #endregion
    }
}
