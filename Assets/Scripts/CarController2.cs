using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WheelInfo
{
    public GameObject leftWheelMesh;
    public GameObject rightWheelMesh;

    //public bool driveWheel; // True for wheels that apply force
    public bool steering;  // True for wheels that steer
}

public class CarController2 : MonoBehaviour {

    private Coroutine oobCoroutine;
    //public GameObject boostIndicator;
    public List<WheelInfo> wheelPairs; // List of all wheel pairs
    public Text speedText;
    public Text gearText;
    public GameObject OobText;
    //public Text revText;
    public GameObject carMesh;
    public GameObject steeringWheel;
    public GameObject speedometer;
    public GameObject revMeter;
    float steeringWheelStartRot;
    float speedometerStartRot;
    float revmeterStartRot;
    public float maxSteeringAngle = 25;
    float acceleration;
    float steering;
    public float sidewaysDragMultiplier;
    Rigidbody rb;
    Animator anim;
    GameManager gm;

    public float maxSpeed;
    int numberOfGears = 6;
    int gearNum = 0;
    float revRangeBoundary = 1f;
    float gearFactor = 0;
    public float revs;
    float currentRevs;
    public float torque;
    public float steerModifier;
    public float airSteerModifier;
    public float airControlTimer; // Time before car can be turned mid-air
    float airTime = 0;
    bool airControl;
    public float slidingSpeed;
    public float angularVelocityLowSpeed;
    public float angularVelocityHighSpeed;
    public float angularVelocityHandBrake;
    [HideInInspector]
    public float moveSpeed;
    float wheelRotation;
    float rayMultiplier = 1;
    float prevVelocity = 2f;
    float prevHeight = 0;

    public Transform frontRightWheel;
    public Transform frontLeftWheel;
    public Transform backRightWheel;
    public Transform backLeftWheel;
    public float rayDistance;
    public float suspensionForce = 10;
    float groundTouchMultiplier; // Value depends on whether the car is touching the ground with one back wheel, both back wheels or neither.
    RaycastHit hit;
    [HideInInspector]
    public bool grounded;
    bool steeringEnabled;
    Vector3 velocity;
    float angle;

    public float slideTimer;
    float timeToSlide;
    public float slideQuickess;
    public GameObject[] smokeParticles;
    public GameObject boostParticles;
    public Transform skidTrail;
    public Transform leftSkidPosBack;
    public Transform rightSkidPosBack;
    public Transform leftSkidPosFront;
    public Transform rightSkidPosFront;
    ParticleSystem leftSmoke;
    ParticleSystem rightSmoke;
    ParticleSystem midSmoke;
    bool sliding;
    private Transform skidLeftBack;
    private Transform skidRightBack;
    private Transform skidLeftFront;
    private Transform skidRightFront;

    float yaw = 0;
    float pitch = 0;
    float roll = 0;

    Vector3 groundDir;
    bool handBraking;
    public static Transform skidTrailsDetachedParent;

    public Transform fr;
    public Transform br;
    public Transform fl;
    public Transform bl;

    public float wallAssistLength = 0.8f;
    bool boosting;
    public float boostTime = 2f;
    public float boostForce = 10f;
    float boostTimeLeft = 0f;
    AudioSource[] audios;
    AudioSource engineSound;
    AudioSource revvingSound;
    AudioSource skidSound;
    AudioSource crashSound;
    //float origPitch;
    bool drivable = true;
    Vector3 lastFrameVelocity = Vector3.zero;
    public GameObject visuals;
    public Light[] brakelights;
    bool oobRunning;

    CameraFollow cam;
    bool skipCountdown;
    LevelManager lm;

