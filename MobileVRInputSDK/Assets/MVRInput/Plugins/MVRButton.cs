namespace MVRInput
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [Serializable]
    public class MVRButtonInfo
    {
        public float nR = 0f;
        public float nG = 0f;
        public float nB = 0f;
        public float nA = 0f;

        public float pR = 0f;
        public float pG = 0f;
        public float pB = 0f;
        public float pA = 0f;

        public float bR = 0f;
        public float bG = 0f;
        public float bB = 0f;
        public float bA = 0f;

        public float dR = 0f;
        public float dG = 0f;
        public float dB = 0f;
        public float dA = 0f;

        public float x = 0f;
        public float y = 0f;
        public float z = 0f;

        public float w = 0f;
        public float h = 0f;

        public float sx = 0f;
        public float sy = 0f;
        public float sz = 0f;

        public float cm = 0f;
        
        public int id = -1;

        public string text = "Button";
    }

    [Serializable]
    public class MVRButtonData
    {
        public int id = -1;
        public int pressed = 0; //0 = false, 1 = true
    }

    public class MVRButton
    {
        private int id = -1;
        private MVRButtonData data = new MVRButtonData();
        private MVRInputManager mInstance = null;
        private MVRController cInstance = null;
        private Transform child = null;
        private GameObject ButtonPrefab;
        private Transform thisObj;
        private bool isGamePad = false;

        public MVRButton(int id, bool isGamePad, Transform thisObj, Transform child = null, System.Object tmp = null)
        {
            this.id = id;
            this.isGamePad = isGamePad;
            if (isGamePad) cInstance = thisObj.GetComponent<MVRController>();
            else mInstance = thisObj.GetComponent<MVRInputManager>();
            data = new MVRButtonData();
            data.id = this.id;
            data.pressed = 1;
            this.ButtonPrefab = Resources.Load("ButtonPrefab") as GameObject;
            this.thisObj = thisObj;

            if (!isGamePad) this.child = child;
            if (isGamePad) LoadButtonInfo(tmp);

            if (!isGamePad)  mInstance.SendMsgEmulator(mInstance.ObjectToByteArray(SaveButtonInfo(child, id)));
            if (!isGamePad) child.GetComponent<Button>().onClick.AddListener(OnButtonSendClick);
        }

        public void OnButtonSendClick()
        {
            mInstance.SendMsgEmulator(mInstance.ObjectToByteArray(data));
        }

        public void OnButtonReceiveClick()
        {
            mInstance.SendMsgEmulator(mInstance.ObjectToByteArray(data));
        }

        private MVRButtonInfo SaveButtonInfo(Transform child, int id)
        {
            MVRButtonInfo mvrButton = new MVRButtonInfo();

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

        private void LoadButtonInfo(System.Object obj)
        {
            MVRButtonInfo mvrButton = obj as MVRButtonInfo;
            GameObject child = GameObject.Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity);
            child.transform.parent = thisObj.transform;

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
        }
    }
}
