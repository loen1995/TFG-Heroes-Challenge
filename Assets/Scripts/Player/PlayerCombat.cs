using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : Photon.MonoBehaviour
{

    public inputPlayer myInputs;
    public Animator myAnimator;
    public Transform attackPoint;
    public Transform groundPoint;
    public float groundRange = 0.5f;
    public CharacterController2D myController;
    public Rigidbody2D myRB2D;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public PlayerController playerController;
    
    

    //Cooldown Attack
    private float attackRate = 0.5f;
    private float nextAttackTime = 0f;

    //Cooldown Damage
    private float damageRate = 0.5f;
    private float nextDamageTime = 0f;

    //UI Buttons
    public Button buttonLeft;
    public Button buttonRight;
    public Button buttonJump;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            if (myInputs.attack && Time.time > nextAttackTime && myController.m_Grounded && playerController.buttonAttack.interactable )
            {

                //Cooldown Attack
                nextAttackTime = Time.time + attackRate;

                //Animation
                myAnimator.SetTrigger("Attacking");
                FindObjectOfType<AudioManager>().Play("Sword");

                //Detect enemies in range
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
                foreach(Collider2D enemy in hitEnemies)
                {
                    Vector2 moveDirection = enemy.transform.position - this.transform.position;
                    //Attack Logic Online
                    PhotonView pView = enemy.GetComponentInParent<PhotonView>();
                    //Debug.Log("We Hit "+pView.instantiationId);
                    pView.RPC("takeSwordDamage", PhotonTargets.All, 10,moveDirection);
                   
                }
                //Disable Movement while Attacking
                buttonLeft.interactable = false;
                buttonRight.interactable = false;
                buttonJump.interactable = false;
            }

            if(!myController.m_Grounded && myAnimator.GetBool("Falling") && Time.time > nextDamageTime)
            {
                //Cooldown Damage
                nextDamageTime = Time.time + damageRate;

                //Detect enemies in range
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(groundPoint.position, groundRange, enemyLayers);
                foreach(Collider2D enemy in hitEnemies)
                {
                    playerController.takeJumpDamage();
                    //Attack Logic Online
                    //PhotonView pView = enemy.GetComponentInParent<PhotonView>();
                    //Debug.Log("We Hit "+pView.instantiationId);
                    //pView.RPC("takeJumpDamage", PhotonTargets.All, 5);
                   
                }
            }

        }
    }

    //Draw on Scene in order to check the attack range
    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    //This function is called when the animation ends in Animation Unity
    public void AttackEnded()
    {
        buttonLeft.interactable = true;
        buttonRight.interactable = true;
        buttonJump.interactable = true;
    }
}
