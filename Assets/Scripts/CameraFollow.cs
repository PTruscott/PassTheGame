using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public Rigidbody2D playerRb;
    public LayerMask groundLayer;
    public Vector3 offset;
    // would really like to up the smoothSpeed, but seems to cause weird jittering interactions with the rotation speed on anything over 1 which gets worse
    // C: fixed by using FixedUpdate instead (probably some bug here but oh well)
    public float smoothSpeed;
    public float rotationSpeed;
    public float zoomSpeed = 1.0f;
    public float targetSize = 5;
    public float minSize = 5;
    public float maxSize = 20;

    float LerpFloat(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    void Update()
    {
        var scroll = Input.mouseScrollDelta.y;
        var diff = (float)Math.Log(targetSize) * scroll * 150f * Time.deltaTime;
        targetSize = Mathf.Clamp(targetSize - diff, minSize, maxSize);
        // if (diff != 0)
        // {
        //     Debug.Log($"scroll: {scroll} diff: {diff / Time.deltaTime} targetSize: {targetSize} size: {GetComponent<Camera>().orthographicSize}");
        // }
    }

    void FixedUpdate()
    {
        Vector3 desiredPosition = playerTransform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * 0.01f);
        transform.position = smoothedPosition;

        var camera = GetComponent<Camera>();
        camera.orthographicSize = LerpFloat(camera.orthographicSize, targetSize, zoomSpeed * 0.5f);
    }
}
