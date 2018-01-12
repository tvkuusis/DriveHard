using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLogo : MonoBehaviour {

    public float speed;

	void Start () {
		
	}
	
	void Update () {
        transform.Rotate(0, Time.deltaTime * speed, 0);
	}
}
