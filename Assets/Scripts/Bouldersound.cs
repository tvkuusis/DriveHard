using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouldersound : MonoBehaviour {
    AudioSource aSource;
    bool played;
    float timer = 2f;
    // Use this for initialization
    void Start () {
        aSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (!played) {
            aSource.pitch = Random.Range(0.8f, 1.1f);
            aSource.Play();
            played = true;
        }
        else {
            timer -= Time.deltaTime;
        }

        if(played && timer < 0) {
            played = false;
            timer = 2f;
        }
    }
}
