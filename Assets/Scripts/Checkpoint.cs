using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    [HideInInspector]
    public bool passed;

    public Transform resetPosition;

    CheckpointManager cm;
    LevelManager lm;
	// Use this for initialization
	void Start () {
        cm = GameObject.Find("CheckpointManager").GetComponent<CheckpointManager>();
        lm = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
	}
	
	// Update is called once per frame
	void Update () {
    }
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !passed) {
            cm.SetLastPassedCheckpoint(this);
        }
    }
}
