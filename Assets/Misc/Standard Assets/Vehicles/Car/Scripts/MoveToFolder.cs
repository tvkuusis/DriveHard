using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MoveToFolder : MonoBehaviour {

    public string folderName;

	void Awake () {
        var mainFolder = GameObject.Find("Objects");
        if(mainFolder == null) {
            mainFolder = new GameObject("Objects");
        }
        var parentFolder = GameObject.Find(folderName);
        if (parentFolder == null) {
            var folder = new GameObject(folderName);
            folder.transform.parent = mainFolder.transform;
            transform.parent = folder.transform;
        }else {
            transform.parent = parentFolder.transform;
            parentFolder.transform.parent = mainFolder.transform;
        }
	}
}
