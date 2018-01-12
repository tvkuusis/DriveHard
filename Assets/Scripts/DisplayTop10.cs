using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayTop10 : MonoBehaviour {
    public Text[] topNames, topTimes;
    LevelManager lm;
    public int displayIndex;
    public Color highLight;
    Color original;
    EventSystem es;

    public Button lastPage, nextPage;
    public Text finishTime, rank;
    public InputField nameInput;
	// Use this for initialization
	void Awake () {
        lm = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
	}
    private void Start() {
        original = topNames[0].color;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.L)) {
            DisplayTop(lm.leaderBoard, displayIndex);
        }
    }
    //string TimeToString(float time) {
    //    int flooredTime = Mathf.FloorToInt(time / 60f);
    //    float runTimeSeconds = (time - 60f * flooredTime);
    //    string paddedTime = "00.00";
    //    return flooredTime.ToString() + ":" + runTimeSeconds.ToString(paddedTime);
    //}

    public void DisplayTop(List<Top10> list, int startIndex) {
        if(displayIndex == 0) {
            lastPage.gameObject.SetActive(false);
        } else {
            lastPage.gameObject.SetActive(true);
        }
        if(displayIndex > list.Count - topNames.Length - 1) {
            nextPage.gameObject.SetActive(false);
        } else {
            nextPage.gameObject.SetActive(true);
        }

        for (int i = startIndex; i < startIndex + topNames.Length; i++) {
            if(i < list.Count) {
                int j = i + 1;
                if(list[i].playerName == nameInput.text && list[i].playerTime == lm.runTime) {
                    topNames[i - startIndex].color = highLight;
                    topTimes[i - startIndex].color = highLight;
                } else {
                    topNames[i - startIndex].color = original;
                    topTimes[i - startIndex].color = original;
                }
                int pad = 10 - list[i].playerName.Length;
                topNames[i - startIndex].text = j.ToString() + ". " + list[i].playerName + ":";
                topTimes[i - startIndex].text = lm.TimeToString(list[i].playerTime);
            } else {
                int j = i + 1;
                topNames[i - startIndex].color = original;
                topNames[i - startIndex].text = j.ToString() + ".";
                topTimes[i - startIndex].text = "";
            }
        }
    }
    public void NextPage(int direction) {
        displayIndex += direction;
        if(displayIndex < 0) {
            displayIndex = 0;
        }
        DisplayTop(lm.leaderBoard, displayIndex);
    }
    public void AddToLeaderBoard() {
        if(nameInput.text.Length == 0) {
            nameInput.text = "anonymous";
        }
        lm.leaderBoard.Add(new Top10(nameInput.text, lm.runTime));
        lm.SortLeaderBoard();
        NextPage(0);
    }
    public void RestartButton() {
        lm.RestartButton();
    }
    public void MenuButton() {
        lm.GoToMenu();
    }
    public void SubmitSave() {
        lm.SaveFromButton();
    }
    public void Endtime() {
        finishTime.text = "Time: " + lm.TimeToString(lm.runTime);
    }
    public void DisplayRank() {
        rank.text = "Rank: " + lm.CompareTimes(lm.runTime).ToString() + ".";
    }
    public void SelectButtonLast() {
        es.SetSelectedGameObject(lastPage.gameObject.activeSelf ? lastPage.gameObject : nextPage.gameObject);
    }
    public void SelectButtonNext() {
        es.SetSelectedGameObject(nextPage.gameObject.activeSelf ? nextPage.gameObject : lastPage.gameObject);
    }
}
