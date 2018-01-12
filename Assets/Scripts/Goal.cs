using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour {
    //public GhostRoute ghoster;
    GhostRoute gr;
    LevelManager lm;
    public GameObject leaderboardHandler;
    CheckpointManager cm;
    bool finished;
    public int lapsToRun = 1;
    [HideInInspector]
    public int currentLap;

    public UnityEvent finish;
     
	void Start () {
        cm = GameObject.Find("CheckpointManager").GetComponent<CheckpointManager>();
        lm = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        gr = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<GhostRoute>();
        currentLap = 1;
        lm.SetLapCount(lapsToRun, currentLap);
    }

    void Update() {

    }
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (cm.CheckCheckpoints() && !finished) {
                if (currentLap == lapsToRun) {
                    finish.Invoke();
                } else {
                    currentLap++;
                    cm.NextLap();
                    lm.SetLapCount(lapsToRun, currentLap);
                }
            }
        }
    }

    public void Finish() {
        finished = true;
        lm.game = GameState.pause;
        leaderboardHandler.SetActive(true);
        leaderboardHandler.GetComponent<DisplayTop10>().Endtime();
        leaderboardHandler.GetComponent<DisplayTop10>().DisplayRank();
        //Camera.main.GetComponent<CameraFollow>().enabled = false;
        //Camera.main.transform.parent = null;
    }
}
