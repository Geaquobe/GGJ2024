using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{

    public enum ObjectType
    {
        GRAB,
        INTERACTION,
        OPEN
    }

    [SerializeField] private ObjectType type = ObjectType.GRAB;
    [SerializeField] private Vector3 position = Vector3.zero;
    [SerializeField] private Vector3 rotation = Vector3.zero;


    [SerializeField] private KeyCode grabKey = KeyCode.A;
    [SerializeField] private KeyCode throwKey = KeyCode.Z;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode openKey = KeyCode.R;
    [SerializeField] private KeyCode collisionKey = KeyCode.T;

    private bool opened = false;




    private void Update()
    {
        if (Input.GetKeyDown(grabKey))
        {
            Grab();
        }
        else if (Input.GetKeyDown(throwKey))
        {
            Throw();
        }
        else if (Input.GetKeyDown(interactKey))
        {
            Interact();
        }
        else if (Input.GetKeyDown(openKey))
        {
            Open();
        }
        else if (Input.GetKeyDown(collisionKey))
        {
            Collision();
        }
    }

    public void Grab()
    {
        Debug.Log("Grab");
        transform.localPosition = position;
        transform.localEulerAngles = rotation;

        // GRAB SOUND
    }

    public void Throw()
    {
        Debug.Log("Throw");

        // THROW SOUND
    }

    public void Interact()
    {
        Debug.Log("Interact");

        // INTERACT SOUND


    }

    public void Open()
    {
        Debug.Log("Open");

        if (opened) // OPEN SOUND
        {

        }
        else // CLOSE SOUND
        {

        }
        transform.localPosition = opened ? rotation : position;
        opened = !opened;
    }

    private void Collision()
    {
        Debug.Log("Collision");

        // COLLISION SOUND

    }

    private void OnCollisionEnter(Collision collision)
    {
        // COLLISION SOUND
    }
}
