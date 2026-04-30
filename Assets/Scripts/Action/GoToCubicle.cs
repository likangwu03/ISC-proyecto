
using System;
using UnityEngine;

public class GoToCubicle : MoveAction
{
    [SerializeField]
    private WorldStateDefinition doctor;
    [SerializeField]
    private WorldStateDefinition treatingPatient;
    [SerializeField]
    private WorldStateDefinition isTreated;

    public override bool PrePerform()
    {
        target = GameManager.Instance.GetDoctor();
        GWorld.Instance.GetWorld().ModifyState(doctor, -1);

        inventory.AddItem(target);
        if (target == null)
            return false;
        return true;
    }

    public override bool PostPerform()
    {

        GWorld.Instance.GetWorld().ModifyState(treatingPatient, 1);

        GameManager.Instance.AddDoctor(target);
        GWorld.Instance.GetWorld().ModifyState(doctor, 1);

        inventory.RemoveItem(target);

        beliefs.ModifyState(isTreated, 1);

        return true;
    }
}
