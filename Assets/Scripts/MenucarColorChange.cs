using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenucarColorChange : MonoBehaviour {
    Renderer rend;
    public Color[] carColors;
	// Use this for initialization
	void Start () {
        ChangeColor();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeColor(){
        rend = GetComponent<Renderer>();
        var i = Random.Range(0, carColors.Length - 1);
        rend.material.color = carColors[i];
    }
}
