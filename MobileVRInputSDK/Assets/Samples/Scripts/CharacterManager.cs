using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MVRInputManager.OnJoystick += MVRInputManager_OnJoystick;
	}

    private void MVRInputManager_OnJoystick(MVRSide screenSide, Vector2 direction)
    {
        Debug.Log(direction);
    }

    // Update is called once per frame
    void OnDisable () {
        MVRInputManager.OnJoystick -= MVRInputManager_OnJoystick;
    }
}
