using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private DynamicQueueLayout queue;

    private Queue<GameObject> patients = new();

    private MinHeap<PriorityActor> triagedPatientsQueue = new();

    private Queue<GameObject> waitingSpots = new();

    private Queue<GameObject> beds = new();

    public int maxInternalRoomPatients = 5;

    private List<Patient> patientList;

    private Queue<GameObject> observations = new();

    [SerializeField]
    private WorldStateDefinition WaitingSpot;
    [SerializeField]
    private WorldStateDefinition Doctor;
    [SerializeField]
    private WorldStateDefinition NumBeds;


    private Dictionary<string,HospitalInfo> hospitalInfoDic =new();

    public Dictionary<string, HospitalInfo> HospitalInfoDic
    {
        get => hospitalInfoDic;
        set => hospitalInfoDic = value;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
            patientList = new List<Patient>();
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        Cursor.visible = false;
        GameObject[] ListWaitingAreas = GameObject.FindGameObjectsWithTag("WaitingSpot");
        foreach (GameObject c in ListWaitingAreas)
        {
            waitingSpots.Enqueue(c);
        }
        if (ListWaitingAreas.Length > 0)
        {
            GWorld.Instance.GetWorld().SetState(WaitingSpot, ListWaitingAreas.Length);
        }


        GameObject[] ListBeds = GameObject.FindGameObjectsWithTag("bed");
        foreach (GameObject c in ListBeds)
        {
            beds.Enqueue(c);
        }

        if (ListBeds.Length > 0)
        {
            GWorld.Instance.GetWorld().SetState(NumBeds, ListBeds.Length);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddPatient(GameObject p)
    {
        patients.Enqueue(p);
    }

    public GameObject GetNextPatient()
    {
        if (patients.Count == 0) return null;
        return patients.Dequeue();
    }

    public void AddWaitingSpot(GameObject d)
    {
        waitingSpots.Enqueue(d);
        GWorld.Instance.GetWorld().ModifyState(WaitingSpot, 1);
    }

    public GameObject GetWaitingSpot()
    {
        if (waitingSpots.Count == 0) return null;
        GWorld.Instance.GetWorld().ModifyState(WaitingSpot, -1);
        return waitingSpots.Dequeue();
    }

    public void AddBed(GameObject bed)
    {
        GWorld.Instance.GetWorld().ModifyState(NumBeds, 1);
        beds.Enqueue(bed);
    }

    public GameObject GetBed()
    {
        if (beds.Count == 0) return null;
        GWorld.Instance.GetWorld().ModifyState(NumBeds, -1);
        return beds.Dequeue();
    }

    public void AddObservation(GameObject patient)
    {
        observations.Enqueue(patient);
    }

    public GameObject GetNextObservation()
    {
        if (observations.Count == 0)
            return null;

        return observations.Dequeue();
    }

    public DynamicQueueLayout GetQueue() { return queue; }


    public void AddToTriageQueue(Patient p)
    {
        triagedPatientsQueue.Push(new PriorityActor(p));
    }

    public Patient GetNextTriagedPatient()
    {
        if (triagedPatientsQueue.Count == 0) return null;

        PriorityActor p = triagedPatientsQueue.Pop();
        return p.Actor;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GWorld.Instance.GetWorld().Clear();
            SceneManager.LoadScene("Menu");
        }
    }



    public List<Patient> getPatientsList()
    {
        return patientList;
    }

}