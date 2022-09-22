using System;
using Unity.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
[Serializable]
public class EntityStats
{
    public float speed;
    public float minigSpeed;
    [ReadOnly] public float goldAmount;
    public float maxAmount;
    public float tiempohastaaburrirse;
}


public class Aldeano : MonoBehaviour
{
    [SerializeField] private EntityStats stats = new EntityStats();
    [ReadOnly][SerializeField] private CentroUrbano m_home;
    [ReadOnly][SerializeField] private States currentState;
    [ReadOnly][SerializeField] private States LastState;

    private Mine mine;
    private FiniteStateMachine finiteStateMachine;
    private float currentActionTime;

    public void Init(CentroUrbano centroUrbano)
    {
        SetFsm();
        m_home = centroUrbano;
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
        finiteStateMachine.AddBehaviour(States.Minig, MiningBehaviour,()=> { Debug.Log("Voy a ir a minar"); }, () => { Debug.Log("deje de minar"); });

        //setea la accion por ir a mina.
        finiteStateMachine.AddBehaviour(States.GoingToMine, GoingToMineBehaviour);

        //setea la accion por ir a casa.
        finiteStateMachine.AddBehaviour(States.GoingToHome, GoingToHomeBehaviour);

        //setear la accion al depositar.
        finiteStateMachine.AddBehaviour(States.Depositing, DepositingBehaviour);

        //setear la accion al iddle.
        finiteStateMachine.AddBehaviour(States.Idle, IddleBehaviour , () => { Debug.Log("estoy Idle"); }, () => { Debug.Log("ya no estoy Idle"); });
       
        //-----Relations-Force------
       
       finiteStateMachine.SetRelation(States.Idle, Flags.ForceToPosition, States.ForceGoingToPosition);
       finiteStateMachine.SetRelation(States.GoingToMine, Flags.ForceToPosition, States.ForceGoingToPosition);
       finiteStateMachine.SetRelation(States.GoingToHome, Flags.ForceToPosition, States.ForceGoingToPosition);
       finiteStateMachine.SetRelation(States.Depositing, Flags.ForceToPosition, States.ForceGoingToPosition);
        finiteStateMachine.SetRelation(States.GoingToMine, Flags.ForceToIdle, States.Idle);

        //-----Behaviours-Forced---

        //finiteStateMachine.SetFlag(ref currentState, Flags.ForceToIdle);
    }
    public void ForceToStop()
    {
        finiteStateMachine.SetRelation(States.Idle, Flags.OnGoToMine, States.GoingToMine);
    }
    public void ForceToWork()
    {
        finiteStateMachine.SetFlag(ref currentState, Flags.OnGoToMine);
    }

    public void IddleBehaviour()
    {
        if (mine)
        {
            finiteStateMachine.SetFlag(ref currentState, Flags.OnGoToMine);
            return;
        }
        if (currentActionTime < stats.tiempohastaaburrirse)
        {
            currentActionTime += Time.deltaTime;
            Debug.Log("porque existo sin proposito?");
        }
        else
        {
            currentActionTime = 0;
            finiteStateMachine.SetFlag(ref currentState, Flags.OnGoToMine);
        }
    }

    private void GoingToHomeBehaviour()
    {
        Vector3 home = new Vector3(m_home.transform.position.x, m_home.transform.position.y);
        Vector3 dir = (home - transform.position);
        dir.Normalize();

        Vector3 movement = dir * stats.speed  * Time.deltaTime;
        transform.position += new Vector3(movement.x, movement.y,0);
        if (Vector3.Distance(home,transform.position) <0.5f)
        {
            if (stats.goldAmount>0)
            {
                finiteStateMachine.SetFlag(ref currentState, Flags.OnReachWithResource);
                return;
            }
            finiteStateMachine.SetFlag(ref currentState, Flags.ForceToIdle);
        }
    }

    private void GoingToMineBehaviour()
    {
        if (mine==null)
        {
            mine = m_home.GetAnyMina();
            if (mine==null)
            {
                finiteStateMachine.SetFlag(ref currentState, Flags.ForceToIdle);
            }
        }
        else
        {
            Vector3 localmine = mine.transform.position;
            if (Vector3.Distance(localmine, transform.position) < 0.5f)
            {
                finiteStateMachine.SetFlag(ref currentState, Flags.OnReachResource);
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
                }
                stats.goldAmount += v;
            }

            if(stats.goldAmount >= stats.maxAmount)
            {
                stats.goldAmount = stats.maxAmount;
                finiteStateMachine.SetFlag(ref currentState, Flags.OnFullInventory);
            }
        }
    }

    private void DepositingBehaviour()
    {
        m_home.Gold += stats.goldAmount;
        stats.goldAmount = 0;
          finiteStateMachine.SetFlag(ref currentState, Flags.OnEmptyInventory);
        Debug.Log("se deposito y tengo inventario vacio");
    }

    public void SetFlag(Flags flag)
    {
        finiteStateMachine.SetFlag(ref currentState, flag);
    }


}
