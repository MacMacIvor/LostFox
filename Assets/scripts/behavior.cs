using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class behavior : MonoBehaviour
{

    [Range(0, 1)]
    public int instantOrSmooth = 0;

    private float movementSpeed = 0;
    [Range(0,100)]
    public float movementSpeedMax = 3;
    [Range(0, 100)]
    public float movementSpeedSpeedUp = 0.6f;
    [Range(0, 100)]
    public float jumpMod = 1.5f;

    private int health = 3;
    [Range(0,100)]
    public float healthCoolDownMax = 5;
    private float healthCoolDown = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            changeSpeed(-1);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            changeSpeed(1);
        }
        else
        {
            switch (instantOrSmooth)
            {
                case 0:
                    changeSpeed(1, true);
                    break;
                case 1:
                    movementSpeed /= 2;
                    if (movementSpeed >= -0.2 && movementSpeed <= 0.2)
                    {
                        movementSpeed = 0;
                    }
                    
                    break;
            }
        }

        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            gameObject.GetComponent<Rigidbody>().velocity += (Vector3.up * jumpMod);
        }

        transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        healthCoolDown -= Time.deltaTime;
    }



    public void changeSpeed(int direction, bool fullStop = false, float extraSpeed = 0)
    {
        switch (instantOrSmooth)
        {
            case 0:
                movementSpeed = movementSpeedMax * direction;
                if (fullStop == true)
                {
                    movementSpeed = 0;
                }
                break;
            case 1:
                movementSpeed += movementSpeedSpeedUp * direction;
                if (Mathf.Abs(movementSpeed) > Mathf.Abs(movementSpeedMax))
                {
                    movementSpeed = movementSpeedMax * direction;
                }
                break;
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Death"))
        {
            if (healthCoolDown <= 0)
            {
                health--;
                if (health == 0)
                {
                    transform.position = Vector3.zero;
                    movementSpeed = 0;
                    health = 3;
                }
                healthCoolDown = healthCoolDownMax;
            }
        }
    }
}
