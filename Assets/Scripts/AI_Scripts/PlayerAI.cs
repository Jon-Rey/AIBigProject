using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    //list of frames is how many frames it takes to run the course
    public List<int> Chromosome { get; private set; }

    // TODO: make sure the chromosome length is always even. adjust on init if needed.

    public float fitness { get; set; }

    [HideInInspector]
    public int internalFrameCount = 0;

    int jumpFramCount = 0;

    private bool isTestRun = true;

    PlayerScript playerScript;

    [HideInInspector]
    public GameObject PlayerGO { get; private set; }


    public enum STATE
    {
        DEAD,
        ACTIVE,
        INACTIVE,
        FINISH
    }
    public STATE currState;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<PlayerScript>();
        playerScript.OnPlayerDeath += HandlePlayerDeath;
        PlayerGO = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        JumpOnFrame();
        internalFrameCount += 1;
        
        if (currState == STATE.FINISH)
            Debug.Log("Yay, you win!");
        
        if(!isTestRun)
        {
            
        }
        else
        {
            if(playerScript.Get_On_ground() == false)
            {
                jumpFramCount += 1;
            }
        }
    }

    private void HandlePlayerDeath(int dist)
    {
        if(isTestRun)
            Debug.Log($"Died with dist traveled={dist} And JumpFrame Count={jumpFramCount}");
        else
            Debug.Log($"Died with dist traveled={dist}");
        currState = STATE.DEAD;
        ResetAi();
    }


    private void ResetAi()
    {
        internalFrameCount = 0;
    }

    public void StartPlayerAI(List<int> _jumpframes)
    {
        isTestRun = false;
        currState = STATE.ACTIVE;
        Chromosome = _jumpframes;
        Physics.IgnoreLayerCollision(6, 8, false);
    }

    public void StartPlayerAI_testRun()
    {
        isTestRun = true;
        Physics.IgnoreLayerCollision(6, 8, true);
    }

    void JumpOnFrame()
    {
        if (currState == STATE.ACTIVE)
        {
            
            if(Chromosome[internalFrameCount] == 1)
            {
                playerScript.Jump();
            }
        }
        else if(isTestRun)
        {
            if(internalFrameCount == 2)
            {
                playerScript.Jump();
            }
        }
    }

}
