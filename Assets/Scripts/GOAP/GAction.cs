using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GAction : MonoBehaviour
{
    // Nombre de la acción
    public string actionName = "Action";
    // Coste de la acción
    public float cost = 1.0f;
    // Duración que debe tomar la acción
    public float duration = 0.0f;
    // Array de estados del mundo como precondiciones
    [SerializeField]
    protected WorldState[] preConditions;
    // Array de estados del mundo como efectos posteriores
    [SerializeField]
    protected WorldState[] afterEffects;
    // Diccionario de precondiciones
    public Dictionary<string, int> preconditions;
    // Diccionario de efectos
    public Dictionary<string, int> effects;

    // Acceso al inventario
    public GInventory inventory;
    // Estado del agente
    public WorldStates beliefs;
    // Si la acción está ejecutando actualmente
    public bool running = false;

    // Constructor
    public GAction()
    {
        preconditions = new Dictionary<string, int>();
        effects = new Dictionary<string, int>();
    }

    protected virtual void Awake()
    {
        if (preConditions != null)
        {
            foreach (WorldState w in preConditions)
            {
                preconditions.Add(w.key, w.value);
            }
        }

        if (afterEffects != null)
        {
            foreach (WorldState w in afterEffects)
            {
                effects.Add(w.key, w.value);
            }
        }

        inventory = GetComponent<GAgent>().inventory;
        beliefs = GetComponent<GAgent>().beliefs;
    }

    public bool IsAchievable()
    {
        return true;
    }

    // Comprueba si la acción es realizable dadas las condiciones del mundo y comparándolas con las precondiciones
    public bool IsAhievableGiven(Dictionary<string, int> conditions)
    {
        foreach (KeyValuePair<string, int> p in preconditions)
        {
            if (!(conditions.ContainsKey(p.Key) && conditions[p.Key] >= p.Value))
            {
                return false;
            }

        }
        return true;
    }

    public abstract bool PrePerform();
    public abstract void Perform();
    public virtual bool PostPerform()
    {
        ApplyEffect();
        return true;
    }

    public virtual bool IsDone()
    {
        return true;
    }

    public void ApplyEffect()
    {
        foreach (WorldState state in afterEffects)
        {
            if(state.isBelief)
            {
                beliefs.ModifyState(state.key, state.value);
            }
            else
            {
                GWorld.Instance.GetWorld().ModifyState(state.key, state.value);
            }
        }
    }

}