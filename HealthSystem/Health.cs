using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    [SerializeField] public int _maxHealth { protected set; get; }
    public int _currentHealth { protected set; get; }

    public bool Dead
    {
        get
        {
            return (_currentHealth <= 0) ? true : false;
        }
    }

    protected virtual void Start()
    {
        _maxHealth = 100;//temp constant
        _currentHealth = _maxHealth;
    }

    public virtual void TakeHit(int damage)
    {

        _currentHealth -= damage;
        GameController.i.UpdateUI();
    }

    public bool isDead()
    {
        return _currentHealth <= 0;
    }
}

