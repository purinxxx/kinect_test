using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class MainCameraControll : MonoBehaviour
    {
        /*
        [SerializeField]
        private MouseLook1 m_MouseLook;
        public Camera m_Camera;
        */

        // Use this for initialization
        void Start()
        {
            //m_MouseLook.Init(transform, m_Camera.transform);

        }

        // Update is called once per frame
        void Update()
        {
            /*
            m_MouseLook.LookRotation(transform, m_Camera.transform);
            transform.Translate(Vector3.up * Input.GetAxis("Vertical1") / 2);
            transform.Translate(Vector3.left * Input.GetAxis("Horizontal1") / 2);
            transform.Translate(Vector3.forward * Input.GetAxis("R2") / 2);
            transform.Translate(Vector3.back * Input.GetAxis("L2") / 2);
            */

            
        }
    }
}