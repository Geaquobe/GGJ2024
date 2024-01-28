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

    [SerializeField] private bool audioTest = false;


    [SerializeField] private KeyCode grabKey = KeyCode.A;
    [SerializeField] private KeyCode throwKey = KeyCode.Z;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode openKey = KeyCode.R;
    [SerializeField] private KeyCode collisionKey = KeyCode.T;

    private bool opened = false;

    [SerializeField]
    private AK.Wwise.Event _Play_SFX_Grab;
    [SerializeField]
    private AK.Wwise.Event _Play_SFX_Throw;
    [SerializeField]
    private AK.Wwise.Event _Play_SFX_Impact;
    [SerializeField]
    private AK.Wwise.Event _Play_SFX_Interact;
    [SerializeField]
    private AK.Wwise.Event _Play_SFX_Open;
    [SerializeField]
    private AK.Wwise.Event _Play_SFX_Close;




    private void Update()
    {
        if(audioTest)
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

    }

    public void Grab()
    {
        Debug.Log("Grab");
        transform.localPosition = position;
        transform.localEulerAngles = rotation;

        _Play_SFX_Grab.Post(gameObject);

        // GRAB SOUND
    }

    public void Throw()
    {
        Debug.Log("Throw");

        _Play_SFX_Throw.Post(gameObject);

        // THROW SOUND
    }

    public void Interact()
    {
        Debug.Log("Interact");

        _Play_SFX_Interact.Post(gameObject);

        // INTERACT SOUND


    }

    public void Open()
    {
        Debug.Log("Open");

        if (opened) // OPEN SOUND
        {
            _Play_SFX_Open.Post(gameObject);
        }
        else // CLOSE SOUND
        {
            _Play_SFX_Close.Post(gameObject);
        }
        transform.localPosition = opened ? rotation : position;
        opened = !opened;
    }

    private void Collision()
    {
        Debug.Log("Collision");

        _Play_SFX_Impact.Post(gameObject);

        // COLLISION SOUND

    }

    private void OnCollisionEnter(Collision collision)
    {
        _Play_SFX_Impact.Post(gameObject);

        // COLLISION SOUND
    }
}
