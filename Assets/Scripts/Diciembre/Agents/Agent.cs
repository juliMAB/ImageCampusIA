using FSM;
using UnityEngine;
using UnityEngine.Events;

namespace Diciembre
{
    
    public class Agent : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Flocking agentFlocking;

        [SerializeField] private float speed;
        [SerializeField] private int maxInventory = 5;

        [SerializeField] private TMPro.TextMeshProUGUI headText;
        [ReadOnly][SerializeField] private CentroUrbano home;
        [ReadOnly][SerializeField] private float currentTime;
        [SerializeField] private int amountInventory;


        [ReadOnly][SerializeField] private States currentState;
        [ReadOnly][SerializeField] private States LastState;
        [ReadOnly][SerializeField] private Flags lastFlag;
        [ReadOnly][SerializeField] private Flags FlagAfterAlert;
        #endregion

        #region PRIVATE_FIELDS
        private MineroPath minerPath;
        private FiniteStateMachine finiteStateMachine;
        private Resource resource;

        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            finiteStateMachine.Update(ref currentState, ref LastState);
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init(CentroUrbano centroUrbano)
        {
            home = centroUrbano;
            minerPath = new MineroPath(0.2f,this.transform,agentFlocking);
            agentFlocking.SetTarget(transform.position);
            SetFsm();

        }
        #endregion

        #region PRIVATE_METHODS
        private void SetFsm()
        {
            finiteStateMachine = new FiniteStateMachine();

            currentState = States.Idle;

            //------Relations-----

            finiteStateMachine.SetRelation(States.Idle, Flags.OnGoMine, States.GoingToMine);
            finiteStateMachine.SetRelation(States.GoingToMine, Flags.OnStartMine, States.Minig);
            finiteStateMachine.SetRelation(States.Minig, Flags.OnFullInventory, States.GoingToHome);
            finiteStateMachine.SetRelation(States.GoingToHome, Flags.OnReachHome, States.Depositing);
            finiteStateMachine.SetRelation(States.Depositing, Flags.OnClearInventory, States.Idle);
            finiteStateMachine.SetRelation(States.GoingToHome, Flags.OnReachHouseAlert, States.AlertInHome);

            for (int i = 0; i < (int)States._Count; i++)
                finiteStateMachine.SetRelation((States)i, Flags.OnIddle, States.Idle);
            for (int i = 0; i < (int)States._Count; i++)
                finiteStateMachine.SetRelation((States)i, Flags.OnAlert, States.GoingToHome);
            for (int i = 0; i < (int)States._Count; i++)
                for (int w = 0; w < (int)Flags._Count; w++)
                    finiteStateMachine.SetRelation(States.AlertInHome, (Flags)w, (States)i);


            //------Behaviours------

            finiteStateMachine.AddBehaviour(States.Idle, IddleBehaviour, () => { headText.text = "Iddle"; currentTime = 1; }, () => { });
            finiteStateMachine.AddBehaviour(States.GoingToMine, GoingToMineBehaviour, () => { headText.text = "GoingToMine"; }, () => { });
            finiteStateMachine.AddBehaviour(States.Minig, MiningBehaviour, () => { headText.text = "Mining"; }, () => { });
            finiteStateMachine.AddBehaviour(States.GoingToHome, GoingToHomeBehaviour, () => { headText.text = "GoingToHome"; }, () => { });
            finiteStateMachine.AddBehaviour(States.Depositing, DepositingBehaviour, () => { headText.text = "Depositing"; }, () => { });
            finiteStateMachine.AddBehaviour(States.AlertInHome, AlertInHomeBehaviour, () => { headText.text = "Alert"; }, () => { });
        }

        public void ForceAlert()
        {
            
            FlagAfterAlert = lastFlag;
            finiteStateMachine.SetFlag(ref currentState, Flags.OnAlert);
            minerPath.firstCall = true;
            lastFlag = Flags.OnAlert;
            return;
        }

        public void ForceBackToWork()
        {
            finiteStateMachine.SetFlag(ref currentState, FlagAfterAlert);
            return;
        }

        private void AlertInHomeBehaviour()
        {
            //holding ForceBackToWork.
        }

        private void MiningBehaviour()
        {
            if (!resource) // lose ref.
            {
                finiteStateMachine.SetFlag(ref currentState, Flags.OnFullInventory);
                return;
            }
            if (currentTime < 0)
            {
                currentTime = 1;
                amountInventory += resource.TakeResource();
                if (amountInventory >= maxInventory) // full inventory.
                {
                    finiteStateMachine.SetFlag(ref currentState, Flags.OnFullInventory);
                    lastFlag = Flags.OnForceFullInventory;
                }
                return;
            }
            else
            {
                currentTime -= Time.deltaTime; //stil mining.
                return;
            }
        }
        private void IddleBehaviour()
        {
            if (resource)
            {
                finiteStateMachine.SetFlag(ref currentState, Flags.OnGoMine);
                lastFlag = Flags.OnGoMine;
                return;
            }
            else
                resource = ResourceSpawner.GetCloserResource(transform.position);
        }
        private void GoingToMineBehaviour()
        {
            if (resource == null)
            {
                finiteStateMachine.SetFlag(ref currentState, Flags.OnIddle);
                lastFlag = Flags.OnIddle;
                minerPath.firstCall = true;
            }
            else
            {
                Vector3 localmine = resource.transform.position;
                minerPath.CallPath(speed, transform.position, localmine,
                () =>
                {
                    if (Vector3.Distance (localmine,transform.position)>1) //si es imposible llegar
                    {
                        finiteStateMachine.SetFlag(ref currentState, Flags.OnIddle);
                        lastFlag = Flags.OnIddle;
                        resource.DestroyResource();
                        return;
                    }
                    finiteStateMachine.SetFlag(ref currentState, Flags.OnStartMine);
                    lastFlag = Flags.OnStartMine;
                    transform.position = localmine;
                    return;
                });
            }
        }
        private void GoingToHomeBehaviour()
        {
            Vector3 home = new Vector3(this.home.transform.position.x, this.home.transform.position.y);

            minerPath.CallPath(speed, transform.position, home,
                () =>
                {
                    if (Flags.OnAlert == lastFlag)
                    {
                        finiteStateMachine.SetFlag(ref currentState, Flags.OnReachHouseAlert);
                        lastFlag = Flags.OnReachHouseAlert;
                        transform.position = home;
                        return;
                    }
                    if (amountInventory > 0)
                    {
                        finiteStateMachine.SetFlag(ref currentState, Flags.OnReachHome);
                        lastFlag = Flags.OnReachHome;
                        transform.position = home;
                        return;
                    }
                    finiteStateMachine.SetFlag(ref currentState, Flags.OnIddle);
                    lastFlag = Flags.OnIddle;
                    transform.position = home;
                    return;
                });
        }
        private void DepositingBehaviour()
        {
            Debug.Log("depo");
            home.gold += amountInventory;
            amountInventory = 0;
            finiteStateMachine.SetFlag(ref currentState, Flags.OnClearInventory);
            lastFlag = Flags.OnClearInventory;
        }

        #endregion
    }
}
