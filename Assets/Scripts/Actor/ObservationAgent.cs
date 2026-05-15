
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservationAgent : GAgent
{
    private static int nextId = 0;

    [SerializeField]
    private WorldStateDefinition observation;


    public override void Start()
    {
        base.Start();
        agentName = "observation" + nextId++;

        SubGoal s1 = new(observation, 1, -1);
        goals.Add(s1, 1);

    }

}
