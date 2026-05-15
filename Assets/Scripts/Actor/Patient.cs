using UnityEngine;
using System;

public class PriorityActor : IComparable<PriorityActor>
{
    private static int nextId = 0;

    public Patient Actor;
    public int Level;
    public int Id { get; private set; }

    public PriorityActor(Patient actor)
    {
        Actor = actor;
        Level = actor.GetTriageLevel();
        Id = nextId++;
    }

    public int CompareTo(PriorityActor other)
    {
        int levelCompare = Level.CompareTo(other.Level);

        if (levelCompare != 0)
            return levelCompare;

        return Id.CompareTo(other.Id);
    }
}

[Serializable]
public class HospitalInfo
{
    public string doctor = "";
    public string receptionist = "";
    public string nurse = "";
    public float doctorStart = -1;
    public float doctorEnd = -1;

    public float receptionistStart = -1;
    public float receptionistEnd = -1;

    public float nurseStart = -1;
    public float nurseEnd = -1;

    public int triageLevel = -1;
}


public class Patient : GAgent
{
    private static int nextId = 0;
    [SerializeField]
    private WorldStateDefinition isHome;

    private string patientName;

    private int triageLevel;

    private HospitalInfo hospitalInfo;

    public HospitalInfo HospitalInfo
    {
        get => hospitalInfo;
        set => hospitalInfo = value;
    }

    private DateTime nextCheckTime;

    public void SetObservationTimer(float seconds)
    {
        nextCheckTime = DateTime.Now.AddSeconds(seconds);
    }

    public bool HasTimeArrived()
    {
        return DateTime.Now >= nextCheckTime;
    }

    public override void Start()
    {
        base.Start();

        ChatBubble chatBubble = GetComponentInChildren<ChatBubble>(true);
        chatBubble.Setup("Hello!", ChatBubble.IconType.Happy);

        agentName = "Patient" + nextId++;
        SetPatientName(agentName);

        GameManager.Instance.getPatientsList().Add(this);
        hospitalInfo = new HospitalInfo();
        GameManager.Instance.HospitalInfoDic.Add(agentName, hospitalInfo);
        SubGoal s = new(isHome, 1, 0);
        goals.Add(s, 4);
    }

    private void OnDestroy()
    {
        GameManager.Instance.getPatientsList().Remove(this);
    }

    public int GetTriageLevel() { return triageLevel; }
    public void SetTriageLevel(int level)
    {
        triageLevel = level;
    }

    public string GetPatientName() { return this.patientName; }
    public void SetPatientName(string name) { patientName = name; }

    //public bool IsHome() { return isHome; }
}