using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    // Queue of patients
    public Queue<GameObject> patients = new Queue<GameObject>();
    // Queue of cubicles
    public Queue<GameObject> cubicles = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            init();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void init()
    {
        Time.timeScale = 5.0f;
        //GWorld.Instance.GetWorld().Clear();

        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cubicle");
        foreach (GameObject c in cubes)
        {
            cubicles.Enqueue(c);
        }
        if (cubes.Length > 0)
        {
            GWorld.Instance.GetWorld().ModifyState("FreeCubicle", cubes.Length);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    // Add patient
    public void AddPatient(GameObject p)
    {
        patients.Enqueue(p);
    }

    // Remove patient
    public GameObject RemovePatient()
    {
        if (patients.Count == 0) return null;
        return patients.Dequeue();
    }

    // Add cubicle
    public void AddCubicle(GameObject p)
    {
        cubicles.Enqueue(p);
    }

    // Remove cubicle
    public GameObject RemoveCubicle()
    {
        if (cubicles.Count == 0) return null;
        return cubicles.Dequeue();
    }
}