using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    [System.Serializable]
    public class CameraController : PlayerControllerUtilities
    {
        [SerializeField] private float _sensitivity = 5.0f;
        [SerializeField] private float _smoothing = 0.1f;
        [SerializeField] private float _minPich = -90;
        [SerializeField] private float _maxPitch = 90;
        [SerializeField] private float _maxRoll = 45;

        [SerializeField] private Vector3 _unarmedCameraOffset;
        [SerializeField] private Vector3 _armedCameraOffset;
        [SerializeField] private float _heightDampPosFactor = 1.0f;
        [SerializeField] private float _heightDampRotFactor = 1.0f;
        [SerializeField] private float _heightDampSmoothing = 5.0f;

        private float _pitchV, _yawV;
        private float _pitch, _yaw;
        private float _currentPitch, _currentYaw;

        private float _pitchAddition;
        private float _rollAddition;

        private Vector3 _controllerMotion;
        private Vector3 _cameraOffset;

        public void UpdateCameraController (Vector3 motion)
        {
            if (_pc._cameraRig.IsValid())
            {
                _controllerMotion = motion;

                ModifyInput();
                SmoothenAngles();
                Roll(_controllerMotion);
                ApplyAngles();
            }
        }

        void ModifyInput()
        {
            _pitch -= _pc.MouseY * _sensitivity;
            _yaw += _pc.MouseX * _sensitivity;

            _pitch = Mathf.Clamp( _pitch, _minPich, _maxPitch);
        }

        void SmoothenAngles()
        {
            _currentPitch = Mathf.SmoothDamp(_currentPitch, _pitch, ref _pitchV, _smoothing);
            _currentYaw = Mathf.SmoothDamp(_currentYaw, _yaw, ref _yawV, _smoothing);
        }


        void ApplyAngles()
        {
            _pc._cameraRig.CameraPivot.localRotation = Quaternion.Euler(_currentPitch + _pitchAddition, 0, _rollAddition);//error nan
            _transform.localRotation = Quaternion.Euler(0, _currentYaw, 0);

            _pc._cameraRig.CameraOffset.localPosition = OffsetAddition(_controllerMotion);//error nan
        }

        void Roll(Vector3 motion)
        {
            motion = _transform.InverseTransformDirection(motion);

            _pitchAddition = Mathf.Lerp(_pitchAddition, motion.y * _heightDampRotFactor, Time.deltaTime * _heightDampSmoothing);
            _rollAddition = motion.x * _maxRoll;
        }

        Vector3 OffsetAddition(Vector3 motion)
        {
            motion = _transform.InverseTransformDirection(motion);

            Vector3 targetOffset = (_pc.Armed != PlayerController.EquipType.None) ? _armedCameraOffset : _unarmedCameraOffset;
            _cameraOffset = Vector3.Lerp(_cameraOffset, targetOffset, Time.deltaTime * 5);

            Vector3 offset = motion * _heightDampPosFactor + _cameraOffset;

            return offset;
        }

    }
}
