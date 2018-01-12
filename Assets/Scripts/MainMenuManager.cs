using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public enum MenuState { mainmenu, start, options, quit, stop, controls, credits}
public class MainMenuManager : MonoBehaviour {
    public float smoothing = 3;
    public Camera mainmenuCam;
    public Transform mainMenuPos;
    public Transform startPos;
    public Transform optionsPos;
    public Transform quitPos;
    public Transform colorPos;
    public Transform creditsPos;
    public GameObject carVisuals;
    public AudioClip ignition;
    public AudioClip spray;
    public AudioClip toolbox;
    public AudioClip paper;
    public AudioClip quitChicken;
    AudioSource aSource;
    public GameObject paintjobEffect;
    public Transform effectPosition;

    public UnityEvent onOptionsSelect;
    public UnityEvent onStartSelect;
    public UnityEvent onMainMenu;
    public UnityEvent onQuitPrompt;
    public UnityEvent onSettings;
    public UnityEvent onCredits;
    public UnityEvent onControls;
    Vector3 targetPos;
    Quaternion targetRot;
    public Text[] mainMenuTexts;
    public Text[] optionsTexts;
    public Text[] quitTexts;
    float yPosMainMenu;
    float yPosOptions;
    float yPosQuit;
    //public Text startText;
    //public Text optionsText;
    //public Text quitText;
    int selected = 0;
    MenuState m_state;
    public float enableDistance;
    bool gamepadRightPressed;
    bool gamepadLeftPressed;
    public Light spotlightControls;
    public Light spotlightCar;
    public Light spotlightBlueprints;

    GameManager gameManager;

