using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Queue<GameObject> patients = new Queue<GameObject>();

    public Queue<GameObject> doctors = new Queue<GameObject>();

    public Queue<GameObject> waitingSpots = new Queue<GameObject>();

    public Queue<GameObject> registers = new Queue<GameObject>();

    [SerializeField]
    private WorldStateDefinition WaitingSpot;
    [SerializeField]
    private WorldStateDefinition Doctor;
    [SerializeField]
    private WorldStateDefinition Register;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            init();
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void init()
    {
        Cursor.visible = false;
        GameObject[] ListWaitingAreas = GameObject.FindGameObjectsWithTag("WaitingSpot");
        foreach (GameObject c in ListWaitingAreas)
        {
            waitingSpots.Enqueue(c);
        }
        if (ListWaitingAreas.Length > 0)
        {
            GWorld.Instance.GetWorld().ModifyState(WaitingSpot, ListWaitingAreas.Length);
        }

        GameObject[] ListDoctors = GameObject.FindGameObjectsWithTag("Doctor");
        foreach (GameObject c in ListDoctors)
        {
            doctors.Enqueue(c);
        }
        if (ListDoctors.Length > 0)
        {
            GWorld.Instance.GetWorld().ModifyState(Doctor, ListDoctors.Length);
        }

        GameObject[] ListRegisters = GameObject.FindGameObjectsWithTag("Register");
        foreach (GameObject c in ListRegisters)
        {
            registers.Enqueue(c);
        }
        if (ListRegisters.Length > 0)
        {
            GWorld.Instance.GetWorld().ModifyState(Register, ListRegisters.Length);
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

    public GameObject RemovePatient()
    {
        if (patients.Count == 0) return null;
        return patients.Dequeue();
    }

    public void AddDoctor(GameObject d)
    {
        doctors.Enqueue(d);
    }

    public GameObject GetDoctor()
    {
        if (doctors.Count == 0) return null;
        return doctors.Dequeue();
    }


    public void AddWaitingSpot(GameObject d)
    {
        waitingSpots.Enqueue(d);
    }

    public GameObject GetWaitingSpot()
    {
        if (waitingSpots.Count == 0) return null;
        return waitingSpots.Dequeue();
    }


    public void AddRegister(GameObject d)
    {
        registers.Enqueue(d);
    }

    public GameObject GetRegister()
    {
        if (registers.Count == 0) return null;
        return registers.Dequeue();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GWorld.Instance.GetWorld().Clear();
            SceneManager.LoadScene("Menu");
        }
    }

}