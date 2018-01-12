using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour {
    Vector3 origRot;
    public float speed = 1;

	// Use this for initialization
	void Start () {
        origRot = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(origRot.x, origRot.y + Time.time * speed, origRot.z);
	}
}
