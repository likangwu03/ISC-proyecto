using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triage : GAction
{

    [SerializeField]
    private WorldStateDefinition receiveTriage;

    public float triageDuration = 10.0f;

    private GameObject pacient;

    [SerializeField]
    private GameObject pos;

    private float endTime;

    public override void Perform()
    {
        endTime = Time.time + triageDuration;
    }

    public override bool IsDone()
    {
        return Time.time >= endTime;
    }

    public override bool PrePerform()
    {
        pacient = GameManager.Instance.RemovePatient();

        if (pacient == null) return false;

        pacient.GetComponent<ReceiveTriage>().target = pos;
        pacient.GetComponent<ReceiveTriage>().Perform();

        return true;
    }

    public override bool PostPerform()
    {
        pacient.GetComponent<GAgent>().beliefs.SetState(receiveTriage, 1);
        GameManager.Instance._patients.Enqueue(pacient);
        return true;
    }
}
