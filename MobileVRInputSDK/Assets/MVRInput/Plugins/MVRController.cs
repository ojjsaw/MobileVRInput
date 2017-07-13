using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MVRInput;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MVRController : MonoBehaviour {

    public static MVRController instance = null;
    private GameObject ButtonPrefab = null;
    public Queue<byte[]> Msgs = new Queue<byte[]>();
    public Hashtable UIElements = new Hashtable();
    public MVRInputConnection connection = null;
    private MVRInputStatus status = MVRInputStatus.NONE;

    void Awake () {

        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        ButtonPrefab = Resources.Load("ButtonPrefab") as GameObject;

        connection = new MVRInputConnection(ConnectionType.GAMEPAD);
        Debug.Log(connection.ConnectToServer("10.0.0.206"));

    }

    void Update()
    {

        byte[] recbuffer = new byte[1024];
        status = connection.CheckConnectionStatus(out recbuffer);
        
        if (status == MVRInputStatus.DATARECEIVED)
        {
            System.Object tmp = connection.ByteArrayToObject(recbuffer);
            if (tmp.GetType() == typeof(MVRButtonInfo))
            {
                GameObject child = GameObject.Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity);
                MVRButton mvrButton = child.AddComponent<MVRButton>();
                mvrButton.Initialize(ConnectionType.GAMEPAD, connection);
                mvrButton.OnReceiveEvent(tmp as MVRButtonInfo);
                UIElements.Add(child.GetComponent<MVRButton>().GetID(), mvrButton);
            }
        }
    }

}
