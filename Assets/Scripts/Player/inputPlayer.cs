using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class inputPlayer : Photon.MonoBehaviour {
    [HideInInspector] public float horizontalAxis { get; private set; }
    [HideInInspector] public float verticalAxis { get; private set; }

    [HideInInspector] public bool jump { get; private set; }
    [HideInInspector] public bool attack { get; private set; }
    [HideInInspector] public bool skill1 { get; private set; }
    [HideInInspector] public bool skill2 { get; private set; }
    [HideInInspector] public bool damage { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //Define Android Inputs
        if (photonView.isMine)
        {
            if(Application.platform == RuntimePlatform.Android)
            {
                horizontalAxis = CrossPlatformInputManager.GetAxis("Horizontal");
                verticalAxis = CrossPlatformInputManager.GetAxis("Vertical");
                jump = CrossPlatformInputManager.GetButtonDown("Jump");
                attack = CrossPlatformInputManager.GetButtonDown("Attack");
            }
            else
            {
                //Define Inputs from Unity Keyboard
                attack = Input.GetButtonDown("Attack");
                skill1 = Input.GetButtonDown("Skill1");
                skill2 = Input.GetButtonDown("Skill2");
                jump = Input.GetButtonDown("Jump");
                horizontalAxis = Input.GetAxis("Horizontal");
                verticalAxis = Input.GetAxis("Vertical");
                damage = Input.GetButtonDown("Damage");
            }
        }
    }
}
