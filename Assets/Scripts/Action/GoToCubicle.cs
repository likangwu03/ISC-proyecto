public class GoToCubicle : MoveAction{

    public override bool PrePerform() {
        target = GameManager.Instance.GetDoctor();
        GWorld.Instance.GetWorld().ModifyState("AvailableDoctor", -1);

        inventory.AddItem(target);
        if (target == null)
            return false;
        return true;
    }

    public override bool PostPerform() {

        GWorld.Instance.GetWorld().ModifyState("TreatingPatient", 1);

        GameManager.Instance.AddDoctor(target);
        GWorld.Instance.GetWorld().ModifyState("AvailableDoctor", 1);

        inventory.RemoveItem(target);

        beliefs.ModifyState("isCured", 1);

        return true;
    }
}
