using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarController3 : MonoBehaviour {

    public Text speedometerText;
    Rigidbody rb;
    float deadZone = 0.1f;
    public float groundedDrag = 3f;
    public float maxVelocity = 50;
    public float hoverForce = 1000;
    //public float gravityForce = 1000f;
    public float hoverHeight = 1.5f;
    public GameObject[] hoverPoints;

    public float forwardAcceleration = 8000f;
    public float reverseAcceleration = 4000f;
    float thrust = 0f;

    public float turnStrength = 1000f;
    float turnValue = 0f;

    public ParticleSystem[] dustTrails = new ParticleSystem[2];

    int layerMask;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.down;

        layerMask = 1 << LayerMask.NameToLayer("Vehicle");
        layerMask = ~layerMask;
    }

    //Uncomment this to see a visual indication of the raycast hit points in the editor window

     void OnDrawGizmos()
      {


       RaycastHit hit;
        for (int i = 0; i<hoverPoints.Length; i++)
        {
        var hoverPoint = hoverPoints[i];
        if (Physics.Raycast(hoverPoint.transform.position,
                            -transform.up, out hit,
                            hoverHeight,
                            layerMask)) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(hoverPoint.transform.position, hit.point);
            Gizmos.DrawSphere(hit.point, 0.5f);
        }
        else {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(hoverPoint.transform.position,
                           hoverPoint.transform.position - transform.up * hoverHeight);
        }
    }
    }

    void Update()
    {
        // Get thrust input
        thrust = 0.0f;
        float acceleration = Input.GetAxis("Vertical");
        if (acceleration > deadZone)
            thrust = acceleration * forwardAcceleration;
        else if (acceleration < -deadZone)
            thrust = acceleration * reverseAcceleration;

        // Get turning input
        turnValue = 0.0f;
        float turnAxis = Input.GetAxis("Horizontal");
        if (Mathf.Abs(turnAxis) > deadZone)
            turnValue = turnAxis;
    }

    void FixedUpdate()
    {

        //  Do hover/bounce force
        RaycastHit hit;
        bool grounded = false;
        for (int i = 0; i < hoverPoints.Length; i++) {
            var hoverPoint = hoverPoints[i];
            if (Physics.Raycast(hoverPoint.transform.position, -Vector3.up, out hit, hoverHeight, layerMask)) {
                rb.AddForceAtPosition(Vector3.up * hoverForce * (1.0f - (hit.distance / hoverHeight)), hoverPoint.transform.position);
                //print(Vector3.up * hoverForce * (1.0f - (hit.distance / hoverHeight)));
                grounded = true;
            }
            else {
                //// Self levelling - returns the vehicle to horizontal when not grounded and simulates gravity
                //if (transform.position.y > hoverPoint.transform.position.y) {
                //    rb.AddForceAtPosition(hoverPoint.transform.up * gravityForce, hoverPoint.transform.position);
                //}
                //else {
                //    rb.AddForceAtPosition(hoverPoint.transform.up * -gravityForce, hoverPoint.transform.position);
                //}
            }
        }

        var emissionRate = 0;
        if (grounded) {
            rb.drag = groundedDrag;
            emissionRate = 10;
        }
        else {
            rb.drag = 0.1f;
            thrust /= 100f;
            turnValue /= 100f;
        }

        //for (int i = 0; i < dustTrails.Length; i++) {
        //    var emission = dustTrails[i].emission;
        //    emission.rate = new ParticleSystem.MinMaxCurve(emissionRate);
        //}

        // Handle Forward and Reverse forces
        if (Mathf.Abs(thrust) > 0)
            rb.AddForce(transform.forward * thrust, ForceMode.Force);

        // Handle Turn forces
        if (turnValue > 0 && rb.velocity.magnitude > 2) {
            rb.AddRelativeTorque(Vector3.up * turnValue * turnStrength);
        }
        else if (turnValue < 0 && rb.velocity.magnitude > 2) {
            rb.AddRelativeTorque(Vector3.up * turnValue * turnStrength);
        }

        // Limit max velocity
        if (rb.velocity.sqrMagnitude > (rb.velocity.normalized * maxVelocity).sqrMagnitude) {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }

        if (speedometerText) {
            speedometerText.text = rb.velocity.magnitude.ToString();
        }
    }
}
