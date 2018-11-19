using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    [SerializeField] private float _minLifetime = 1.0f;
    [SerializeField] private float _maxLifetime = 2.0f;

	void Start ()
    {
        Invoke("DestroyThisObject", Random.Range(_minLifetime, _maxLifetime));
	}
	
	void DestroyThisObject ()
    {
        Destroy(gameObject);
	}
}
