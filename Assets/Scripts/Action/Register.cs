public class Register : MoveAction
{
    public override bool PrePerform() {
        target = GameManager.Instance.GetRegister();
        GWorld.Instance.GetWorld().ModifyState("AvailableRegister", -1);

        inventory.AddItem(target);
        if (target == null)
            return false;
        return true;
    }

    public override bool PostPerform() {
        ApplyEffect();
        GameManager.Instance.AddRegister(target);
        GWorld.Instance.GetWorld().ModifyState("AvailableRegister", 1);

        return true;
    }
}
