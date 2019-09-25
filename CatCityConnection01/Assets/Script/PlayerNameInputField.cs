using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.Demo.PunBasics
{

    /// <summary>
    /// プレイヤー名入力フィールド。 ユーザーが自分の名前を入力すると、ゲームのプレーヤーの上に表示されます。
    /// </summary>

    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {

        #region Private Constants

        // 入力ミスを防ぐためにPlayerPrefキーを保存します
        const string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehavior CallBacks

        /// <summary>
        /// 初期化フェーズ中にUnityによってGameObjectで呼び出されるMonoBehaviourメソッド。
        /// </summary>

        // Start is called before the first frame update
        void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;

        }

        #endregion


        #region Public Methods

        /// プレーヤーの名前を設定し、将来のセッションのためにPlayerPrefsに保存します。
        /// <param name="value">The name of the Player</param>
        /// 

        public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("プレイヤーネームがnullか空白です");
                return;
            }
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion

    }



}


