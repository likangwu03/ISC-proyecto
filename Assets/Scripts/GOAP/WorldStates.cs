using System.Collections.Generic;

[System.Serializable]
public class WorldState
{
    public string key;
    public int value;
    public bool isBelief;
}

public class WorldStates
{
    private Dictionary<string, int> states;

    public WorldStates()
    {
        states = new Dictionary<string, int>();
    }

    // Comprobar si existe una clave
    public bool HasState(string key)
    {
        return states.ContainsKey(key);
    }

    // Añadir al diccionario
    private void AddState(string key, int value)
    {
        states.Add(key, value);
    }

    public void ModifyState(string key, int value)
    {
        if (HasState(key))
        {
            states[key] += value;
            // Si es menor o igual a cero, eliminarlo
            if (states[key] <= 0)
            {
                RemoveState(key);
            }
        }
        else
        {
            AddState(key, value);
        }
    }

    // Método para eliminar un estado
    public void RemoveState(string key)
    {
        if (HasState(key)) states.Remove(key);
    }

    // Establecer un estado
    public void SetState(string key, int value)
    {
        if (HasState(key)) states[key] = value;
        else AddState(key, value);
    }

    public Dictionary<string, int> GetStates()
    {
        return states;
    }

    public void Clear()
    {
        states.Clear();
    }
}