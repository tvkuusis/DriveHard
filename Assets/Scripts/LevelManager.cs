using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { pause , running};

public class LevelManager : MonoBehaviour {
    [HideInInspector]
    public List<Ghost> bestRun;
    [HideInInspector]
    public List<float> bestRunTime = new List<float>();
    [HideInInspector]
    public float runTime;
    [HideInInspector]
    public List<Top10> leaderBoard = new List<Top10>();

    GameObject car;
    SaveLoadUtility slu;
    CheckpointManager cm;
    GameManager gm;

    [Tooltip("Amount to rotate camera at start")]
    public float rotationAngle;
    [Tooltip("Time in which to rotate")]
    public float rotationTime;
    [Tooltip("String for save/load")]
    public string ghostSave;
    Text countDownText, checkPointTime, lapCount, runTimeText;

    GhostRoute gr;
    public GameState game;

    AudioSource[] audios;
    AudioSource countSound;
    AudioSource goSound;

    public bool skipCountdown;

    private void Awake() {
        car = GameObject.FindGameObjectWithTag("Player");
        lapCount = GameObject.Find("LapCount").GetComponent<Text>();
        countDownText = GameObject.Find("CountDownText").GetComponent<Text>();
        checkPointTime = GameObject.Find("CheckPointTime").GetComponent<Text>();
        runTimeText = GameObject.Find("RunTime").GetComponent<Text>();
        if(GameObject.FindGameObjectWithTag("GameManager") != null) {
            gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        gr = GetComponent<GhostRoute>();
        cm = GameObject.Find("CheckpointManager").GetComponent<CheckpointManager>();
    }
    
    void Start () {
        audios = GetComponents<AudioSource>();
        countSound = audios[0];
        goSound = audios[1];
        game = GameState.pause;
        if(gm != null) {
            slu = gm.GetComponent<SaveLoadUtility>();
            slu.LoadGame(ghostSave);
        }
        StartCoroutine(Countdown(rotationAngle, rotationTime));
        //for (int i = 0; i < 25; i++) {
        //    leaderBoard.Add(new Top10("test", Random.Range(0f, 100f)));
        //}
    }
	
	void Update () {
		if(game == GameState.pause) {
            car.GetComponent<CarController2>().Drivable(false);
        } else if (game == GameState.running) {
            runTime += Time.deltaTime;
            DisplayTime(runTime, runTimeText);
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            SortLeaderBoard();
        }
    }

    IEnumerator Countdown(float angle, float time) {
        float t = 0f;
        while (t < time) {
            if (!skipCountdown) {
                t += Time.deltaTime;
            }else {
                t = time;
            }
            //Camera.main.transform.RotateAround(car.transform.position, Vector3.up, (angle / time) * Time.deltaTime);
            yield return null;
        }
        if(bestRun.Count > 0) {
            gr.SendGhost(bestRun);
        }
        countDownText.enabled = true;
        for (int i = 4; i > 0; i--) {
            if(i == 1) {
                countDownText.color = Color.green;
                countDownText.text = "GO";
                goSound.Play();
                game = GameState.running;
                GiveControl();
                //Camera.main.GetComponent<CameraFollow>().enabled = true;
            } else {
            countDownText.text = (i-1).ToString();
            countSound.Play();
            }
            yield return new WaitForSeconds(1f);
        }
        countDownText.enabled = false;
    }

    public void Save(string saveGameName, List<Ghost> ghostList, List<float> floatList, List<Top10> top10){
        if(slu != null) {
            slu.SaveGame(saveGameName, ghostList, floatList, top10);
        }
    }
    public void SaveFromButton() {
        if (bestRun.Count == 0 || gr.thisRun.Count < bestRun.Count) {
            Save(ghostSave, gr.thisRun, cm.checkPointTimes, leaderBoard);
        } else {
            Save(ghostSave, bestRun, bestRunTime, leaderBoard);
        }
    }

    public IEnumerator DisplayTimeDifference(float timeDifference) {
        checkPointTime.enabled = true;
        if(timeDifference < 0) {
            checkPointTime.color = Color.green;
            checkPointTime.text = timeDifference.ToString("F2");
        } else {
            checkPointTime.color = Color.red;
            checkPointTime.text = "+" + TimeToString(timeDifference);
        } 
        //else {
        //    checkPointTime.color = Color.red;
        //}
        yield return new WaitForSeconds(2f);
        checkPointTime.enabled = false;
    }

    public void SetLapCount(int lapsToRun, int currentLap) {
        lapCount.text = currentLap.ToString() + "/" + lapsToRun.ToString();
    }

    public string TimeToString(float time) {
        int flooredTime = Mathf.FloorToInt(time / 60f);
        float secondsExtracted = (time - 60f * flooredTime);
        string paddedTime = "00.00";
        if(time > 60f) {
            return flooredTime.ToString() + ":" + secondsExtracted.ToString(paddedTime);
        } else {
            return secondsExtracted.ToString(paddedTime);
        }
    }

    void DisplayTime(float time, Text text) {
        text.text = TimeToString(time);
    }

    static int SortByTime(Top10 a, Top10 b) {
        return a.playerTime.CompareTo(b.playerTime);
    }
    public int CompareTimes(float time) {
        for(int i = 0; i < leaderBoard.Count; i++) {
            if(time < leaderBoard[i].playerTime) {
                return i + 1;
            }
        }
        return leaderBoard.Count + 1;
    }
    public void SortLeaderBoard() {
        leaderBoard.Sort(SortByTime);
    }
    public void GoToMenu() {
        gm.LoadScene(0);
    }
    public void RestartButton() {
        gm.RestartLevel();
    }
    void GiveControl() {
        car.GetComponent<CarController2>().Drivable(true);
    }
}
