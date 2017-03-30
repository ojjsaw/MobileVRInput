using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVRInput;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MVRInputManager : MonoBehaviour {

    public MVRController emulatorController;
    public Hashtable UIElements = new Hashtable();

    // Use this for initialization
    void Start () {

        int numOfMVRUI = transform.childCount;
        for (int i = 0; i < numOfMVRUI; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.GetComponent<Button>() != null)
            {
                UIElements.Add(i, child.GetComponent<Button>());
                SendMsgEmulator(ObjectToByteArray(SaveButtonInfo(child, i)));
            }

            MVRButtonData data = new MVRButtonData();
            data.id = 10;
            data.pressed = 1;

            SendMsgEmulator(ObjectToByteArray(data));

        }

        
    }
	
    private MVRButton SaveButtonInfo(Transform  child, int id)
    {
        MVRButton mvrButton = new MVRButton();

        Image img = child.GetComponent<Image>();
        mvrButton.bR = img.color.r;
        mvrButton.bG = img.color.g;
        mvrButton.bB = img.color.b;
        mvrButton.bA = img.color.a;

        Button bttn = child.GetComponent<Button>();
        ColorBlock cb = bttn.colors;
        mvrButton.nR = cb.normalColor.r;
        mvrButton.nG = cb.normalColor.g;
        mvrButton.nB = cb.normalColor.b;
        mvrButton.nA = cb.normalColor.a;

        mvrButton.pR = cb.pressedColor.r;
        mvrButton.pG = cb.pressedColor.g;
        mvrButton.pB = cb.pressedColor.b;
        mvrButton.pA = cb.pressedColor.a;

        mvrButton.dR = cb.disabledColor.r;
        mvrButton.dG = cb.disabledColor.g;
        mvrButton.dB = cb.disabledColor.b;
        mvrButton.dA = cb.disabledColor.a;

        mvrButton.cm = cb.colorMultiplier;

        RectTransform rt = child.GetComponent<RectTransform>();
        mvrButton.x = rt.localPosition.x;
        mvrButton.y = rt.localPosition.y;
        mvrButton.z = rt.localPosition.z;

        mvrButton.sx = rt.localScale.x;
        mvrButton.sy = rt.localScale.y;
        mvrButton.sz = rt.localScale.z;

        mvrButton.w = rt.sizeDelta.x;
        mvrButton.h = rt.sizeDelta.y;

        mvrButton.text = child.GetComponentInChildren<Text>().text;
        mvrButton.id = id;

        return mvrButton;
    }

    private static byte[] ObjectToByteArray(System.Object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    void SendMsgEmulator(byte[] msg)
    {
        emulatorController.Msgs.Enqueue(msg);
    }

	// Update is called once per frame
	void Update () {
		
        
	}
}
