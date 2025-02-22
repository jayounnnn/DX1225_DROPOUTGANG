using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float arcForce = 5f;
    [SerializeField]private float maxTravelDistance = 10f;

    private Vector3 startPosition;
    private GameObject rockItemPrefab;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Ensure Rigidbody exists
        if (rb == null)
        {
            Debug.LogError("Rigidbody is missing on Rock prefab! Add it in Unity.");
        }
        rb.useGravity = true;
        rb.isKinematic = false;

        startPosition = transform.position;
        rockItemPrefab = InventoryManager.instance.GetItemPrefab("RockItem");
        if (rockItemPrefab == null)
        {
            Debug.LogError("RockItem prefab not found in InventoryManager!");
        }
    }

    public void Initialize(Vector3 playerPosition, Vector3 direction)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody is not set in Rock script!");
            return;
        }

        // Set start position to player's transform at the moment of throwing
        startPosition = playerPosition;

        // Ensure Rigidbody is ready for physics interactions
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Calculate the arc trajectory (forward + upward force)
        Vector3 throwDirection = direction.normalized * throwForce + Vector3.up * arcForce;
        rb.AddForce(throwDirection, ForceMode.Impulse); // Use Impulse for immediate physics effect
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                StunState.ForceStun(enemy);
                Debug.Log("Enemy hit! Stunned.");
            }
        }
        else
        {
            // Spawn RockItem at collision point
            if (rockItemPrefab != null)
            {
                Instantiate(rockItemPrefab, transform.position, Quaternion.identity);
                Debug.Log("Spawned RockItem at " + transform.position);
            }
            else
            {
                Debug.LogError("RockItem prefab is null! Check InventoryManager.");
            }
        }
        // Destroy rock after collision (regardless of what it hit)
        Destroy(gameObject);
    }
}
