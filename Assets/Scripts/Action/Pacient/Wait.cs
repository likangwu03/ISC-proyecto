using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Wait : MoveAction
{
    private GameObject spot;
    [SerializeField]
    private WorldStateDefinition NumBeds;

    public override bool PrePerform()
    {
        spot = GameManager.Instance.GetWaitingSpot();
        if (target == null) target = spot;
        return true;
    }

    public override bool IsDone()
    {
        Dictionary<WorldStateDefinition, int> w = GWorld.Instance.GetWorld().GetStates();
        return w.ContainsKey(NumBeds) && w[NumBeds] > 0;
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