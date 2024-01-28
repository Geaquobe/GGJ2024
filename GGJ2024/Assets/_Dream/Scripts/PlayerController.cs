using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;

    [SerializeField] private float groundDrag;

    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpCooldown = 0.1f;
    [SerializeField] private float airMultiplier = 2f;
    private bool canJump = false;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode grabKey = KeyCode.E;
    [SerializeField] private KeyCode throwKey = KeyCode.Mouse0;




    [SerializeField] private Transform orientation;

    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool grounded;

    [SerializeField] private Transform objectHolder;
    [SerializeField] private Transform objectCamera;

    //[SerializeField] private float throwForce = 500f;
    [SerializeField] private float minThrow = 50f;
    [SerializeField] private float maxThrow = 500f;
    [SerializeField] private float timeMinToMax = 1f;
    private float throwForce = 0;


    [SerializeField] private float pickUpRange = 5f;
    private float rotationSensitivity = 1f;


    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rigidbody;


    private void Start()
    {
        LayerNumber = LayerMask.NameToLayer("Hold");

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;

        canJump = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);

        MyInput();
        SpeedControl();

        //if(grounded)
        //{
        //    canJump = true;
        //}
        //else
        //{
        //    canJump = false;

        //}

        rigidbody.drag = grounded ? groundDrag : 0;

        
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
        {
            heldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
            heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //assign Rigidbody
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = objectHolder.transform; //parent object to holdposition
            heldObj.layer = LayerNumber; //change the object layer to the holdLayer
            //make sure object doesnt collide with player, it can cause weird bugs
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), this.GetComponentInChildren<CapsuleCollider>(), true);
            pickUpObj.transform.GetComponent<Object>().Grab();
        }
    }

    void DropObject()
    {
        //re-enable collision with player
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), this.GetComponent<Collider>(), false);
        heldObj.layer = 0; //object assigned back to default layer
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null; //unparent object
        heldObj = null; //undefine game object
    }

    void MoveObject()
    {
        //keep object position the same as the holdPosition position
        heldObj.transform.position = objectHolder.transform.position;
    }

    void RotateObject()
    {
        //    if (Input.GetKey(KeyCode.R))//hold R key to rotate, change this to whatever key you want
        //    {
        //        canDrop = false; //make sure throwing can't occur during rotating

        //        //disable player being able to look around
        //        //mouseLookScript.verticalSensitivity = 0f;
        //        //mouseLookScript.lateralSensitivity = 0f;

        //        float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
        //        float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
        //        //rotate the object depending on mouse X-Y Axis
        //        heldObj.transform.Rotate(Vector3.down, XaxisRotation);
        //        heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        //    }
        //    else
        //    {
        //        //re-enable player being able to look around
        //        //mouseLookScript.verticalSensitivity = originalvalue;
        //        //mouseLookScript.lateralSensitivity = originalvalue;
        //        canDrop = true;
        //    }
    }

void ThrowObject()
    {
        //same as drop function, but add force to object before undefining it
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), this.GetComponentInChildren<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(objectCamera.forward * throwForce);
        heldObjRb.AddTorque(objectCamera.forward * throwForce * 0.01f/* + objectCamera.right * throwForce * 0.01f + objectCamera.up * throwForce * 0.01f*/);
        heldObj = null;
    }
    void StopClipping() //function only called when dropping/throwing
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && grounded && canJump)
        {
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(grabKey))
        {
            Debug.Log("Grab");
            if (heldObj == null) //if currently not holding anything
            {
                //perform raycast to check if player is looking at object within pickuprange
                RaycastHit hit;
                Color color = Color.red;
                Debug.DrawLine(objectCamera.position, objectCamera.TransformDirection(Vector3.forward) * pickUpRange, color);
                if (Physics.Raycast(objectCamera.position, objectCamera.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    //make sure pickup tag is attached
                    if (hit.transform.gameObject.tag == "PickUp")
                    {
                        //pass in object hit into the PickUpObject function
                        PickUpObject(hit.transform.gameObject);
                    }
                    else if (hit.transform.gameObject.tag == "Open")
                    {
                        hit.transform.GetComponent<Object>().Open();
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    StopClipping(); //prevents object from clipping through walls
                    DropObject();
                }
            }
        }
        if (heldObj != null) //if player is holding object
        {
            MoveObject(); //keep object position at holdPos
            RotateObject();
            if (Input.GetKeyDown(throwKey) && canDrop == true) //Mous0 (leftclick) is used to throw, change this if you want another button to be used)
            {
                throwForce = minThrow;
            }
            else if (Input.GetKey(throwKey))
            {
                throwForce = throwForce > maxThrow ? maxThrow : throwForce + (maxThrow - minThrow) * Time.deltaTime;
            }
            else if (Input.GetKeyUp(throwKey))
            {
                StopClipping();
                ThrowObject();
            }
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(grounded)
        {
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Acceleration);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rigidbody.velocity = new Vector3(limitedVelocity.x, rigidbody.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        canJump = false;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
    }
}
