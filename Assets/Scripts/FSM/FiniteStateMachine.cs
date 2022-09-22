using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum States
{
    Undefined = -1,
    Idle,
    Resting,
    GoingToMine,
    Minig,
    Depositing,
    GoingToHome,

    ForceGoingToPosition,
    ForceGoingToIdle,
    ForceGoingToHome,

    _Count
}

public enum Flags
{
    Undefined = -1,
    OnFullInventory,
    OnReachResource,
    OnReachWithResource,
    OnGoToMine,
    OnEmptyInventory,
    OnTired,
    OnAlert,

    ForceToWork,
    ForceToPosition,
    ForceToHome,
    ForceToIdle,

    _Count
}

public class State
{
    public Action OnEntryBehaviour;
    public List<Action> behaviours;
    public Action OnExitBehaviour;
}

public class FiniteStateMachine
{
    private States[,] relations;
    private Dictionary<States, State> behaviours;

    public void ResetRelations()
    {
        relations = new States[(int)States._Count, (int)Flags._Count];
        for (int i = 0; i < (int)States._Count; i++)
            for (int j = 0; j < (int)Flags._Count; j++)
                relations[i, j] = States.Undefined;

        behaviours = new Dictionary<States, State>();
    }

    public FiniteStateMachine()
    {
        ResetRelations();
    }

    public void SetRelation(States sourceState, Flags flag, States destinationState)
    {
        if (relations[(int)sourceState, (int)flag] == States.Undefined)
        relations[(int)sourceState, (int)flag] = destinationState;
    }

    public void SetFlag(ref States currentState, Flags flag)
    {
        if (relations[(int)currentState, (int)flag] != States.Undefined)
            currentState = relations[(int)currentState, (int)flag];
        else
        Debug.Log(Enum.GetName(typeof(States),currentState) + " + " + Enum.GetName(typeof(Flags), flag) + " no tienen relacion");
    }

    public void SetBehaviour(States state, Action behaviour, Action onEntryBehaviour = null, Action onExitBehaviour = null)
    {
        State newState = new State();
        newState.behaviours = new List<Action>();
        newState.behaviours.Add(behaviour);
        newState.OnEntryBehaviour = onEntryBehaviour;
        newState.OnExitBehaviour = onExitBehaviour;

        if (behaviours.ContainsKey(state))
            behaviours[state] = newState;
        else
            behaviours.Add(state, newState);
    }

    public void AddBehaviour(States state, Action behaviour, Action onEntryBehaviour = null, Action onExitBehaviour = null)
    {
        if (behaviours.ContainsKey(state))
        {
            if (!behaviours[state].behaviours.Any(p => p == behaviour)) // no meter el action 2 veces.
            {
                behaviours[state].behaviours.Add(behaviour);

            }

        }
        else
        {
            State newState = new State();
            newState.behaviours = new List<Action>();
            newState.behaviours.Add(behaviour);
            newState.OnEntryBehaviour = onEntryBehaviour;
            newState.OnExitBehaviour = onExitBehaviour;
            behaviours.Add(state, newState);
        }
    }

    public void Update(ref States currentState, ref States lastState)
    {
        if(lastState != currentState)
        {
            if (behaviours.ContainsKey(lastState))
            {
                Action OnExit = behaviours[lastState].OnExitBehaviour;
                if (OnExit != null)
                {
                    OnExit?.Invoke();
                }
            }
            lastState = currentState;
            if (behaviours.ContainsKey(currentState))
            {
                Action OnEntry = behaviours[currentState].OnEntryBehaviour;
                if (OnEntry!=null)
                {
                    OnEntry?.Invoke();
                }
            }
        }
        if (behaviours.ContainsKey(currentState))
        {
            List<Action> actions = behaviours[currentState].behaviours;
            if (actions != null)
                for (int i = 0; i < actions.Count; i++)
                    if (actions[i] != null)
                        actions[i].Invoke();
        }
    }
}
