using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public Rigidbody2D playerRb;
    public LayerMask groundLayer;
    public Vector3 offset;
    // would really like to up the smoothSpeed, but seems to cause weird jittering interactions with the rotation speed on anything over 1 which gets worse
    public float smoothSpeed;
    public float rotationSpeed;


    void LateUpdate() {
        Vector3 desiredPosition = playerTransform.position+offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        if (playerRb.IsTouchingLayers(groundLayer)) { 
            Quaternion desiredRotation = playerTransform.rotation;
            Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = smoothedRotation;
        }
    }
}
