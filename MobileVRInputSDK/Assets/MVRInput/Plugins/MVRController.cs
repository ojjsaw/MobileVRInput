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

	// Use this for initialization
	void Awake () {

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        ButtonPrefab = Resources.Load("ButtonPrefab") as GameObject;

    }

    // Update is called once per frame
    void Update () {

        byte[] recbuffer = new byte[1024];
        if(ReceiveMsgEmulator(out recbuffer))
        {
            System.Object tmp = ByteArrayToObject(recbuffer);
            if (tmp.GetType() == typeof(MVRButtonInfo))
            {
                GameObject child = GameObject.Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity);
                MVRButton mvrButton = child.AddComponent<MVRButton>();
                mvrButton.Initialize(ConnectionType.GAMEPAD);
                mvrButton.OnReceiveEvent(tmp as MVRButtonInfo);
                UIElements.Add(child.GetComponent<MVRButton>().GetID(), mvrButton);
            }
                
                //LoadButtonInfo(tmp);
            
            if (tmp.GetType() == typeof(MVRButtonData))
            {
                MVRButtonData data = tmp as MVRButtonData;
                if (UIElements.Contains(data.id)) (UIElements[data.id] as MVRButton).OnReceiveEvent(tmp as MVRButtonData);

            }
        }
	}

    bool ReceiveMsgEmulator(out byte[] buffer)
    {
        if (Msgs.Count > 0)
        {
            buffer = Msgs.Dequeue();
            return true;
        }

        buffer = null;
        return false;
    }

    private void LoadButtonInfo(System.Object obj)
    {
        MVRButtonInfo mvrButton = obj as MVRButtonInfo;
        GameObject child = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity);
        child.transform.parent = this.transform;

        child.GetComponent<Image>().color = new Color(mvrButton.bR, mvrButton.bG, mvrButton.bB, mvrButton.bA);

        ColorBlock cb = new ColorBlock();
        cb.normalColor = new Color(mvrButton.nR, mvrButton.nG, mvrButton.nB, mvrButton.nA);
        cb.pressedColor = new Color(mvrButton.pR, mvrButton.pG, mvrButton.pB, mvrButton.pA);
        cb.disabledColor = new Color(mvrButton.dR, mvrButton.dG, mvrButton.dB, mvrButton.dA);
        cb.colorMultiplier = mvrButton.cm;
        child.GetComponent<Button>().colors = cb;

        child.GetComponent<RectTransform>().localPosition = new Vector3(mvrButton.x, mvrButton.y, mvrButton.z);
        child.GetComponent<RectTransform>().localScale = new Vector3(mvrButton.sx, mvrButton.sy, mvrButton.sz);
        child.GetComponent<RectTransform>().sizeDelta = new Vector2(mvrButton.w, mvrButton.h);

        child.GetComponentInChildren<Text>().text = mvrButton.text;
        child.name = mvrButton.id.ToString();

        UIElements.Add(mvrButton.id, child.GetComponent<Button>());
    }

    private void UpdateButton(System.Object obj)
    {
        MVRButtonData bd = obj as MVRButtonData;
        //UIElements[bd.id]
    }
    public static System.Object ByteArrayToObject(byte[] arrBytes)
    {
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return obj;
        }
    }

}
