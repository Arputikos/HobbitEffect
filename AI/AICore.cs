using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AIHealth))]
public class AICore : MonoBehaviour
{
    protected NavMeshAgent _navMeshAgent;
    protected Animator _animator;
    protected AIHealth _aiHealth;

    Rigidbody[] bones;

    protected virtual void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _aiHealth = GetComponent<AIHealth>();

        bones = GetComponentsInChildren<Rigidbody>();

        SwitchBones(true);
    }



    static public Vector3 GetRandomMapPosition()
    {
        Bounds levelBounds = GameController.i.levelBounds;
        Vector3 position = new Vector3(Random.Range(levelBounds.min.x, levelBounds.max.x), 0, Random.Range(levelBounds.min.z, levelBounds.max.z));
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 100, 1))
            return hit.position;
        else
            return levelBounds.center;
    }

    public void Death()
    {
        _animator.enabled = false;
        SwitchBones(false);
        Invoke("DestroyDis", 5.0f);
    }

    void SwitchBones(bool disable)
    {
        foreach (var bone in bones)
        {
            bone.isKinematic = disable;
            bone.detectCollisions = !disable;
        }
    }

    void DestroyDis()
    {
        Destroy(gameObject);
    }
}
