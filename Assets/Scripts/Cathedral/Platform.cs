using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{

    public Rigidbody2D myRB2D;

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    private float movement = 3;
    private Vector3 m_Velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(0, movement);
        // And then smoothing it out and applying it to the character
        myRB2D.velocity = Vector3.SmoothDamp(myRB2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
      

        if (this.transform.position.y <= -3.5f)
        {
            movement = 3;

        }

        if (this.transform.position.y >= 3.50f)
        {
            movement = -3;

        }


    }
}
