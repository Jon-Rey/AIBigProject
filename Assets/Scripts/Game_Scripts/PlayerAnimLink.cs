using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimLink : MonoBehaviour
{
    PlayerScript pScript;
    Animator pAnimator;
    // Start is called before the first frame update
    void Start()
    {
        pScript = GetComponent<PlayerScript>();
        if(pScript == null)
        {
            throw new System.Exception("[PlayerAnimLink] This script requires the player to have PlayerScript.cs attached to it.");
        }
        pAnimator = GetComponent<Animator>();
        if (pAnimator == null)
        {
            throw new System.Exception("[PlayerAnimLink] This script requires the player to have animator attached to it.");
        }

    }


    // Update is called once per frame
    void Update()
    {
        pAnimator.SetBool("Jump", !pScript.Get_On_ground());
    }
}
