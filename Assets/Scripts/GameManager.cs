using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace io.github.onikenx
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        public static GameManager Instance;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        #endregion

        private void Start()
        {
            Instance = this;
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManager.GetActiveScene().name);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0{", SceneManagerHelper.ActiveSceneName);
                }
            }
        }


        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the laucher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            base.OnPlayerEnteredRoom(other);
            Debug.Log("OnPlayerEnteredRoom() " + other.NickName); //not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("OnPlayerEnteredRoom IsMasterClient " + PhotonNetwork.IsMasterClient); //called before OnPlayerLeftRoom
                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            //not seen if you're the player connecting
            Debug.Log("OnPlayerEnteredRoom() " + other.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                //called before OnPlayerLeftRoom
                Debug.Log("OnPlayerEnteredRoom IsMasterClient " + PhotonNetwork.IsMasterClient);
                LoadArena();
            }
        }

        #endregion


        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Private Methods

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork: Trying to load a level but we are not the master Client");
            }

            Debug.LogFormat("PhotonNetwork: Loading Level: {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        #endregion
    }
}