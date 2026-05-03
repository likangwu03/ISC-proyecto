using System.Collections.Generic;
using UnityEngine;

public abstract class GAction : MonoBehaviour
{
    // Nombre de la acción
    public string actionName = "Action";
    // Coste de la acción
    public float cost = 1.0f;

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

    public int repeat = 0;

    protected ChatBubble chatBubble;

    protected virtual void Awake()
    {
        preconditions = new Dictionary<WorldStateDefinition, int>();
        effects = new Dictionary<WorldStateDefinition, int>();
        chatBubble = GetComponentInChildren<ChatBubble>(true);

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
    public bool IsAchievableGiven(Dictionary<WorldStateDefinition, int> conditions)
    {
        foreach (var (key, requiredValue) in preconditions)
        {

            if (conditions.ContainsKey(key))
            {
                if(requiredValue <= 0)
                {
                    if (conditions[key] > 0) return false;
                }
                else
                {
                    if (conditions[key] < requiredValue) return false;
                }
            }
            else
            {
                if(requiredValue > 0) return false; 
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