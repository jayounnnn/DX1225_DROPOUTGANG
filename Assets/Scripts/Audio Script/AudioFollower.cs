using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFollower : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }
}
