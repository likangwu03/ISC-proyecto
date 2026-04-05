using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject cameraFP;
    public GameObject cameraFree;
    void Start()
    {
        // Solo activamos la cámara de primera persona al inicio
        cameraFP.SetActive(true);
        cameraFree.SetActive(false);
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