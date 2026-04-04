using UnityEngine;
using UnityEngine.AI;

public class MoveAction : GAction
{
    public GameObject target;
    public string targetTag;

    protected NavMeshAgent navAgent;

    protected override void Awake()
    {
        base.Awake();
        navAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        if (target == null && !string.IsNullOrEmpty(targetTag))
        {
            target = GameObject.FindGameObjectWithTag(targetTag);
        }
    }

    protected void MoveToTarget()
    {
        if (target != null)
        {
            navAgent.SetDestination(target.transform.position);
        }
    }

    public override bool IsDone()
    {
        if (navAgent.pathPending) return false;

        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude < 0.01f)
                return true;
        }

        return false;
    }

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
        MoveToTarget();
    }
}