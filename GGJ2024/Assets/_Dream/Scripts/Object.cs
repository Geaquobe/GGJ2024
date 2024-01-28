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
    private bool opened = false;
    public void Grab()
    {
        transform.localPosition = position;
        transform.localEulerAngles = rotation;

        // GRAB SOUND
    }

    public void Throw()
    {
        // THROW SOUND
    }

    public void Interact()
    {
        // INTERACT SOUND

    }

    public void Open()
    {
        if (opened) // OPEN SOUND
        {

        }
        else // CLOSE SOUND
        {

        }
        transform.localPosition = opened ? rotation : position;
        opened = !opened;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // COLLISION SOUND
    }
}
