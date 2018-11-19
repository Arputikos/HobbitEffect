using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    [System.Serializable]
    public class Footsteps : PlayerControllerUtilities
    {
        public AudioClip[] walkClips;
        public AudioClip[] runClips;
        public AudioClip[] weaponClips;

        AudioClip[] targetClips;

        public AudioSource[] footSounds;
        public AudioSource weaponAudio;
        float nextStepTime;
        int footIndex;

        public override void Initialize(PlayerController playerController, Transform transform)
        {
            base.Initialize(playerController, transform);
        }

        public void UpdateFootsteps(float bobCycle)
        {
            if (bobCycle > nextStepTime && _pc.CharacterController.isGrounded)
            {
                targetClips = (!_pc._running) ? walkClips : runClips;
                
                nextStepTime = bobCycle + 0.5f;

                int i = Random.Range(0, targetClips.Length);
                footSounds[footIndex].clip = targetClips[i];
                footSounds[footIndex].Play();

                if (weaponAudio != null && weaponClips.Length > 0)
                {
                    i = Random.Range(0, weaponClips.Length);
                    weaponAudio.clip = weaponClips[i];
                    weaponAudio.Play();
                }

                if (footIndex == footSounds.Length - 1)
                    footIndex = 0;
                else
                    footIndex++;
            }
        }
    }
}
