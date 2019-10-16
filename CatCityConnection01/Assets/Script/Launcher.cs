using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


namespace com.ryu.catcityconnection
{


    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        /// Tooltipで↓のように書くと、マウス載せたら説明がでるようになるみたい。
        [Tooltip("ユーザーが名前を入力し、接続して再生できるUiパネル")]
        [SerializeField]
        private GameObject controlPanel = default;

        [Tooltip("接続が進行中であることをユーザーに通知するUIラベル")]
        [SerializeField]
        private GameObject progressLabel = default;

        /// 部屋ごとのプレーヤーの最大数。 部屋がいっぱいになると、新しいプレイヤーが参加できなくなり、新しい部屋が作成されます。
        [Tooltip("部屋ごとのプレーヤーの最大数。 部屋がいっぱいになると、新しいプレイヤーが参加できなくなり、新しい部屋が作成されます")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;
        

    #endregion

    #region Private Fields

        /// <summary>
        /// このクライアントのバージョン番号。 ユーザーはgameVersionによって互いに分離されています（これにより、重大な変更を加えることができます）
        /// </summary>
        string gameVersion = "1";

        /// <summary>
        /// 現在のプロセスを追跡します。 接続は非同期であり、Photonからのいくつかのコールバックに基づいているため、
        /// Photonからのコールバックを受信したときの動作を適切に調整するには、これを追跡する必要があります。
        /// 通常、これはOnConnectedToMaster（）コールバックに使用されます。
        /// </summary>
        /// 
        bool isConnecting;


        #endregion


        #region MonoBehaviour CallBacks

        /// <summary>
        /// 初期初期化フェーズ中にUnityによってGameObjectで呼び出されるMonoBehaviourメソッド。
        /// </summary>
        void Awake()
        {
            //これにより、マスタークライアントでPhotonNetwork.LoadLevel（）を使用でき、同じ部屋のすべてのクライアントがレベルを自動的に同期できるようになります。
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// 初期化フェーズ中にUnityによってGameObjectで呼び出されるMonoBehaviourメソッド。
        /// </summary>

        void Start()
        {
            //Connect();
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

    #endregion

    #region Public Methods

        /// <summary>
        /// 接続プロセスを開始します。
        /// -既に接続されている場合、ランダムな部屋に参加しようとします
        /// -まだ接続されていない場合、このアプリケーションインスタンスをPhoton Cloud Networkに接続します
        /// </summary>

        public void Connect()
        {

            // ルームに参加する意志を追跡します。ゲームから戻ったときに、接続されているコールバックを取得するため、何をすべきかを知る必要があるからです。
            isConnecting = true;

            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            //接続されているかどうかを確認し、接続されている場合は参加し、そうでない場合はサーバーへの接続を開始します。
            if (PhotonNetwork.IsConnected)
            {
                //#Criticalこの時点で、ランダムルームへの参加を試みる必要があります。 失敗した場合、OnJoinRandomFailed（）で通知され、作成されます。
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                //＃重要、何よりもまずPhoton Online Serverに接続する必要があります。
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

            if (isConnecting)
            {
                //#Critical：最初に行うことは、既存の潜在的な部屋に参加することです。 良い場合は、OnJoinRandomFailed（）でコールバックされます
                PhotonNetwork.JoinRandomRoom();

            }

        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);

            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            //#Critical：ランダムルームへの参加に失敗しました。存在しないか、すべてが一杯です。 心配いりません、新しい部屋を作ります。
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom});
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

            // #Critical：最初のプレーヤーである場合にのみロードします。それ以外の場合は、インスタンスシーンを同期するために `PhotonNetwork.AutomaticallySyncScene`に依存します。
            if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {

                Debug.Log("'Room for 1' をロード");

                // #Critical
                // Load the Room Level.

                PhotonNetwork.LoadLevel("Room for 1");

            }




        }

        #endregion


    }

}

