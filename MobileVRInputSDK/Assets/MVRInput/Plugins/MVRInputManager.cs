using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVRInput;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class MVRInputManager : MonoBehaviour
{

    public MVRController emulatorController;
    public GameObject buttonPrefab;
    // Use this for initialization
    void Start()
    {

        int numOfMVRUI = transform.childCount;
        for (int i = 0; i < numOfMVRUI; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.GetComponent<Button>() != null)
            {
                MVRButton buttonEvent = new MVRButton(i, this.GetComponent<MVRInputManager>(), child, buttonPrefab, this.transform);              
            }


            MVRButtonData data = new MVRButtonData();
            data.id = 10;
            data.pressed = 1;

            SendMsgEmulator(ObjectToByteArray(data));

        }


    }

    

    public byte[] ObjectToByteArray(System.Object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public void SendMsgEmulator(byte[] msg)
    {
        emulatorController.Msgs.Enqueue(msg);
    }

    // Update is called once per frame
    void Update()
    {


    }
}