    void Start () {
        if (GameObject.FindGameObjectWithTag("GameManager") != null) {
            gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            visuals.GetComponent<Renderer>().material.color = gm.c_color;
            print("GM found");
        }
        cam = Camera.main.GetComponent<CameraFollow>();
        lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        audios = GetComponents<AudioSource>();
        engineSound = audios[0];
        revvingSound = audios[1];
        skidSound = audios[2];
        crashSound = audios[3];
        steeringWheelStartRot = steeringWheel.transform.localEulerAngles.z;
        steeringWheel.transform.localRotation = Quaternion.Euler(20, 0, 180);
        speedometerStartRot = speedometer.transform.localEulerAngles.z;
        revmeterStartRot = revMeter.transform.localEulerAngles.z;
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = angularVelocityLowSpeed;
        ResetSlideTimer();
        leftSmoke = smokeParticles[0].GetComponent<ParticleSystem>();
        rightSmoke = smokeParticles[1].GetComponent<ParticleSystem>();
        midSmoke = smokeParticles[2].GetComponent<ParticleSystem>();
        rb.centerOfMass = Vector3.down;
        gearText.text = "1";
        if (skidTrailsDetachedParent == null) {
            skidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
        }
        boostParticles.GetComponent<ParticleSystem>().Stop();

        //oobCoroutine = WaitAndShowOOBText();
        //Fabric.EventManager.Instance.PostEvent("Engine");
        //aSource = GetComponent<AudioSource>();
        //origPitch = aSource.pitch;
    }

    void Update(){

        if (drivable) {
            acceleration = Input.GetAxis("Vertical") * torque;
            steering = maxSteeringAngle * Input.GetAxis("Horizontal");
            if (acceleration < 0) {
                foreach (Light brakelight in brakelights) {
                    brakelight.enabled = true;
                }
            }else if(!handBraking){
                foreach (Light brakelight in brakelights) {
                    brakelight.enabled = false;
                }
            }
            // Handbrake
            if (Input.GetButtonDown("Brake") && grounded && !handBraking && moveSpeed > 0.5f) {
                HandBraking();
            }
            else if (Input.GetButtonUp("Brake") || !grounded) {
                StopHandBraking();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0) && !skipCountdown) {
            cam.ActivateCamera();
            lm.skipCountdown = true;
            skipCountdown = true;
        }

        AngleCheck();
        GroundCheck();
        SteeringCheck();
        AirControl();


        if (boosting) {
            boostTimeLeft -= Time.deltaTime;
            if(boostTimeLeft < 0) {
                boosting = false;
                boostParticles.GetComponent<ParticleSystem>().Stop();
            }
        }
        UpdateDashboardVisuals();
    }

    void FixedUpdate () {
        Move();
        CalculateRevs();
        GearCheck();
        //print(gearNum + " " + revs);
    }

    void PositionWheelMeshes(WheelInfo wheelPair, float steerAngle, float speed)
    {
        // Rotate wheels visually
        float s = wheelPair.steering ? steerAngle : 0;
        wheelRotation += moveSpeed * 1.6f;
        wheelPair.leftWheelMesh.transform.localRotation = Quaternion.Euler(wheelRotation, s, 0);
        wheelPair.rightWheelMesh.transform.localRotation = Quaternion.Euler(-wheelRotation, s + 180, 0);
    }

    void ResetSlideTimer(){
        timeToSlide = slideTimer;
    }

