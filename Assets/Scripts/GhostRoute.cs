using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostRoute : MonoBehaviour {
    GameObject car;
    public GameObject carVisuals;
    public GameObject ghostCar;
    Goal goal;

    Vector3 startPosition;
    Quaternion startRotation;
    GameObject ghostClone;
    LevelManager lm;

    [HideInInspector]
    public List<Ghost> thisRun = new List<Ghost>();
    [HideInInspector]
    public List<Ghost> ghostRun;
    //public List<Ghost> bestRun;

    int i;
    bool ghostActive;

    private void Awake() {
        lm = GetComponent<LevelManager>();
        car = GameObject.FindGameObjectWithTag("Player");
        goal = GameObject.FindGameObjectWithTag("Goal").GetComponent<Goal>();
    }
    void Start() {
        
        //startPosition = car.transform.position;
        //startRotation = car.transform.rotation;
    }
    private void Update() {

    }
    void FixedUpdate() {
        if(lm.game == GameState.running) {
        thisRun.Add(new Ghost(car.transform.position, carVisuals.transform.rotation));
            if (ghostActive) {
                SelectNextPosition();
            }
        }
        
    }
    void SelectNextPosition() {
        if (i < ghostRun.Count) {
            ghostClone.transform.position = ghostRun[i].ghostPosition;
            ghostClone.transform.rotation = ghostRun[i].ghostRotation;
            i++;
        }
    }
    public void SendGhost(List<Ghost> list) {
        if (ghostClone != null) {
            Destroy(ghostClone);
        }
        ghostRun = new List<Ghost>(list);
        i = 0;
        ghostClone = Instantiate(ghostCar, car.transform.position, car.transform.rotation);
        ghostActive = true;
    }
}
