using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldStateDefinition : IEquatable<WorldStateDefinition>
{
    public string key;
    public bool isBelief;

    public override bool Equals(object obj)
    {
        return Equals(obj as WorldStateDefinition);
    }

    public bool Equals(WorldStateDefinition other)
    {
        if (other is null)
            return false;

        return key == other.key;
    }

    public override int GetHashCode()
    {
        return key?.GetHashCode() ?? 0;
    }
}

[System.Serializable]
public class WorldState
{
    public int value;
    public WorldStateDefinition def;
}


public class WorldStates
{
    private Dictionary<WorldStateDefinition, int> states;

    public WorldStates()
    {
        states = new Dictionary<WorldStateDefinition, int>();
    }

    // Comprobar si existe una clave
    public bool HasState(WorldStateDefinition key)
    {
        return states.ContainsKey(key);
    }

    // Añadir al diccionario
    private void AddState(WorldStateDefinition key, int value)
    {
        states.Add(key, value);
    }

    public void ModifyState(WorldStateDefinition key, int value)
    {
        if (HasState(key))
        {
            states[key] += value;
            if (states[key] <= 0)
            {
                RemoveState(key);
            }
        }
        else if(value > 0)
        {
            AddState(key, value);
        }
    }

    // Método para eliminar un estado
    public void RemoveState(WorldStateDefinition key)
    {
        if (HasState(key)) states.Remove(key);
    }

    // Establecer un estado
    public void SetState(WorldStateDefinition key, int value)
    {
        if (HasState(key)) states[key] = value;
        else AddState(key, value);
    }

    public Dictionary<WorldStateDefinition, int> GetStates()
    {
        return states;
    }

    public void Clear()
    {
        states.Clear();
    }
}