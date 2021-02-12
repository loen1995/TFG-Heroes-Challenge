using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Unity.Editor;

public class Launcher : Photon.PunBehaviour
{

    [SerializeField] private GameObject canvasMenu;
    [SerializeField] private GameObject canvasConnecting;

    private string _gameVersion = "1";
    /// <summary>
    /// The PUN loglevel.
    /// </summary>
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players and so new room will be created.
    /// </summary>
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players and so new room will be created")]
    public byte MaxPlayersPerRoom = 2;

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

    private void Awake()
    {
        // #Critical
        // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;


        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // #NotImportant
        // Force LogLevel
        PhotonNetwork.logLevel = Loglevel;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Disable because now we call this function from button Play 
        //Connect();


    }

    private void Update()
    {
   
    }

    public void Connect()
    {
        Debug.Log("CONNECTING");
        canvasMenu.SetActive(false);
        canvasConnecting.SetActive(true);
        // we check if we are connected or not, we join if we are, else we initiate the connection to the server.
        if (PhotonNetwork.connected)
        {
            
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
            JoinRandom(LobbyManager.arena);
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            isConnecting = PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    //Photon CallBacks
   
    public override void OnConnectedToMaster()
    {
        if (isConnecting) 
        { 
            Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
            JoinRandom(LobbyManager.arena);
            isConnecting = false;
        }
    }
    
    public override void OnDisconnectedFromPhoton()
    {
        SceneManager.LoadScene("Lobby");
        isConnecting = false;
        Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        if(LobbyManager.arena == 1)
        {
            CreateRoom(PhotonNetwork.playerName, 2, LobbyManager.arena);
        }
        if (LobbyManager.arena == 2)
        {

            CreateRoom(PhotonNetwork.playerName, 2, LobbyManager.arena);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.automaticallySyncScene to sync our instance scene.
        if (PhotonNetwork.isMasterClient)
        {
            if (LobbyManager.arena == 1)
            {
                PhotonNetwork.LoadLevel("Forest");
            }
            if (LobbyManager.arena == 2)
            {
                PhotonNetwork.LoadLevel("Cathedral");
            }
        }
    }


    public void CreateRoom(string name, byte maxPlayers, object arena)
    {
        ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable() { { "Arena", arena }};
        string[] roomPropsInLobby = { "Arena"};
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = roomProps;
        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
        roomOptions.MaxPlayers = maxPlayers;
        PhotonNetwork.CreateRoom(name, roomOptions, TypedLobby.Default);
    }

    public void JoinRandom(object arena)
    {
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
        { "Arena",arena }
        };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties,2);
    }


}
