using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Photon.MonoBehaviour, IPunObservable
{
    //Unity Variables
    public CharacterController2D myController;
    public inputPlayer myInputs;
    public Animator myAnimator;
    public Rigidbody2D myRB2D;
    public HealthBar myHealthBar;
    public HealthBar otherHealthBar;
    public Canvas myCanvas; 
    public SpriteRenderer mySR;
    public GameManager GM;
    public Button buttonLeft;
    public Button buttonRight;
    public Button buttonJump;
    public Button buttonAttack;
    public static bool leave = false;
    public PhotonView gameView;
    [SerializeField] private GameObject WinMenu;
    [SerializeField] private GameObject DeathMenu;
    [SerializeField] private GameObject Aura;

    //Code Variables
    private float horizontalMove = 0f;
    private float verticalMove = 0f;
    private bool faceRight; //For determining which way the player is currently facing.
    private bool otherFaceRight; // For determining which way the other player is currently facing.
    private float runSpeed = 40f;
    private bool jump;
    private float thrust=700; 
    private bool m_FacingRight = true;
    public bool alive = true;
    public bool otherAlive = true; // For determining if the otyer player is alive;
    private int maxHealth = 100;
    public int currentHealth;
    public int newGold;
    public int newTrophys;
    public string aura;

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
            GM = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameView = this.GetComponent<PhotonView>();
            this.gameObject.tag = "myPlayer";
            aura = LobbyManager.aura;

            if (aura != "None")
            {
                GameObject.Find(LobbyManager.aura).GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        else
        {
            GM = GameObject.Find("GameManager").GetComponent<GameManager>();
            this.gameObject.tag = "otherPlayer";
            this.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
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
            Debug.Log(aura);
            myCanvas.gameObject.SetActive(false);
            this.gameObject.layer=11; //Set Other player in layer Enemies
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        if (photonView.isMine) // My Player
        {

            GM = GameObject.Find("GameManager").GetComponent<GameManager>();

           

            WinMenu = GameObject.FindGameObjectWithTag("CanvasVictory");
            DeathMenu = GameObject.FindGameObjectWithTag("CanvasLose");

            
            //Check GameRoom
            if (!PhotonNetwork.room.IsOpen && alive && leave == false)
            {
                WinMenu.GetComponent<Canvas>().enabled = true;
                GameObject.FindGameObjectWithTag("victoryGold").GetComponent<TMP_Text>().text = "+" + newGold.ToString();
                GameObject.FindGameObjectWithTag("victoryTrophys").GetComponent<TMP_Text>().text = "+" + newTrophys.ToString();
            }

            if (!PhotonNetwork.room.IsOpen && !alive)
            {
                DeathMenu.GetComponent<Canvas>().enabled = true;
                GameObject.FindGameObjectWithTag("defeatGold").GetComponent<TMP_Text>().text = "+" + newGold.ToString();
                if (newTrophys >= 0)
                {
                    GameObject.FindGameObjectWithTag("defeatTrophys").GetComponent<TMP_Text>().text = "+" + newTrophys.ToString();
                }
                else
                {
                    GameObject.FindGameObjectWithTag("defeatTrophys").GetComponent<TMP_Text>().text = newTrophys.ToString();
                }
                
            }


            if (myController.m_Grounded)
            {
                myAnimator.SetBool("Falling", false);
            }

            //SHOULD BE IN START BUT NOT WORKING WITH DontDestroyOnLoad
            Camera.main.GetComponent<Camera2DFollow>().setTarget(this.gameObject);
            Camera.main.GetComponent<CameraFollow>().setTarget(this.gameObject);

            //Move our character
            if(buttonLeft.interactable && buttonRight.interactable)
            {
                horizontalMove = myInputs.horizontalAxis;
                verticalMove = myInputs.verticalAxis;
            }
            if (horizontalMove > 0 && !m_FacingRight)
            {
                faceRight = true;
                Flip();
            }
            else if (horizontalMove < 0 && m_FacingRight)
            {
                faceRight = false;
                Flip();
            }

            //Jump
            if (myInputs.jump && buttonJump.interactable)
            {
                Jump();
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
                    takeFallDamage(30);
                    if (LobbyManager.arena == 1)
                    {
                        this.transform.position = new Vector3(Random.Range(0.0f, 6.0f), 4f, 0f);
                    }
                    if (LobbyManager.arena == 2)
                    {
                        this.transform.position = new Vector3(Random.Range(-6.0f, 2.0f), 0f, 0f);
                    }

                }
            }
           
            //Die
            if (currentHealth <= 0 && alive)
            {
                Die();
            }

            //Win
            if(PhotonNetwork.room.PlayerCount == 2)
            {
                if (GameObject.FindGameObjectWithTag("otherPlayer").GetComponent<PlayerController>().alive == false && otherAlive)
                {
                    otherAlive = false;
                    Win();
                }
            }

            if (!alive || !otherAlive)
            {
                disableButtons();
            }

        }
        else //The Other Player
        {


            Debug.Log(aura);
            if (aura != "None")
            {
                GameObject.FindGameObjectWithTag("otherPlayer").transform.Find(aura).GetComponent<SpriteRenderer>().enabled = true;
            }

            //Flip Other Player
            if (!otherFaceRight)
            {
                mySR.flipX = true;
            }
            else
            {
                mySR.flipX = false;
            }

            if (currentHealth <= 0)
            {
                
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (photonView.isMine && buttonLeft.interactable && buttonRight.interactable)
        {
            myController.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
            jump = false;
        }

    }

    [PunRPC]
    public void takeSwordDamage(int damage, Vector2 moveDirection)
    {
        if (photonView.isMine)
        {
            disableButtons();
            currentHealth -= damage;
            myHealthBar.setHealth(currentHealth);

            FindObjectOfType<AudioManager>().Play("Hit");
            myAnimator.SetTrigger("Hurted");

            //Define Knocback Strength 
            switch (currentHealth <= 30 ? "High":
                    currentHealth > 30 ? "Medium" : "Default")
            {
                case "High":
                    myRB2D.AddForce(moveDirection.normalized * thrust*3);
                    myRB2D.AddForce(transform.up * (thrust - 100));
                    break;
                case "Medium":
                    myRB2D.AddForce(moveDirection.normalized * thrust*2);
                    myRB2D.AddForce(transform.up * (thrust - 100));
                    break;
                case "Default":
                    myRB2D.AddForce(moveDirection.normalized * thrust);
                    myRB2D.AddForce(transform.up * (thrust - 100));
                    break;
            } 
        }
    }

    public void takeJumpDamage()
    {
        if (photonView.isMine)
        {
            //disableButtons();
            currentHealth -= 5;
            myHealthBar.setHealth(currentHealth);

            FindObjectOfType<AudioManager>().Play("Hit");
            myAnimator.SetTrigger("Hurted");

            myRB2D.AddForce(transform.up * (thrust));
        }
    }

    public void takeFallDamage(int damage)
    {
        if (photonView.isMine)
        {
            currentHealth -= damage;
            myHealthBar.setHealth(currentHealth);
        }
    }

    public void hurtEnd()
    {
        buttonJump.interactable = true;
        buttonAttack.interactable = true;
        buttonLeft.interactable = true;
        buttonRight.interactable = true;
    }

   

    public void OnLanding()
    {
        myAnimator.SetBool("isJumping", false);
        myAnimator.SetBool("Falling", false);
        buttonJump.interactable = true;
        buttonAttack.interactable = true;
        buttonLeft.interactable = true;
        buttonRight.interactable = true;
    }


    private void Flip()
    {
        if (photonView.isMine)
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
       
    }

    public void Jump()
    {
        FindObjectOfType<AudioManager>().Play("Jump");
        myAnimator.SetBool("isJumping", true);
        jump = true;
        buttonJump.interactable = false;
        buttonAttack.interactable = false;
    }

    private void Die()
    {
        //GameManager.Instance.LeaveRoom();
        myAnimator.SetTrigger("Dying");
        disableButtons();
        alive = false;
        GM.Death();

    }
    
    public void Win()
    {
        //GameManager.Instance.LeaveRoom();
        disableButtons();
        GM.Win();
    }

    public void disableButtons()
    {
        buttonJump.interactable = false;
        buttonAttack.interactable = false;
        buttonLeft.interactable = false;
        buttonRight.interactable = false;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(faceRight);
            stream.SendNext(currentHealth);
            stream.SendNext(alive);
            stream.SendNext(aura);
        }
        else
        {
            // Network player, receive data
            this.otherFaceRight = (bool)stream.ReceiveNext();
            this.currentHealth = (int)stream.ReceiveNext();
            this.alive = (bool)stream.ReceiveNext();
            this.aura = (string)stream.ReceiveNext();
        }
    }



}
