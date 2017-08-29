using UnityEngine;
using MVRInput;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

public class MVRInputManager : MonoBehaviour
{
    public static MVRInputManager instance = null;
    public MVRController emulatorController;
    public MVRInputConnection connection = null;
    private MVRInputStatus status = MVRInputStatus.DISCONNECTED;
    private byte[] buffer = new byte[354];
    private GameObject ButtonPrefab = null;
    public Hashtable UIElements = new Hashtable();
    public Text ipText;
    private Transform screen;
    
    public delegate void OrientationData(Quaternion gyro, Vector3 accelerometer);
    public static event OrientationData OnOrientation;

    public delegate void TouchSwipeData(MVRSide screenSide, MVRSide swipeDirection);
    public static event TouchSwipeData OnTouchSwipe;

    public delegate void JoystickData(MVRSide screenSide, Vector2 direction);
    public static event JoystickData OnJoystick;

    public bool enableTouchSwipe = false;
    public bool enableOrientation = false;
    public bool enableJoystick = false;

    public RectTransform finger1;
    public RectTransform finger2;

    public GameObject leftJoystick = null;

    void Awake()
    {
        if (instance == null)instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    void Start()
    {
        connection = new MVRInputConnection(ConnectionType.APP);
        ipText.text = "IP: " + connection.ServerIP;

    }

    void InitController()
    {
        screen = transform.GetChild(0);
        int numOfMVRUI = screen.childCount;
        for (int i = 0; i < numOfMVRUI; i++)
        {
            Transform child = screen.GetChild(i);

            if (child.GetComponent<Button>() != null)
            {
                MVRButton mvrButton = child.gameObject.AddComponent<MVRButton>();
                mvrButton.Initialize(ConnectionType.APP, connection, i);
                UIElements.Add(child.GetComponent<MVRButton>().GetID(), mvrButton);
            }

        }
    }

    void Update()
    {

        byte[] recbuffer = new byte[1024];
        status = connection.CheckConnectionStatus(out recbuffer);

        if (status == MVRInputStatus.CONNECTED)
        {
            InitController();
            float jw, jh;
            if (enableJoystick)
            {
                leftJoystick.SetActive(false);
                jw = leftJoystick.GetComponent<RectTransform>().localScale.x;
                jh = leftJoystick.GetComponent<RectTransform>().localScale.y;
            }

            EnableConfiguration(enableOrientation, enableTouchSwipe);
            ipText.text = "CONNECTED";
        }
        else if (status == MVRInputStatus.DATARECEIVED)
        {
            System.Object tmp = connection.ByteArrayToObject(recbuffer);

            if (tmp.GetType() == typeof(MVRButtonData))
            {
                MVRButtonData data = tmp as MVRButtonData;
                if (UIElements.Contains(data.id)) (UIElements[data.id] as MVRButton).OnReceiveEvent(tmp as MVRButtonData);
            }
            else if (tmp.GetType() == typeof(MVROrientationData))
            {
                MVROrientationData data = tmp as MVROrientationData;
                if (OnOrientation != null)
                    OnOrientation(new Quaternion(data.g_x, data.g_y, data.g_z, data.g_w), new Vector3(data.a_x, data.a_y, data.a_z));

                //screen.localRotation = new Quaternion(data.x, data.y, data.z, data.w);
            }
            else if (tmp.GetType() == typeof(MVRTouchSwipeData))
            {
                MVRTouchSwipeData data = tmp as MVRTouchSwipeData;
                if (OnTouchSwipe != null)
                    OnTouchSwipe((MVRSide)data.scrnSide, (MVRSide)data.swipeDirection);
            }
            else if (tmp.GetType() == typeof(MVRTouchData))
            {
                MVRTouchData data = tmp as MVRTouchData;

                if(enableJoystick)
                {
                    if(data.x1 < 50 && data.y1 < 80 && data.x1 > 0 && data.y1 > 0)
                    {
                        if (!finger1.GetComponent<Image>().enabled && !leftJoystick.activeSelf)
                        {
                            leftJoystick.SetActive(true);
                            leftJoystick.GetComponent<RectTransform>().localPosition = new Vector3(
                      ((data.x1 / 100) * screen.GetComponent<RectTransform>().rect.width) - screen.GetComponent<RectTransform>().rect.width / 2,
                      ((data.y1 / 100) * screen.GetComponent<RectTransform>().rect.height) - screen.GetComponent<RectTransform>().rect.height / 2,
                       0);
                        }
                        else
                        {
                            leftJoystick.transform.GetChild(0).GetComponent<RectTransform>().position = finger1.position;
                            if (OnJoystick != null)
                                OnJoystick(MVRSide.LEFT, leftJoystick.transform.GetChild(0).GetComponent<RectTransform>().localPosition);
                        }
                    }
                    else if (data.x2 < 50 && data.y2 < 80 && data.x2 > 0 && data.y2 > 0)
                    {
                        if (!finger2.GetComponent<Image>().enabled && !leftJoystick.activeSelf)
                        {
                            leftJoystick.SetActive(true);
                            leftJoystick.GetComponent<RectTransform>().localPosition = new Vector3(
                      ((data.x2 / 100) * screen.GetComponent<RectTransform>().rect.width) - screen.GetComponent<RectTransform>().rect.width / 2,
                      ((data.y2 / 100) * screen.GetComponent<RectTransform>().rect.height) - screen.GetComponent<RectTransform>().rect.height / 2,
                       0);
                        }
                        else
                        {
                            leftJoystick.transform.GetChild(0).GetComponent<RectTransform>().position = finger2.position;
                            if (OnJoystick != null)
                                OnJoystick(MVRSide.LEFT, leftJoystick.transform.GetChild(0).GetComponent<RectTransform>().localPosition);
                        }
                    }
                    else
                    {
                       // leftJoystick.transform.GetChild(0).GetComponent<RectTransform>().localPosition = Vector3.zero;

                        if (leftJoystick.activeSelf) leftJoystick.SetActive(false);
                    }
                }

                if (!finger1.GetComponent<Image>().enabled) finger1.GetComponent<Image>().enabled = true;
                finger1.GetComponent<RectTransform>().localPosition = new Vector3(
                   ((data.x1 / 100) * screen.GetComponent<RectTransform>().rect.width) - screen.GetComponent<RectTransform>().rect.width / 2,
                   ((data.y1 / 100) * screen.GetComponent<RectTransform>().rect.height) - screen.GetComponent<RectTransform>().rect.height / 2,
                    0);

                if (data.x1 == -1 && data.y1 == -1)
                {
                    finger1.GetComponent<Image>().enabled = false;
                }

                if (!finger2.GetComponent<Image>().enabled) finger2.GetComponent<Image>().enabled = true;
                finger2.GetComponent<RectTransform>().localPosition = new Vector3(
                   ((data.x2 / 100) * screen.GetComponent<RectTransform>().rect.width) - screen.GetComponent<RectTransform>().rect.width / 2,
                   ((data.y2 / 100) * screen.GetComponent<RectTransform>().rect.height) - screen.GetComponent<RectTransform>().rect.height / 2,
                    0);

                if (data.x2 == -1 && data.y2 == -1)
                {
                    finger2.GetComponent<Image>().enabled = false;
                }

            }
            
        }
    }

    void OnDisable()
    {
        DisableAllconfiguration();
        connection.Close();
        connection = null;
        enableTouchSwipe = false;
        enableOrientation = false;
    }

    public void DisableAllconfiguration()
    {
        MVRConfigurationData data = new MVRConfigurationData();
        data.enableOrientationData = false;
        data.enableTouchSwipeData = false;
        data.enableButtonData = false;
        data.enableTouchData = false;

        connection.SendToOther(connection.ObjectToByteArray(data));
    }

    public void EnableConfiguration(bool orientation, bool touchswipe)
    {
        MVRConfigurationData data = new MVRConfigurationData();
        data.enableOrientationData = orientation;
        data.enableTouchSwipeData = touchswipe;

        data.enableButtonData = true;
        data.enableTouchData = true;
        connection.SendToOther(connection.ObjectToByteArray(data));
    }
}

public enum MVRSide
{
    LEFT = 0,
    RIGHT = 1,
    UP = 2,
    DOWN = 3
}
