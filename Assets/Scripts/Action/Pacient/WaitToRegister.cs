using UnityEngine;

public class WaitToRegister : MoveAction
{
    private int pos = -1;
    private bool advance = false;
    private DynamicQueueLayout queue;

    [SerializeField]
    private WorldStateDefinition canRegister;

    public override bool PrePerform()
    {
        queue = GameManager.Instance.getQueue();
        pos = queue.Enqueue(gameObject);
        return true;
    }

    public override void Perform()
    {
        advance = true;
        navAgent.SetDestination(queue.GetPosition(pos));
    }

    public override bool IsDone()
    {
        return pos == 0 && beliefs.HasState(canRegister);
    }

    public override bool PostPerform()
    {
        advance = false;
        return false;
    }

    public override void Suspended() {
        queue.Leave(pos);
    }

    private void Update()
    {
        if (advance)
        {
            if (base.IsDone() && pos > 0)
            {
                int prePos = pos;
                pos = queue.advance(pos);
                if(pos != prePos)
                {
                    navAgent.SetDestination(queue.GetPosition(pos));
                }
            }
        }
    }

}
