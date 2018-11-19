using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIZombie : AICore
{
    private enum ZOMBIE_STATE
    {
        IDLE,
        WALK,
        SCREAM,
        ATTACK,
        DIE,

        _COUNT
    }


    protected float moveSpeed;//per second, depends on animation
    private int attackAnimID;
    private int moveAnimID;
    private int deathAnimID;

    ZOMBIE_STATE state;

    static private float lookRange = 10.0f;//temp constant
    static private float attackRange = 1.57f;
    static private int damage = 10;
    private GameObject player;

    private float timeToScream;
    //private float timeToWalk;
    //private float timerWalk;
    private float timer;

    private bool dead;

    private bool attacking;

    private string currAnimName;//read only use

    protected void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        state = ZOMBIE_STATE.IDLE;
        attackAnimID = Random.Range(0, 2) + 1;
        moveAnimID = Random.Range(0, 4) + 1;
        deathAnimID = Random.Range(0, 2) + 1;

        moveSpeed = moveAnimID * 0.44f;
        timer = timeToScream =  0;
        RandomizeTimeToScream();

        dead = false;
        attacking = false;

        _navMeshAgent.speed = moveSpeed;
    }

    void Update()
    {
        if (dead)
            return;

        timer += Time.deltaTime;

        //if is going to be dead
        if (_aiHealth.isDead())
            ChangeState(ZOMBIE_STATE.DIE);
        if(IsAnimationFinished())
            Think();
        else
        {
            //some animation is playing
            switch (state)
            {
                case ZOMBIE_STATE.IDLE:
                    Think();
                    break;
                case ZOMBIE_STATE.WALK:
                    Think();
                    break;
                case ZOMBIE_STATE.SCREAM:
                    break;
                case ZOMBIE_STATE.ATTACK:
                    transform.LookAt(GameController.i.player.transform.position);
                    Think();
                    break;
                case ZOMBIE_STATE.DIE:
                    break;
                case ZOMBIE_STATE._COUNT:
                default:
                    break;
            }
        }

    }

    void Think()
    {
        if (dead)
            return;

        //bool dontInterrupt = ((state != ZOMBIE_STATE.ATTACK &&  state != ZOMBIE_STATE.SCREAM) || IsAnimationFinished());

        if ((player.transform.position - this.transform.position).magnitude <= attackRange)//jak odejdzie player za daleko to przestaje atakować
        {
            if (state != ZOMBIE_STATE.ATTACK)
            {
                attacking = true;
                ChangeState(ZOMBIE_STATE.ATTACK);
            }
            return;
        }
        else
            attacking = false;

        if ((player.transform.position - this.transform.position).magnitude <= lookRange)
        {
            if (state != ZOMBIE_STATE.WALK)
            {
                _navMeshAgent.SetDestination(player.transform.position);
                ChangeState(ZOMBIE_STATE.WALK);
            }
            return;
        }
        if (timer >= timeToScream)
        {
            ChangeState(ZOMBIE_STATE.SCREAM);
            RandomizeTimeToScream();
            return;
        }
        //else if (timerWalk >= timeToWalk)
        //{
        //    timeToWalk = 0;
        //    _navMeshAgent.SetDestination(GetRandomMapPosition());
        //    ChangeState(ZOMBIE_STATE.WALK);
        //}
        //else
        if (state != ZOMBIE_STATE.IDLE)
            ChangeState(ZOMBIE_STATE.IDLE);
    }

    void RandomizeTimeToScream()
    {
        timeToScream = Random.Range(3f, 10f);
        timer = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    attacking = true;
        //    ChangeState(ZOMBIE_STATE.ATTACK);
        //}
    }

    private void OnCollisionExit(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    //attacking = false; - will be not attacking when player is further a little bit
        //    Think();
        //}
    }

    void ChangeState(ZOMBIE_STATE newState)
    {
        //Debug.Log("new state " + newState);
        //if (state == newState)
        //    return;

        switch (newState)
        {
            case ZOMBIE_STATE.IDLE:
                _navMeshAgent.isStopped = true;
                currAnimName = "idle";
                this.GetComponent<Animator>().Play(currAnimName, 0);
                break;
            case ZOMBIE_STATE.WALK:
                //_navMeshAgent.SetDestination(player.transform.position);
                _navMeshAgent.isStopped = false;
                currAnimName = "w" + moveAnimID.ToString();
                this.GetComponent<Animator>().Play(currAnimName, 0);
                break;
            case ZOMBIE_STATE.SCREAM:
                _navMeshAgent.isStopped = true;
                currAnimName = "scream1";
                this.GetComponent<Animator>().Play(currAnimName, 0);
                break;
            case ZOMBIE_STATE.ATTACK:
                _navMeshAgent.isStopped = true;
                currAnimName = "a" + attackAnimID.ToString();
                this.GetComponent<Animator>().Play(currAnimName, 0);
                break;
            case ZOMBIE_STATE.DIE:
                state = ZOMBIE_STATE.DIE;
                _navMeshAgent.isStopped = true;
                dead = true;
                //delete the collider
                if (GetComponent<CapsuleCollider>())
                    Destroy(GetComponent<CapsuleCollider>());
                _navMeshAgent.isStopped = true;
                if(GetComponent<Rigidbody>())
                    Destroy(GetComponent<Rigidbody>());
                Death();
                break;
            case ZOMBIE_STATE._COUNT:
            default:
                break;
        }

        state = newState;
    }

    void ReplayAnim()
    {
        this.GetComponent<Animator>().Play(currAnimName, 0, 0);
    }

    bool IsAnimationFinished()
    {
        return (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(currAnimName) &&
        GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
    }

    void DoDamage()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().TakeHit(damage);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>().Play();//temp
        //GameObject fxInstance = Instantiate(_bloodVFX, hit.point, Quaternion.LookRotation(hit.normal));
        //fxInstance.transform.parent = hit.transform;
        GameController.i.UpdateUI();
    }
}
