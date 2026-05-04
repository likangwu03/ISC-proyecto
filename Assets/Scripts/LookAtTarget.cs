using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform paciente;
    public Vector3 rotationOffset;

    void Update()
    {
        if (paciente == null) return;
        Vector3 direccion = paciente.position - transform.position;
        direccion.y = 0;

        if (direccion != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direccion);
            transform.rotation = lookRotation * Quaternion.Euler(rotationOffset);
        }
    }
}