using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    [SerializeField] private SphereCollider[] detectors;

    public void EnableCollider(int index)
    {
        if (index < 0 || index >= detectors.Length)
        {
            //Debug.LogError($"Index {index} is out of bounds! Detectors array size: {detectors.Length}");
            return;
        }


        SphereCollider detector = detectors[index];
        detector.enabled = true;

        LayerMask layer = LayerMask.GetMask("Target");

        Collider[] hitColliders = Physics.OverlapSphere(detector.transform.position, detector.radius, layer);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            detector.enabled = false;

            Damageable target = hitColliders[i].GetComponent<Damageable>();
            if (target != null)
            {
                target.TakeDamage(10);
            }

            Debug.Log("Hit");
        }
    }
}