    void Move(){
        Vector3 rayOffset = rb.velocity * Time.fixedDeltaTime;
        // Front wheel suspension to prevent bumpy behaviour on the edge of two colliders or slopes
        if (steeringEnabled) {
            rb.AddForceAtPosition(transform.up * suspensionForce * (1.0f - (hit.distance / 0.5f)), frontRightWheel.transform.position + rayOffset);
            Debug.DrawRay(frontRightWheel.transform.position + rayOffset, transform.up * suspensionForce * (1.0f - (hit.distance / 0.5f)), Color.magenta);
            rb.AddForceAtPosition(transform.up * suspensionForce * (1.0f - (hit.distance / 0.5f)), frontLeftWheel.transform.position + rayOffset);
            Debug.DrawRay(frontLeftWheel.transform.position + rayOffset, transform.up * suspensionForce * (1.0f - (hit.distance / 0.5f)), Color.magenta);

            // Backwheel suspension
            rb.AddForceAtPosition(transform.up * suspensionForce * 0.5f * (1.0f - (hit.distance / 0.5f)), backRightWheel.transform.position + rayOffset);
            Debug.DrawRay(backRightWheel.transform.position + rayOffset, transform.up * suspensionForce * 0.5f * (1.0f - (hit.distance / 0.5f)), Color.magenta);
            rb.AddForceAtPosition(transform.up * suspensionForce * 0.5f * (1.0f - (hit.distance / 0.5f)), backLeftWheel.transform.position + rayOffset);
            Debug.DrawRay(backLeftWheel.transform.position + rayOffset, transform.up * suspensionForce * 0.5f * (1.0f - (hit.distance / 0.5f)), Color.magenta);
        }

        WallAssist();

        // Additional downward force on backwheels to make car stay better on vertical walls
        if (steeringEnabled) {
            rb.AddForceAtPosition(-transform.up * 3, backRightWheel.transform.position + rayOffset);
            Debug.DrawRay(backRightWheel.transform.position + rayOffset, -transform.up * 3, Color.cyan);
            rb.AddForceAtPosition(-transform.up * 3, backLeftWheel.transform.position + rayOffset);
            Debug.DrawRay(backLeftWheel.transform.position + rayOffset, -transform.up * 3, Color.cyan);
        }
        //print(Mathf.InverseLerp(0.5f, 2, slidingSpeed / moveSpeed));


        // Get movespeed
        var localVel = transform.InverseTransformDirection(rb.velocity);
        moveSpeed = localVel.z;
        if(moveSpeed < 0) {
            steering *= -1;
        }
        //print(moveSpeed);
        revvingSound.volume = moveSpeed / 80;

        // Steering efficiency decreases when moveSpeed increases until slidingSpeed is met to make the car controlling easier on low speeds
        if (Mathf.Abs(moveSpeed) < slidingSpeed) {
            var currentAngularVelocity = Mathf.InverseLerp(0.5f, 2, slidingSpeed / Mathf.Abs(moveSpeed));
            rb.maxAngularVelocity = currentAngularVelocity * angularVelocityLowSpeed;
        }

        // Values to check if the car is higher/lower than during the previous update. Used for the uphill/downhill speed loss compensation
        float currentHeight = transform.position.y;
        float currentVelocity = rb.velocity.magnitude;
        float heightDelta = prevHeight - currentHeight;


        // Acceleration and limit top speed
        if (Mathf.Abs(moveSpeed) < maxSpeed && grounded && !handBraking) {
            rb.AddForce(transform.forward * acceleration * groundTouchMultiplier, ForceMode.Acceleration);

            // Uphill / Downhill speed loss compensation
            if (heightDelta < -0.01f && moveSpeed > 20) {
                rb.AddForce(transform.forward * groundTouchMultiplier * acceleration * (2 * Physics.gravity.y * heightDelta) / (currentVelocity + prevVelocity), ForceMode.Force);
            }
            else if (heightDelta > 0.01f && moveSpeed > 20) {
                rb.AddForce(-transform.forward * groundTouchMultiplier * acceleration * (2 * Physics.gravity.y * heightDelta) / (currentVelocity + prevVelocity), ForceMode.Force);
            }
        }
        else if (handBraking) {
            rb.AddForce(-transform.forward * 15f * groundTouchMultiplier, ForceMode.Acceleration);
        }

        // Steering
        if (Mathf.Abs(moveSpeed) > 1 && steeringEnabled) {
            rb.AddTorque(transform.up * steering * steerModifier);
        }

        foreach (WheelInfo wheelPairs in wheelPairs) {
            PositionWheelMeshes(wheelPairs, steering, moveSpeed);
        }

        //Sideways drag
        velocity = transform.InverseTransformDirection(rb.velocity);
        if (steeringEnabled && grounded) {
            rb.AddForce(transform.right * -velocity.x * sidewaysDragMultiplier);
        }

        // Downforce
        if (grounded && moveSpeed > 0.5 * maxSpeed) {
            rb.AddForce(-transform.up * Mathf.Abs(moveSpeed) * 0.2f);
        }

        // Gravity boost in air to make the car less floaty during jumps
        //if (!grounded && moveSpeed < maxSpeed) {
        if (!grounded) {
            rb.AddForce(-Vector3.up * 8f);
        }

        if (boosting) {
            Vector3 boostDir = new Vector3(transform.forward.x, 0, transform.forward.z);
            //Vector3 f = new Vector3(transform.forward)
            rb.AddForce(transform.forward * 30, ForceMode.Acceleration);
        }

        SlideCheck();
        var s = Mathf.RoundToInt(moveSpeed * 3);
        s = s < 0 ? 0 : s;
        speedText.text = s.ToString();

        prevVelocity = rb.velocity.magnitude;
        prevHeight = transform.position.y;

        Debug.DrawLine(transform.position, transform.position + rb.velocity, Color.green);

        if (handBraking) {
            if (moveSpeed > 10f) {
                Quaternion rot = Quaternion.AngleAxis(steering * 2f, Vector3.up);
                carMesh.transform.rotation = Quaternion.Lerp(carMesh.transform.rotation, transform.rotation * rot, Time.deltaTime);
                timeToSlide = 0;
            }
            else {
                StopHandBraking();
            }
        }

        var vel = transform.InverseTransformDirection(rb.velocity);
        float v = vel.magnitude;

        //print(v - lastFrameVelocity.magnitude);
        if(v - lastFrameVelocity.magnitude < -10f) {
            crashSound.pitch = Random.Range(0.8f, 1.1f);
            crashSound.Play();
        }

        if (localVel.magnitude > 100) {
            rb.velocity = lastFrameVelocity;
        }
        lastFrameVelocity = rb.velocity;
    }

