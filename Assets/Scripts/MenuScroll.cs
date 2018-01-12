using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuScroll : MonoBehaviour {

    public Text levelName;

    public float moveSpeed;
    public float smallSize;
    public GameObject[] levels;
    Vector3 startPos;
    Vector3 nextPos;
    float offset = 0;
    float middlePoint;
    float screenWidth;
    int currentLevel = 0;
    bool gamepadRightPressed;
    bool gamepadLeftPressed;
    GameManager gm;
    public UnityEvent onLevelLoading;

	void Start () {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        startPos = transform.position;
        nextPos = new Vector3(levels[currentLevel].transform.position.x, startPos.y, 0);
        print(currentLevel);
    }

	void Update () {
        //print(currentLevel);
        float h = Input.GetAxis("Horizontal");

        if (h > 0.5f && !gamepadRightPressed) {
            NextLevel();
            gamepadRightPressed = true;
        }
        if (h < -0.5f && !gamepadLeftPressed) {
            PreviousLevel();
            gamepadLeftPressed = true;
        }
        if (Mathf.Abs(h) < 0.5f) {
            gamepadLeftPressed = false;
            gamepadRightPressed = false;
        }

        //if (Input.GetKeyDown(KeyCode.RightArrow)) {
        //    NextLevel();
        //}else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
        //        PreviousLevel();         
        //}
        levelName.text = levels[currentLevel].transform.name;
        if (Input.GetButtonDown("Start") || Input.GetKeyDown(KeyCode.JoystickButton0)) {
            onLevelLoading.Invoke();
            gm.LoadScene(currentLevel + 1);
        }

        transform.position = Vector3.Lerp(transform.position, startPos + nextPos, Time.deltaTime * moveSpeed);

        foreach (GameObject lvl in levels) {
            var distance = Mathf.Abs(lvl.transform.position.x - transform.position.x);
            var newSize = 1 - Mathf.Abs(distance / smallSize);
            newSize = newSize < 0.1f ? 0.1f : newSize;
            lvl.transform.localScale = new Vector3(newSize / 2, newSize / 2, 1);
        }
    }

    void NextLevel(){
        if (currentLevel < levels.Length - 1) {
            currentLevel++;
            levelName.text = levels[currentLevel].transform.name;
            nextPos = new Vector3(levels[currentLevel].transform.position.x, startPos.y, 0);
        }
    }

    void PreviousLevel(){
        if (currentLevel > 0) {
            currentLevel--;
            levelName.text = levels[currentLevel].transform.name;
            nextPos = levels[currentLevel].transform.position;
            nextPos = new Vector3(levels[currentLevel].transform.position.x, startPos.y, 0);
        }
    }
}
