using UnityEngine;

public class ExitWaitingRoom : GAction
{
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
            GWorld.Instance.GetWorld().ModifyState("AvailableWaitingSpot", 1);
            inventory.RemoveItem(target);
        }


    }
}
