using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AdmissionsAgent : GAgent
{
    private static int nextId = 0;

    [SerializeField]
    private WorldStateDefinition patientRegistration;


    public override void Start()
    {
        base.Start();
        agentName = "receptionist" + nextId++;

        SubGoal s1 = new(patientRegistration, 1, -1);
        goals.Add(s1, 1);

    }

}