using UnityEngine;
using UnityEngine.AI;

public class MoveAction : GAction
{
    public GameObject target;
    public string targetTag;

    public float duration = 2.0f;
    private float elapsedTime = 0.0f;
    private bool reachedTarget = false;

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
            reachedTarget = false;
            elapsedTime = 0.0f;
            navAgent.SetDestination(target.transform.position);
        }
    }

    public override bool IsDone()
    {
        if (!reachedTarget)
        {
            if (navAgent.pathPending) return false;

            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude < 0.01f)
                {
                    elapsedTime = 0.0f;
                    reachedTarget = true;
                }
            }
            return false;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            return elapsedTime >= duration;
        }
    }

    public override bool PrePerform()
    {
        return true;
    }

    public override void Perform()
    {
        MoveToTarget();
    }
}