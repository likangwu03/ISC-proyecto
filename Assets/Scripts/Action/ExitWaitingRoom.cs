using UnityEngine;

public class ExitWaitingRoom : GAction
{
    [SerializeField]
    private WorldStateDefinition availableWaitingSpot;
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }

    public override void Perform()
    {

        ApplyEffect();
        GameObject target = inventory.FindItemWithTag("WaitingSpot");
        if (target != null)
        {
            GameManager.Instance.AddWaitingSpot(target);
            GWorld.Instance.GetWorld().ModifyState(availableWaitingSpot, 1);
            inventory.RemoveItem(target);
        }


    }
}
