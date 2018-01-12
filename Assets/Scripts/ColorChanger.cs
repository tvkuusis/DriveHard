using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour {
    Renderer rend;
    public Color[] carColors;
    int c = 0;
    [HideInInspector]
    public Color currentColor;
    GameManager gm;

    void Start()
    {
        rend = GetComponent<Renderer>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        //if(gm.c_color != null) {
            rend.material.color = gm.c_color;
        //}
        //else {
        //    ChangeColor();
        //}

        //ChangeColor();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeColor()
    {
        if(c < carColors.Length - 1) {
            c++;
        }
        else {
            c = 0;
        }
        //var i = Random.Range(0, carColors.Length - 1);
        rend.material.color = carColors[c];
        currentColor = carColors[c];
        GameObject.Find("GameManager").GetComponent<GameManager>().c_color = carColors[c];
    }
}
