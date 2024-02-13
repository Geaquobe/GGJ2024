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

    [TabGroup("tab2", "Jump")]
    [SerializeField] private float jumpUpVel = 7.5f;

    private Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        
        if(Physics.Raycast(playerCollider.bounds.center, Vector3.down, out hit, groundDistance))
        {
            Debug.DrawLine(playerCollider.bounds.center, hit.point, Color.red);
            Debug.DrawLine(hit.point, new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.center.y - (groundDistance - hit.point.y), playerCollider.bounds.center.z), Color.yellow);
            //Debug.Log(hit.collider.name);

            Vector3 vel = rb.velocity;
            Vector3 rayDir = transform.TransformDirection(DownDir);

            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = hit.rigidbody;
            if(hitBody != null)
            {
                otherVel = hitBody.velocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);

            float relVel = rayDirVel - otherDirVel;

            float y = hit.distance - floatingDistance;

            float springForce = (y * springStrength) - (relVel * springDamper);

            rb.AddForce(DownDir * springForce);

            if(hitBody != null)
            {
                hitBody.AddForceAtPosition(rayDir * -springForce, hit.point);
            }
        } 
        else
        {
            Debug.DrawLine(playerCollider.bounds.center, Vector3.down * groundDistance, Color.blue);
        }

        UpdateRotation();
    }

    public void UpdateRotation()
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
