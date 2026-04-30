using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubGoal{

    public Dictionary<WorldStateDefinition, int> sGoals;
    // -1 = infinito
    public int repeat;

    public SubGoal(WorldStateDefinition s, int i,int repeat = 0) {

        sGoals = new Dictionary<WorldStateDefinition, int>
        {
            { s, i }
        };
        this.repeat = repeat;
    }

    public SubGoal(int repeat = 0)
    {
        sGoals = new Dictionary<WorldStateDefinition, int>();
        this.repeat = repeat;
    }
}


public class GAgent : MonoBehaviour
{
    public List<GAction> actions = new();
    public Dictionary<SubGoal, int> goals = new();

    public GInventory inventory = new();
    public WorldStates beliefs = new();

    private GPlanner planner;

    private Queue<GAction> actionQueue;
    public GAction currentAction;

    private SubGoal currentGoal;

    private bool invoked = false;
    private bool needsReplan = false;


    public void Awake()
    {
        planner = new GPlanner();
    }

    public void Start()
    {
        actions.AddRange(GetComponents<GAction>());
    }

    public void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
    }

    void HandleCurrentAction()
    {
        if (currentAction == null || !currentAction.running)
            return;

        if (!currentAction.IsDone())
            return;

        CompleteAction();
    }


    void LateUpdate()
    {
        HandleCurrentAction();

        if (currentAction == null)
            Replan();

        if (currentAction != null && currentAction.running)
            return;

        if (actionQueue == null || actionQueue.Count == 0)
        {
            FinishGoal();
            return;
        }

        ExecuteNextAction();
    }


    void Replan()
    {
        needsReplan = false;
        actionQueue = null;
        currentGoal = null;

        var sortedGoals = goals.OrderByDescending(g => g.Value);

        foreach (var sg in sortedGoals)
        {
            Queue<GAction> plan = planner.Plan(actions, sg.Key.sGoals, beliefs);

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