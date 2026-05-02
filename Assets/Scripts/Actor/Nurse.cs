using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nurse : GAgent
{
    [SerializeField]
    private WorldStateDefinition triage;


    public override void Start()
    {
        base.Start();
        SubGoal s1 = new(triage, 1, -1);
        goals.Add(s1, 1);

    }

}