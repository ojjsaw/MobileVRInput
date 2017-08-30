using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {

    private float max = 60f;
    private float factor = 1f;

	// Use this for initialization
	void Start () {
        MVRInputManager.OnJoystick += MVRInputManager_OnJoystick;
	}

    private void MVRInputManager_OnJoystick(MVRSide screenSide, Vector2 direction)
    {
        Debug.Log(direction);

        if (Mathf.Abs(direction.x) > max || Mathf.Abs(direction.y) > max) return;

        GetComponent<Rigidbody>().AddForce(new Vector3(direction.x, 0f, direction.y) * factor);
    }

    // Update is called once per frame
    void OnDisable () {
        MVRInputManager.OnJoystick -= MVRInputManager_OnJoystick;
    }
}
