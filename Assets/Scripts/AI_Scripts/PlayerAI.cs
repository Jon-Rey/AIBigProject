using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    //list of frames is how many frames it takes to run the course
    public List<int> Chromosome { get; private set; }
    
    // TODO: make sure the chromosome length is always even. adjust on init if needed.

    public int fitness { get; set; }

    int internalFrameCount = 0;

    int jumpFramCount = 0;

    PlayerScript playerScript;

    [HideInInspector]
    public GameObject PlayerGO { get; private set; }


    public enum STATE
    {
        DEAD,
        ACTIVE,
        INACTIVE,
        TESTRUN
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



        if(currState == STATE.TESTRUN)
        {
            if(playerScript.Get_On_ground() == false)
            {
                jumpFramCount += 1;
            }
        }
    }

    private void HandlePlayerDeath(int dist)
    {
        if(currState == STATE.TESTRUN)
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
        currState = STATE.ACTIVE;
        Chromosome = _jumpframes;
    }

    public void StartPlayerAI_testRun()
    {
        currState = STATE.TESTRUN;
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
        else if(currState == STATE.TESTRUN)
        {
            if(internalFrameCount == 2)
            {
                playerScript.Jump();
            }
        }
    }


    
}
