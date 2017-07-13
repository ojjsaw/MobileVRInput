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

    void Awake()
    {
        if (instance == null)instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    void Start()
    {
        connection = new MVRInputConnection(ConnectionType.APP);
    }

    void InitController()
    {
        Transform screen = transform.GetChild(0);
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
            InitController();
        else if (status == MVRInputStatus.DATARECEIVED)
        {
            System.Object tmp = connection.ByteArrayToObject(recbuffer);

            if (tmp.GetType() == typeof(MVRButtonData))
            {
                MVRButtonData data = tmp as MVRButtonData;
                if (UIElements.Contains(data.id)) (UIElements[data.id] as MVRButton).OnReceiveEvent(tmp as MVRButtonData);
            }
        }
    }

    

}
