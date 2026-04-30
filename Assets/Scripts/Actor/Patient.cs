using UnityEngine;

public class Patient : GAgent
{
    [SerializeField]
    private WorldStateDefinition isWaiting;
    [SerializeField]
    private WorldStateDefinition isTreated;
    [SerializeField]
    private WorldStateDefinition isHome;
    [SerializeField]
    private WorldStateDefinition hasRegistered;
    new void Start()
    {

        // Call the base start
        base.Start();
        // Set up the subgoal "isWaiting"
        SubGoal s1 = new SubGoal(isWaiting, 1, 1);
        // Add it to the goals
        goals.Add(s1, 1);

        // Set up the subgoal "isTreated"
        SubGoal s2 = new SubGoal(isTreated, 1, 0);
        // Add it to the goals
        goals.Add(s2, 5);

        // Set up the subgoal "isHome"
        SubGoal s3 = new SubGoal(isHome, 1, 0);
        // Add it to the goals
        goals.Add(s3, 4);

        // Set up the subgoal "hasRegistered"
        SubGoal s4 = new SubGoal(hasRegistered, 1, 0);
        // Add it to the goals
        goals.Add(s4, 2);
    }

}