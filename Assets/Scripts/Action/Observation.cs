
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Observation : MoveAction
{
    private GameObject patient;
    [SerializeField] private GameObject ini;
    [SerializeField]
    private WorldStateDefinition toBed;

    public override bool PrePerform()
    {
        if (patient == null) patient = GameManager.Instance.GetNextObservation();
        if (patient != null && patient.GetComponent<Patient>().HasTimeArrived()) 
            target = patient;
        else target = ini;

        return true;
    }


    public override bool PostPerform()
    {
        if (patient != null && target != ini) { 
            patient.GetComponent<Patient>().beliefs.SetState(toBed, -1);
            patient = null;
        }
        return true;
    }
}