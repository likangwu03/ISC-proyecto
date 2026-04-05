using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject cameraFP;
    public GameObject cameraFree;
    void Start()
    {
        cameraFP.SetActive(false);
        cameraFree.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cameraFP.SetActive(!cameraFP.activeSelf);
            cameraFree.SetActive(!cameraFree.activeSelf);

        }
    }
}