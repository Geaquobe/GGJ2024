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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
