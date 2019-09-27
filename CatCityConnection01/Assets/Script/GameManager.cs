using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;


namespace com.ryu.catcityconnection
{

    public class GameManager : MonoBehaviourPunCallbacks
    {

        #region Photon Callbacks

        // Roomから出たらシーン0（launcher)をロードする。
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); //あなたが接続しているプレーヤーの場合は表示されません

            //Photonのマスタークライアントの場合のみLoadArenaを呼び出す
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); //OnPlayerLeftRoomの前に呼び出されます

                LoadArena();
            }

        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); //他の切断時に見られる

            //Photonのマスタークライアントの場合のみLoadArenaを呼び出す
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); //OnPlayerLeftRoomの前に呼び出されます

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
                Debug.LogError("PhotonNetwork : レベルをロードしようとしましたが、私たちはマスタークライアントではありません");
            }

            Debug.LogFormat("PhotonNetwork : 読み込みレベル：{0} ",PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);

        }

        #endregion


    }

}

