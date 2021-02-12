using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;
using System;

public class GameManager : Photon.PunBehaviour
{

    static public GameManager Instance;

    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;
    [SerializeField] private Canvas DeathMenu;
    [SerializeField] private TMP_Text DeathGold;
    [SerializeField] private TMP_Text DeathTrophys;
    [SerializeField] private Canvas WinMenu;
    [SerializeField] private TMP_Text WinGold;
    [SerializeField] private TMP_Text WinTrophys;
    public PlayerController playerController;
    public int playerCount;


    //Firebase
    DatabaseReference myDBRef;

    private string userId;
    private int userGold = 0;
    private int userTrophys = 0;


    private void Start()
    {
        //Firebase
        myDBRef = FirebaseDatabase.DefaultInstance.RootReference;

        //Get user Data 
        FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWith(task =>
        {
            DataSnapshot snapshot = task.Result;
            this.userGold = Int32.Parse(snapshot.Child(userId).Child("gold").Value.ToString());
            this.userTrophys = Int32.Parse(snapshot.Child(userId).Child("trophys").Value.ToString());
        });

        //If Photon is not connected, return to launcher
        if (!PhotonNetwork.connected) // 1
        {
            SceneManager.LoadScene("Launcher");
            return;
        }

        //Initalize UserId from Photon
        userId = PhotonNetwork.AuthValues.UserId;

        Instance = this;
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (PlayerController.LocalPlayerInstance == null && PhotonNetwork.room.PlayerCount==1)
            {
                Debug.Log("We are Instantiating LocalPlayer from " + Application.loadedLevelName+ "THE ID IS "+ Quaternion.identity);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                switch (LobbyManager.arena)
                {
                    case 1:
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(UnityEngine.Random.Range(0.0f, 6.0f), 4f, 0f), Quaternion.identity, 0);
                        playerCount++;
                        break;
                    case 2:
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(UnityEngine.Random.Range(-6.0f, 2.0f), 0f, 0f), Quaternion.identity, 0);
                        playerCount++;
                        break;
                }

                

            }
            if (PlayerController.LocalPlayerInstance == null && PhotonNetwork.room.PlayerCount == 2)
            {
                Debug.Log("We are Instantiating LocalPlayer from " + Application.loadedLevelName + "THE ID IS " + Quaternion.identity);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                switch (LobbyManager.arena)
                {
                    case 1:
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(UnityEngine.Random.Range(12.0f, 19.0f), 4f, 0f), Quaternion.identity, 0);
                        playerCount++;
                        break;
                    case 2:
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(UnityEngine.Random.Range(14.0f, 20.0f), 0f, 0f), Quaternion.identity, 0);
                        playerCount++;
                        break;
                }
            }
        }
    }

    private void Update()
    {
        if (playerController == null)
        {
            //Initalize
            playerController = GameObject.FindGameObjectWithTag("myPlayer").GetComponent<PlayerController>();
        }
        
    }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
            {
                SceneManager.LoadScene("Lobby");
            }

        public void LeaveRoom()
        {
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.LeaveRoom();
        }

        void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            //Start the game selectin arena for max num players
            //Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
            //PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.room.playerCount);
            if (LobbyManager.arena == 1)
            {
                PhotonNetwork.LoadLevel("Forest");
            }
            if (LobbyManager.arena == 2)
            {
                PhotonNetwork.LoadLevel("Cathedral");
            }
            
            
    }

        //Photon Messages
        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                LoadArena();
            }
        }


        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                LoadArena();
            }
        }

    public void Death()
    {
        PhotonNetwork.room.IsOpen = false;
        DeathMenu.GetComponent<Canvas>().enabled = true;
        FindObjectOfType<AudioManager>().Stop("Theme");
        FindObjectOfType<AudioManager>().Play("Lose");

        //Set New Gold
        int newGold = UnityEngine.Random.Range(5, 15);
        int newTrophys = UnityEngine.Random.Range(-6, 0);
        int gold2DB = this.userGold + newGold;
        int trophys2DB = this.userTrophys + newTrophys;
        playerController.newGold = newGold;
        playerController.newTrophys = newTrophys;
        //Update Canvas
        DeathGold.text = "+"+ newGold.ToString();
        DeathTrophys.text = newTrophys.ToString();

        //Control for negative Trophys
        if (trophys2DB <= 0)
        {
            trophys2DB = 0;
        }
        //Update user data to DB
        myDBRef.Child("users").Child(userId).Child("gold").SetValueAsync(gold2DB);
        myDBRef.Child("users").Child(userId).Child("trophys").SetValueAsync(trophys2DB);
    }

    public void Win()
    {
        PhotonNetwork.room.IsOpen = false;
        WinMenu.GetComponent<Canvas>().enabled = true;
        FindObjectOfType<AudioManager>().Stop("Theme");
        FindObjectOfType<AudioManager>().Play("Win");

        //Set New Gold
        int newGold = UnityEngine.Random.Range(8, 25);
        int newTrophys = UnityEngine.Random.Range(0, 10);
        playerController.newGold = newGold;
        playerController.newTrophys = newTrophys;
        int gold2DB = this.userGold + newGold;
        int trophys2DB = this.userTrophys + newTrophys;
        //Update Canvas
        WinGold.text = "+" + newGold.ToString();
        WinTrophys.text = "+" + newTrophys.ToString();

        
        //Update user data to DB
        myDBRef.Child("users").Child(userId).Child("gold").SetValueAsync(gold2DB);
        myDBRef.Child("users").Child(userId).Child("trophys").SetValueAsync(trophys2DB);
    }

}

