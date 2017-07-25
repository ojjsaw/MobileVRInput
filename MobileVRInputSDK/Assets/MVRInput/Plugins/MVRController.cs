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
    public string connectedIP = "NONE";
    private MVRTouchSwipeData swipeData = new MVRTouchSwipeData();
    private float minSwipeDistY;
    private Vector2 startPos;
    private float minSwipeDistX;
    private int pixelCenter;
    private int skipCount = 0;

    void Awake () {

        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        ButtonPrefab = Resources.Load("ButtonPrefab") as GameObject;
    }

    public MVRInputStatus Connect(string ipaddress)
    {
        connection = new MVRInputConnection(ConnectionType.GAMEPAD);
        var sts = connection.ConnectToServer(ipaddress);
        if (sts != MVRInputStatus.CONNECTED)
        {
            connection.Close();
            connection = null;
        }
        else
        {
            connectedIP = ipaddress;

            Input.gyro.enabled = true;
            sendOrientation = true;

            minSwipeDistY = Camera.main.pixelHeight / 4.5f;
            minSwipeDistX = Camera.main.pixelWidth / 5f;
            pixelCenter = Camera.main.pixelWidth / 2;
            startPos = Vector2.zero;
            skipCount = 0;

            StartCoroutine("OrientationProcessor");
        }
        return sts;
    }


    void Update()
    {
        if (connection != null && !connection.IsConnected)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            Debug.Log("deleting");
            UIElements.Clear();

            connection.Close();
            connection = null;
        }
    
        if (connection == null && connectedIP != "NONE")
        {
            Connect(connectedIP);
        }

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

        if(Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            if (touch.position.x > pixelCenter) swipeData.scrnSide = 1; // right scrn side
            else if (touch.position.x < pixelCenter) swipeData.scrnSide = 0; // left scrn side
            else return;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    float swipeDistVertical = (new Vector3(0, touch.position.y, 0) - new Vector3(0, startPos.y, 0)).magnitude;
                    if (swipeDistVertical > minSwipeDistY)
                    {
                        float swipeValue = Mathf.Sign(touch.position.y - startPos.y);
                        if (swipeValue > 0)
                        {
                            swipeData.swipeDirection = 2; //up
                            skipCount++;
                            if(skipCount > 1) connection.SendToOther(connection.ObjectToByteArray(swipeData));
                        }
                        else if (swipeValue < 0)
                        {
                            swipeData.swipeDirection = 3; //down
                            skipCount++;
                            if (skipCount > 1) connection.SendToOther(connection.ObjectToByteArray(swipeData));
                        }
                    }
                    float swipeDistHorizontal = (new Vector3(touch.position.x, 0, 0) - new Vector3(startPos.x, 0, 0)).magnitude;
                    if (swipeDistHorizontal > minSwipeDistX)
                    {
                        float swipeValue = Mathf.Sign(touch.position.x - startPos.x);
                        if (swipeValue > 0)
                        {
                            swipeData.swipeDirection = 1; //right
                            skipCount++;
                            if (skipCount > 1) connection.SendToOther(connection.ObjectToByteArray(swipeData));
                        }
                        else if (swipeValue < 0)
                        {
                            swipeData.swipeDirection = 0; //left
                            skipCount++;
                            if (skipCount > 1) connection.SendToOther(connection.ObjectToByteArray(swipeData));
                        }
                    }
                    break;
                default:
                    break;
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
