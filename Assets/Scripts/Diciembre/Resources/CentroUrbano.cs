using System.Collections.Generic;
using UnityEngine;

namespace Diciembre
{
    public class CentroUrbano : MonoBehaviourSingleton<CentroUrbano>
    {
        #region EXPOSED_FIELDS
        [SerializeField] private List<Agent> agents = null;
        [SerializeField] public int gold = 0;
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            agents = new List<Agent>();
        }
        #endregion

        #region PUBLIC_METHODS
        public void AddAgent(Agent agent) => agents.Add(agent);
        #endregion
    }
}
