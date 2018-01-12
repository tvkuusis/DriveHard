using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHelper : MonoBehaviour {

    InputField _inputField;


    // Use this for initialization
    void Start() {
        _inputField = GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Submit")) {
            _inputField.DeactivateInputField();
        }
    }
}
