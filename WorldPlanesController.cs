using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;


public class WorldPlanesController : MonoBehaviour
{
    [SerializeField] PostProcessingProfile normal, negative;

    bool isNegative;

    PostProcessingBehaviour postProcessingBehaviour;
    Animator animator;

    public GameObject profileChangeVFXprefab;

    public Transform cameraCenter;

	void Start ()
    {
        postProcessingBehaviour = GetComponent<PostProcessingBehaviour>();
        animator = GetComponent<Animator>();

        if (postProcessingBehaviour.profile == negative)
            isNegative = true;
	}
	
	void Update ()
    {
		if(Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("Swap");
            ProfileChangeAnim();
            Invoke("SwapProfiles", 1.0f);
        }
	}

    void SwapProfiles()
    {
        if(!isNegative)
            postProcessingBehaviour.profile = negative;
        else
            postProcessingBehaviour.profile = normal;

        isNegative = !isNegative;
        GameController.i.ChangeWorlds();
    }

    void ProfileChangeAnim()
    {
        GameObject vfx = Instantiate(profileChangeVFXprefab, cameraCenter.position, cameraCenter.transform.rotation, this.transform);
        vfx.transform.Translate(-0.22f, 0, -1.04f, Space.Self);
    }
}
