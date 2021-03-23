using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjScript : MonoBehaviour
{
    public GameObject Parent;
    private float Jump_force;
    private bool On_ground;

    // Start is called before the first frame update
    void Start()
    {
        On_ground = true;
    }

    // Update is called once per frame
    void Update()
    {
        
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

        }
    }
    IEnumerator Respawn()
    {
        Moving = false;
        Player.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        transform.position = Spawn.transform.position;
        Player.SetActive(true);
        Moving = true;
    }
}
