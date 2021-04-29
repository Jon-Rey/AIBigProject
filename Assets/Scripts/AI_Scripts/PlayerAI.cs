using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAI : MonoBehaviour
{
    //list of frames is how many frames it takes to run the course
    public List<int> Chromosome { get; set; }

    // TODO: make sure the chromosome length is always even. adjust on init if needed.

    public float fitness { get; set; }

    [FormerlySerializedAs("internalFrameCount")] [HideInInspector]
    public int ChromosomeLength = 0;

    [HideInInspector]
    public int CurrentChromosomeIndex = 0;

    int jumpFramCount = 0;

    private bool isTestRun = true;

    PlayerScript playerScript;

    public enum STATE
    {
        DEAD,
        ACTIVE,
        INACTIVE,
        FINISH
    }
    public STATE currState = STATE.INACTIVE;

    // Start is called before the first frame update
    void Awake()
    {
        playerScript = GetComponent<PlayerScript>();
        playerScript.OnPlayerDeath += HandlePlayerDeath;
    }

    // Update is called once per frame
    void Update()
    {
        JumpOnFrame();
        if (playerScript.Moving)
        {
            CurrentChromosomeIndex += 1;
        }
        if (currState == STATE.FINISH)
        {
            Debug.Log("Yay, you win!");
        }
        
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

    private void HandlePlayerDeath(int currentFrame)
    {
        CurrentChromosomeIndex = 0;
        currState = STATE.DEAD;
        playerScript.Moving = false;
    }

    public void ResetAi()
    {
        currState = STATE.INACTIVE;
        playerScript.Respawn();
    }

    public void StartPlayerAI()
    {
        isTestRun = false;
        currState = STATE.ACTIVE;
        playerScript.Moving = true;
        Physics.IgnoreLayerCollision(6, 8, false);
    }

    public void StartPlayerAI_testRun()
    {
        isTestRun = true;
        playerScript.Moving = true;
        // ignore spikes
        Physics.IgnoreLayerCollision(6, 8, true);
    }

    void JumpOnFrame()
    {
        if (!isTestRun && currState == STATE.ACTIVE)
        {
            try 
            {

                if (Chromosome[CurrentChromosomeIndex] == 1)
                {
                    playerScript.Jump();
                }
            }
            catch
            {
                //Forgive me bash ;-;
            }

        }
        else if(isTestRun)
        {
            if(CurrentChromosomeIndex == 2)
            {
                playerScript.Jump();
            }
        }
    }
}
