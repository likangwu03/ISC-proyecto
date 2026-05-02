using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nurse : GAgent
{
    private static int nextId = 0;

    [SerializeField]
    private WorldStateDefinition triage;


    public override void Start()
    {
        base.Start();
        agentName = "Nurse" + nextId++;

        SubGoal s1 = new(triage, 1, -1);
        goals.Add(s1, 1);

    }

}