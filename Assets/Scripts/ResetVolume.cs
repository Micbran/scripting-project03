using UnityEngine;

public class ResetVolume : MonoBehaviour
{
    [SerializeField] Vector3 returnToPosition = Vector3.zero;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = returnToPosition;
    }
}
