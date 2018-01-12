using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashingText : MonoBehaviour {

    public float flashSpeed = 1f;
    float timer;
    Text text;

	void Start () {
        text = GetComponent<Text>();
        timer = flashSpeed;
	}
	
	void Update () {
        timer -= Time.deltaTime * flashSpeed;

        if(timer < 0) {
            text.enabled = text.enabled == true ? false : true;
            timer = text.enabled == true ? flashSpeed : flashSpeed / 3f;
        }
	}
}
