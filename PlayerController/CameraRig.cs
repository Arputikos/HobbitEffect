using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    [System.Serializable]
    public class CameraRig
    {
        [SerializeField] private Transform _cameraPivot;
        [SerializeField] private Transform _cameraOffset;
        [SerializeField] private Transform _cameraBobHolder;
        [SerializeField] private Transform _cameraShakeHolder;

        public Transform CameraPivot
        {
            get { return _cameraPivot; }
        }

        public Transform CameraOffset
        {
            get { return _cameraOffset; }
        }

        public Transform CameraBobHolder
        {
            get { return _cameraBobHolder; }
        }

        public Transform CameraShakeHolder
        {
            get { return _cameraShakeHolder; }
        }

        public bool IsValid()
        {
            if (_cameraPivot != null && _cameraOffset != null && _cameraBobHolder != null && _cameraShakeHolder != null)
                return true;

            Debug.LogError("Camera Rig is invalid!!!111oneone");
            return false;
        }
    }
}
