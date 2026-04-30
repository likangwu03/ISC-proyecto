using UnityEngine;

public class Register : MoveAction
{

    [SerializeField]
    private WorldStateDefinition register;
    public override bool PrePerform()
    {
        target = GameManager.Instance.GetRegister();
        GWorld.Instance.GetWorld().ModifyState(register, -1);

        inventory.AddItem(target);
        if (target == null)
            return false;
        return true;
    }

    public override bool PostPerform()
    {
        ApplyEffect();
        inventory.RemoveItem(target);
        GameManager.Instance.AddRegister(target);
        GWorld.Instance.GetWorld().ModifyState(register, 1);

        return true;
    }
}
