using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    //[HideInInspector]
    public Color c_color;

    private void Awake() {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	void Update () {

    }
    public void LoadScene(int buildIndex) {
        SceneManager.LoadScene(buildIndex);
    }
    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Quit() {
        Application.Quit();
    }
}
