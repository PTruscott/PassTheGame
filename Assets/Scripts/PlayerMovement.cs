using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public float speed;
    public float rotationSpeed;
    public float downSpeed;

    public float jumpForce;

    public float xInput;

    public string planetTag = "Planet";
    public LayerMask groundLayer;
    public Transform feetPosition;
    public float groundCheck;

    // 0 is on the ground, 1 is in the air and haven't djumped. 2 is djumped
    public int doubleJumpState;
    public bool isJumping;

    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;

    public float gravityStrength = 0.25f;

    public GameObject stringPrefab;
    public float hingeBreakForce = 1000.0f;

    private GameObject prevString;

    // Start is called before the first frame update
    void Start()
    {
        // var p1 = CreateStringPiece(new Vector3(5, 0, 0), toPos: new Vector3(6, 0, 0));
        // var p2 = CreateStringPiece(new Vector3(4, 0, 0), p1);
        // var p3 = CreateStringPiece(new Vector3(3, 0, 0), p2);
    }

    Vector2 GetGravityAt(Vector2 position, float mass)
    {
        var planets = GameObject.FindGameObjectsWithTag(planetTag);
        var gravity = new Vector2();
        foreach (var planet in planets)
        {
            // Assume circle
            // A=pi∗a/2∗b/2
            var area = Math.PI * planet.transform.localScale.x / 2 * planet.transform.localScale.y / 2;
            var planetMass = area;
            var toPlanet = new Vector2(planet.transform.position.x, planet.transform.position.y) - position;
            // F = (G * M * m) / (r^2)
            var r = toPlanet.magnitude;
            var planetGravity = toPlanet * (float)((Physics2D.gravity.magnitude * gravityStrength * mass * planetMass) / (r * r));
            gravity += planetGravity;
            Debug.DrawLine(position, position + planetGravity * 0.05f, Color.green);
        }
        Debug.DrawLine(position, position + gravity * 0.05f, Color.red);
        return gravity;
    }

    // Update is called once per frame
    void Update()
    {
        // counteract the gravity before applying our own
        // playerRb.AddForce(-Physics2D.gravity, ForceMode2D.Force);

        // Vector2 closestPoint = FindClosestPoint(transform.position, groundLayer);
        // SmoothRotateTowardsPoint(closestPoint);

        // Debug.DrawLine(transform.position, closestPoint, Color.red); // Draw a line from currentPosition to the closest point

        // if (playerRb.IsTouchingLayers(groundLayer)) {
        //     transformOnGround = CreateDeepCopy(transform, transformOnGround);
        //     Vector2 pos = transform.position;
        //     newGravityDirection = closestPoint-pos;
        // }

        // xInput = Input.GetAxisRaw("Horizontal");
        // Vector2 inputVel = transformOnGround.right * speed;

        // if (xInput != 0)
        // {
        //     inputVel = inputVel * xInput;
        //     playerRb.velocity = CalculatePerpendicularComponent(playerRb.velocity, inputVel) + inputVel;
        // }
        // else
        // {
        //     playerRb.velocity = CalculatePerpendicularComponent(playerRb.velocity, inputVel);
        // }

        // if (playerRb.IsTouchingLayers(groundLayer))
        // {
        //     if (!Input.GetButton("Jump") && !Input.GetKey(KeyCode.S))
        //     {
        //         // if on ground we reset
        //         doubleJumpState = 0;
        //     }

        //     if (Input.GetButtonDown("Jump"))
        //     {
        //         playerRb.velocity = transformOnGround.up * jumpForce;
        //     }

        //     if (Input.GetKeyDown(KeyCode.S))
        //     {
        //         playerRb.velocity = transformOnGround.up * -jumpForce;
        //     }


        // }


        // if (Input.GetButtonDown("Jump") && doubleJumpState == 1)
        // {
        //     playerRb.velocity = transformOnGround.up * jumpForce;
        //     doubleJumpState = 2;
        // }

        // if (Input.GetKeyDown(KeyCode.S) && doubleJumpState == 1)
        // {
        //     playerRb.velocity = transformOnGround.up * -jumpForce;
        //     doubleJumpState = 2;
        // }

        // if (Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.S))
        // {
        //     // only allow double jump post key being released
        //     if (doubleJumpState == 0)
        //     {
        //         doubleJumpState = 1;
        //     }
        // }
    }

    GameObject CreateStringPiece(Vector3 fromPos, GameObject prevStringPiece = null, Vector3? toPos = null)
    {
        var newString = Instantiate(stringPrefab);

        var anchorOffset = 0.05f;

        var toPosValue = toPos ?? prevStringPiece.transform.TransformPoint(new Vector3(-0.5f + anchorOffset * 2, 0, 0));
        var diff = toPosValue - fromPos;
        newString.transform.position = (fromPos + toPosValue) / 2;
        newString.transform.localScale = new Vector3(diff.magnitude, newString.transform.localScale.y, newString.transform.localScale.z);
        var rotation = Mathf.Rad2Deg * (float)Math.Atan2(diff.y, diff.x);
        newString.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.forward);
        // Debug.Log($"fromPos: {fromPos}; toPos: {toPosValue}; rotation: {rotation}");

        if (prevStringPiece != null)
        {
            var hinge = prevStringPiece.AddComponent<HingeJoint2D>();
            hinge.anchor = new Vector2(-0.5f + anchorOffset, 0);
            hinge.connectedBody = newString.GetComponent<Rigidbody2D>();
            hinge.connectedAnchor = new Vector2(0.5f - anchorOffset, 0);
            hinge.breakForce = hingeBreakForce;
            // Debug.Log($"anchor: {hinge.anchor}; connectedAnchor: {hinge.connectedAnchor}");
        }
        return newString;
    }

    // 50 ticks a second
    void FixedUpdate()
    {
        var gravity = GetGravityAt(transform.position, playerRb.mass);
        playerRb.AddForce(gravity);

        var down = gravity.normalized;
        var right = new Vector2(-down.y, down.x);
        var xInput = Input.GetAxisRaw("Horizontal");
        playerRb.AddForce(right * xInput * Time.fixedDeltaTime * 300.0f);
        Debug.DrawLine(transform.position, transform.position + new Vector3(right.x, right.y, 0), Color.blue);

        var yInput = Input.GetAxisRaw("Vertical");
        playerRb.AddForce(-down * yInput * Time.fixedDeltaTime * 500.0f);

        var towardsMouseVec3 = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        var towardsMouse = new Vector3(towardsMouseVec3.x, towardsMouseVec3.y);
        Debug.DrawLine(transform.position, transform.position + new Vector3(towardsMouse.x, towardsMouse.y, 0), Color.magenta);
        // Physics2D.IgnoreLayerCollision(7, 7);
        if (Input.GetMouseButton(0))
        {
            prevString = CreateStringPiece(transform.position, prevString, prevString != null ? null : transform.position + towardsMouse * Time.deltaTime);
            prevString.GetComponent<Rigidbody2D>().velocity = towardsMouse * 2.0f;
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), prevString.GetComponent<Collider2D>());
        }
        else
        {
            prevString = null;
        }

        // if (KBCounter > 0)
        // {
        //     playerRb.velocity = transformOnGround.up * KBForce;

        //     KBCounter -= Time.deltaTime;
        // }

        // if (Input.GetKey(KeyCode.Q))
        // {
        //     Quaternion localRotation = Quaternion.Euler(0f, 0f, rotationSpeed);
        //     transform.rotation = transform.rotation * localRotation;
        // }

        // if (Input.GetKey(KeyCode.E))
        // {
        //     Quaternion localRotation = Quaternion.Euler(0f, 0f, -rotationSpeed);
        //     transform.rotation = transform.rotation * localRotation;
        // }
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
        if (targetT == null)
        {
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

        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle + 90);
    }
}
