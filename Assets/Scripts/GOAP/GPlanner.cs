using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Node parent;
    public float cost;
    public Dictionary<WorldStateDefinition, int> state;
    public GAction action;
    public Dictionary<Type, int> actionCounts;


    public Node(Node parent, float cost, Dictionary<WorldStateDefinition, int> allStates, GAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<WorldStateDefinition, int>(allStates);
        this.action = action;


        this.actionCounts = parent != null ? new Dictionary<Type, int>(parent.actionCounts) : new Dictionary<Type, int>();

        if (action != null)
        {
            Type actionType = action.GetType();

            if (!this.actionCounts.ContainsKey(actionType))
                this.actionCounts[actionType] = 0;

            this.actionCounts[actionType]++;
        }
    }

    public Node(Node parent, float cost, Dictionary<WorldStateDefinition, int> allStates, Dictionary<WorldStateDefinition, int> beliefStates, GAction action)
    {

        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<WorldStateDefinition, int>(allStates);
        foreach (var (key, value) in beliefStates)
        {
            this.state.Add(key, value);
        }

        this.action = action;


        this.actionCounts = parent != null ? new Dictionary<Type, int>(parent.actionCounts) : new Dictionary<Type, int>();

        if (action != null)
        {
            Type actionType = action.GetType();

            if (!this.actionCounts.ContainsKey(actionType))
                this.actionCounts[actionType] = 0;

            this.actionCounts[actionType]++;
        }
    }

    public bool CanUseAction(GAction actionToCheck)
    {
        Type actionType = actionToCheck.GetType();

        int usedCount = actionCounts.GetValueOrDefault(actionType, 0);

        return usedCount <= actionToCheck.repeat;
    }

    public int CompareTo(Node other) => cost.CompareTo(other.cost);

}

public class GPlanner
{
    public Queue<GAction> Plan(List<GAction> actions, Dictionary<WorldStateDefinition, int> goal, WorldStates beliefStates)
    {
        Queue<GAction> queue = new();

        Node startNode = new Node(null, 0f, GWorld.Instance.GetWorld().GetStates(), beliefStates.GetStates(), null);

        if (GoalAchieved(goal, startNode.state))
        {
            return queue;
        }

        MinHeap<Node> openSet = new();
        Dictionary<string, float> visited = new();

        openSet.Push(startNode);
        Node cheapestGoal = null;

        // Djkistra
        while (openSet.Count > 0)
        {
            Node current = openSet.Pop();

            // Se ha llegado al nodo objetivo porque el coste del nuevo camino es mayor que el ya existente
            if (cheapestGoal != null && current.cost >= cheapestGoal.cost)
                break;

            string stateHash = HashState(current.state);
            if (!(visited.ContainsKey(stateHash) && visited[stateHash] <= current.cost))
            {
                visited[stateHash] = current.cost;

                foreach (GAction action in actions)
                {
                    if (!action.IsAchievableGiven(current.state)) continue;
                    if (!current.CanUseAction(action)) continue;

                    var nextState = ApplyEffects(current.state, action);

                    var child = new Node(current, current.cost + action.cost, nextState, action);

                    if (GoalAchieved(goal, nextState))
                    {
                        // Guardamos el más barato que llegue al objetivo
                        if (cheapestGoal == null || child.cost < cheapestGoal.cost)
                            cheapestGoal = child;
                    }
                    else
                    {
                        openSet.Push(child);
                    }
                }
            }
        }

        if (cheapestGoal == null)
        {
            Debug.Log("NO PLAN");
            return null;
        }

        List<GAction> result = new();
        for (Node n = cheapestGoal; n != null; n = n.parent)
            if (n.action != null)
                result.Insert(0, n.action);

        foreach (var a in result)
        {
            queue.Enqueue(a);
        }
        return queue;
    }

    private Dictionary<WorldStateDefinition, int> ApplyEffects(Dictionary<WorldStateDefinition, int> currentState, GAction action)
    {
        var nextState = new Dictionary<WorldStateDefinition, int>(currentState);
        foreach (var eff in action.effects)
        {
            if (!nextState.ContainsKey(eff.Key))
                nextState.Add(eff.Key, eff.Value);
            else
                nextState[eff.Key] += eff.Value;
        }
        return nextState;
    }


    private string HashState(Dictionary<WorldStateDefinition, int> state)
    {
        var sorted = new SortedDictionary<string, int>();

        foreach (var kv in state)
        {
            if (kv.Value >= 0)
                sorted[kv.Key.key] = kv.Value;
        }

        return string.Join("|", sorted.Select(kv => $"{kv.Key}:{kv.Value}"));
    }

    public bool GoalAchieved(Dictionary<WorldStateDefinition, int> goal, Dictionary<WorldStateDefinition, int> state)
    {
        foreach (var g in goal)
        {
            if (!state.TryGetValue(g.Key, out int stateValue))
            {
                if (g.Value > 0) return false;
                continue;
            }

            if (g.Value > 0 && stateValue < g.Value) return false;
        }
        return true;
    }
}