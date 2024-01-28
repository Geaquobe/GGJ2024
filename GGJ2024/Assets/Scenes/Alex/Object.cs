using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{

    public enum ObjectType
    {
        GRAB,
        INTERACTION
    }

    [SerializeField] private ObjectType type = ObjectType.GRAB;
    [SerializeField] private Vector3 position = Vector3.zero;
    [SerializeField] private Vector3 rotation = Vector3.zero;

    public void Grab()
    {
        Debug.Log("Object Grabbed");
        Debug.Log(transform.eulerAngles);
        transform.localPosition = position;
        transform.localEulerAngles = rotation;
        Debug.Log(transform.eulerAngles);
    }
}
