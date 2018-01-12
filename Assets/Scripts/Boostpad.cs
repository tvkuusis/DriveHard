using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boostpad : MonoBehaviour {

    public float textureSpeed;
    AudioSource aSource;
    Renderer rend;

    private void Start(){
        aSource = GetComponent<AudioSource>();
        rend = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider c){
        if(c.CompareTag("Player")) {
            //print("BOOOOooooost");
            aSource.Play();
            c.GetComponentInParent<CarController2>().Boost();
        }
    }

    private void Update()
    {
        float offset = textureSpeed * Time.time;
        rend.material.SetTextureOffset("_MainTex", new Vector2(-offset, 0));
    }
}
