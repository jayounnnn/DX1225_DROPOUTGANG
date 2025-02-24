using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] public DoorOpenMethod openMethod;
    [SerializeField] public bool isOpen = false;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private float detectionRadius = 3f;
    public enum DoorOpenMethod { ElectricalPuzzle, Key }

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Transform playerTransform;


    private void Start()
    {
        // Save initial rotation as "closed" position
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + openAngle, transform.eulerAngles.z);
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // Call this function when minigame is completed
    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            StartCoroutine(RotateDoor(openRotation));
        }
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        float timeElapsed = 0f;
        Quaternion startRotation = transform.rotation;
        Debug.Log("Opening door");
        while (timeElapsed < 1f)
        {
            timeElapsed += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed);
            yield return null;
        }

        transform.rotation = targetRotation; // Ensure final position
    }

    public bool PlayerIsNearby()
    {
        if (playerTransform == null) return false;
        return Vector3.Distance(transform.position, playerTransform.position) <= detectionRadius;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection radius in Unity editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
