using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public float speed;
    public float rotationSpeed;
    public float downSpeed;

    public float jumpForce;

    public float xInput;

    public LayerMask groundLayer;
    public Transform feetPosition;
    public float groundCheck;

    // 0 is on the ground, 1 is in the air and haven't djumped. 2 is djumped
    public int doubleJumpState;
    public bool isJumping;

    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;

    Transform transformOnGround;

    public Vector2 newGravityDirection = new Vector2(0f, -1f);
    public float gravityStrength = 0.25f;

    public bool preserveDirection = true;

    // Start is called before the first frame update
    void Start()
    {
        // transformOnGround is needed to make control as expected in the air
        transformOnGround = CreateDeepCopy(transform);
    }

    // Update is called once per frame
    void Update() {

        Vector2 closestPoint = FindClosestPoint(transform.position, groundLayer);
        SmoothRotateTowardsPoint(closestPoint);

        Debug.DrawLine(transform.position, closestPoint, Color.red); // Draw a line from currentPosition to the closest point

        if (playerRb.IsTouchingLayers(groundLayer)) { 
            transformOnGround = CreateDeepCopy(transform, transformOnGround);
            Vector2 pos = transform.position;
            newGravityDirection = closestPoint-pos;
        }

        Vector2 gravityForce = newGravityDirection * Physics2D.gravity.magnitude * gravityStrength;
        playerRb.AddForce(gravityForce);

        xInput = Input.GetAxisRaw("Horizontal");
        Vector2 inputVel = transformOnGround.right * speed;

        if (xInput != 0) {
            inputVel = inputVel * xInput;
            playerRb.velocity = CalculatePerpendicularComponent(playerRb.velocity, inputVel) + inputVel; 
        }
        else {
            playerRb.velocity = CalculatePerpendicularComponent(playerRb.velocity, inputVel); 
        }

        if (playerRb.IsTouchingLayers(groundLayer)) { 
            if (!Input.GetButton("Jump") && !Input.GetKey(KeyCode.S)) {
                // if on ground we reset
                doubleJumpState = 0;
            }

            if (Input.GetButtonDown("Jump")) {
                playerRb.velocity = transformOnGround.up * jumpForce;
            }

            if (Input.GetKeyDown(KeyCode.S)) {
                playerRb.velocity = transformOnGround.up * -jumpForce;
            }            
        }


        if (Input.GetButtonDown("Jump") && doubleJumpState == 1) {
            playerRb.velocity = transformOnGround.up * jumpForce;
            doubleJumpState = 2;
        }

        if (Input.GetKeyDown(KeyCode.S) && doubleJumpState == 1) {
            playerRb.velocity = transformOnGround.up * -jumpForce;
            doubleJumpState = 2;
        }

        if (Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.S)) {
            // only allow double jump post key being released
            if (doubleJumpState == 0) {
                doubleJumpState = 1;
            }  
        } 
    }

    // 50 ticks a second
    void FixedUpdate() {

        // knockback jumps
        if (KBCounter > 0) {
            playerRb.velocity = transformOnGround.up * KBForce;

            KBCounter -= Time.deltaTime;
        }
    }

    private Vector2 CalculatePerpendicularComponent(Vector2 v1, Vector2 v2)
    {
        // Calculate the projection of v1 onto v2
        float dotProduct = Vector2.Dot(v1, v2);
        float v2MagnitudeSquared = v2.sqrMagnitude;
        float scalar = dotProduct / v2MagnitudeSquared;
        Vector2 projection = v2 * scalar;

        // Subtract the projection from v1 to get the perpendicular component
        Vector2 perpendicularComponent = v1 - projection;

        return perpendicularComponent;
    }

    private Transform CreateDeepCopy(Transform source, Transform targetT = null)
    {
        if (targetT == null) {
            GameObject newGameObject = new GameObject("DeepCopyTransform"); // Create an empty GameObject
            targetT = newGameObject.transform;
        }

        // Copy position, rotation, and scale from the source to the new transform
        targetT.position = source.position;
        targetT.rotation = source.rotation;
        targetT.localScale = source.localScale;

        return targetT;
    }

    Vector2 FindClosestPoint(Vector2 position, LayerMask layerMask)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, float.MaxValue, layerMask);
        float minDistance = float.MaxValue;
        Vector2 closestPoint = Vector2.zero;

        foreach (Collider2D collider in colliders)
        {
            Vector2 colliderPosition = collider.ClosestPoint(position);
            float distance = Vector2.Distance(position, colliderPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = colliderPosition;
            }
        }

        return closestPoint;
    }

    void SmoothRotateTowardsPoint(Vector2 targetPos)
    {
        Vector2 currentPosition = transform.position;
        Vector2 direction = targetPos - currentPosition;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // couldn't quite get smooth rotation working so binned for now
        // Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        // Debug.Log(targetAngle);
        // Debug.Log(targetRotation);
        // Debug.Log(transform.rotation);

        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle+90);
    }
}
