using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreatPatient : GAction
{

    [SerializeField]
    private WorldStateDefinition isTreated;
    [SerializeField]
    private WorldStateDefinition internalRoomPatients;
    [SerializeField]
    private WorldStateDefinition toBed;
    [SerializeField]
    private float TreatDuration = 10.0f;

    private GameObject patient;

    [SerializeField]
    private GameObject pos;

    private float endTime;

    public override void Perform()
    {
        endTime = Time.time + TreatDuration;
    }

    public override bool IsDone()
    {
        return Time.time >= endTime;
    }

    public override bool PrePerform()
    {
        Patient p = GameManager.Instance.GetNextTriagedPatient();

        if (p == null) return false;
        patient = p.gameObject;


        patient.GetComponent<ReceiveTreatment>().target = pos;
        patient.GetComponent<ReceiveTreatment>().Perform();

        p.HospitalInfo.doctor = gameObject.GetComponent<Doctor>().GetAgentName();
        p.HospitalInfo.doctorStart = Time.time;

        gameObject.GetComponent<LookAtTarget>().paciente = patient.transform;
        return true;
    }

    public override bool PostPerform()
    {
        patient.GetComponent<GAgent>().beliefs.SetState(isTreated, 1);
        Patient p = patient.GetComponent<Patient>();
        p.HospitalInfo.doctorEnd = Time.time;

        if (Random.value > -1)
        {
            p.beliefs.SetState(toBed, 1);
            p.SetObservationTimer(10);
        }
        else GWorld.Instance.GetWorld().ModifyState(internalRoomPatients, -1);


        gameObject.GetComponent<LookAtTarget>().paciente = null;
        return true;
    }
}
