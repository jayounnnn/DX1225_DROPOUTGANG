using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCulling : MonoBehaviour
{
    public float renderDistance = 50f;  
    private Plane[] frustumPlanes;
    private GameObject[] trees;

    void Start()
    {
        trees = GameObject.FindGameObjectsWithTag("Tree");
    }

    void Update()
    {
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        foreach (GameObject tree in trees)
        {
            Renderer rend = tree.GetComponent<Renderer>();
            if (rend == null)
                continue;

            float distance = Vector3.Distance(transform.position, tree.transform.position);

            bool inView = GeometryUtility.TestPlanesAABB(frustumPlanes, rend.bounds);
            if (distance > renderDistance || !inView)
            {
                if (rend.enabled)
                    rend.enabled = false;
            }
            else
            {
                if (!rend.enabled)
                    rend.enabled = true;
            }
        }
    }
}
