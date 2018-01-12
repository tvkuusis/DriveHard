using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NowPlayingPopUp : MonoBehaviour {

    public float speed;
    public float amplitude;
    Text popupText;
    Color color;
    Vector3 origPos;
    float alpha = 0;
    bool increasing = true;

	void Awake () {
        popupText = GetComponent<Text>();
        popupText.text = "";
        origPos = transform.position;
        color = popupText.color;
        popupText.color = new Color(color.r, color.g, color.b, alpha);
	}
	

	void Update () {
        if (increasing) {
            alpha += Time.deltaTime * 0.5f;
            if(alpha > 1.5f) {
                increasing = false;
            }
        }else {
            alpha -= Time.deltaTime * 0.5f;
        }

        transform.position = origPos + new Vector3(0, Mathf.Sin(Time.time * speed) * amplitude, 0);
        popupText.color = new Color(color.r, color.g, color.b, alpha);
        //print(alpha);
	}

    public void ChangeText(string TrackName){
        popupText.text = "Now playing: \n'" + TrackName + "'";
        increasing = true;
        alpha = 0;
    }
}
