using UnityEngine;

public class MoveToRegister : MoveAction
{

    [SerializeField]
    private WorldStateDefinition isRegistered;

    public override bool IsDone()
    {
        return beliefs.HasState(isRegistered);
    }
    public override bool PostPerform()
    {
        return true;
    }

    public override void Perform()
    {
        MoveToTarget();
    }

}
