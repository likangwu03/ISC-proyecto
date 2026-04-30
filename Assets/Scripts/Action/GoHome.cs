public class GoHome : MoveAction
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        ApplyEffect();
        Destroy(gameObject);
        return true;
    }
}
