using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Diciembre
{
    public class UI : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Button spawnAgentButton;
        [SerializeField] private Button spawnResourceButton;
        [SerializeField] private Button AlertButton;
        [SerializeField] private Button EndAlert;
        #endregion

        #region PUBLIC_FIELDS
        public void Init(UnityAction actionOnSpawnAgent, UnityAction actionOnSpawnResource)
        {
            spawnAgentButton.onClick.AddListener(actionOnSpawnAgent);
            spawnResourceButton.onClick.AddListener(actionOnSpawnResource);
        }
        public void AddAgentToButton(Agent agent)
        {
            AlertButton.onClick.AddListener(agent.ForceAlert);
            EndAlert.onClick.AddListener(agent.ForceBackToWork);
        }
        #endregion
    }
}