    void SlideCheck()
    {
        if (!handBraking) {
            if (Mathf.Abs(steering) >= maxSteeringAngle * 0.8f && moveSpeed > slidingSpeed && grounded) {
                timeToSlide -= Time.deltaTime;
                if (timeToSlide < 0) { // Car is sliding
                    rb.AddForce(-transform.forward * acceleration * 0.1f * groundTouchMultiplier, ForceMode.Acceleration);
                    rb.maxAngularVelocity = Mathf.Lerp(rb.maxAngularVelocity, angularVelocityHighSpeed, slideQuickess * Time.deltaTime);
                    //Make car mesh turn more than the actual car rotates to make a sliding effect
                    Quaternion rot = Quaternion.AngleAxis(steering, Vector3.up);
                    carMesh.transform.rotation = Quaternion.Lerp(carMesh.transform.rotation, transform.rotation * rot, Time.deltaTime);
                    TurnSmokeOn();
                }
            }
            else if (moveSpeed > slidingSpeed && steering < maxSteeringAngle * 0.8f && grounded) { // Car is not sliding because car is not rotating eough
                timeToSlide = timeToSlide < slideTimer ? timeToSlide + Time.deltaTime * 3 : slideTimer;
                rb.maxAngularVelocity = Mathf.Lerp(rb.maxAngularVelocity, angularVelocityLowSpeed, slideQuickess * Time.deltaTime);
                //carMesh.transform.rotation = transform.rotation;
                carMesh.transform.rotation = Quaternion.Lerp(carMesh.transform.rotation, transform.rotation, Time.deltaTime * 10);
                TurnSmokeOff();
            }
            else { // Car is going too slow for sliding
                timeToSlide = timeToSlide < slideTimer ? timeToSlide + Time.deltaTime : slideTimer;
                rb.maxAngularVelocity = Mathf.Lerp(rb.maxAngularVelocity, angularVelocityLowSpeed, slideQuickess * Time.deltaTime);
                carMesh.transform.rotation = Quaternion.Lerp(carMesh.transform.rotation, transform.rotation, Time.deltaTime * 10);
                TurnSmokeOff();
            }
        }
    }

