using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RangedAI : AICore
{
    [SerializeField] GameObject waypointsHolder;

    [Header("Locomotion")]
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float patrolWaitTime = 1.0f;
    public float lookRange = 10.0f;

    [Header("Shooting")]
    public float fireRate = 5.0f;
    public float fireRandomizer = 2.0f;
    public Vector2 damageRange = new Vector2(2, 5);
    public ParticleSystem shootTrails;
    public Transform muzzle;
    float timer;
    bool shooting;

    bool playerSpotted;

    private float patrolTimer;
    private int waypointIndex;

    private List<Waypoint> waypoints = new List<Waypoint>();

    Transform playerTransform;


	protected override void Awake ()
    {
        base.Awake();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

		for(int i = 0; i < waypointsHolder.transform.childCount; i++)
        {
            waypoints.Add(waypointsHolder.transform.GetChild(i).GetComponent<Waypoint>());
        }
	}
	
	void Update ()
    {
        if (!_aiHealth.Dead)
        {
            Patrolling();
            UpdateAnimator();
            Checking();

            if (playerSpotted)
                EngagePlayer();

        }
        else
        {
            _navMeshAgent.isStopped = true;
            if (GetComponent<CapsuleCollider>())
                Destroy(GetComponent<CapsuleCollider>());
            Death();
        }
	}

    void Checking()
    {
        if ((playerTransform.position - this.transform.position).magnitude <= lookRange)
        {
            playerSpotted = true;
        }

    }

    void Patrolling()
    {
        if(waypoints[waypointIndex].Owner == this)
        {
            _navMeshAgent.destination = waypoints[waypointIndex].transform.position;

            if (_navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance)
            {
                patrolTimer += Time.deltaTime;
                if (patrolTimer >= patrolWaitTime)
                {
                    waypoints[waypointIndex].Release();
                    IncreaseIndex();
                    patrolTimer = 0.0f;
                }

            }
            else
            {
                patrolTimer = 0.0f;
            }
        }
        else
        {
            if(waypoints[waypointIndex].Owner == null)
            {
                waypoints[waypointIndex].Claim(this);
            }
            else
            {
                IncreaseIndex();
            }
        }

    }

    void IncreaseIndex()
    {
        if (waypointIndex == waypoints.Count - 1)
        {
            waypointIndex = 0;
        }
        else
        {
            waypointIndex++;
        }
    }

    void UpdateAnimator()
    {
        Vector3 move = _navMeshAgent.desiredVelocity;
        Vector3 localMove = transform.InverseTransformDirection(move);

        float vertical = localMove.z;
        float horizontal = localMove.x;

        _animator.SetFloat("Vertical", vertical);
        _animator.SetFloat("Horizontal", horizontal);
    }

    void EngagePlayer()
    {
        transform.LookAt(playerTransform);
        Attack();
    }

    void Attack()
    {
        timer -= Time.deltaTime;


        //RaycastHit hit;
        //if (Physics.Raycast(muzzle.position, Camera.main.transform.position - muzzle.position, out hit))
        //{
        //if (hit.transform.tag == "Player")
        //{
        //shooting = true;
        if (timer <= 0)
        {
            _animator.SetTrigger("Shot");
            shootTrails.Emit(1);
            timer = 1 / fireRate + Random.Range(0, fireRandomizer);
            GetComponent<AudioSource>().Play();
            playerTransform.GetComponentInParent<PlayerHealth>().TakeHit((int)Random.Range(damageRange.x, damageRange.y));

        }

            
        
    }
}
