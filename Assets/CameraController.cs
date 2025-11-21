using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float speed = 0.006f;
    [SerializeField]
    private float zOffset = -40f;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            var targetPosition = new Vector3(target.position.x, target.position.y, zOffset);
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed);
        }
    }
}
