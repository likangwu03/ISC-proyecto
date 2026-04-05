using UnityEngine;

public class Spawn : MonoBehaviour
{

    public GameObject patientPrefab;

    // Tiempo inicial antes del primer spawn
    [SerializeField] private float initialDelay = 5f;

    // Intervalo de spawn configurable
    [SerializeField] private float minSpawnTime = 15f;
    [SerializeField] private float maxSpawnTime = 17f;

    void Start()
    {
        Invoke(nameof(SpawnPatient), initialDelay);
    }

    void SpawnPatient()
    {

        Instantiate(patientPrefab, transform.position, Quaternion.identity);

        float nextTime = Random.Range(minSpawnTime, maxSpawnTime);
        Invoke(nameof(SpawnPatient), nextTime);
    }
}