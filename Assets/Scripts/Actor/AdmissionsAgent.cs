using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AdmissionsAgent : GAgent
{
    [SerializeField]
    private WorldStateDefinition patientRegistration;


    new void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal(patientRegistration, 1, -1);
        goals.Add(s1, 1);

    }

}