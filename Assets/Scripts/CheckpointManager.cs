using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour {

    GameObject car;
    public Checkpoint[] checkpoints;
    public List<float> checkPointTimes = new List<float>();

    Checkpoint lastPassedCheckpoint, nextCheckpoint;
    int checkPointIndex, i;
    LevelManager lm;
    Goal goal;
    AudioSource aSource;
	
	void Start () {
        car = GameObject.FindGameObjectWithTag("Player");
        lm = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        goal = GameObject.FindGameObjectWithTag("Goal").GetComponent<Goal>();
        aSource = GetComponent<AudioSource>();
        lastPassedCheckpoint = goal.GetComponent<Checkpoint>();
        checkpoints = GetComponentsInChildren<Checkpoint>();
        if(checkpoints.Length > 0) {
            nextCheckpoint = checkpoints[0];
        }
        i = 0;
    }
    
    void Update () {
        if (Input.GetButtonDown("Reset") && lm.game == GameState.running) {
            ResetToCheckpoint();
        }
    }
    public void SetLastPassedCheckpoint(Checkpoint cp) {
        if (cp == nextCheckpoint) {
            lastPassedCheckpoint = cp;
            cp.passed = true;
            checkPointTimes.Add(lm.runTime);
            print("time on checkpoint: " + lm.runTime);
            aSource.Play();
            if (lm.bestRunTime.Count > 0) {
               StartCoroutine(lm.DisplayTimeDifference(checkPointTimes[checkPointIndex] - lm.bestRunTime[checkPointIndex]));
            }
            if(i < checkpoints.Length) {
                checkPointIndex++;
                i++;
                if (i == checkpoints.Length) {
                    i = 0;
                }
                nextCheckpoint = checkpoints[i];
                print("next checkpoint: " + nextCheckpoint);
            }
        }
    }
    void ResetToCheckpoint() {
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        car.transform.position = lastPassedCheckpoint.resetPosition.position;
        car.transform.forward = lastPassedCheckpoint.GetComponent<Transform>().up;
        car.GetComponent<CarController2>().HideOobText();
    }
    public bool CheckCheckpoints() {
        for (int i = 0; i < checkpoints.Length; i++) {
            if (checkpoints[i].passed == false) {
                return false;
            }
        }
        return true;
    }
    public void NextLap() {
        print("starting next lap");
        foreach (var cp in checkpoints) {
            cp.passed = false;
        }
    }
}
