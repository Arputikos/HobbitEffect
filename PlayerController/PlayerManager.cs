using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class PlayerManager : MonoBehaviour
    {
        private PlayerController _playerController;
        private Crosshair _crosshair;

        private bool isPaused;

        public bool IsPaused
        {
            get { return isPaused; }
        }

        private void Awake()
        {
            ToggleCursor(false);
            _playerController = GetComponentInChildren<PlayerController>();
            _crosshair = GetComponentInChildren<Crosshair>();
        }

        private void Update()
        {
            UpdateInput();

            if (_playerController.Armed != PlayerController.EquipType.None)
            {
                _crosshair.gameObject.SetActive(true);
            }
            else
            {
                _crosshair.gameObject.SetActive(false);
            }

            _crosshair.SetSpread(_playerController.CheapVelocityMagnitude() * 10);
        }

        void UpdateInput()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause(!isPaused);
            }
        }

        void ToggleCursor(bool enabled)
        {
            if(!enabled)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }

        void TogglePause(bool pause)
        {
            ToggleCursor(pause);
            if(pause)
            {
                Time.timeScale = 0.00001f;
                isPaused = true;
            }
            else
            {
                Time.timeScale = 1.0f;
                isPaused = false;
            }
        }
    }
}
