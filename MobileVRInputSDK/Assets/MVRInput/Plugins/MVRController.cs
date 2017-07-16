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
    private bool sendOrientation = false;
    private byte[] toggleBytes; 

    void Awake () {

        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        ButtonPrefab = Resources.Load("ButtonPrefab") as GameObject;
    }

    public MVRInputStatus Connect(string ipaddress)
    {
        connection = new MVRInputConnection(ConnectionType.GAMEPAD);
        var sts = connection.ConnectToServer(ipaddress);
        if (sts != MVRInputStatus.CONNECTED) connection = null;
        else
        {
            MVRToggleData toggleData = new MVRToggleData();
            toggleData.toggle = true;
            toggleBytes = connection.ObjectToByteArray(toggleData);

            Input.gyro.enabled = true;
            sendOrientation = true;
            StartCoroutine("OrientationProcessor");

        }
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

        int touchCount = Input.touchCount;
        if (touchCount > 1)
        {
            if (Input.GetTouch(0).position.y > (Screen.height - Screen.height / 10)
                && Input.GetTouch(1).position.y > (Screen.height - Screen.height / 10))
            {
                if(Input.GetTouch(0).position.x < Screen.width / 10
                    || Input.GetTouch(1).position.x < Screen.width / 10)
                {
                    connection.SendToOther(toggleBytes);
                }
            }

        }
    }

    IEnumerator OrientationProcessor()
    {
        while(sendOrientation)
        {
            var rotation = Input.gyro.attitude;
            orientationData.g_x = rotation.x;
            orientationData.g_y = rotation.y;
            orientationData.g_z = rotation.z;
            orientationData.g_w = rotation.w;

            var acc = Input.acceleration;
            orientationData.a_x = acc.x;
            orientationData.a_y = acc.y;
            orientationData.a_z = acc.z;

            connection.SendToOther(connection.ObjectToByteArray(orientationData));

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnDisable()
    {
        sendOrientation = false;
        StopCoroutine("OrientationProcessor");
        Input.gyro.enabled = false;
    }
}
