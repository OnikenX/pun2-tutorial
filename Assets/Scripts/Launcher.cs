using System;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    [Tooltip(
        "The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;


    [Tooltip("The UI Panel to let the user enter name, connect and play")] [SerializeField]
    private GameObject controlPanel;

    [Tooltip("The UI Label to inform the user that the connection is in progress")] [SerializeField]
    private GameObject progressLabel;

    private Text progressLabel_text = null;

    #region Private Serializable Fields

    #endregion

    #region Private Fields

    /// <summary>
    /// This client's version number.
    /// Users are separated from each other by
    /// gameVersion (which allows you to make breaking changes).
    ///</summary>
    string gameVersion = "1";

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

    #endregion

    #region MonoBehavior CallBacks

    /// Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Started...");
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        progressLabel_text = progressLabel.GetComponent<Text>();
    }


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity
    /// during early initialization phase.
    /// </summary>
    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the
        // master client and all clients in the same room sync their
        // level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Pun Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        progressLabel_text.text = "Connected to master";
        // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()

        // we don't want to do anything if we are not attempting to join a room.
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        if (isConnecting)
        {
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(
            "PUN Basics Tutorial/Launcher: OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        progressLabel_text.text = "Join failed, creating room...";
        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics tutorial/launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        // #critical: We only load if we are the first player, else we rely on PhotonNetwork.AutomaticallySyncScene to sync ourinstance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1'");

            // #Critical
            //Load the Room Level
            PhotonNetwork.LoadLevel("Room for 1");
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Pun Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason " + cause);
        progressLabel?.SetActive(false);
        controlPanel?.SetActive(true);
        isConnecting = false;
    }

    #endregion


    #region Public Methods

    // start the connection process.
    // - if already connected, we attempt joining a random room 
    // - if not yet connected, Connect this application instance to
    //      Photon Cloud Network
    public void Connect()
    {
        // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
        Debug.Log("Called Connect()");
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        progressLabel_text.text = "Connecting...";

        //we check if we are connected or not. we join if we are, else we 
        //initiate the connection to the server
        if (PhotonNetwork.IsConnected)
        {
            //#Critical we need at this point to attempt joining a Random Room.
            //If it fails, we'll get notified in OnJoinRandomFailed() and we'll
            // create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical , we must first and foremost connect to Photon Online Server
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }

        progressLabel_text.text += ".";
    }

    #endregion


    #region Private Methods

    private void LogAndLabel(string message)
    {
        Debug.Log(message);
        progressLabel.GetComponent<Text>().text = message;
    }

    #endregion
}