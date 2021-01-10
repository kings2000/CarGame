using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspension : MonoBehaviour
{

    public List<GameObject> springs;
    public List<GameObject> wheels;
    public Dictionary<GameObject, GameObject> points;

    public Vector3 cgOffset;

    public float springStiffness;
    public float maxForce;
    public float maxDistance;
    public float wheelRaduis = 0.85f;
    public float dampingFactor;


    private Rigidbody rbody;
    private Vector3 initialCg;
    
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        initialCg = rbody.centerOfMass;
        points = new Dictionary<GameObject, GameObject>();
        for (int i = 0; i < springs.Count; i++)
        {
            points.Add(springs[i], wheels[i]);
        }

    }


    private void FixedUpdate()
    {

        //Vector3 cg = rbody.centerOfMass;
        //cg += cgOffset;
        rbody.centerOfMass = initialCg + cgOffset;
        //print(cg);

        for (int i = 0; i < springs.Count; i++)
        {
            GameObject obj = springs[i];
            GameObject wheel;
            bool found = points.TryGetValue(obj, out wheel);

            Debug.DrawLine(obj.transform.position, obj.transform.position - (transform.up * springStiffness), Color.red);
            if (Physics.Raycast(obj.transform.position, -transform.up, out RaycastHit hit, maxDistance))
            {
                
                float damping = dampingFactor * Vector3.Dot(rbody.GetPointVelocity(obj.transform.position), obj.transform.up);
                Vector3 force = transform.up * maxForce * Time.fixedDeltaTime * Mathf.Max(((maxDistance - (hit.distance + wheelRaduis)) / maxDistance - damping), 0);
                rbody.AddForceAtPosition(force, hit.point, ForceMode.Force);

                if(found)
                    wheel.transform.localPosition = new Vector3(wheel.transform.localPosition.x, Mathf.Min(Mathf.Max((wheelRaduis - hit.distance), -springStiffness),0), wheel.transform.localPosition.z);

            }else if (found)
            {
                wheel.transform.localPosition = new Vector3(wheel.transform.localPosition.x, ((-springStiffness)), wheel.transform.localPosition.z);
            }
        }
        
    }

}
