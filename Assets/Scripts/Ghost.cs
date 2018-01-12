using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ghost {
    public Vector3 ghostPosition;
    public Quaternion ghostRotation;

    public Ghost(Vector3 pos, Quaternion rot) {
        ghostPosition = pos;
        ghostRotation = rot;
    }
}
