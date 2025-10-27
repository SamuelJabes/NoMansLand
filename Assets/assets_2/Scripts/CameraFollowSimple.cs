using UnityEngine;

public class CameraFollowSimple : MonoBehaviour
{
    public Transform target;                 // Player
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothTime = 0.15f;         // 0 = grudado; 0.1~0.2 = suave

    Vector3 vel;

    void LateUpdate()
    {
        if (!target) return;
        Vector3 desired = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref vel, smoothTime);
    }
}
