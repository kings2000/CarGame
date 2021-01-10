using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float hoverHeight = 3.5f;
    public float hoverforce = 65f;

    Rigidbody carBody;

    private void Awake()
    {
        carBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        
    }
}
