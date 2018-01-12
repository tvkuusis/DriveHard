using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState { chase, seat, hood, bumper , preview}

public class CameraFollow : MonoBehaviour {

    public GameObject car;
    public GameObject carVisual;
    CarController2 gc;
    public float vertOffset;
    public float horOffset;
    public float positionDamping;
    public float angleDamping;
    Vector3 posOffset;
    Vector3 newPos;
    Quaternion newRot;
    Vector3 carPos;
    float yRot;
    bool inAir;
    float airCamTimer = 0.3f;
    float timer;
    float angle;
    float origFov;
    Camera cam;
    public Transform[] cameraPositions;
    int currentPosition;
    Transform cameraOrigin;
    CameraState currentCam;
    public float fovEffect;
    public GameObject[] interior;
    public GameObject sideviewMirror;
    bool animationEnded;

	void Start () {
        //SwitchCamera(0);
        //transform.position = car.transform.position - transform.forward * horOffset + transform.up * vertOffset;
        gc = car.GetComponent<CarController2>();
        timer = airCamTimer;
        cam = Camera.main;
        origFov = cam.fieldOfView;
        currentCam = CameraState.preview;
    }

    void Update(){

        if (animationEnded) {
            if (Input.GetButtonDown("Camera")) {
                var newPosition = currentPosition == cameraPositions.Length - 1 ? 0 : currentPosition + 1;
                SwitchCamera(newPosition);
                //print("Camera switch to position " + newPosition);
            }
        }
    }



    void FixedUpdate () {
        //if (gc.grounded) {
        //    inAir = false;
        //    timer = airCamTimer;
        //    MoveCamGround();
        //}else {
        //    if (!inAir) {
        //        newPos = Vector3.up * vertOffset;
        //        posOffset = newPos;
        //        yRot = transform.localEulerAngles.y;
        //        inAir = true;
        //    }else {
        //        timer -= Time.deltaTime;
        //        if (timer < 0 && currentCam == CameraState.chase) {
        //            //MoveCamAir();
        //            MoveCamGround();
        //        }
        //        else {
        //            MoveCamGround();
        //        }
        //    }
        //}
        if (animationEnded) {
            MoveCamGround();
            if (currentCam == CameraState.chase) {
                cam.fieldOfView = origFov + Mathf.Abs(gc.moveSpeed) * fovEffect;
            }
        }
    }

    void MoveCamGround(){
        if (currentCam == CameraState.chase) {
            angle = Vector3.Angle(car.transform.position - (transform.position - transform.up * vertOffset), car.transform.forward);
            transform.position = Vector3.Lerp(transform.position, car.transform.position - transform.forward * horOffset + transform.up * vertOffset, positionDamping * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, car.transform.rotation, Time.deltaTime * angle * angleDamping);
        } else if (currentCam == CameraState.seat || currentCam == CameraState.hood || currentCam == CameraState.bumper) {
            transform.position = cameraOrigin.position;
        }
    }

    void MoveCamAir(){
        transform.position = car.transform.position + posOffset;
        transform.localRotation = Quaternion.Euler(0, yRot, 0);
        transform.position -= transform.forward * 1f * horOffset;
    }

    void SwitchCamera(int i){
        cameraOrigin = cameraPositions[i];
        currentPosition = i;

        // 0 = Chase
        // 1 = Seat
        // 2 = Hood
        // 3 = Bumper

        if(i == 0) {
            currentCam = CameraState.chase;
            transform.parent = null;
            HideInterior(true);
        }
        else if(i == 1) {
            currentCam = CameraState.seat;
            transform.parent = carVisual.transform;
            transform.rotation = carVisual.transform.rotation;
            cam.fieldOfView = origFov;
            HideInterior(false);
        }else if(i == 2) {
            currentCam = CameraState.hood;
            transform.parent = carVisual.transform;
            transform.rotation = carVisual.transform.rotation;
            cam.fieldOfView = origFov;
            HideInterior(true);
        }else if(i == 3) {
            currentCam = CameraState.bumper;
            transform.parent = carVisual.transform;
            transform.rotation = carVisual.transform.rotation;
            cam.fieldOfView = origFov;
            HideInterior(true);
        }
    }

    void HideInterior(bool hide){
        if (hide) {
            for(int i = 0; i < interior.Length; i++) {
                interior[i].SetActive(false);
            }
            sideviewMirror.SetActive(false);
        }
        else {
            for (int i = 0; i < interior.Length; i++) {
                interior[i].SetActive(true);
            }
            sideviewMirror.SetActive(true);
        }
    }

    public void ActivateCamera()
    {
        print("Camera activated");
        animationEnded = true;
        GetComponent<Animator>().enabled = false;
        SwitchCamera(0);
        currentCam = CameraState.chase;
    }
}