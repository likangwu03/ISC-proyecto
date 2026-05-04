using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ReceiveTreatment : MoveAction
{
    private GameObject spot;
    [SerializeField]
    private WorldStateDefinition isTreated;

    public override bool PrePerform()
    {
        spot = GameManager.Instance.GetWaitingSpot();
        if (target == null) target = spot;
        return true;
    }

    public override bool IsDone()
    {
        return beliefs.HasState(isTreated);
    }


    public override void Perform()
    {
        MoveToTarget();
    }


    public override bool PostPerform()
    {
        GameManager.Instance.AddWaitingSpot(spot);
        return true;
    }
}