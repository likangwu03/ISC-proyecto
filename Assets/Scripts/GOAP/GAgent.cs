using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubGoal{

    public Dictionary<string, int> sGoals;
    // -1 = infinito
    public int repeat;

    public SubGoal(string s, int i,int repeat = 0) {

        sGoals = new Dictionary<string, int>
        {
            { s, i }
        };
        this.repeat = repeat;
    }

    public SubGoal(int repeat = 0)
    {
        sGoals = new Dictionary<string, int>();
        this.repeat = repeat;
    }
}


public class GAgent : MonoBehaviour
{
    // Almacena nuestra lista de acciones
    public List<GAction> actions = new List<GAction>();
    // Diccionario de subobjetivos

    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    // Inventario
    public GInventory inventory = new GInventory();
    // Creencias
    public WorldStates beliefs = new WorldStates();

    // Acceso al planificador
    GPlanner planner;
    // Cola de acciones
    Queue<GAction> actionQueue;
    // Acción actual
    public GAction currentAction;
    // Subobjetivo actual
    SubGoal currentGoal;
    bool invoked = false;

    public void Awake()
    {
        planner = new GPlanner();
    }

    public void Start()
    {
        GAction[] acts = GetComponents<GAction>();
        foreach (GAction a in acts)
        {
            actions.Add(a);
        }
    }

    public void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
    }


    void LateUpdate()
    {
        HandleCurrentAction();

        if (currentAction != null && currentAction.running)
            return;

        if (actionQueue == null)
            TryCreatePlan();

        if (actionQueue == null)
            return;

        if (actionQueue.Count == 0)
        {
            FinishGoal();
            return;
        }

        ExecuteNextAction();
    }

    void HandleCurrentAction()
    {
        if (currentAction == null || !currentAction.running)
            return;

        if (!currentAction.IsDone())
            return;

        if (!invoked)
        {
            Invoke(nameof(CompleteAction), currentAction.duration);
            invoked = true;
        }
    }

    void TryCreatePlan()
    {
        var sortedGoals = goals.OrderByDescending(g => g.Value);

        foreach (var sg in sortedGoals)
        {
            var plan = planner.plan(actions, sg.Key.sGoals, beliefs);

            if (plan == null)
                continue;

            actionQueue = plan;
            currentGoal = sg.Key;
            break;
        }
    }

    void FinishGoal()
    {
        actionQueue = null;

        if (currentGoal.repeat == -1)
            return;

        currentGoal.repeat--;

        if (currentGoal.repeat < 0)
            goals.Remove(currentGoal);
    }

    void ExecuteNextAction()
    {
        currentAction = actionQueue.Dequeue();

        if (!currentAction.PrePerform())
        {
            actionQueue = null;
            return;
        }

        currentAction.running = true;
        currentAction.Perform();
    }

}