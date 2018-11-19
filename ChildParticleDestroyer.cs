using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildParticleDestroyer : MonoBehaviour {

    //destroys this object when particled in children and this are finished


    //List<ParticleSystem> systems;
    float timeDead = 0;
    float max = 0;


    void Start()
    {
        //systems = new List<ParticleSystem>();
        //assign all particle systems
        int children = this.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            if (this.transform.GetChild(i))
            {
                var ps = this.transform.GetChild(i).GetComponent<ParticleSystem>();
                if (ps)
                    if (max < ps.main.duration + ps.main.startDelay.constant)
                        max = ps.main.duration + ps.main.startDelay.constant;
            }
            //systems.Add(this.transform.GetChild(i).GetComponent<ParticleSystem>());
        }
        //if (this.GetComponent<ParticleSystem>())
        //    systems.Add(this.gameObject.GetComponent<ParticleSystem>());

       // Debug.Log("systems = " + systems.Count);

        if (max <= 0)
            Destroy(this.gameObject);

    }

	void Update ()
    {
            if (timeDead > max)
                Destroy(this.gameObject);

        timeDead += Time.deltaTime;
    }
}
