using UnityEngine;

public class Spawn : MonoBehaviour {

    // Grab our prefab
    public GameObject patientPrefab;
    // Number of patients to spawn
    public int numPatients;

    void Start() {

        Invoke("SpawnPatient", 5.0f);
    }

    void SpawnPatient() {

        // Instantiate numPatients at the spawner
        Instantiate(patientPrefab, this.transform.position, Quaternion.identity);
        // Invoke this method at random intervals
        Invoke("SpawnPatient", Random.Range(8.0f, 10.0f));
    }

    // Update is called once per frame
    void Update() {

    }
}
