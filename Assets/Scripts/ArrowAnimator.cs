using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAnimator : MonoBehaviour {

    public Material arrow;
    public float scrollSpeed = 0.5f;
    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        float offset = Time.time * scrollSpeed;
        arrow.mainTextureOffset = new Vector2(-offset, 0);
    }
}