    void GearCheck(){
        float f = Mathf.Abs(moveSpeed / maxSpeed);
        float upgearlimit = (1 / (float)numberOfGears) * (gearNum + 1);
        float downgearlimit = (1 / (float)numberOfGears) * gearNum;

        //print("Upper: " + upgearlimit + ", lower: " + downgearlimit);

        if (gearNum > 0 && f < downgearlimit) {
            gearNum--;
            gearText.text = (gearNum + 1).ToString();
        }

        if (f > upgearlimit && (gearNum < (numberOfGears - 1))) {
            gearNum++;
            gearText.text = (gearNum + 1).ToString();
        }
    }

    void CalculateGearFactor()
    {
        float f = (1 / (float)numberOfGears);
        // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
        // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
        var targetGearFactor = Mathf.InverseLerp(f * gearNum, f * (gearNum + 1), Mathf.Abs(moveSpeed / maxSpeed));
        gearFactor = Mathf.Lerp(gearFactor, targetGearFactor, Time.deltaTime * 5f);
    }

    void CalculateRevs()
    {
        // calculate engine revs (for display / sound)
        // (this is done in retrospect - revs are not used in force/power calculations)
        CalculateGearFactor();
        var gearNumFactor = gearNum / (float)numberOfGears;
        var revsRangeMin = ULerp(0f, revRangeBoundary, CurveFactor(gearNumFactor));
        var revsRangeMax = ULerp(revRangeBoundary, 1f, gearNumFactor);
        currentRevs = ULerp(revsRangeMin, revsRangeMax, gearFactor) * revs;
        //revText.text = Mathf.RoundToInt(currentRevs).ToString();
        var newPitch = Mathf.InverseLerp(0, 6000, currentRevs) * 4f;
        engineSound.pitch = Mathf.Clamp(newPitch, 1, 4f);
        //print(currentRevs);
    }

    // simple function to add a curved bias towards 1 for a value in the 0-1 range
    private static float CurveFactor(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }

