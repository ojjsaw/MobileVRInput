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
    private MVROrientationData orientationData = new MVROrientationData();
    void Awake () {

        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        ButtonPrefab = Resources.Load("ButtonPrefab") as GameObject;

        Input.gyro.enabled = true;
    }

    public MVRInputStatus Connect(string ipaddress)
    {
        connection = new MVRInputConnection(ConnectionType.GAMEPAD);
        var sts = connection.ConnectToServer(ipaddress);
        if (sts != MVRInputStatus.CONNECTED) connection = null;
        return sts;
    }

    void Update()
    {
        if (connection == null) return;

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

        var rotation = Input.gyro.attitude;
        orientationData.x = rotation.x;
        orientationData.y = rotation.y;
        orientationData.z = rotation.z;
        orientationData.w = rotation.w;

        connection.SendToOther(connection.ObjectToByteArray(orientationData));

    }

    private void OnDisable()
    {
        Input.gyro.enabled = false;
    }
}
