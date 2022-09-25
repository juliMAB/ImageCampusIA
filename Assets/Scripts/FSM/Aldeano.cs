using System;
using UnityEngine;
[Serializable]
public class EntityStats
{
    public float speed;
    public float minigSpeed;
    [ReadOnly] public float goldAmount;
    public float maxAmount;
    public float tiempohastaaburrirse;
    public float timepoHastaDescanso;
}
namespace FSM
{
    public class Aldeano : MonoBehaviour
    {
        [ReadOnly][SerializeField] public int ID;
        [ReadOnly][SerializeField] private static int Cuantity;

        [SerializeField] private EntityStats stats = new EntityStats();
        [ReadOnly][SerializeField] private CentroUrbano m_home;
        [ReadOnly][SerializeField] private States currentState;
        [ReadOnly][SerializeField] private States LastState;
        [ReadOnly][SerializeField] private Flags lastFlag;
        [SerializeField] private AldeanoData nombre;

        private Mine mine;
        private FiniteStateMachine finiteStateMachine;
        private float currentActionTime;

        public void Init(CentroUrbano centroUrbano)
        {
            SetFsm();
            m_home = centroUrbano;
            nombre.Init(Cuantity);
            Cuantity++;
        }

        private void Update()
        {
            finiteStateMachine.Update(ref currentState, ref LastState);
        }

        private void SetFsm()
        {
            finiteStateMachine = new FiniteStateMachine();

            currentState = States.Idle;

            //------Relations-----

            // instruccion de ir a minar.
            finiteStateMachine.SetRelation(States.Idle, Flags.OnGoToMine, States.GoingToMine);

            //ir a minar, llegar a minar.
            finiteStateMachine.SetRelation(States.GoingToMine, Flags.OnReachResource, States.Minig);

            //minando, irese a casa.
            finiteStateMachine.SetRelation(States.Minig, Flags.OnFullInventory, States.GoingToHome);

            //va a casa, deposita.
            finiteStateMachine.SetRelation(States.GoingToHome, Flags.OnReachWithResource, States.Depositing);

            //deposita, termina de depositar -> idle.
            finiteStateMachine.SetRelation(States.Depositing, Flags.OnEmptyInventory, States.Idle);

            //------Behaviours------

            //setea la accion por minar.
            finiteStateMachine.AddBehaviour(States.Minig, MiningBehaviour, () => { Debug.Log("Voy a ir a minar"); }, () => { Debug.Log("deje de minar"); });

            //setea la accion por ir a mina.
            finiteStateMachine.AddBehaviour(States.GoingToMine, GoingToMineBehaviour);

            //setea la accion por ir a casa.
            finiteStateMachine.AddBehaviour(States.GoingToHome, GoingToHomeBehaviour);

            //setear la accion al depositar.
            finiteStateMachine.AddBehaviour(States.Depositing, DepositingBehaviour);

            //setear la accion al iddle.
            finiteStateMachine.AddBehaviour(States.Idle, IddleBehaviour, () => { Debug.Log("estoy Idle"); }, () => { Debug.Log("ya no estoy Idle"); });

            //setear la accion al descansar.
            finiteStateMachine.AddBehaviour(States.Resting, RestingBehaviour, () => { Debug.Log("Comenzo el descanso"); }, () => { Debug.Log("termino el descanso"); });

            finiteStateMachine.AddBehaviour(States.ForceGoingToHome, ForceGoingToHomeBehaviour, () => { Debug.Log("Me vuelvo corriendo a casa"); }, () => { Debug.Log("ta bien"); });
            //-----Relations-Force------

            finiteStateMachine.SetRelation(States.Idle, Flags.ForceToPosition, States.ForceGoingToPosition);
            finiteStateMachine.SetRelation(States.GoingToMine, Flags.ForceToPosition, States.ForceGoingToPosition);
            finiteStateMachine.SetRelation(States.GoingToHome, Flags.ForceToPosition, States.ForceGoingToPosition);
            finiteStateMachine.SetRelation(States.Depositing, Flags.ForceToPosition, States.ForceGoingToPosition);
            finiteStateMachine.SetRelation(States.GoingToMine, Flags.ForceToIdle, States.Idle);

            //va de cualquier estado a descansar (en Tired).
            for (int i = 0; i < (int)States._Count; i++)
            {
                finiteStateMachine.SetRelation((States)i, Flags.OnTired, States.Resting);
            }
            //va de cualquier estado a iddle (en forceidle).
            for (int i = 0; i < (int)States._Count; i++)
            {
                finiteStateMachine.SetRelation((States)i, Flags.ForceToIdle, States.Idle);
            }
            //va de cualquier estado a ir a casa (en alerta).
            for (int i = 0; i < (int)States._Count; i++)
            {
                finiteStateMachine.SetRelation((States)i, Flags.OnAlert, States.ForceGoingToHome);
            }

            finiteStateMachine.SetRelation(States.ForceGoingToHome, Flags.OnGoToMine, States.GoingToMine);


            finiteStateMachine.SetRelation(States.Resting, Flags.OnFullInventory, States.GoingToHome);
            finiteStateMachine.SetRelation(States.Resting, Flags.OnGoToMine, States.GoingToMine);
            finiteStateMachine.SetRelation(States.Resting, Flags.OnReachResource, States.Minig);
            finiteStateMachine.SetRelation(States.Resting, Flags.OnEmptyInventory, States.GoingToHome);
        }

