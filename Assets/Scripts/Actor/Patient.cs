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
        Level = actor.triageLevel;
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


public class Patient : GAgent
{
    [SerializeField]
    private WorldStateDefinition isHome;
    //[SerializeField]
    //private WorldStateDefinition isWaiting;
    //[SerializeField]
    //private WorldStateDefinition isTreated;
    //[SerializeField]
    //private WorldStateDefinition hasRegistered;

    public string agentName;
    public int triageLevel;

    new void Start()
    {
        GameManager.Instance.patientList.Add(this);
      // Call the base start
        base.Start();
 /*         // Set up the subgoal "isWaiting"
        SubGoal s1 = new SubGoal(isWaiting, 1, 1);
        // Add it to the goals
        goals.Add(s1, 1);

        // Set up the subgoal "isTreated"
        SubGoal s2 = new SubGoal(isTreated, 1, 0);
        // Add it to the goals
        goals.Add(s2, 5);


        // Set up the subgoal "hasRegistered"
        SubGoal s4 = new SubGoal(hasRegistered, 1, 0);
        // Add it to the goals
        goals.Add(s4, 2);*/

        // Set up the subgoal "isHome"
        SubGoal s3 = new SubGoal(isHome, 1, 0);
        // Add it to the goals
        goals.Add(s3, 4);
        
    }

    private void OnDestroy()
    {
        GameManager.Instance.patientList.Remove(this);
    }
}