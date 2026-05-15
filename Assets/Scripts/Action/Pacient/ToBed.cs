using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToBed : MoveAction
{
    private GameObject bed;
    [SerializeField]
    private WorldStateDefinition internalRoomPatients;
    [SerializeField]
    private WorldStateDefinition toBed;
    public override bool PrePerform()
    {
        bed = GameManager.Instance.GetBed();
        if (target == null) target = bed;
        GWorld.Instance.GetWorld().ModifyState(internalRoomPatients, -1);
        GameManager.Instance.AddObservation(gameObject);
        return true;
    }

    public override bool IsDone()
    {
        return !beliefs.HasState(toBed) || beliefs.GetStates()[toBed]<=0;
    }

    public override bool PostPerform()
    {
        GameManager.Instance.AddBed(bed);
        return true;
    }
}
