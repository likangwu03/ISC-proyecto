using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using static BarrelGeneration;

public class BarrelGeneration : MonoBehaviour
{
    // Barrel properties storing (for telemetry purposes)
    [System.Serializable]
    public struct BarrelProps
    {
        public string name;
        public Vector3 spawnOffset; // Offset relative to the spawner
    }

    // How many barrels will spawn
    [SerializeField]
    private List<BarrelProps> barrelsToSpawn = new List<BarrelProps>();

    private int spawnedBarrelsCount;

    // Where barrels will appear
    [SerializeField]
    public Transform spawnPoint;

    // Barrels list to iterate over and apply a different behaviour to each of barrel
    private List<GameObject> spawnedBarrels = new List<GameObject>();

    [System.Serializable]
    public class BarrelData
    {
        public string name;
        //public float x, y, z;
        public string generatedAt;
        public string destroyedAt;

    }

    [System.Serializable]
    public class BarrelDataList
    {
        public List<BarrelData> barrels;
    }

    private WebSocketClient wsc;

    private Dictionary<string, BarrelData> barrelDataDict;


    void Start()
    {
        wsc = WebSocketClient.getSharedInstance();

        spawnedBarrelsCount = 0;

        barrelDataDict = new Dictionary<string, BarrelData>();

        SpawnBarrels();

        InvokeRepeating(nameof(SendBarrelPositions), 0.2f, 0.1f);
    }

    // Call this to spawn all barrels
    public void SpawnBarrels()
    {
        //spawnedBarrels.Clear();

        for (int i = 0; i < barrelsToSpawn.Count; i++)
        {
            BarrelProps barrelIInfo = barrelsToSpawn[i];

            if (spawnedBarrels.Count > i){
                
                GameObject spawnedBarrelI = spawnedBarrels[i];

                if (!spawnedBarrelI.activeInHierarchy)
                {
                    spawnedBarrels[i] = SpawnSingleBarrel(barrelIInfo);
                }
            }

            else
            {
                spawnedBarrels.Add(SpawnSingleBarrel(barrelIInfo));
            }
        }

    }

    void OnDrawGizmos()
    {
        if (spawnPoint == null) return;

        Gizmos.color = Color.green;

        foreach (var barrel in barrelsToSpawn)
        {
            Vector3 worldPos = spawnPoint.TransformPoint(barrel.spawnOffset);
            Gizmos.DrawSphere(worldPos, 0.2f);
            Gizmos.DrawLine(spawnPoint.position, worldPos);
        }
    }

    // Example: spawn a single barrel at a custom offset
    public GameObject SpawnSingleBarrel(BarrelProps barrel)
    {
        // Get a barrel from the pool
        GameObject barrelObj = ObjectPool.getSharedInstance().GetPooledObject();

        if (barrelObj != null)
        {
            // Set position & rotation
            //barrelObj.transform.position = spawnPoint.TransformPoint(barrel.spawnOffset);
            barrelObj.transform.position = spawnPoint.position + barrel.spawnOffset;
            barrelObj.transform.rotation = spawnPoint.rotation;

            // Activating the object will make it visible
            barrelObj.SetActive(true);

            // Barrel name (for telemetry and tracking purposes)
            barrelObj.name = barrel.name!="" ? barrel.name : "Barrel " + (spawnedBarrelsCount + 1);

            // Add physics for a more realist approach
            Rigidbody rb = barrelObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = spawnPoint.TransformDirection(barrel.spawnOffset.normalized);
                rb.velocity = dir * 5f;
            }

            spawnedBarrelsCount++;
        }

        else
        {
            return null;
        }

        return barrelObj;
    }

    void SendBarrelPositions()
    {
        if (wsc != null)
        {
            BarrelDataList dataList = new BarrelDataList();
            dataList.barrels = new List<BarrelData>();

            foreach (var barrel in spawnedBarrels)
            {
                if (barrel == null || !barrel.activeInHierarchy) continue;

                if (!barrelDataDict.ContainsKey(barrel.name)){

                    BarrelData barrelData = new BarrelData{
                        name = barrel.name,
                        generatedAt = DateTime.Now.ToString("HH:mm:ss.fff"),
                        destroyedAt = ""
                        /*x = pos.x,
                        y = pos.y,
                        z = pos.z*/
                    };

                    dataList.barrels.Add(barrelData);

                    barrelDataDict[barrel.name] = barrelData;
                }

                else
                {
                    dataList.barrels.Add(barrelDataDict[barrel.name]);
                }

                //Vector3 pos = barrel.transform.position;

                /*dataList.barrels.Add(new BarrelData
                {
                    name = barrel.name,
                    generatedAt = DateTime.Now.ToString("M/d/yy"),
                    destroyedAt = ""
                    x = pos.x,
                    y = pos.y,
                    z = pos.z
                });*/
            }

            string json = JsonUtility.ToJson(dataList);
            Debug.Log("Sending: " + json);
            wsc.SendData(json);
        }
    }

    void Update()
    {
        foreach (var barrelObj in spawnedBarrels)
        {
            // Get a barrel from the pool (TODO)
           // GameObject barrelObj = ObjectPool.getSharedInstance().GetPooledObject();

            if (barrelObj != null)
            {
                // Set position & rotation
                float yPos = barrelObj.transform.position.y;
                float zPos = barrelObj.transform.position.z;

                if(yPos < -30 || zPos > 30)
                {
                    barrelDataDict[barrelObj.name].destroyedAt = DateTime.Now.ToString("HH:mm:ss.fff");
                    SendBarrelPositions();
                    barrelObj.SetActive(false);
                }

            }
        }
        SpawnBarrels();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 200, 100), "Generated barrels: " + spawnedBarrelsCount);
        GUI.Label(new Rect(0, 20, 200, 100), "Respawned barrels: " + (spawnedBarrelsCount - barrelsToSpawn.Count));
    }
}