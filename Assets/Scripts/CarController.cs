using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarController : MonoBehaviour
{

    public Wheel[] wheels;
    public Collider collider;

    [Header("Car Spec")]
    public float topSpeed = 30f;
    public float wheelBase;
    public float rearTrack;
    public float turnRaduis;
    public float ForwardAcceleration = 200;
    public float floorCooeficient = 0.05f;
    public float frictionDrag;
    public float grip;
    


    [Range(0.0f, 20f)]
    [SerializeField] float driftIntensity = 1f;
    [SerializeField] AnimationCurve turnInputCurve;

    public float steerInput;
    public Transform cgObject;

    private float ackermanAngleLeft;
    private float ackermanAngleRight;
    private bool drift;
    private float maxZRotation = 30f;

    Rigidbody rbody;
    Bounds bounds = new Bounds();
    private float speed;

    private void Start()
    {
        //bounds = collider.bounds;
        //bounds.Encapsulate(,);

        rbody = GetComponent<Rigidbody>();
        for (int i = 0; i < wheels.Length; i++)
        {
            bounds.Encapsulate(wheels[i].wheel.transform.position);
            wheels[i].wheel.ForwardAcceleration = ForwardAcceleration;
            
        }
        rbody.centerOfMass = cgObject.localPosition;
    }

    private void Update()
    {
        steerInput = Input.GetAxisRaw("Horizontal");
        drift = Input.GetKey(KeyCode.Q) && rbody.velocity.sqrMagnitude > 100;
         
        if (steerInput > 0)
        {
            ackermanAngleLeft  = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRaduis + (rearTrack/2))) * steerInput;
            ackermanAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRaduis - (rearTrack/2))) * steerInput;

        }else if(steerInput < 0)
        {
            ackermanAngleLeft  = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRaduis - (rearTrack / 2))) * steerInput;
            ackermanAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRaduis + (rearTrack / 2))) * steerInput;
        }
        else
        {
            ackermanAngleLeft  = 0;
            ackermanAngleRight = 0;
        }
         
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i].wheelType == WheelType.FrontLeft)
                wheels[i].wheel.steerAngle = ackermanAngleLeft;
            if (wheels[i].wheelType == WheelType.FrontRight)
                wheels[i].wheel.steerAngle = ackermanAngleRight;

            wheels[i].wheel.grip = grip;
        }
    }

    private void FixedUpdate()
    {
       

        rbody.velocity = Vector3.MoveTowards(rbody.velocity, Vector3.zero, frictionDrag * Time.deltaTime);
        Vector3 vel = transform.InverseTransformDirection(rbody.velocity);

        speed = vel.z * 3.6f;
        steerInput = Input.GetAxisRaw("Horizontal") * turnRaduis;
        

        //drift
        if (drift)
        {
            print(drift);
            Vector3 driftForce = -transform.right;
            driftForce.y = 0.0f;
            driftForce.Normalize();

            if (steerInput != 0)
                driftForce *= rbody.mass * (speed / 7f)  * (steerInput / turnRaduis);
            Vector3 driftTorque = Vector3.up * 0.01f * (steerInput / turnRaduis);

            rbody.AddForce(driftForce * driftIntensity, ForceMode.Force);
            rbody.AddTorque(driftTorque * driftIntensity, ForceMode.VelocityChange);
        }

        //vel.z = Mathf.Clamp(vel.z, -topSpeed, topSpeed);
        if (vel.z > topSpeed) vel.z = topSpeed;
        else if (vel.z < -topSpeed) vel.z = -topSpeed;
        rbody.velocity = transform.TransformDirection(vel);

        //transform.localRotation = ClampRotationAroundXAxis(transform.localRotation);
    }

    float Angle(float raw)
    {
        raw = (raw) % 360;             // Mod by 360, to not exceed 360
        if (raw > 180.0f) raw -= 360.0f;
        if (raw < -180.0f) raw += 360.0f;
        return raw;
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleX = Mathf.Clamp(angleX, -maxZRotation, maxZRotation);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(bounds.center, bounds.size);
    }
}

 

[System.Serializable]
public struct Wheel
{
    public CarWheel wheel;
    public WheelType wheelType;
}

public enum WheelType
{
    FrontLeft,
    FrontRight,
    BackLeft,
    BackRight
}
