using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTransparency : MonoBehaviour {
    Transform car;
    MeshRenderer visuals;
    public Material ghostcar;
    public float magnitudeDivider;
    // Use this for initialization
    Vector3 offset;
    Color original;
    void Start() {
        visuals = GetComponent<MeshRenderer>();
        car = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        original = ghostcar.color;
    }

    // Update is called once per frame
    void Update() {
        offset = car.position - transform.position;
        visuals.material.color = new Color(original.r, original.g, original.b, offset.magnitude / magnitudeDivider);
        //foreach (var rend in visuals) {
        //    rend.GetComponent<Material>().color = new Color(original.r, original.g, original.b, offset.magnitude);
        //}
    }
}
