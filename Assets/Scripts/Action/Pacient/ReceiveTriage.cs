using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ReceiveTriage : MoveAction
{
    private GameObject spot;
    [SerializeField]
    private WorldStateDefinition receiveTriage;

    public override bool PrePerform()
    {
        spot = GameManager.Instance.GetWaitingSpot();
        target = spot;
        return true;
    }

    public override bool IsDone()
    {
        return beliefs.HasState(receiveTriage);
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