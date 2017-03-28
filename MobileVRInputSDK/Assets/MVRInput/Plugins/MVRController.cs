using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MVRInput;

public class MVRController : MonoBehaviour {

	// Use this for initialization
	void Awake () {

        int numOfMVRUI = transform.childCount;
        for(int i=0; i < numOfMVRUI; i++)
        {
            Transform child = transform.GetChild(i);

            if(child.GetComponent<Button>() != null)
            {
                MVRButton mvrButton = new MVRButton();

                Button bttn = child.GetComponent<Button>();
                ColorBlock cb = bttn.colors;

                RectTransform rt = child.GetComponent<RectTransform>();
                
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
