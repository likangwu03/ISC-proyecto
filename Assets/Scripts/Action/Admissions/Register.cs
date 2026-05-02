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

        if(state.HasState(internalRoomPatients) && state.GetStates()[internalRoomPatients] >= GameManager.Instance.maxInternalRoomPatients) return false;

        pacient = GameManager.Instance.getQueue().Leave(0);

        if(pacient == null) return false;

        pacient.GetComponent<MoveToRegister>().target = pos;
        pacient.GetComponent<GAgent>().beliefs.SetState(canRegistered, 1);


        return true;
    }

    public override bool PostPerform()
    {
        pacient.GetComponent<GAgent>().beliefs.SetState(isRegistered, 1);
        GWorld.Instance.GetWorld().ModifyState(internalRoomPatients, 1);
        GameManager.Instance.AddPatient(pacient);
        return true;
    }


}
