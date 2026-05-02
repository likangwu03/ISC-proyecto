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
    private WorldStateDefinition internal_room_patients;

    public float registrationDuration = 3.0f;

    private GameObject pacient;

    [SerializeField]
    private GameObject pos;

    private float endTime;

    public override void Perform()
    {
        pacient.GetComponent<GAgent>().beliefs.SetState(canRegistered, 1);
        endTime = Time.time + registrationDuration;
    }

    public override bool IsDone()
    {
        return Time.time >= endTime;
    }

    public override bool PrePerform()
    {
        WorldStates state = GWorld.Instance.GetWorld();

        if(state.HasState(internal_room_patients) && state.GetStates()[internal_room_patients] >= GameManager.Instance.max_internal_room_patients) return false;

        pacient = GameManager.Instance.getQueue().Leave(0);

        if(pacient == null) return false;

        pacient.GetComponent<MoveToRegister>().target = pos;
        pacient.GetComponent<GAgent>().beliefs.SetState(canRegistered, 1);


        return true;
    }

    public override bool PostPerform()
    {
        pacient.GetComponent<GAgent>().beliefs.SetState(isRegistered, 1);
        GWorld.Instance.GetWorld().ModifyState(internal_room_patients, 1);
        GameManager.Instance.AddPatient(pacient);
        return true;
    }


}
