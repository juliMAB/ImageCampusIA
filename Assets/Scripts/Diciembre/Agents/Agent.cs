using FSM;
using UnityEngine;

namespace Diciembre
{
    
    public class Agent : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private CentroUrbano home;

        [ReadOnly][SerializeField] private States currentState;
        [ReadOnly][SerializeField] private States LastState;
        [ReadOnly][SerializeField] private Flags lastFlag;
        #endregion

        #region PRIVATE_FIELDS
        private FiniteStateMachine finiteStateMachine;
        private Resource resource;

        #endregion

        #region PUBLIC_METHODS
        public void Init(CentroUrbano centroUrbano)
        {
            SetFsm();
            home = centroUrbano;
        }
        #endregion

        #region PRIVATE_METHODS
        private void SetFsm()
        {
            finiteStateMachine = new FiniteStateMachine();

            currentState = States.Idle;

            //------Relations-----

            // instruccion de ir a minar.
            finiteStateMachine.SetRelation(States.Idle, Flags.OnGoToMine, States.GoingToMine);

            //------Behaviours------

            //setea la accion por minar.
            finiteStateMachine.AddBehaviour(States.Minig, MiningBehaviour, () => { Debug.Log("Voy a ir a minar"); }, () => { Debug.Log("deje de minar"); });

            //-----Relations-Force------

            finiteStateMachine.SetRelation(States.Idle, Flags.ForceToPosition, States.ForceGoingToPosition);
        }
        private void MiningBehaviour()
        {
           
        }
        #endregion
    }
}
