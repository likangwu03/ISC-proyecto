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
    public Dictionary<WorldStateDefinition, int> preconditions;
    // Diccionario de efectos
    public Dictionary<WorldStateDefinition, int> effects;

    // Acceso al inventario
    public GInventory inventory;
    // Estado del agente
    public WorldStates beliefs;
    // Si la acción está ejecutando actualmente
    public bool running = false;

    // Constructor
    public GAction()
    {
        preconditions = new Dictionary<WorldStateDefinition, int>();
        effects = new Dictionary<WorldStateDefinition, int>();
    }

    protected virtual void Awake()
    {
        if (preConditions != null)
        {
            foreach (WorldState w in preConditions)
            {
                preconditions.Add(w.def, w.value);
            }
        }

        if (afterEffects != null)
        {
            foreach (WorldState w in afterEffects)
            {
                effects.Add(w.def, w.value);
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
    public bool IsAhievableGiven(Dictionary<WorldStateDefinition, int> conditions)
    {
        foreach (KeyValuePair<WorldStateDefinition, int> p in preconditions)
        {

            if (conditions.ContainsKey(p.Key))
            {
                if(p.Value <= 0)
                {
                    if (conditions[p.Key] > 0) return false;
                }
                else
                {
                    if (conditions[p.Key] < p.Value) return false;
                }
            }
            else
            {
                if(p.Value > 0) {  return false; }
            }
        }
        return true;
    }

    public abstract bool PrePerform();
    public abstract void Perform();
    public virtual void Suspended() { }
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
            if(state.def.isBelief)
            {
                beliefs.ModifyState(state.def, state.value);
            }
            else
            {
                GWorld.Instance.GetWorld().ModifyState(state.def, state.value);
            }
        }
    }

}