using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [SerializeField] private Button spawnAgentButton;
    [SerializeField] private Button spawnResourceButton;
    #endregion

    #region PUBLIC_FIELDS
    public void Init(UnityAction actionOnSpawnAgent, UnityAction actionOnSpawnResource)
    {
        spawnAgentButton.onClick.AddListener(actionOnSpawnAgent);
        spawnResourceButton.onClick.AddListener(actionOnSpawnResource);
    }
    #endregion
}
