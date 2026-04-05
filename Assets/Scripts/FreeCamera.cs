using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public float moveSpeed = 10f;       // Velocidad de desplazamiento
    public float lookSensitivity = 3f;  // Sensibilidad del mouse
    private float yaw = 0f;
    private float pitch = 0f;

    void Update()
    {
        // Movimiento con WASD
        float x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float y = 0f;

        if (Input.GetKey(KeyCode.E)) y += moveSpeed * Time.deltaTime; // Subir
        if (Input.GetKey(KeyCode.Q)) y -= moveSpeed * Time.deltaTime; // Bajar

        transform.Translate(x, y, z);

        // Rotación con el mouse
        yaw += Input.GetAxis("Mouse X") * lookSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * lookSensitivity;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }
}