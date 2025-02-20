using UnityEngine;

public class ConeCast3D : MonoBehaviour
{
    public float coneAngle = 30f;
    public float maxDistance = 10f;
    public int rayCountHorizontal = 10;
    public int rayCountVertical = 5;

    private Transform detectedPlayer;

    void Update()
    {
        detectedPlayer = CastCone3D();
    }

    public Transform GetDetectedPlayer()
    {
        return detectedPlayer;
    }

    private Transform CastCone3D()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        for (int i = 0; i < rayCountHorizontal; i++)
        {
            for (int j = 0; j < rayCountVertical; j++)
            {
                Vector3 rayDirection = GetDirectionWithinCone3D(direction, coneAngle, i, j);
                RaycastHit hit;

                if (Physics.Raycast(origin, rayDirection, out hit, maxDistance))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.DrawLine(origin, hit.point, Color.red);
                        return hit.collider.transform;
                    }
                }
            }
        }
        return null;
    }

    private Vector3 GetDirectionWithinCone3D(Vector3 direction, float coneAngle, int horizontalIndex, int verticalIndex)
    {
        float halfAngle = coneAngle * 0.5f * Mathf.Deg2Rad;
        float horizontalAngle = Mathf.Lerp(-halfAngle, halfAngle, (float)horizontalIndex / (rayCountHorizontal - 1));
        float verticalAngle = Mathf.Lerp(-halfAngle, halfAngle, (float)verticalIndex / (rayCountVertical - 1));

        Vector3 rayDirection = Quaternion.Euler(0, horizontalAngle * Mathf.Rad2Deg, 0) * direction;
        rayDirection = Quaternion.Euler(verticalAngle * Mathf.Rad2Deg, 0, 0) * rayDirection;

        return rayDirection.normalized;
    }
}
