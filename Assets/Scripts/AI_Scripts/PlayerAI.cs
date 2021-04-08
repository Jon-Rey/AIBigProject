using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{

    public float jump_time_buffer = 1.0f;

    //key is distance, list is the times
    KeyValuePair<float, List<float>> TrackedJumpTimes;
    Queue<float> JumpQueue;

    float internalTimer = 0;
    float timerBuffer = 0.05f;


    
    float curr_jump_time;

    PlayerScript playerScript;

    public enum STATE
    {
        DEAD,
        ACTIVE,
        INACTIVE
    }
    STATE currState;

    

    public PlayerAI(List<float> _timestojump)
    {
        playerScript = GetComponent<PlayerScript>();
        currState = STATE.INACTIVE;
        JumpQueue = new Queue<float>();
        foreach(float time in JumpQueue)
        {
            JumpQueue.Enqueue(time);
        }

        curr_jump_time = internalTimer + jump_time_buffer;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        JumpOnTime();
        internalTimer += Time.deltaTime;
    }

    void JumpOnTime()
    {
        if (currState == STATE.ACTIVE)
        {
            if (JumpQueue.Count != 0)
            {
                float jumpTime = JumpQueue.Peek();
                if (internalTimer < jumpTime + timerBuffer || internalTimer >= jumpTime - timerBuffer)
                {
                    playerScript.Jump();
                    TrackedJumpTimes.Value.Add(JumpQueue.Dequeue());
                }
            }
            else
            {
                if (internalTimer >= curr_jump_time)
                {
                    playerScript.Jump();
                    TrackedJumpTimes.Value.Add(internalTimer);
                    curr_jump_time = internalTimer + jump_time_buffer;
                }
            }
        }
    }
}
