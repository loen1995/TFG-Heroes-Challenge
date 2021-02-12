using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Photon.MonoBehaviour, IPunObservable
{

    public CharacterController2D myController;
    public inputPlayer myInputs;
    public Animator myAnimator;
    public Rigidbody2D myRB2D;
    public HealthBar myHealthBar;
    public HealthBar otherHealthBar;
    public Canvas myCanvas; 
    public SpriteRenderer mySR;
 

    public float horizontalMove = 0f;
    public float verticalMove = 0f;
    public bool faceRight; //For determining which way the player is currently facing.
    public bool otherFaceRight; // For determining which way the other player is currently facing.
    public float runSpeed = 40f;
    public bool jump;
    public Button buttonLeft;
    public Button buttonRight;
    public Button buttonJump;
    public Button buttonAttack;

    //Test Damage
    private int maxHealth = 100;
    public int currentHealth;
    public int otherCurrentHealth;

    //Test multiplayer
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;


    // Start is called before the first frame update
    void Start()
    {
        if (photonView.isMine)
        { 
            //Health
            currentHealth = maxHealth;
            myHealthBar.setHealth(currentHealth);
            Camera.main.GetComponent<Camera2DFollow>().setTarget(this.gameObject);
            Camera.main.GetComponent<CameraFollow>().setTarget(this.gameObject);
        }
    }

    private void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.isMine)
        {
            PlayerController.LocalPlayerInstance = this.gameObject;
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            myCanvas.enabled = true;
        }
        else
        {
            myCanvas.gameObject.SetActive(false);
            this.gameObject.layer=11; //Set Other player in layer Enemies
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {

            //SHOULD BE IN START BUT NOT WORKING WITH DontDestroyOnLoad
            Camera.main.GetComponent<Camera2DFollow>().setTarget(this.gameObject);
            Camera.main.GetComponent<CameraFollow>().setTarget(this.gameObject);

            //Move our character
            horizontalMove = myInputs.horizontalAxis;
            verticalMove = myInputs.verticalAxis;

            if (horizontalMove > 0)
            {
                faceRight = true;
            }
            else if (horizontalMove < 0)
            {
                faceRight = false;
            }

            //Flip
            if (!faceRight)
            {
                mySR.flipX = true;
            }
            else
            {
                mySR.flipX = false;
            }

            //Jump
            if (myInputs.jump && buttonJump.interactable)
            {
                FindObjectOfType<AudioManager>().Play("Jump");
                myAnimator.SetBool("isJumping", true);
                jump = true;
                buttonJump.interactable = false;
                buttonAttack.interactable = false;
            }

            //Apply speed
            if (buttonLeft.interactable && buttonRight.interactable)
            {
                horizontalMove *= runSpeed;
            }


            //Animations
            myAnimator.SetFloat("Speed", Mathf.Abs(horizontalMove));

            //Fall
            if (myRB2D.velocity.y < 0 && myController.m_Grounded == false)
            {
                myAnimator.SetBool("Falling", true);
                buttonAttack.interactable = false;
                buttonJump.interactable = false;
                if (this.transform.position.y <= -7)
                {
                    takeDamage(30);
                    this.transform.position = new Vector3(Random.Range(0.0f,6.0f),4f, 0f);
                }
            }
           


            //Health Test
            if (myInputs.damage)
            {
                takeDamage(20);
            }
            if (currentHealth <= 0)
            {
                GameManager.Instance.LeaveRoom();
            }


        }
        else
        {
            //Flip
            if (!otherFaceRight)
            {
                mySR.flipX = true;
            }
            else
            {
                mySR.flipX = false;
            }
        }
    }
    
    public void OnLanding()
    { 
            myAnimator.SetBool("isJumping", false);
            myAnimator.SetBool("Falling", false);
            buttonJump.interactable = true;
            buttonAttack.interactable = true;
    }
    
    private void FixedUpdate()
    {
        if (photonView.isMine)
        {
            myController.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
            jump = false;
        }

    }
    [PunRPC]
    public void takeDamage(int damage)
    {
            //Debug.Log("ESTA ES LA ID QUE RECIBO" + idPlayer + " | ESTA ES MI ID " + photonView.instantiationId);
            currentHealth -= damage;
            myHealthBar.setHealth(currentHealth);
            
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(faceRight);
            //stream.SendNext(currentHealth);
        }
        else
        {
            // Network player, receive data
            this.otherFaceRight = (bool)stream.ReceiveNext();
            /*
            this.otherCurrentHealth = (int)stream.ReceiveNext();
            otherHealthBar.setHealth(otherCurrentHealth);
            */
        }
    }
}
