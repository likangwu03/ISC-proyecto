using UnityEngine;

public class GoToWaitingRoom : MoveAction
{
    [SerializeField]
    private WorldStateDefinition waitingSpot;
    public override bool PrePerform()
    {
        if (target == null)
        {
            target = GameManager.Instance.GetWaitingSpot();
            GWorld.Instance.GetWorld().ModifyState(waitingSpot, -1);
            inventory.AddItem(target);
        }
        return target != null;
    }

    public override bool PostPerform()
    {
        ApplyEffect();
        return true;
    }
}
