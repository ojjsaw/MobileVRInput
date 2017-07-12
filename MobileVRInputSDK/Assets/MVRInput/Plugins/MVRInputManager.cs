using UnityEngine;
using MVRInput;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MVRInputManager : MonoBehaviour
{
    public static MVRInputManager instance = null;
    public MVRController emulatorController;
    public MVRInputConnection connection = null;
    private MVRInputStatus status = MVRInputStatus.DISCONNECTED;
    private byte[] buffer = new byte[354];

    public GameObject buttonPrefab;
    // Use this for initialization

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
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
                mvrButton.Initialize(ConnectionType.APP, i);
            }

        }
    }


    public void SendMsgEmulator(byte[] msg)
    {
       // Debug.Log(msg.Length);
        // emulatorController.Msgs.Enqueue(msg);
        connection.SendToOther(msg);
    }

    public void SendData(byte[] msg)
    {
        emulatorController.Msgs.Enqueue(msg);
    }

    // Update is called once per frame
    void Update()
    {
        status = connection.CheckConnectionStatus(out buffer);
        if (status == MVRInputStatus.CONNECTED)
            InitController();
      //  Debug.Log("Server" + status);
    }
}
