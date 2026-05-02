using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : GAgent
{
    [SerializeField]
    private WorldStateDefinition treatPatient;

    new void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal(treatPatient, 1, -1);
        goals.Add(s1, 1);

    }

}