using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : GAction
{

    [SerializeField]
    private WorldStateDefinition isRegistered;

    [SerializeField]
    private WorldStateDefinition canRegistered;

    [SerializeField]
    private WorldStateDefinition internalRoomPatients;
    [SerializeField]
    private float registrationDuration = 3.0f;

    private GameObject patient;

    [SerializeField]
    private GameObject pos;

    private float endTime;

    public override void Perform()
    {
        patient.GetComponent<GAgent>().beliefs.SetState(canRegistered, 1);
        endTime = Time.time + registrationDuration;
    }

    public override bool IsDone()
    {
        return Time.time >= endTime;
    }

    public override bool PrePerform()
    {
        WorldStates state = GWorld.Instance.GetWorld();

        if(state.HasState(internalRoomPatients) && state.GetStates()[internalRoomPatients] >= GameManager.Instance.maxInternalRoomPatients) return false;

        patient = GameManager.Instance.GetQueue().Leave(0);

        if (patient == null) return false;

        Patient p = patient.GetComponent<Patient>();
        p.HospitalInfo.receptionist = gameObject.GetComponent<AdmissionsAgent>().GetAgentName();
        p.HospitalInfo.receptionistStart = Time.time;

        patient.GetComponent<MoveToRegister>().target = pos;
        patient.GetComponent<GAgent>().beliefs.SetState(canRegistered, 1);

        gameObject.GetComponent<LookAtTarget>().paciente = patient.transform;

        return true;
    }

    public override bool PostPerform()
    {
        patient.GetComponent<GAgent>().beliefs.SetState(isRegistered, 1);
        GWorld.Instance.GetWorld().ModifyState(internalRoomPatients, 1);
        GameManager.Instance.AddPatient(patient);
        Patient p = patient.GetComponent<Patient>();
        p.HospitalInfo.receptionistEnd = Time.time;
        gameObject.GetComponent<LookAtTarget>().paciente = null;

        return true;
    }


}
