using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class PlayerAnimator : ControllerCore
    {
        [Header("Hands IK")]
        [SerializeField] private Transform _rightHandIKTarget;
        [SerializeField] [Range(0.0f, 1.0f)] private float _rightHandPosWeight = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float _rightHandRotWeight = 1.0f;

        [SerializeField] private Transform _leftHandIKTarget;
        [SerializeField] [Range(0.0f, 1.0f)] private float _leftHandPosWeight = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float _leftHandRotWeight = 1.0f;

        [Header("Look IK")]
        [SerializeField] private float _lookDistance = 30.0f;
        [SerializeField] private float _lookLerpSpeed = 5.0f;
        [SerializeField] private LayerMask _layerMask;

        [SerializeField] [Range(0.0f, 1.0f)] private float _mainWeight = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float _bodyWeight = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float _headWeight = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float _eyesWeight = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float _clampWeight = 1.0f;

        [System.Serializable]
        public struct WeaponRig
        {
            public Transform _weaponTransform;
            public Transform _armedWeaponParent;
            public Vector3 _armedWeaponPosition;
            public Vector3 _armedWeaponRotation;

            public Transform _disarmedWeaponParent;
            public Vector3 _disarmedWeaponPosition;
            public Vector3 _disarmedWeaponRotation;
        }

        [SerializeField] private WeaponRig weapon1, weapon2;
        private WeaponRig currentWeapon;

        private Animator _animator;
        private PlayerController _pc;

        private Vector3 _rawLookPos, _finalLookPos;

        bool noWeapons = true;


        protected override void Start()
        {
            _animator = GetComponent<Animator>();
            _pc = GetComponentInParent<PlayerController>();

            UnequipWeapon(weapon1);
            UnequipWeapon(weapon2);
        }

        protected override void Update()
        {
            UpdateAnimator();
            FixWeaponIK();

            _rawLookPos = GetLookPos();

            if(_pc.Armed == PlayerController.EquipType.Steel)
            {
                currentWeapon = weapon1;
                noWeapons = false;
            }
            else if(_pc.Armed == PlayerController.EquipType.Silver)
            {
                currentWeapon = weapon2;
                noWeapons = false;
            }
            else
            {
                noWeapons = true;
            }

        }

        void UpdateAnimator()
        {
            _animator.SetFloat("Horizontal", Horizontal);
            _animator.SetFloat("Vertical", Vertical);
            _animator.SetBool("Armed", !noWeapons);
        }

        void FixWeaponIK()
        {
            if (!noWeapons)
            {
                currentWeapon._weaponTransform.LookAt(_rawLookPos);
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            _animator.SetLookAtPosition(_rawLookPos);
            if (!noWeapons)
            {
                _animator.SetLookAtWeight(_mainWeight, _bodyWeight, _headWeight, _eyesWeight, _clampWeight);

                _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandIKTarget.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandIKTarget.rotation);
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _rightHandPosWeight);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _rightHandRotWeight);

                _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandIKTarget.position);
                _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandIKTarget.rotation);
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _leftHandPosWeight);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _leftHandRotWeight);
            }
            else
            {
                _animator.SetLookAtWeight(_mainWeight, _bodyWeight * 0.25f, _headWeight, _eyesWeight, _clampWeight);
            }


        }

        Vector3 GetLookPos()
        {
            Transform cameraTransform = Camera.main.transform;
            RaycastHit hit;

            if(Physics.Raycast(cameraTransform.position, cameraTransform.transform.forward, out hit, _lookDistance, _layerMask))
            {
                return hit.point;
            }
            return cameraTransform.position + cameraTransform.forward * _lookDistance;
        }



        public void EquipWeapon()
        {
            currentWeapon._weaponTransform.parent = currentWeapon._armedWeaponParent;
            currentWeapon._weaponTransform.localPosition = currentWeapon._armedWeaponPosition;
            currentWeapon._weaponTransform.localRotation = Quaternion.Euler(currentWeapon._armedWeaponRotation);
        }

        public void UnequipWeapon()
        {
            currentWeapon._weaponTransform.parent = currentWeapon._disarmedWeaponParent;
            currentWeapon._weaponTransform.localPosition = currentWeapon._disarmedWeaponPosition;
            currentWeapon._weaponTransform.localRotation = Quaternion.Euler(currentWeapon._disarmedWeaponRotation);
        }

        public void UnequipWeapon(WeaponRig weapon)
        {
            weapon._weaponTransform.parent = weapon._disarmedWeaponParent;
            weapon._weaponTransform.localPosition = weapon._disarmedWeaponPosition;
            weapon._weaponTransform.localRotation = Quaternion.Euler(weapon._disarmedWeaponRotation);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_rawLookPos, 0.125f);
        }

        public void Shoot()
        {
            _animator.SetTrigger("Shoot");
        }
    }
}
