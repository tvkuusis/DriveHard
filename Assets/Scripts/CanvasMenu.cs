using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasMenu : MonoBehaviour {
    public GameObject panel, leaderboardHandler;
    public GameObject controlsImage;
    EventSystem es;
    GameObject storeSelected;

    GameManager gm = GameManager.instance;
	// Use this for initialization
	void Start () {
        es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        storeSelected = es.firstSelectedGameObject;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Menu") && !leaderboardHandler.activeSelf && !controlsImage.activeSelf) {
            es.SetSelectedGameObject(panel.activeSelf ? es.firstSelectedGameObject : null);
            panel.SetActive(!panel.activeSelf);
            es.SetSelectedGameObject(panel.activeSelf ? es.firstSelectedGameObject : storeSelected);

            //es.SetSelectedGameObject(es.firstSelectedGameObject);
            //es.currentSelectedGameObject.GetComponent<Button>()
            //Time.timeScale = panel.activeSelf ? 0 : 1;
        }
        if (es.currentSelectedGameObject != storeSelected) {
            if (es.currentSelectedGameObject == null) {
                es.SetSelectedGameObject(storeSelected);
            } else {
                storeSelected = es.currentSelectedGameObject;
            }
        }
    }
    public void Restart() {
        gm.RestartLevel();
    }
    public void GoToMenu() {
        gm.LoadScene(0);
    }
    //public void ReturnTimescale() {
    //    Time.timeScale = 1;

    //}
}
