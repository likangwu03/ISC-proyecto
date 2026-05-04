using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Animator animatorDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animatorDoor.SetBool("OpenDoor", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animatorDoor.SetBool("OpenDoor", false);
        }
    }
}