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
    private bool Moving;
    private bool On_ground;

    // Start is called before the first frame update
    void Start()
    {
        On_ground = true;
        Moving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Moving)
        {
            oldpos = transform.position;
            transform.position += new Vector3(-Speed * 0.5f*Time.deltaTime, 0.0f, 0.0f);
            if (oldpos == transform.position)
            {
                StartCoroutine(Respawn());
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Get_On_ground())
                {
                    Debug.Log("Jump");
                    Jump();
                }
                else
                {
                    Debug.Log("Not able to jump");
                }
            }
        }
    }
    public void Jump()
    {
        GetComponent<Rigidbody>().AddForce(0.0f, Jump_force, 0.0f);
        On_ground = false;
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
        if (collision.collider.tag == "Ground" || collision.collider.tag == "Block")
        {
            On_ground = true;
        }
        else if (collision.collider.tag == "Spikey")
        {
            StartCoroutine(Respawn());
        }
    }
    IEnumerator Respawn()
    {
        Debug.Log("Die");
        Moving = false;
        Player.SetActive(false);
        yield return new WaitForSeconds(0.75f);
        transform.position = Spawn.transform.position;
        Player.SetActive(true);
        Moving = true;
    }
}
