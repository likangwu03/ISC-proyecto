public class GoToWaitingRoom : MoveAction{
    public override bool PrePerform() {
        target = GameManager.Instance.GetWaitingSpot();
        GWorld.Instance.GetWorld().ModifyState("AvailableWaitingSpot", -1);

        inventory.AddItem(target);
        if (target == null)
            return false;
        return true;
    }

    public override bool PostPerform() {
        // GWorld.Instance.GetWorld().ModifyState("Waiting", 1);
        GameManager.Instance.AddWaitingSpot(target);

        GWorld.Instance.GetWorld().ModifyState("AvailableWaitingSpot", 1);
        return true;
    }
}
