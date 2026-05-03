using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Doctor : GAgent
{
    private static int nextId = 0;

    [SerializeField]
    private WorldStateDefinition treatPatient;

    public override void Start()
    {
        base.Start();
        agentName = "Doctor" + nextId++;

        SubGoal s1 = new(treatPatient, 1, -1);
        goals.Add(s1, 1);

    }
}