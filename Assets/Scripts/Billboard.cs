using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private BillboardType billboardType;

    [Header("Lock Rotation")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    private Vector3 originalRotation;
    private Transform myTransform;

    public enum BillboardType { LookAtCamera, CameraForward };

    private void Awake()
    {
        originalRotation = transform.rotation.eulerAngles;
        myTransform = transform;
    }

    void LateUpdate()
    {
        Camera camera = Camera.allCameras[0];
        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
                myTransform.LookAt(camera.transform.position, Vector3.up);
                break;
            case BillboardType.CameraForward:
                myTransform.forward = camera.transform.forward;
                break;
            default:
                break;
        }
        myTransform.rotation *= Quaternion.Euler(0f, 180f, 0f);

        Vector3 rotation = myTransform.rotation.eulerAngles;

        if (lockX) rotation.x = originalRotation.x;
        if (lockY) rotation.y = originalRotation.y;
        if (lockZ) rotation.z = originalRotation.z;

        myTransform.rotation = Quaternion.Euler(rotation);
    }
}

