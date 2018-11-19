using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    RangedAI owner;

    public RangedAI Owner
    {
        get { return owner; }
    }

    public void Claim(RangedAI ai)
    {
        owner = ai;
    }

    public void Release()
    {
        owner = null;
    }

}