        public void ForceGoingToHomeBehaviour()
        {
            Vector3 home = new Vector3(m_home.transform.position.x, m_home.transform.position.y);
            Vector3 dir = (home - transform.position);
            dir.Normalize();

            Vector3 movement = dir * stats.speed * Time.deltaTime;
            transform.position += new Vector3(movement.x, movement.y, 0);
            if (Vector3.Distance(home, transform.position) < 0.5f)
            {
                if (stats.goldAmount > 0)
                {
                    finiteStateMachine.SetFlag(ref currentState, lastFlag);
                    return;
                }
                finiteStateMachine.SetFlag(ref currentState, lastFlag);
            }
        }
        public void ForceToStop()
        {
            finiteStateMachine.SetRelation(States.Idle, lastFlag, States.GoingToMine);
        }
        public void ForceToWork()
        {
            finiteStateMachine.SetFlag(ref currentState, Flags.OnGoToMine);
            lastFlag = Flags.OnGoToMine;
        }

        public void RestingBehaviour()
        {
            if (currentActionTime < stats.timepoHastaDescanso)
            {
                currentActionTime += Time.deltaTime;
            }
            else
            {
                currentActionTime = 0;
                finiteStateMachine.SetFlag(ref currentState, lastFlag);
            }
        }

        public void IddleBehaviour()
        {
            if (mine)
            {
                finiteStateMachine.SetFlag(ref currentState, Flags.OnGoToMine);
                lastFlag = Flags.OnGoToMine;
                return;
            }
            if (currentActionTime < stats.tiempohastaaburrirse)
            {
                currentActionTime += Time.deltaTime;
            }
            else
            {
                currentActionTime = 0;
                finiteStateMachine.SetFlag(ref currentState, Flags.OnGoToMine);
                lastFlag = Flags.OnGoToMine;
            }
        }

        private void GoingToHomeBehaviour()
        {
            Vector3 home = new Vector3(m_home.transform.position.x, m_home.transform.position.y);
            Vector3 dir = (home - transform.position);
            dir.Normalize();

            Vector3 movement = dir * stats.speed * Time.deltaTime;
            transform.position += new Vector3(movement.x, movement.y, 0);
            if (Vector3.Distance(home, transform.position) < 0.5f)
            {
                if (stats.goldAmount > 0)
                {
                    finiteStateMachine.SetFlag(ref currentState, Flags.OnReachWithResource);
                    lastFlag = Flags.OnReachWithResource;
                    return;
                }
                finiteStateMachine.SetFlag(ref currentState, Flags.ForceToIdle);
                lastFlag = Flags.ForceToIdle;
            }
        }

        private void GoingToMineBehaviour()
        {
            if (mine == null)
            {
                mine = m_home.GetAnyMina();
                if (mine == null)
                {
                    finiteStateMachine.SetFlag(ref currentState, Flags.ForceToIdle);
                    lastFlag = Flags.ForceToIdle;
                }
            }
            else
            {
                Vector3 localmine = mine.transform.position;
                if (Vector3.Distance(localmine, transform.position) < 0.5f)
                {
                    finiteStateMachine.SetFlag(ref currentState, Flags.OnReachResource);
                    lastFlag = Flags.OnReachResource;
                    return;
                }
                else
                {
                    Vector3 dir = (localmine - transform.position);
                    dir.Normalize();

                    Vector3 movement = dir * stats.speed * Time.deltaTime;
                    transform.position += new Vector3(movement.x, movement.y, 0);
                }
            }
        }

        private void MiningBehaviour()
        {
            if (!mine)
                finiteStateMachine.SetFlag(ref currentState, Flags.OnFullInventory);
            if (currentActionTime < stats.minigSpeed)
            {
                currentActionTime += Time.deltaTime;
                Debug.Log("minando...");
            }
            else
            {
                currentActionTime = 0;

                if (mine)
                {
                    float v = mine.TakeResource(stats);
                    if (v == 0)
                    {
                        finiteStateMachine.SetFlag(ref currentState, Flags.OnFullInventory);
                        lastFlag = Flags.OnFullInventory;
                    }
                    stats.goldAmount += v;
                }

                if (stats.goldAmount >= stats.maxAmount)
                {
                    stats.goldAmount = stats.maxAmount;
                    finiteStateMachine.SetFlag(ref currentState, Flags.OnFullInventory);
                    lastFlag = Flags.OnFullInventory;
                }
            }
        }

        private void DepositingBehaviour()
        {
            m_home.Gold += stats.goldAmount;
            stats.goldAmount = 0;
            finiteStateMachine.SetFlag(ref currentState, Flags.OnEmptyInventory);
            lastFlag = Flags.OnEmptyInventory;
            Debug.Log("se deposito y tengo inventario vacio");
        }

        public void SetFlag(Flags flag)
        {
            finiteStateMachine.SetFlag(ref currentState, flag);
        }


    }
}