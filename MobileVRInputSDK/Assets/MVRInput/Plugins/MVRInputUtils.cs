namespace MVRInput
{
    using System;

    public enum ConnectionType
    {
        APP,
        GAMEPAD
    }

    public enum MVRInputStatus
    {
        DISCONNECTED,
        CONNECTED,
        DATARECEIVED,
        DATASENT,
        FAILEDTOSEND,
        FAILEDTOCONNECT,
        NONE
    }

    public enum MVRInputMsgType
    {
        BUTTON
    }

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

        public float hR = 0f;
        public float hG = 0f;
        public float hB = 0f;
        public float hA = 0f;

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
        public int pressed = -1; //0 = Enter, 1 = Down, 2 = Up
    }

    [Serializable]
    public class MVROrientationData
    {
        public float g_x, g_y, g_z, g_w;
        public float a_x, a_y, a_z;
    }

    [Serializable]
    public class MVRTouchSwipeData
    {
        public int scrnSide; //0 = left, 1= right
        public int swipeDirection; // 0-left, 1-right, 2-up, 3-down
    }

    [Serializable]
    public class MVRConfigurationData
    {
        public bool enableOrientationData;
        public bool enableTouchSwipeData;
        public bool enableButtonData;
        public bool enableTouchData;
    }

    [Serializable]
    public class MVRTouchData
    {
        public float x1, y1;
        public float x2, y2;
    }

}
