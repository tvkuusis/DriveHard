using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {
    public GameObject walls;
	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnBehaviour());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    IEnumerator SpawnBehaviour() {
        yield return new WaitForSeconds(3f);
        //car.transform.parent = null;
        //cam.transform.parent = null;
        walls.SetActive(false);
    }
}