    void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_state = MenuState.mainmenu;
        mainmenuCam.transform.position = mainMenuPos.position;
        yPosMainMenu = mainMenuTexts[0].transform.position.y;
        yPosOptions = optionsTexts[0].transform.position.y;
        yPosQuit = quitTexts[0].transform.position.y;
        ActivateButton(selected);
        targetPos = mainMenuPos.position;
        aSource = GetComponent<AudioSource>();
	}
	

	void Update () {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0)) {
            if (m_state == MenuState.mainmenu) {
                if (selected == 0) {
                    MoveToLevelSelect();
                }
                else if (selected == 1) {
                    MoveToOptions();
                    selected = 0;
                }
                else if (selected == 2) {
                    MoveToQuit();
                    aSource.clip = quitChicken;
                    aSource.pitch = Random.Range(0.9f, 1.1f);
                    aSource.Play();
                    //selected = 1;
                }
            }else if (m_state == MenuState.options) {
                if (selected == 0) {
                    onSettings.Invoke();
                    m_state = MenuState.controls;
                    aSource.clip = toolbox;
                    aSource.pitch = Random.Range(0.9f, 1.1f);
                    aSource.Play();
                }
                else if (selected == 1) {
                    //print("Color change");
                    ColorChanger cChanger = carVisuals.GetComponent<ColorChanger>();
                    cChanger.ChangeColor();
                    var particleEffect = Instantiate(paintjobEffect, effectPosition, true);
                    particleEffect.GetComponent<ParticleSystem>().startColor = cChanger.currentColor;
                    aSource.clip = spray;
                    aSource.pitch = Random.Range(0.9f, 1.1f);
                    aSource.Play();
                    Destroy(particleEffect, 5);
                    gameManager.GetComponent<GameManager>().c_color = cChanger.currentColor;
                }else if (selected == 2) {
                    onCredits.Invoke();
                    m_state = MenuState.credits;
                    aSource.clip = paper;
                    aSource.pitch = Random.Range(0.9f, 1.1f);
                    aSource.Play();
                }
                else if (selected == 3) {
                    MoveToMainMenu();
                    spotlightBlueprints.enabled = false;
                }
            }else if(m_state == MenuState.controls) {
                onControls.Invoke();
                selected = 0;
                m_state = MenuState.options;
            }else if(m_state == MenuState.credits) {
                MoveToOptions();
                selected = 2;
                m_state = MenuState.options;
            }else if(m_state == MenuState.quit) {
                if(selected == 0) {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
                }else if(selected == 1) {
                    MoveToMainMenu();
                }
            }
        }

        float h = Input.GetAxis("Horizontal");

        if (Mathf.Abs(h) < 0.5f) {
            gamepadLeftPressed = false;
            gamepadRightPressed = false;
        }

        if (m_state == MenuState.mainmenu) {
            if (Input.GetKeyDown(KeyCode.RightArrow) || h > 0.5f && !gamepadRightPressed) {
                if (selected < mainMenuTexts.Length - 1) {
                    selected++;
                    ActivateButton(selected);
                    gamepadRightPressed = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || h < -0.5f && !gamepadLeftPressed) {
                if (selected > 0) {
                    selected--;
                    gamepadLeftPressed = true;
                    ActivateButton(selected);
                }
            }
        }
        else if (m_state == MenuState.options) {
            if (Input.GetKeyDown(KeyCode.RightArrow) || h > 0.5f && !gamepadRightPressed) {
                if (selected < optionsTexts.Length - 1) {
                    selected++;
                    ActivateButton(selected);
                    gamepadRightPressed = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || h < -0.5f && !gamepadLeftPressed) {
                if (selected > 0) {
                    selected--;
                    ActivateButton(selected);
                    gamepadLeftPressed = true;
                }
            }
            if(selected == 0) {
                spotlightControls.enabled = true;
            }
            else {
                spotlightControls.enabled = false;
            }

            if (selected == 1) {
                MoveToColorChange();
                spotlightCar.enabled = true;
            }
            else {
                spotlightCar.enabled = false;
            }
            if(selected == 2 || selected == 3) {
                MoveToCredits();
                spotlightBlueprints.enabled = true;
            }else {
                spotlightBlueprints.enabled = false;
            }
            if (selected == 0) {
                MoveToOptions();
            }
        }
        else if (m_state == MenuState.quit) {
            if (Input.GetKeyDown(KeyCode.RightArrow) || h > 0.5f && !gamepadRightPressed) {
                if (selected < quitTexts.Length - 1) {
                    selected++;
                    ActivateButton(selected);
                    gamepadRightPressed = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || h < -0.5f && !gamepadLeftPressed) {
                if (selected > 0) {
                    selected--;
                    ActivateButton(selected);
                    gamepadLeftPressed = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1)) {
            MoveToMainMenu();
        }

        mainmenuCam.transform.position = Vector3.Lerp(mainmenuCam.transform.position, targetPos, Time.deltaTime * smoothing);
        mainmenuCam.transform.rotation = Quaternion.Slerp(mainmenuCam.transform.rotation, targetRot, Time.deltaTime * smoothing);

        float currentDistance = Vector3.Distance(mainmenuCam.transform.position, targetPos);

        if(m_state == MenuState.start && currentDistance < enableDistance) {
            onStartSelect.Invoke();
            //carVisuals.GetComponent<MenucarColorChange>().ChangeColor();
            m_state = MenuState.stop;
        }else if(m_state == MenuState.mainmenu && currentDistance < enableDistance) {

        }

        //print(Vector3.Distance(mainmenuCam.transform.position, targetPos));
	}

    public void ActivateButton(int i){
        if (m_state == MenuState.mainmenu) {
            for (int j = 0; j < mainMenuTexts.Length; j++) {
                if (i == j) {
                    mainMenuTexts[j].GetComponent<HoverUI>().enabled = true;
                    mainMenuTexts[j].GetComponent<Text>().color = Color.green;
                }
                else {
                    mainMenuTexts[j].transform.position = new Vector3(mainMenuTexts[j].transform.position.x, yPosMainMenu, mainMenuTexts[j].transform.position.z);
                    mainMenuTexts[j].GetComponent<HoverUI>().enabled = false;
                    mainMenuTexts[j].GetComponent<Text>().color = Color.red;
                }
            }
        }else if (m_state == MenuState.options) {
            for (int j = 0; j < optionsTexts.Length; j++) {
                if (i == j) {
                    optionsTexts[j].GetComponent<HoverUI>().enabled = true;
                    optionsTexts[j].GetComponent<Text>().color = Color.green;
                }
                else {
                    optionsTexts[j].transform.position = new Vector3(optionsTexts[j].transform.position.x, yPosOptions, optionsTexts[j].transform.position.z);
                    optionsTexts[j].GetComponent<HoverUI>().enabled = false;
                    optionsTexts[j].GetComponent<Text>().color = Color.red;
                }
            }
        }else if(m_state == MenuState.quit) {
            for (int j = 0; j < quitTexts.Length; j++) {
                if (i == j) {
                    quitTexts[j].GetComponent<HoverUI>().enabled = true;
                    quitTexts[j].GetComponent<Text>().color = Color.green;
                }
                else {
                    quitTexts[j].transform.position = new Vector3(quitTexts[j].transform.position.x, yPosQuit, quitTexts[j].transform.position.z);
                    quitTexts[j].GetComponent<HoverUI>().enabled = false;
                    quitTexts[j].GetComponent<Text>().color = Color.red;
                }
            }
        }
    }

    public void MoveToOptions(){
        m_state = MenuState.options;
        ActivateButton(selected);
        targetPos = optionsPos.position;
        targetRot = optionsPos.rotation;
        onOptionsSelect.Invoke();
    }

    public void MoveToLevelSelect(){
        m_state = MenuState.start;
        aSource.clip = ignition;
        aSource.pitch = Random.Range(0.9f, 1.1f);
        aSource.Play();
        targetPos = startPos.position;
        targetRot = startPos.rotation;
        //onStartSelect.Invoke();
    }

    public void MoveToQuit(){
        m_state = MenuState.quit;
        selected = 1;
        ActivateButton(selected);
        targetPos = quitPos.position;
        targetRot = quitPos.rotation;
        onQuitPrompt.Invoke();
    }

    public void MoveToMainMenu(){
        m_state = MenuState.mainmenu;
        selected = 0;
        ActivateButton(selected);
        targetPos = mainMenuPos.position;
        targetRot = mainMenuPos.rotation;
        onMainMenu.Invoke();
    }

    public void MoveToColorChange(){
        targetPos = colorPos.position;
        targetRot = colorPos.rotation;
    }

    public void MoveToCredits(){
        targetPos = creditsPos.position;
        targetRot = creditsPos.rotation;
    }
}
