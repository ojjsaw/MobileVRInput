namespace MVRInput
{
    using UnityEngine;

    public class MVROrientation : MonoBehaviour
    {
        MVROrientationData data = new MVROrientationData();

        // Use this for initialization
        void Start()
        {
             Input.gyro.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDisable()
        {
            Input.gyro.enabled = false;
        }
    }

}
