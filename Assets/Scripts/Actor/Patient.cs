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
    public string administration = "";
    public string nurse = "";
    public float doctorStart = -1;
    public float doctorEnd = -1;

    public float administrationStart = -1;
    public float administrationEnd = -1;

    public float nurseStart = -1;
    public float nurseEnd = -1;
}


public class Patient : GAgent
{
    [SerializeField]
    private WorldStateDefinition isHome;

    private readonly string agentName;
    private int triageLevel;

    private HospitalInfo hospitalInfo;

    public override void Start()
    {
        GameManager.Instance.patientList.Add(this);
        base.Start();
        SubGoal s = new(isHome, 1, 0);
        goals.Add(s, 4);

        
    }

    private void OnDestroy()
    {
        GameManager.Instance.patientList.Remove(this);
    }

    public string GetAgentName() { return agentName; }
    public int GetTriageLevel() {  return triageLevel; }
    public void SetTriageLevel(int level)
    {
        triageLevel = level;
    }
}