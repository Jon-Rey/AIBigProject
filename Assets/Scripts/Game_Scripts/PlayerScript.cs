using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject Player;
    public GameObject Spawn;
    public float Speed = 10.0f;
    public float Jump_force = 300.0f;
    public Vector3 oldpos;
    public bool Moving = false;
    private bool On_ground;
    private Rigidbody Rigidbody;
    private PlayerAI PlayerAIScript;

    public delegate void PlayerDiedEvent(int framesTraveled);
    public event PlayerDiedEvent OnPlayerDeath;

    int frameCount = 0;


    void Awake()
    {
        PlayerAIScript = Player.GetComponent<PlayerAI>();
        Rigidbody = GetComponent<Rigidbody>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        On_ground = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Moving) return;
        
        frameCount += 1;
        oldpos = transform.position;
        transform.position += new Vector3(Speed * 0.5f*Time.deltaTime, 0.0f, 0.0f);
    }

    public void Jump()
    {
        if (Get_On_ground())
        {
            Rigidbody.AddForce(0.0f, Jump_force, 0.0f);
            On_ground = false;
        }
    }
    public void Set_Jump_force(float jump)
    {
        Jump_force = jump;
    }

    public bool Get_On_ground()
    {
        return On_ground;
    }


    void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.tag == "Ground")
        {
            On_ground = true;
        }
        else if (collision.collider.tag == "Spikey")
        {
            if (OnPlayerDeath != null)
            {
                OnPlayerDeath(frameCount);
            }
        }
        else if (collision.collider.tag == "Finish")
        {
            PlayerAIScript.currState = PlayerAI.STATE.FINISH;
        }
    }
    
    public void Respawn()
    {
        if (OnPlayerDeath != null)
        {
            OnPlayerDeath(frameCount);
        }
        Moving = false;
        // Player.SetActive(false);
        Player.transform.position = Spawn.transform.position;
        // Player.SetActive(true);
        // Moving = true;
    }


}
