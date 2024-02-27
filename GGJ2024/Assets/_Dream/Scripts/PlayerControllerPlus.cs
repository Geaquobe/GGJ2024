using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerControllerPlus : MonoBehaviour
{
    private Rigidbody rb;
    private Collider playerCollider;

    [TabGroup("Raycast")]
    [SerializeField] private Vector3 DownDir = new Vector3(0,-1,0);
    [TabGroup("Raycast")]
    [SerializeField] private float groundDistance = 1;
    [TabGroup("Raycast")]
    [SerializeField] private float floatingDistance = 0.5f;

    [TabGroup("Spring")]
    [SerializeField] private float springStrength = 1f;
    [TabGroup("Spring")]
    [SerializeField] private float springDamper = 1f;
    [TabGroup("Spring")]
    [SerializeField] private float rotationStrength = 1f;
    [TabGroup("Spring")]
    [SerializeField] private float rotationDamper = 1f;

    [TabGroup("tab2", "Locomotion")]
    [SerializeField] private float maxSpeed = 8;
    [TabGroup("tab2", "Locomotion")]
    [SerializeField] private float acceleration = 200;
    [TabGroup("tab2", "Locomotion")]
    [SerializeField] private AnimationCurve accelerationFactorFromDot;
    [TabGroup("tab2", "Locomotion")]
    [SerializeField] private float maxAccelForce = 150;
    [TabGroup("tab2", "Locomotion")]
    [SerializeField] private AnimationCurve maxAccelerationFactorFromDot;
    [TabGroup("tab2", "Locomotion")]
    [SerializeField] private Vector3 forceScale = new Vector3(1,0,1);
    [TabGroup("tab2", "Locomotion")]
    [SerializeField] private float gravityScaleDrop = 10;

    [TabGroup("tab2", "Jump")]
    [SerializeField] private float jumpUpVel = 7.5f;
    [TabGroup("tab2", "Jump")]
    [SerializeField] private AnimationCurve jumpUpVelFactorFromExistingY;
    [TabGroup("tab2", "Jump")]
    [SerializeField] private AnimationCurve analogJumpUpForce;
    [TabGroup("tab2", "Jump")]
    [SerializeField] private float jumpTerminalVelocity = 22.5f;
    [TabGroup("tab2", "Jump")]
    [SerializeField] private float jumpDuration = 0.6667f;


    [SerializeField] private Transform orientation;
    [SerializeField] private Transform objectHolder;

    [SerializeField] private float xSensitivity = 100f;
    [SerializeField] private float ySensitivity = 100f;
    [SerializeField] private Camera camera;
    [SerializeField] private Vector3 moveForceScale = new Vector3(1f, 0f, 1f);
    private float xRotation = 0f;
    private float yRotation = 0f;

    private Vector3 moveInput;
    private Vector3 goalVelocity;



    private Quaternion initialRotation;
    private float speedFactor = 1f;
    private float maxAccelForceFactor = 1f;
    private float leanFactor = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        initialRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
    }

    private void FixedUpdate()
    {
        ApplyPosition();
        ApplyHover();
        ApplyRotation();
    }

    private void LateUpdate()
    {
        ApplyCamera();
    }

    private void ProcessInputs()
    {
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Debug.Log(moveInput);
    }

    private void ApplyPosition()
    {
        Vector3 unitGoal = moveInput.x * orientation.right + moveInput.z * orientation.forward;
        Vector3 unitVel = goalVelocity.normalized;
        float velDot = Vector3.Dot(unitGoal, unitVel);
        float accel = acceleration * accelerationFactorFromDot.Evaluate(velDot);
        Vector3 goalVel = unitGoal * maxSpeed * speedFactor;
        goalVelocity = Vector3.MoveTowards(goalVelocity, goalVel, accel * Time.fixedDeltaTime);
        Vector3 neededAccel = (goalVelocity - rb.velocity) / Time.fixedDeltaTime;
        float maxAccel = maxAccelForce * maxAccelerationFactorFromDot.Evaluate(velDot) * maxAccelForceFactor;
        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
        //rb.AddForceAtPosition(Vector3.Scale(neededAccel * rb.mass, moveForceScale), transform.position + new Vector3(0f, transform.localScale.y * leanFactor, 0f));
        rb.AddForceAtPosition(Vector3.Scale(neededAccel * rb.mass, moveForceScale), playerCollider.bounds.center);

    }

    private void ApplyHover()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerCollider.bounds.center, Vector3.down, out hit, groundDistance))
        {
            Debug.DrawLine(playerCollider.bounds.center, hit.point, Color.red);
            Debug.DrawLine(hit.point, new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.center.y - (groundDistance - hit.point.y), playerCollider.bounds.center.z), Color.yellow);
            //Debug.Log(hit.collider.name);

            Vector3 vel = rb.velocity;
            Vector3 rayDir = transform.TransformDirection(DownDir);

            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = hit.rigidbody;
            if (hitBody != null)
            {
                otherVel = hitBody.velocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);

            float relVel = rayDirVel - otherDirVel;

            float y = hit.distance - floatingDistance;

            float springForce = (y * springStrength) - (relVel * springDamper);

            rb.AddForce(DownDir * springForce);

            //if (hitBody != null)
            //{
            //    hitBody.AddForceAtPosition(rayDir * -springForce, hit.point);
            //}
        }
        else
        {
            Debug.DrawLine(playerCollider.bounds.center, Vector3.down * groundDistance, Color.blue);
        }
    }

    private void ApplyRotation()
    {
        Quaternion characterCurrent = transform.rotation;
        Quaternion toGoal = ShortestRotation(initialRotation, characterCurrent);

        Vector3 rotAxis;
        float rotDegrees;

        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;

        rb.AddTorque((rotAxis * (rotRadians * rotationStrength)) - (rb.angularVelocity * rotationDamper));
    }

    private void ApplyCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        yRotation += mouseX;

        camera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    public static Quaternion ShortestRotation(Quaternion a, Quaternion b)
    {
        if (Quaternion.Dot(a, b) < 0)
        {
            return a * Quaternion.Inverse(Multiply(b, -1));
        }
        else return a * Quaternion.Inverse(b);
    }



    public static Quaternion Multiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }
}
