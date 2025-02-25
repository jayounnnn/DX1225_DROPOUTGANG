/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHandler : MonoBehaviour
{
    [SerializeField] private SphereCollider[] detectors;

    public void EnableEnemyCollider(int index)
    {
        if (index < 0 || index >= detectors.Length)
        {
            //Debug.LogError($"Index {index} is out of bounds! Detectors array size: {detectors.Length}");
            return;
        }

        SphereCollider detector = detectors[index];
        detector.enabled = true;

        Collider[] hitColliders = Physics.OverlapSphere(detector.transform.position, detector.radius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            detector.enabled = false;

            if (hitColliders[i].CompareTag("Player")) 
            {
                Damageable target = hitColliders[i].GetComponent<Damageable>();
                PlayerCombat targetParry = target.GetComponent<PlayerCombat>();

                if (targetParry != null && targetParry.IsParrying())
                {
                    Rigidbody enemyRb = GetComponent<Rigidbody>();
                    if (enemyRb != null)
                    {
                        Vector3 knockbackDirection = (enemyRb.transform.position - target.transform.position).normalized;
                        enemyRb.AddForce(knockbackDirection * 5f, ForceMode.Impulse);
                    }
                }

                if (target != null)
                {
                    target.TakeDamage(10);
                }

                Debug.Log("Enemy hit Player");
            }
        }
    }
}
*/