using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WheelList
{
    public GameObject leftWheelMesh;
    public Transform leftWheelPosition;
    public GameObject rightWheelMesh;
    public Transform rightWheelPosition;
    public bool driveWheel; // True for wheels that apply force
    public bool steering;  // True for wheels that steer
}

public class ScratchController : MonoBehaviour {

    public List<float> transmission;

    float acceleration;
    bool grounded;
    Vector3 moveDir;
    int layerMask;
    public float rayDistance = 1;
    public List<WheelList> wheelPairs; // List of all wheel pairs
    //public GameObject[] wheels;
    public float wheelHeight;
    public float gravity = 9.81f;
    public LayerMask whatIsGround;
    public float steeringSpeed;
    public float torque;
    public float motorBrakeStrength;
    public float brakeStrength;
    public float handBrakeStrength;
    bool handBraking;
    Ray ray;
    RaycastHit hit;
    Rigidbody rb;

    public Text speedometerText;

    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    void Update(){
        foreach(WheelList wheelPair in wheelPairs) {
            Debug.DrawRay(wheelPair.leftWheelPosition.position, -transform.up, Color.red);
            Debug.DrawRay(wheelPair.rightWheelPosition.position, -transform.up, Color.red);
        }

        if (!grounded) {
            moveDir += (Vector3.down * Time.deltaTime * gravity);
        }

        acceleration = Input.GetAxis("Vertical");
        if (acceleration > 0.1f) {
            Accelerate();
        }else if(acceleration == 0 && !handBraking) {
            MotorBrake();
        }else if(acceleration < 0 && moveDir.z > -0.03f) {
            Reverse();
        }

        if (Input.GetKey(KeyCode.Space)) {
            HandBrake();
        }

        if (moveDir.sqrMagnitude > 0) {
            float steering = Input.GetAxis("Horizontal");
            transform.Rotate(0, steering * Time.deltaTime * steeringSpeed, 0, Space.Self);
        }

        if (Physics.Raycast(transform.position, -transform.up, out hit, rayDistance, whatIsGround)) {
            Debug.DrawRay(transform.position, -transform.up * rayDistance, Color.red);
            grounded = true;
            moveDir.y = 0;
        }
        else {
            grounded = false;
        }

        transform.Translate(moveDir);

        print(moveDir.sqrMagnitude);
        speedometerText.text = (moveDir.magnitude).ToString();
    }

    public void Accelerate(){
        moveDir += Vector3.forward * Time.deltaTime * acceleration * torque;
    }

    public void MotorBrake(){
        moveDir.z = Mathf.Lerp(moveDir.z, 0, Time.deltaTime * motorBrakeStrength);
    }

    public void HandBrake(){
        moveDir.z = Mathf.Lerp(moveDir.z, 0, Time.deltaTime * handBrakeStrength);
    }

    public void Reverse(){
        moveDir.z += Time.deltaTime * acceleration * brakeStrength;
    }

    public void Stop(){
        moveDir.z = 0;
    }
}