    // unclamped version of Lerp, to allow value to exceed the from-to range
    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }

    void SteeringCheck(){
        steeringEnabled = false;
        bool rightSteer = false;
        bool leftSteer = false;

        //Check if front wheels are touching the ground and enable steering
        Physics.Raycast(frontRightWheel.position, -transform.up, out hit, rayDistance * 0.5f);
        Debug.DrawRay(frontRightWheel.position, -transform.up * rayDistance * 0.5f, Color.green);
        if (hit.collider != null) {
            //steeringEnabled = true;
            rightSteer = true;
        }
        Physics.Raycast(frontLeftWheel.position, -transform.up, out hit, rayDistance * 0.5f);
        Debug.DrawRay(frontLeftWheel.position, -transform.up * rayDistance * 0.5f, Color.green);
        if (hit.collider != null) {
            //steeringEnabled = true;
            leftSteer = true;
        }

        if (rightSteer && leftSteer) {
            steeringEnabled = true;
        }

        // Mark the car grounded if at least one back wheel is grounded.
        grounded = groundTouchMultiplier > 0 ? true : false;
    }

    void AngleCheck(){
        // Calculate the angle of the car, if it's pointing upwards make raycasts lengths longer to prevent unwanted behaviour on slopes
        angle = Vector3.Angle(-transform.up, Vector3.up);
        rayMultiplier = 1;
        if (angle < 70) {
            rayMultiplier = 3;
        }
    }

    void GroundCheck(){
        //Reset ground touch multiplier
        groundTouchMultiplier = 0;
        //Check if car is touching the ground with the backwheels, add .5f to groundTouchMultiplier for each grounded wheel. So one wheel on ground -> half force, 2 wheels on ground -> full force.
        Physics.Raycast(backLeftWheel.position, -transform.up * rayMultiplier, out hit, rayDistance);
        Debug.DrawRay(backLeftWheel.position, -transform.up * rayDistance * rayMultiplier, Color.red);
        if (hit.collider != null) {
            groundTouchMultiplier += 0.5f;
        }
        Physics.Raycast(backRightWheel.position, -transform.up * rayMultiplier, out hit, rayDistance);
        Debug.DrawRay(backRightWheel.position, -transform.up * rayDistance * rayMultiplier, Color.red);
        if (hit.collider != null) {
            groundTouchMultiplier += 0.5f;
        }

        if(groundTouchMultiplier < 0.1f) {
            TurnSmokeOff();
        }
    }

    void AirControl(){
        var t = Time.deltaTime;
        if (!grounded) {
            airTime += t;
            if (airTime > airControlTimer) {
                airControl = true;
            }
        }
        else {
            airTime = 0;
            airControl = false;
        }

        // yaw: left stick horizontal
        // pitch: right stick vertical
        // roll: right stick horizontal


        if (airControl) {
            float speed = Time.deltaTime * 66;
            if (Mathf.Abs(Input.GetAxis("Yaw")) < 0.5 && yaw > 0) {
                yaw -= speed;
            }
            else {
                yaw += Input.GetAxis("Yaw") * speed;
            }

            if (Mathf.Abs(Input.GetAxis("Pitch")) < 0.5 && pitch > 0) {
                pitch -= speed;
            }
            else {
                pitch += Input.GetAxis("Pitch") * speed;
            }

            if (Mathf.Abs(Input.GetAxis("Roll")) < 0.5 && roll > 0) {
                roll -= speed;
            }
            else {
                roll += Input.GetAxis("Roll") * speed;
            }
            transform.Rotate(pitch, -roll, -yaw, Space.Self);
        }

            pitch = 0;
            yaw = 0;
            roll = 0;

    }

    void UpdateDashboardVisuals(){
        // Steering wheel rotation
        float newRot = steeringWheelStartRot - steering * 3 + 180;
        steeringWheel.transform.localRotation = Quaternion.Euler(20, 0, Mathf.Lerp(steeringWheel.transform.localEulerAngles.z, newRot, Time.deltaTime * 5f));

        // Speedometer pointer rotation
        if (moveSpeed > 0) {
            speedometer.transform.localRotation = Quaternion.Euler(0, 0, speedometerStartRot + moveSpeed * 3f/2f * 3);
        }else {
            speedometer.transform.localRotation = Quaternion.Euler(0, 0, speedometerStartRot);
        }

        // Revmeter pointer rotation
        if(moveSpeed > 0) {
            revMeter.transform.localRotation = Quaternion.Euler(0, 0, revmeterStartRot + currentRevs / 20f);
        }
        else {
            revMeter.transform.localRotation = Quaternion.Euler(0, 0, revmeterStartRot);
        }
    }

    void HandBraking(){
        handBraking = true;
        print("handbraking");
        foreach (Light brakelight in brakelights) {
            brakelight.enabled = true;
        }
        rb.maxAngularVelocity = angularVelocityHandBrake;
        TurnSmokeOn();
    }

    void StopHandBraking(){
        handBraking = false;
        foreach (Light brakelight in brakelights) {
            brakelight.enabled = false;
        }
        //rb.maxAngularVelocity = angularVelocityLowSpeed;
        TurnSmokeOff();
    }

    void TurnSmokeOff(){
        sliding = false;
        if (skidLeftBack != null) {
            skidLeftBack.transform.parent = null;
            skidRightBack.transform.parent = null;
            skidLeftFront.transform.parent = null;
            skidRightFront.transform.parent = null;
            Destroy(skidLeftBack.gameObject, 10);
            Destroy(skidRightBack.gameObject, 10);
            Destroy(skidLeftFront.gameObject, 10);
            Destroy(skidRightFront.gameObject, 10);
        }

        //EndSkidTrail();
        var em = leftSmoke.emission;
        em.enabled = false;
        em = rightSmoke.emission;
        em.enabled = false;
        em = midSmoke.emission;
        em.enabled = false;

        skidSound.Stop();
    }

    void TurnSmokeOn(){
        if(grounded && !sliding) {
            skidLeftBack = Instantiate(skidTrail, leftSkidPosBack);
            skidLeftBack.transform.position = leftSkidPosBack.position + transform.up * 0.1f;
            skidLeftBack.transform.parent = leftSkidPosBack.transform;
            skidRightBack = Instantiate(skidTrail, rightSkidPosBack);
            skidRightBack.transform.position = rightSkidPosBack.position + transform.up * 0.1f;
            skidRightBack.transform.parent = rightSkidPosBack.transform;
            skidLeftFront = Instantiate(skidTrail, leftSkidPosFront);
            skidLeftFront.transform.position = leftSkidPosFront.position + transform.up * 0.3f;
            skidRightFront = Instantiate(skidTrail, rightSkidPosFront);
            skidRightFront.transform.position = rightSkidPosFront.position + transform.up * 0.3f;
            sliding = true;
            skidSound.Play();
        }
        //StartSkidTrail();

        var em = leftSmoke.emission;
        em.enabled = true;
        em = rightSmoke.emission;
        em.enabled = true;
        em = midSmoke.emission;
        em.enabled = true;
    }

    void WallAssist(){
        Vector3 speedOffset = rb.velocity * Time.fixedDeltaTime;
        RaycastHit wallHit;
        bool backWheelHit = false;

        // Back Right
        if (Physics.Raycast(br.position + speedOffset, -br.transform.right, out wallHit, wallAssistLength)) {
            if (wallHit.transform.tag == "Wall" && grounded) {
                //rb.AddTorque(-transform.up * 50, ForceMode.VelocityChange);
                rb.AddForce(-transform.right * 4, ForceMode.VelocityChange);
                transform.Rotate(0, -2f, 0);
                backWheelHit = true;
            }
        }
        else {
            backWheelHit = false;
        }
        Debug.DrawRay(br.position + speedOffset, -br.transform.right * wallAssistLength, Color.white);

        // Front Right
        if (!backWheelHit) {
            if (Physics.Raycast(fr.position + speedOffset + transform.forward * 0.8f, transform.right, out wallHit, wallAssistLength)) {
                if (wallHit.transform.tag == "Wall" && grounded) {
                    rb.AddTorque(-transform.up * 200, ForceMode.VelocityChange);
                    rb.AddForce(-transform.right * 2, ForceMode.VelocityChange);
                    rb.AddForce(-transform.forward, ForceMode.VelocityChange);
                    transform.Rotate(0, -4f, 0);
                }
            }
            Debug.DrawRay(fr.position + speedOffset + transform.forward * 0.8f, transform.right * wallAssistLength, Color.white);
        }

        backWheelHit = false;

        //Back Left
        if (Physics.Raycast(bl.position + speedOffset, -bl.transform.right, out wallHit, wallAssistLength)) {
            if (wallHit.transform.tag == "Wall" && grounded) {
                //rb.AddTorque(transform.up * 50, ForceMode.VelocityChange);
                rb.AddForce(transform.right * 4, ForceMode.VelocityChange);
                transform.Rotate(0, 2f, 0);
                backWheelHit = true;
            }
        }
        else {
            backWheelHit = false;
        }
        Debug.DrawRay(bl.position + speedOffset, -bl.transform.right * wallAssistLength);

        // Front Left
        if (!backWheelHit) {
            if (Physics.Raycast(fl.position + speedOffset + transform.forward * 0.8f, -transform.right, out wallHit, wallAssistLength)) {
                if (wallHit.transform.tag == "Wall" && grounded) {
                    rb.AddTorque(transform.up * 200, ForceMode.VelocityChange);
                    rb.AddForce(transform.right * 2, ForceMode.VelocityChange);
                    rb.AddForce(-transform.forward, ForceMode.VelocityChange);
                    transform.Rotate(0, 4f, 0);
                }
            }
            Debug.DrawRay(fl.position + speedOffset + transform.forward * 0.8f, -transform.right * wallAssistLength);
        }

        // Bumper left
        Vector3 dir = transform.forward * 2f - 0.5f * transform.right;
        Vector3 bumperOffset = transform.forward * 0.8f + transform.right * 0.2f;
        dir = dir.normalized;
        if(Physics.Raycast(fl.position + speedOffset + bumperOffset, dir * 1f, out wallHit, wallAssistLength)) {
            if(wallHit.transform.tag == "Wall") {
                rb.AddTorque(transform.up * 200, ForceMode.VelocityChange);
                rb.AddForce(transform.right * 2, ForceMode.VelocityChange);
                rb.AddForce(-transform.forward, ForceMode.VelocityChange);
                transform.Rotate(0, 4f, 0);
            }
        }
        Debug.DrawRay(fl.position + speedOffset + bumperOffset, dir * wallAssistLength, Color.blue);

        // Bumper right
        dir = transform.forward * 2f + 0.5f * transform.right;
        bumperOffset = transform.forward * 0.8f - transform.right * 0.2f;
        dir = dir.normalized;
        if (Physics.Raycast(fr.position + speedOffset + bumperOffset, dir * 1f, out wallHit, wallAssistLength)) {
            if (wallHit.transform.tag == "Wall") {
                rb.AddTorque(-transform.up * 200, ForceMode.VelocityChange);
                rb.AddForce(-transform.right * 2, ForceMode.VelocityChange);
                rb.AddForce(-transform.forward, ForceMode.VelocityChange);
                transform.Rotate(0, -4f, 0);
            }
        }
        Debug.DrawRay(fr.position + speedOffset + bumperOffset, dir * wallAssistLength, Color.blue);
    }

    public void Boost(){
        boosting = true;
        boostTimeLeft = boostTime;
        boostParticles.GetComponent<ParticleSystem>().Play();
        //rb.AddForce(transform.forward * boostAmount, ForceMode.Impulse);
    }

    public void Drivable(bool canDrive){
        drivable = canDrive == true ? true : false;

        if (!canDrive) {
            acceleration = 0;
            steering = 0;
        }
    }

    //private void OnTriggerEnter(Collider c) {
    //    If layer = 9(ground), activate oob text
    //    if (c.gameObject.layer.ToString() == "9") {
    //        oobCoroutine = StartCoroutine(WaitAndShowOOBText());

    //    }
    //}

    //private void OnTriggerExit(Collider c) {
    //    if (c.gameObject.layer.ToString() == "9") {
    //        StopCoroutine(oobCoroutine);
    //    }
    //}
    private void OnCollisionEnter(Collision c) {
        // If layer = 9 (ground), activate oob text
        if (c.gameObject.layer.ToString() == "9" && !oobRunning) {
            oobCoroutine = StartCoroutine(WaitAndShowOOBText());
        }
    }

    private void OnCollisionExit(Collision c) {
        if (c.gameObject.layer.ToString() == "9") {
            StopCoroutine(oobCoroutine);
            oobRunning = false;
        }
    }

    public void HideOobText(){
        OobText.SetActive(false);
    }

    public void ShowOobText(){
        OobText.SetActive(true);
    }

    public IEnumerator WaitAndShowOOBText(){
        oobRunning = true;
        yield return null;
        ShowOobText();
    }
}
