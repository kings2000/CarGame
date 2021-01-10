using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheel : MonoBehaviour
{

    public Rigidbody rigidbody;

    //public WheelType wheelType;

    [Header("Suspension")]
    public float restLenght;
    public float springTravel;
    public float springStiffness;
    public float damperStiffness;

    [Header("Wheel")]
    public float steerTime;

    public Transform wheelObj;

    private float minLenght;
    private float maxLenght;
    private float lastLenght;
    private float springLenght;
    private float springVelocity;
    private float springForce;
    private float damperForce;

    [HideInInspector] public float steerAngle;
    [HideInInspector] public float ForwardAcceleration;
    [HideInInspector] public float grip;

    private Vector3 suspensionForce;
    private Vector3 wheelVelocityLS;

    private float fx;
    private float fy;
    private float wheelAngle;
    private float fxAxis;
    private float directionalTorque;
    

    public float wheelRadius;

    void Start()
    {
        rigidbody = GetComponentInParent<Rigidbody>();
        
    }


    private void Update()
    {
        minLenght = restLenght - springTravel;
        maxLenght = restLenght + springTravel;
        wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, steerTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * wheelAngle);
    }

    public void Move(float motion)
    {
        fxAxis = motion;
    }

    public void Brake()
    {
        //float carVelocityX = rigidbody.velocity.z * Mathf.Sign(transform.forward.z);
        //float carVelocityY = rigidbody.velocity.x * Mathf.Sign(transform.forward.x);
        float v = transform.InverseTransformDirection(rigidbody.velocity).z;
        fxAxis = -v * .25f;
    }

    private void FixedUpdate()
    {
        
        Debug.DrawLine(transform.position, transform.position -transform.up * (springLenght), Color.red);
        
        fxAxis = Input.GetAxis("Vertical");

        float rpm = (rigidbody.velocity.magnitude) / (Mathf.PI * wheelRadius * 2);
        directionalTorque = (rigidbody.GetPointVelocity(wheelObj.position).z * transform.forward).z;
        wheelObj.Rotate(Vector3.right * rpm * 360 * Time.deltaTime * Mathf.Sign(directionalTorque));

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLenght + wheelRadius)){


            if (Input.GetKey(KeyCode.Space))
            {
                Brake();
            }


            lastLenght = springLenght;
            springLenght = hit.distance - wheelRadius;
            springLenght = Mathf.Clamp(springLenght, minLenght, restLenght);
            springVelocity = (lastLenght - springLenght) / Time.fixedDeltaTime;
            springForce = springStiffness * (restLenght - springLenght);
            damperForce = damperStiffness * springVelocity;

            suspensionForce = (springForce + damperForce) * transform.up;

            wheelVelocityLS = transform.InverseTransformDirection(rigidbody.GetPointVelocity(hit.point));
            fx =  fxAxis * 0.1f *springForce;
            
            fy = wheelVelocityLS.x * grip * springForce;

            Vector3 forwardForce = (fx * transform.forward * ForwardAcceleration) + (fy * -transform.right) ;
            rigidbody.AddForceAtPosition(suspensionForce + forwardForce, hit.point, ForceMode.Force);
            wheelObj.localPosition = new Vector3(wheelObj.localPosition.x, (-springLenght), wheelObj.localPosition.z);
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(wheelObj != null)
        {
            Gizmos.DrawWireSphere(wheelObj.position, wheelRadius);
            Gizmos.DrawLine(transform.position, wheelObj.position);
        }
            

    }
}
