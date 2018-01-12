using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverUI : MonoBehaviour {

    public bool horizontal;
    public bool vertical;
    public float hSpeed;
    public float hAmplitude;
    public float vSpeed;
    public float vAmplitude;

    Vector3 origPos;

	void Start () {
        origPos = gameObject.transform.position;
	}
	
	void Update () {
        float h = 0;
        float v = 0;
        var t = Time.time;

        if(horizontal) {
            h = Mathf.Sin(t * hSpeed) * hAmplitude;
        }

        if(vertical) {
            v = Mathf.Sin(t * vSpeed) * vAmplitude;
        }

        transform.position = new Vector3(origPos.x + h, origPos.y + v, origPos.z);
	}
}
