using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{

    public float jump_time_buffer = 0.5f;


    List<float> TrackedJumpTimes;
    Queue<float> JumpQueue;
    private float MaxDistTraveled = 0f;

    float internalTimer = 0;
    float timerBuffer = 0.05f;


    
    float curr_jump_time;
    float initial_jump_time;

    PlayerScript playerScript;


    public enum STATE
    {
        DEAD,
        ACTIVE,
        INACTIVE
    }
    STATE currState;

    // Start is called before the first frame update
    void Start()
    {
        // sign up to listen for player death event
        playerScript.OnPlayerDeath += HandlePlayerDeath;
        TrackedJumpTimes = new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        JumpOnTime();
        internalTimer += Time.deltaTime;

        if(playerScript.Moving == false)
        {
            ResetAi();
        }
    }

    private void HandlePlayerDeath(float dist)
    {
        Debug.Log($"Died with dist traveled={dist}");
        MaxDistTraveled = dist;
        currState = STATE.DEAD;
    }

public void ResetAi()
{
    TrackedJumpTimes.Clear();
    internalTimer = 0.0f;
    curr_jump_time = initial_jump_time;
}

    public void StartPlayerAI(List<float> _timestojump)
    {
        playerScript = GetComponent<PlayerScript>();
        currState = STATE.ACTIVE;
        JumpQueue = new Queue<float>();
        foreach (float time in JumpQueue)
        {
            JumpQueue.Enqueue(time);
        }

        initial_jump_time = curr_jump_time = internalTimer + jump_time_buffer;
        Debug.Log($"curr_jump_time: {curr_jump_time}");
    }

    void JumpOnTime()
    {
        if (currState == STATE.ACTIVE)
        {
            if (JumpQueue.Count != 0)
            {
                float jumpTime = JumpQueue.Peek();
                if (internalTimer >= jumpTime)
                {
                    playerScript.Jump();
                    TrackedJumpTimes.Add(JumpQueue.Dequeue());
                    curr_jump_time = internalTimer + jump_time_buffer;
                }
            }
            else
            {
                if (internalTimer >= curr_jump_time)
                {
                    playerScript.Jump();
                    TrackedJumpTimes.Add(internalTimer);
                    curr_jump_time = internalTimer + jump_time_buffer;
                }
            }
        }
    }

}
