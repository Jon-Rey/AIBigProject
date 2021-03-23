using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject Player;
    public GameObject Spawn;
    public PlayerObjScript ObjScript;
    public float Speed;
    public float Jump_force;
    private bool Moving;
    // Start is called before the first frame update
    void Start()
    {
        Moving = true;
        ObjScript.Set_Jump_force(Jump_force);
    }

    // Update is called once per frame
    void Update()
    {
        if (Moving)
        {
            transform.position += new Vector3(-Speed * 0.5f, 0.0f, 0.0f);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (ObjScript.Get_On_ground())
                {
                    Debug.Log("Jump");
                    ObjScript.Jump();
                }
                else
                {
                    Debug.Log("Not able to jump");
                }
            }
        }
    }
}
