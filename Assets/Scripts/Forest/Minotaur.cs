using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minotaur : MonoBehaviour
{

    public Rigidbody2D myRB2D;
    public Animator myAnimator;
    public SpriteRenderer mySR;

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    public float movement = 2f;
    private Vector3 m_Velocity = Vector3.zero;

    //Cooldown Attack
    private float flipRate = 0.5f;
    private float nextFlipTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
 

    private void Update()
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(movement, myRB2D.velocity.y);
        // And then smoothing it out and applying it to the character
        myRB2D.velocity = Vector3.SmoothDamp(myRB2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        myAnimator.SetFloat("Speed", Mathf.Abs(myRB2D.velocity.x));

        if (this.transform.position.x <= 3.25f)
        {
            if (Time.time > nextFlipTime)
            {
                //Cooldown Attack
                nextFlipTime = Time.time + flipRate;
                Flip();
            }
            movement = 2;
            
        }

        if(this.transform.position.x >= 9.50f)
        {
            if(Time.time > nextFlipTime)
            {
                //Cooldown Attack
                nextFlipTime = Time.time + flipRate;
                Flip();
            }
            movement = -2;
            
        }
    }


   
    private void Flip()
    {
            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
       

    }

}
