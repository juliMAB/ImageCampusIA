using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SC_FSM", menuName = "Fsm/AldeanoData")]
public class AldeanoBehaviour : ScriptableObject
{
    public States[,] relations;
    public Dictionary<States, State> behaviours;
    private void SetFsm()
    {
    }
}
