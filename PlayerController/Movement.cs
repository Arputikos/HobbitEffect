using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    [System.Serializable]
    public class Movement : PlayerControllerUtilities
    {
        [Header("Motion")]
        [SerializeField] private float _walkSpeed = 7.0f;
        [SerializeField] private float _runSpeed = 14.0f;
        [SerializeField] private float _speedTransitionSpeed = 3.0f;
        [SerializeField] private float _motionSmoothSpeed = 5.0f;

        [Header("Jumping")]
        [SerializeField] private float _jumpSpeed = 5.0f;
        [SerializeField] private float _ascendingGravityMultiplier = 2.0f;
        [SerializeField] private float _descendingGravityMultiplier = 4.0f;
        [SerializeField] private LayerMask _ceilingDetectionMask; 

        private float _moveSpeed;
        private Vector3 _smoothenMotion;

        public Vector3 SmoothenMotion
        {
            get { return _smoothenMotion; }
        }

        private Vector3 _jumpMotion;
        private float _currentHeight;

        public override void Initialize(PlayerController playerController, Transform transform)
        {
            base.Initialize(playerController, transform);

            _moveSpeed = _walkSpeed;
        }

        public void UpdateMovement()
        {
            _moveSpeed = Mathf.Lerp(_moveSpeed, CalculateMoveSpeed(), Time.deltaTime * _speedTransitionSpeed);
            UpdateJumping();
        }

        private float CalculateMoveSpeed()
        {
            if (!_pc.Sprint)
            {
                _pc._running = false;
                return _walkSpeed;
            }

            _pc._running = true;
            return _runSpeed;
        }

        private Vector3 CalculateMotion()
        {
            Vector3 m_motion = new Vector3(_pc.Horizontal, 0, _pc.Vertical);

            if (m_motion.magnitude > 1)
                m_motion.Normalize();

            m_motion *= _moveSpeed * Time.deltaTime;

            m_motion = _transform.localRotation * m_motion;

            return m_motion;
        }

        public void Jump(float jumpForce)
        {
            _currentHeight = jumpForce;
        }

        private void UpdateJumping()
        {
            if (_pc.CharacterController.isGrounded)
            {
                if (_pc.Jump)
                {
                    Jump(_jumpSpeed);
                }
            }
            else
            {
                float m_gravityMultiplier;
                if (_pc.CharacterController.velocity.y > 0)
                {
                    m_gravityMultiplier = _ascendingGravityMultiplier;
                }
                else
                {
                    m_gravityMultiplier = _descendingGravityMultiplier;
                }

                if (CeilingDetected())
                {
                    _currentHeight = -2;
                }
                _currentHeight += Physics.gravity.y * m_gravityMultiplier * Time.deltaTime;

            }

            _jumpMotion = new Vector3(0, _currentHeight, 0) * Time.deltaTime;
        }

        public bool CeilingDetected()
        {
            Collider[] colliders = Physics.OverlapSphere(_transform.position + new Vector3(0, _pc.CharacterController.height + 0.01f, 0), _pc.CharacterController.radius, _ceilingDetectionMask, QueryTriggerInteraction.Ignore);
            if (colliders.Length > 1)
                return true;

            return false;
        }

        public void UpdateMovementPhysics()
        {
            _smoothenMotion = Vector3.Lerp(_smoothenMotion, CalculateMotion(), Time.deltaTime * _motionSmoothSpeed);

            Vector3 m_finalMotion = _smoothenMotion + _jumpMotion;

            _pc.CharacterController.Move(m_finalMotion);
        }
    }
}
