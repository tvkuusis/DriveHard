using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Top10 {
    public string playerName;
    public float playerTime;
	
    public Top10(string name, float time) {
        playerName = name;
        playerTime = time;
    }
}
