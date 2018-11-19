using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    [System.Serializable]
    public class Bob : PlayerControllerUtilities
    {
        [SerializeField] private float bobSpeed = 0.66f;
        [SerializeField] private float stepLength = 0.3f;

        [Space]
        [SerializeField]
        [Range(0.5f, 20.0f)]
        private float globalBobDivider = 5.0f;
        float globalBobMultiplier;

        [SerializeField] [Range(-0.5f, 0.5f)] private float posBobX = 0.05f;
        [SerializeField] [Range(-0.5f, 0.5f)] private float posBobY = 0.1f;
        [SerializeField] [Range(-0.5f, 0.5f)] private float posBobZ = 0.05f;

        [SerializeField] [Range(-10.0f, 10.0f)] private float rotBobX = 5.0f;
        [SerializeField] [Range(-10.0f, 10.0f)] private float rotBobY = 0.5f;
        [SerializeField] [Range(-10.0f, 10.0f)] private float rotBobZ = 2.0f;

        float m_bobPX;
        float m_bobPY;
        float m_bobPZ;

        float m_bobRX;
        float m_bobRY;
        float m_bobRZ;

        float m_bobCycle;
        public float BobCycle
        {
            get { return m_bobCycle; }
        }

        Vector3 orgPos;
        Quaternion orgRot;

        Vector3 targetPosBob;
        Quaternion targetRotBob;

        Vector3 previousPosition;
        Vector3 previousVelocity;

        Transform m_bobTransform;

        public override void Initialize(PlayerController playerController, Transform transform)
        {
            base.Initialize(playerController, transform);

            m_bobTransform = _pc._cameraRig.CameraBobHolder;

            orgPos = m_bobTransform.localPosition;
            orgRot = m_bobTransform.localRotation;

        }

        public void DoBob()
        {
            UpdateBobValues(out targetPosBob, out targetRotBob);
            ExecuteBob();
        }

        void UpdateBobValues(out Vector3 posBob, out Quaternion rotBob)
        {
            m_bobCycle += BobCycleAddition();

            if (!_pc.CharacterController.isGrounded)
            {
                globalBobMultiplier = Mathf.Lerp(globalBobMultiplier, 0, Time.deltaTime * 5);
            }
            else
            {
                globalBobMultiplier = Mathf.Lerp(globalBobMultiplier, _pc.CharacterController.velocity.magnitude / globalBobDivider, Time.deltaTime * 10);
            }

            m_bobPX = -Mathf.Sin(m_bobCycle * Mathf.PI * 2) * posBobX * globalBobMultiplier;
            m_bobPY = -Mathf.Cos(m_bobCycle * Mathf.PI * 4) * posBobY * globalBobMultiplier;
            m_bobPZ = Mathf.Sin(m_bobCycle * Mathf.PI * 2) * posBobZ * globalBobMultiplier;

            m_bobRX = Mathf.Sin(m_bobCycle * Mathf.PI * 4) * rotBobX * globalBobMultiplier;
            m_bobRY = -Mathf.Cos(m_bobCycle * Mathf.PI * 2) * rotBobY * globalBobMultiplier;
            m_bobRZ = -Mathf.Cos(m_bobCycle * Mathf.PI * 2) * rotBobZ * globalBobMultiplier;

            posBob = new Vector3(m_bobPX, m_bobPY, m_bobPZ);
            rotBob = Quaternion.Euler(m_bobRX, m_bobRY, m_bobRZ);
        }

        void ExecuteBob()
        {
            m_bobTransform.localPosition = targetPosBob + orgPos;
            m_bobTransform.localRotation = targetRotBob;
        }

        float BobCycleAddition()
        {
            Vector3 velocity = (_transform.position - previousPosition) / (Time.deltaTime + 0.0001f);
            Vector3 velocityChange = velocity - previousVelocity;

            previousPosition = _transform.position;
            previousVelocity = velocity;

            float flatVelocity = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;
            float stepLengthen = 1 + (flatVelocity * stepLength);

            float cycle = (flatVelocity / stepLengthen) * (Time.deltaTime * bobSpeed);
            return cycle;
        }

        public float GetBobMagnitude()
        {
            return m_bobPX;
        }

    }
}
