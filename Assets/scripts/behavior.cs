using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class behavior : MonoBehaviour
{

    [Range(0, 1)]
    public int instantOrSmooth = 0;

    private float movementSpeed = 0;
    [Range(0,100)]
    public float movementSpeedMax = 1;
    [Range(0, 100)]
    public float movementSpeedSpeedUp = 0.6f;
    [Range(0, 100)]
    public float jumpMod = 1.5f;

    private int health = 3;
    [Range(0,100)]
    public float healthCoolDownMax = 5;
    private float healthCoolDown = 0;

    [Range(0, 100)]
    public float dashStrength = 10;
    private bool wantsToDash = false;

    [Range(0, 100)]
    public float sprintStrengthSaved = 2.5f;
    private float sprintStrength = 1.0f;
    private bool isSprinting = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            wantsToDash = true;
        }

        if (Input.GetKeyDown(KeyCode.X)){
            isSprinting = !isSprinting;
            if (isSprinting == true)
            {
                sprintStrength = sprintStrengthSaved;
            }
            else
            {
                sprintStrength = 1.0f;
            }
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            switch (wantsToDash)
            {
                case true:
                    changeSpeed(1, false, 1);
                    wantsToDash = false;
                    break;
                case false:
                    changeSpeed(1);
                    wantsToDash = false;
                    break;
            }

        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            
            switch (wantsToDash)
            {
                case true:
                    changeSpeed(-1, false, 1);
                    wantsToDash = false;
                    break;
                case false:
                    changeSpeed(-1);
                    wantsToDash = false;
                    break;
            }
        }
        else
        {
            switch (instantOrSmooth)
            {
                case 0:
                    changeSpeed(1, true);
                    break;
                case 1:
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(gameObject.GetComponent<Rigidbody>().velocity.x / 1.05f, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                    if (gameObject.GetComponent<Rigidbody>().velocity.x >= -0.2 && gameObject.GetComponent<Rigidbody>().velocity.x <= 0.2)
                    {
                        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                    }

                    break;
            }
        }

        

        if((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && gameObject.GetComponent<Rigidbody>().velocity.y == 0)
        {
            gameObject.GetComponent<Rigidbody>().velocity += (Vector3.up * jumpMod);
        }

        
        healthCoolDown -= Time.deltaTime;
    }



    public void changeSpeed(int direction, bool fullStop = false, float extraSpeed = 0)
    {
        switch (instantOrSmooth)
        {
            case 0:
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(movementSpeedMax * direction * sprintStrength, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                if (fullStop == true)
                {
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                }
                if (extraSpeed == 1)
                {
                    //gameObject.GetComponent<Rigidbody>().velocity = new Vector3(dashStrength * direction, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                    transform.position += Vector3.right * dashStrength * direction;
                }
                break;
            case 1:
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(gameObject.GetComponent<Rigidbody>().velocity.x + movementSpeedSpeedUp * direction * sprintStrength, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                if (Mathf.Abs(gameObject.GetComponent<Rigidbody>().velocity.x) > Mathf.Abs(movementSpeedMax * sprintStrength))
                {
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(movementSpeedMax * direction, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                }
                if (extraSpeed == 1)
                {
                    //gameObject.GetComponent<Rigidbody>().velocity = new Vector3(dashStrength * direction, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                    transform.position += Vector3.right * dashStrength * direction;
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
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                    health = 3;
                }
                healthCoolDown = healthCoolDownMax;
            }
        }
    }
}
