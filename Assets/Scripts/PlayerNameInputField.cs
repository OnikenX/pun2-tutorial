using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


using Photon.Pun;
using Photon.Realtime;


/// Player name input field. Let the user input his name, will appear above the player in the game.
[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{

    #region Peivate Constants 

    ///
    /// Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";
    
    #endregion

    #region MonoBehavior Callbacks
    /// <summary>
    /// MonoBehaviour melhod called on GameObject by Unity during initialization phase
    /// </summary>
    void Start()
    {
                string defaultName = "Player";
                InputField _inputField = this.GetComponent<InputField>();
                if(_inputField!=null)
                {
                    if(PlayerPrefs.HasKey(playerNamePrefKey))
                    {
                        defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                        _inputField.text = defaultName;
                    }
                }

                PhotonNetwork.NickName = defaultName;
    }
    #endregion

    #region Public Methods
    public void SetPlayerName(string value){
        if(string.IsNullOrEmpty(value)){
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }

    #endregion

}
