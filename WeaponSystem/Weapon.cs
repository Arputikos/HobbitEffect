using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    [SerializeField] private int _weaponIndex;
    [SerializeField] private float _fireRate = 10.0f;

    [SerializeField] private AudioClip[] _shootClips;

    [SerializeField] private Animator _muzzleAnimator;
    [SerializeField] private GameObject _sparkVFX, _bloodVFX;
    [SerializeField] private ParticleSystem _shootTrails;
    [SerializeField] private LayerMask _layerMask;

    PlayerController _pc;

    AudioSource _audioSource;

    float timer;

    bool isSelected;

    static public int DAMAGE_FOR_ZOMBIE = 20;//temp constant
    static public int DAMAGE_FOR_HUMAN = 25;


    void Start ()
    {
        _audioSource = GetComponent<AudioSource>();
        _pc = GetComponentInParent<PlayerController>();
	}
	
	void Update ()
    {
        if (_weaponIndex == (int)_pc.Armed)
        {
            Shooting();
        }

        timer -= Time.deltaTime;
	}

    void Shooting()
    {
        if(Input.GetMouseButton(0) && timer <= 0)
        {
            Shoot();
            timer = 1 / _fireRate;

            //ammo change todo
            //GameController.i.UpdateUI();//ammo
        }
    }

    void Shoot()
    {
        ShootAudio();

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _layerMask))
        {
            
            if (hit.transform.gameObject.CompareTag("zombie"))//silver
            {
                if (2 == _weaponIndex)
                {
                    hit.transform.gameObject.GetComponent<AIHealth>().TakeHit(DAMAGE_FOR_ZOMBIE);
                    GameObject fxInstance = Instantiate(_bloodVFX, hit.point, Quaternion.LookRotation(hit.normal));
                    fxInstance.transform.parent = hit.transform;
                }
            }//todo elsse if not zombie
            else if (hit.transform.gameObject.CompareTag("human"))//steeel
            {
                if (1 == _weaponIndex)
                {
                    hit.transform.gameObject.GetComponent<AIHealth>().TakeHit(DAMAGE_FOR_HUMAN);
                    GameObject fxInstance = Instantiate(_bloodVFX, hit.point, Quaternion.LookRotation(hit.normal));
                    fxInstance.transform.parent = hit.transform;
                }
            }//todo elsse if not zombie
            else
            {
                GameObject fxInstance = Instantiate(_sparkVFX, hit.point, Quaternion.LookRotation(hit.normal));
                fxInstance.transform.parent = hit.transform;
            }
        }

        _shootTrails.Emit(1);

        GetComponentInParent<PlayerAnimator>().Shoot();
        _muzzleAnimator.SetTrigger("Shoot");
    }

    void ShootAudio()
    {
        byte audioRandomizer = (byte)Random.Range(0, _shootClips.Length);
        _audioSource.clip = _shootClips[audioRandomizer];
        _audioSource.Play();

    }
}
