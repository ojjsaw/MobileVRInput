using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public float hor, ver;
        public GameObject steeringWheel;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void None() //FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif

         //   Debug.Log("h: " + h + ", v: " + v + "j :" + handbrake);

        }

        void OnEnable()
        {
            MVRInputManager.OnOrientation += MVRInputManager_OnOrientation;
        }

        private void FixedUpdate()
        {
            m_Car.Move(hor, ver, ver, 0f);
        }

        private void MVRInputManager_OnOrientation(Quaternion gyro, Vector3 acc)
        {
            hor = acc.x;
            steeringWheel.transform.Rotate(new Vector3(0, hor*8, 0));
        }

        void OnDisable()
        {
            MVRInputManager.OnOrientation -= MVRInputManager_OnOrientation;
        }

    }
}
