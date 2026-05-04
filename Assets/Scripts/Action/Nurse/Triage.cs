using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triage : GAction
{

    [SerializeField]
    private WorldStateDefinition receiveTriage;

    [SerializeField] 
    private float triageDuration = 10.0f;

    private GameObject patient;

    [SerializeField]
    private GameObject pos;

    private float endTime;

    public override void Perform()
    {
        chatBubble.Setup("Howdy", ChatBubble.IconType.TreatingPatient);
        endTime = Time.time + triageDuration;
    }

    public override bool IsDone()
    {
        return Time.time >= endTime;
    }

    public override bool PrePerform()
    {
        patient = GameManager.Instance.GetNextPatient();

        if (patient == null) return false;

        patient.GetComponent<ReceiveTriage>().target = pos;
        patient.GetComponent<ReceiveTriage>().Perform();


        Patient p = patient.GetComponent<Patient>();
        p.HospitalInfo.nurse = gameObject.GetComponent<Nurse>().GetAgentName();
        p.HospitalInfo.nurseStart = Time.time;

        gameObject.GetComponent<LookAtTarget>().paciente = patient.transform;
        return true;
    }

    public int GetWeightedRandomLevel()
    {
        int roll = Random.Range(0, 15);

        if (roll == 0) return 1;        // ~6.7%
        if (roll <= 2) return 2;        // ~13.3%
        if (roll <= 5) return 3;        // ~20%
        if (roll <= 9) return 4;        // ~26.7%
        return 5;                       // ~33.3%
    }

    public override bool PostPerform()
    {
        chatBubble.StartFadeOut();

        patient.GetComponent<GAgent>().beliefs.SetState(receiveTriage, 1);
        Patient p = patient.GetComponent<Patient>();
        p.SetTriageLevel(GetWeightedRandomLevel());
        p.HospitalInfo.triageLevel = p.GetTriageLevel();
        p.HospitalInfo.nurseEnd = Time.time;
        GameManager.Instance.AddToTriageQueue(p);

        gameObject.GetComponent<LookAtTarget>().paciente = null;

        return true;
    }
}
