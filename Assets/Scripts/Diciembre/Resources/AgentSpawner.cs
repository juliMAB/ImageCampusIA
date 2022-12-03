using UnityEngine;
using UnityEngine.UI;

namespace Diciembre
{
    public class AgentSpawner : MonoBehaviour
    {
        #region EXPOSED_FIELD
        [SerializeField] private Button spawnAgentButton;

        [SerializeField] private GameObject agentPrefab;
        [SerializeField] private CentroUrbano townCenter;
        [SerializeField] private Transform agentConteiner;
        #endregion

        #region PUBLIC_METHODS
        public void SpawnAldeano()
        {
            GameObject agentGO = Instantiate(agentPrefab, townCenter.transform.position, Quaternion.identity, agentConteiner);
            Agent agent = agentGO.GetComponent<Agent>();
            townCenter.AddAgent(agent);
            agent.Init(townCenter);
        }
        private void Start()
        {
            spawnAgentButton.onClick.AddListener(SpawnAldeano);
        }
        #endregion
    }
}
