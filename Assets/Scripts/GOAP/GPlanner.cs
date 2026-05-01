using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : IComparable<Node> { 
    public Node parent;
    public float cost;
    public Dictionary<WorldStateDefinition, int> state;
    public GAction action;
    public Dictionary<Type, int> actionCounts;


    public Node(Node parent, float cost, Dictionary<WorldStateDefinition, int> allStates, GAction action) {
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

    public Node(Node parent, float cost, Dictionary<WorldStateDefinition, int> allStates, Dictionary<WorldStateDefinition, int> beliefStates, GAction action) {

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

public class MinHeap<T> where T : IComparable<T>
{
    private readonly List<T> _data = new List<T>();

    public int Count => _data.Count;

    public void Push(T item)
    {
        _data.Add(item);
        BubbleUp(_data.Count - 1);
    }

    public T Pop()
    {
        T top = _data[0];
        int last = _data.Count - 1;
        _data[0] = _data[last];
        _data.RemoveAt(last);
        if (_data.Count > 0) SiftDown(0);
        return top;
    }

    private void BubbleUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (_data[i].CompareTo(_data[parent]) >= 0) break;
            (_data[i], _data[parent]) = (_data[parent], _data[i]);
            i = parent;
        }
    }

    private void SiftDown(int i)
    {
        int n = _data.Count;
        while (true)
        {
            int smallest = i, l = 2 * i + 1, r = 2 * i + 2;
            if (l < n && _data[l].CompareTo(_data[smallest]) < 0) smallest = l;
            if (r < n && _data[r].CompareTo(_data[smallest]) < 0) smallest = r;
            if (smallest == i) break;
            (_data[i], _data[smallest]) = (_data[smallest], _data[i]);
            i = smallest;
        }
    }
}


public class GPlanner
{
    public Queue<GAction> Plan(List<GAction> actions, Dictionary<WorldStateDefinition, int> goal, WorldStates beliefStates)
    {

        Node startNode = new Node(null, 0f, GWorld.Instance.GetWorld().GetStates(), beliefStates.GetStates(), null);

        MinHeap<Node> openSet  = new();
        HashSet<string> visited = new();

        openSet.Push(startNode);
        Node cheapestGoal = null;

        while (openSet.Count > 0)
        {
            Node current = openSet.Pop();

            // encontrrado
            if (cheapestGoal != null && current.cost >= cheapestGoal.cost)
                break;

            string stateHash = HashState(current.state);
            if (visited.Contains(stateHash))
                continue;
            visited.Add(stateHash);

            foreach (GAction action in actions)
            {
                if (!action.IsAhievableGiven(current.state)) continue;
                if (!current.CanUseAction(action)) continue;

                var nextState = new Dictionary<WorldStateDefinition, int>(current.state);
                foreach (var eff in action.effects)
                {
                    if (!nextState.ContainsKey(eff.Key))
                        nextState.Add(eff.Key, eff.Value);
                    else
                        nextState[eff.Key] += eff.Value;
                }

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

        if (cheapestGoal == null)
        {
            Debug.Log("NO PLAN");
            return null;
        }

        List<GAction> result = new();
        for (Node n = cheapestGoal; n != null; n = n.parent)
            if (n.action != null)
                result.Insert(0, n.action);

        Queue<GAction> queue = new();
        foreach (var a in result)
        {
            queue.Enqueue(a);
        }

        return queue;
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

    private bool GoalAchieved(Dictionary<WorldStateDefinition, int> goal, Dictionary<WorldStateDefinition, int> state)
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