using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    private float healthCoolDownMax = 5; //No longer really used
    private float healthCoolDown = 0;

    [Range(0, 100)]
    public float dashStrength = 10;
    private bool wantsToDash = false;

    [Range(0, 100)]
    public float sprintStrengthSaved = 2.5f;
    private float sprintStrength = 1.0f;
    private bool isSprinting = false;

    private bool isGrounded = false;

    public Animator fox;
    public RawImage[] foxHealth;

    [Range(0, 3)]
    public float flashTime = 0.2f;
    [Range(0, 3)]
    public float flashDuration = 1.0f;

    public ParticleSystem dust;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
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

            transform.GetChild(1).transform.LookAt(transform.position + new Vector3(0,0,1));

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
            transform.GetChild(1).transform.LookAt(transform.position + new Vector3(0,0,-1));
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

        

        if((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded == true)
        {
            gameObject.GetComponent<Rigidbody>().velocity += (Vector3.up * jumpMod);
            isGrounded = false;
            fox.SetBool("jumping", isGrounded);

        }
        if (isGrounded == true)
        {
            createDust(1);
        }
        else
        {
            createDust(0);
        }
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


    //Player flashed when damaged
    public void activateFlash()
    {
        StartCoroutine(flash(flashDuration, flashTime));
    }

    IEnumerator flash(float timeMax, float timeInterval)
    {
        float totTime = 0;
        do
        {
            Renderer foxRender = GetComponentsInChildren<Renderer>()[1];

            foxRender.enabled = false;

            yield return new WaitForSeconds(timeInterval);
            totTime += Time.deltaTime;


            foxRender.enabled = true;

            yield return new WaitForSeconds(timeInterval);
            totTime += Time.deltaTime;


        } while (totTime < timeMax);
        healthCoolDown = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Death"))
        {
            if (healthCoolDown <= 0)
            {
                health--;
                activateFlash();
                if (health == 0)
                {
                    transform.position = Vector3.zero;
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                    health = 3;
                    foxHealth[1].transform.position -= Vector3.up * 100;
                    foxHealth[2].transform.position -= Vector3.up * 100;
                    
                }
                else
                {
                    foxHealth[health].transform.position += Vector3.up * 100;
                }
                healthCoolDown = healthCoolDownMax;
            }
        }
        if(collision.gameObject.transform.position.y + collision.gameObject.GetComponent<Collider>().bounds.size.y / 2 < transform.position.y) //Make sure the collision is the underneath ish of the fox
        {
            isGrounded = true;
            fox.SetBool("jumping", isGrounded);
        }
    }

    public void createDust(int num)
    {
        switch (num)
        {
            case 1:
                dust.Play();
                break;
            case 0:
                dust.Stop();
                break;
        }
    }

}
