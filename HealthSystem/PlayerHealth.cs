using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    protected virtual void Start()
    {
        _maxHealth = 100;//temp constant
        _currentHealth = _maxHealth;
    }
}
