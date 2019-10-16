using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using Photon.Pun;

namespace com.ryu.catcityconnection
{
    /// <summary>
    /// Player manager
    /// ビームの入力判定と発射を行う。
    /// </summary>

    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {

        #region Public Fields

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        #endregion


        #region IPunObservable implementation


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                //私たちはこのプレーヤーを所有しています：他の人にデータを送信します
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
            }
            else
            {
                //ネットワークプレーヤー、データ受信
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }
        }

        #endregion



        #region Private Fields

        [Tooltip("Beam GameObject to control")]
        [SerializeField]
        private GameObject beams = default;
        // true の時はユーザーがビームを出しているとき。
        bool IsFiring;

        [Tooltip("The current Health of our player")]
        public float Health = 1.0f;

        [Tooltip("プレイヤーＵＩのprefab")]
        [SerializeField]
        private GameObject playerUiPrefab = default;


        #endregion

        #region MonoBegavior Callbacks
        /// <summary>
        /// 初期化時に呼び出される
        /// </summary>
        /// 

        public void Awake()
        {
            if(beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }

            //GameManager.csで使用：localPlayerインスタンスを追跡して、レベルが同期されたときにインスタンス化を防止します
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            //インスタンスがレベル同期に耐えるように、ロード時に破棄しないようにフラグを立てます。これにより、レベルのロード時にシームレスなエクスペリエンスを提供します。
            DontDestroyOnLoad(this.gameObject);

        }


        public void Start()
        {
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }



            if (playerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(playerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

        }

        /// <summary>
        /// 毎フレームの処理
        /// </summary>
        public void Update()
        {
            if (photonView.IsMine)
            {
                ProcessInputs();

            }


            // ビームトリガーがアクティブの時
            if (beams != null && IsFiring != beams.activeSelf)
            {
                beams.SetActive(IsFiring);
            }

            if (Health <= 0)
            {
                GameManager.Instance.LeaveRoom();
            }


        }

        /// <summary>
        /// Collider 'other'がトリガーに入ったときに呼び出されるMonoBehaviourメソッド。
        /// ビームが当たったらhealthに影響を与える。
        /// ※ジャンプ中にビーム出すと自爆するぞ
        /// これを回避するには、コライダーを離すか、ビームがプレイヤーに属していると判定するといいよ。
        /// </summary>
        /// 
        void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            if (!other.name.Contains("Beam"))
            {
                return;
            }
            Health -= 0.1f;
        }

        /// <summary>
        /// MonoBehaviourメソッドは、トリガーに触れるすべてのコライダーの「その他」に対してフレームごとに1回呼び出されます。.
        /// ビームがプレーヤーに触れている間に健康に影響を与えます
        /// </summary>
        /// 
        void OnTriggerStay(Collider other)
        {
            // ローカルプレイヤーの場合は何もしない
            if (!photonView.IsMine)
            {
                return;
            }
            if (!other.name.Contains("Beam"))
            {
                return;
            }
            // ビームが当たり続けるとHealthがずっと減ってプレイヤーが死んじゃうので、移動する。
            Health -= 0.1f * Time.deltaTime;

        }

        #if !UNITY_5_4_OR_NEWER

        // <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>

        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }

        #endif

        void CalledOnLevelWasLoaded(int level)
        {
            // 私たちがアリーナの外にいるかどうかを確認し、そうであれば、安全地帯のアリーナの中心の周りにスポーンします
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

        }

        #endregion

        #region Custom

        // 入力処理。ユーザーがFireを押しているフラグを保持します。
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!IsFiring)
                {
                    IsFiring = true;
                }
            }
            if (Input.GetButtonUp("Fire1"))
            {
                if (IsFiring)
                {
                    IsFiring = false;
                }
            }
        }

#endregion

    }

}


