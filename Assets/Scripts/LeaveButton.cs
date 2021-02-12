using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveButton : Photon.PunBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(PhotonNetwork.room.PlayerCount == 2)
        {
            this.gameObject.SetActive(false);
        }

    }

    public void leaveRoom()
    {
        PlayerController.leave = true;
        FindObjectOfType<AudioManager>().Play("Menus");
        GameObject.Find("GameManager").GetComponent<GameManager>().LeaveRoom();
        Destroy(this);
    }
}
