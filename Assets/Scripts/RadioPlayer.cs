using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioPlayer : MonoBehaviour {

    public AudioClip[] tracks;
    AudioSource aSource;
    public float longPressTime;
    public float switchDelay = 1;
    public GameObject popupText;
    float timer = 0;
    bool shutdown;
    int i = 0;
    public bool randomFirstTrack = true;
    public bool menuRadio = false;
    //int lastSongNo;

    void Start() {
        aSource = GetComponent<AudioSource>();
        int t;
        if (randomFirstTrack) {
            t = Random.Range(0, tracks.Length);
        }
        else {
            t = 0;
        }
        aSource.clip = tracks[t];
        i = t;
        aSource.Play();
        popupText.GetComponent<NowPlayingPopUp>().ChangeText(tracks[t].name);
    }

    void Update() {
        if (Input.GetButton("Next Track") && !shutdown && !menuRadio) {
            StartCounting();
        }else if (Input.GetButtonUp("Next Track") && !shutdown && !menuRadio) {
            StopCounting();
        }else if (shutdown) {
            if(Input.GetButtonDown("Next Track") && !menuRadio) {
                shutdown = false;
                timer = 0;
                SwitchTrack();
                //SwitchTrack();
            }
        }

        // Move to next song automatically
        if (!aSource.isPlaying && !shutdown) {
            SwitchTrack();
        }
    }

    void StartCounting(){
        timer += Time.deltaTime;
        if (timer > longPressTime) {
            ShutDown();
            return;
        }else {
            //print("counting... " + timer);
        }
    }

    void StopCounting(){
        //print("stopped counting... " + timer);
        if(timer > longPressTime) {
            //print("Radio shut down");
            timer = 0;
        }else {
            SwitchTrack();
        }
    }

    void SwitchTrack(){
        //int i = Random.Range(0, tracks.Length);
        //if(i == lastSongNo) {
        //    SwitchTrack();
        //    return;
        //}
        //lastSongNo = i;
        if(i == tracks.Length - 1) {
            i = 0;
        }else {
            i++;
        }
        aSource.Stop();
        aSource.clip = tracks[i];
        aSource.PlayDelayed(switchDelay);
        popupText.GetComponent<NowPlayingPopUp>().ChangeText(tracks[i].name);
        //print(tracks[i].name);
        //print("Next track number " + i);
        timer = 0;
    }

    void ShutDown(){
        aSource.Stop();
        //print("Radio shut down");
        shutdown = true;
    }
}
