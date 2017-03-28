namespace MVRInput
{
    public class MVRButton
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

        public int id = -1;

        public string text = "Button";
    }

    public class MVRButtonData
    {
        public int id = -1;
        public int pressed = 0; //0 = false, 1 = true
    }
}
