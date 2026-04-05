public class GoToWaitingRoom : MoveAction{
    public override bool PrePerform() {
        if (target == null)
        {
            target = GameManager.Instance.GetWaitingSpot();
            GWorld.Instance.GetWorld().ModifyState("AvailableWaitingSpot", -1);
            inventory.AddItem(target);
        }
        return target != null;
    }

    public override bool PostPerform() {
        ApplyEffect();
        return true;
    }
}
