using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nurse : GAgent
{
    [SerializeField]
    private WorldStateDefinition triage;


    new void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal(triage, 1, -1);
        goals.Add(s1, 1);

    }

}