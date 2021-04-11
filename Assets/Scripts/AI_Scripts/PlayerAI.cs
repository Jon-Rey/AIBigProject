using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{

    public float jump_time_buffer = 1.0f;


    List<float> TrackedJumpTimes;
    Queue<float> JumpQueue;

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
