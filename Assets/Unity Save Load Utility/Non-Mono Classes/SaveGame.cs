using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveGame {
	
	public string savegameName = "New SaveGame";
	public string saveDate;
	public List<SceneObject> sceneObjects = new List<SceneObject>();
    public List<Ghost> bestRun;
    public List<float> bestRunTimes;
    public List<Top10> top10;
    
    public SaveGame() {

    }

    public SaveGame(string s, string d, List<SceneObject> list, List<Ghost> ghostList, List<float> checkpointTimes, List<Top10> top) {
        savegameName = s;
        saveDate = d;
        sceneObjects = list;
        bestRun = new List<Ghost>(ghostList);
        bestRunTimes = new List<float>(checkpointTimes);
        top10 = new List<Top10>(top);
    }
}
