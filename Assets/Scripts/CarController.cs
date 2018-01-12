using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WheelPairInfo {
    public WheelCollider leftWheel;
    public GameObject leftWheelMesh;
    public WheelCollider rightWheel;
    public GameObject rightWheelMesh;

    public bool driveWheel; // True for wheels that apply force
    public bool steering;  // True for wheels that steer
}

public class CarController : MonoBehaviour {

    public List<WheelPairInfo> wheelPairInfos; // List of all wheel pairs
    public float maxMotorTorque;
    public float maxSteeringAngle;
    Rigidbody rb;
    public float torque;
    public Text torqueText;
    public Text velocityText;


    public void Start(){
        rb = GetComponent<Rigidbody>();
    }

    public void PositionWheelMeshes(WheelPairInfo wheelPair, float steerAngle){
        wheelPair.leftWheelMesh.transform.Rotate(Vector3.right, Time.deltaTime * wheelPair.leftWheel.rpm * 5, Space.Self);
        //if (wheelPair.steering) {
        //    Quaternion wheelAngle = wheelPair.leftWheelMesh.transform.localRotation;
        //    print(wheelAngle);
        //    Quaternion wheelAngleNew = Quaternion.Euler(wheelAngle.x, wheelAngle.y + steerAngle, 0);
        //    wheelPair.leftWheelMesh.transform.Rotate(wheelAngleNew.eulerAngles, Space.World);
        //}

        wheelPair.rightWheelMesh.transform.Rotate(-Vector3.right, Time.deltaTime * wheelPair.rightWheel.rpm * 5, Space.Self);
    }

    public void FixedUpdate(){
        torque = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        float brake = -1 * Mathf.Clamp(Input.GetAxis("Vertical"), -1, 0);

        foreach (WheelPairInfo wheelPair in wheelPairInfos) {
            wheelPair.leftWheel.brakeTorque = 0;
            wheelPair.rightWheel.brakeTorque = 0;

            // Steering
            if (wheelPair.steering) {
                wheelPair.leftWheel.steerAngle = steering;
                wheelPair.rightWheel.steerAngle = steering;
            }

            // Drive wheel
            if (wheelPair.driveWheel) {
                wheelPair.leftWheel.motorTorque = torque;
                wheelPair.rightWheel.motorTorque = torque;
                wheelPair.leftWheel.brakeTorque = brake;
                wheelPair.rightWheel.brakeTorque = brake;
            }

            PositionWheelMeshes(wheelPair, steering);
        }

        torqueText.text = "Torque: " + torque.ToString();
        velocityText.text = "Velocity: " + rb.velocity.magnitude.ToString();
    }
}
