using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using Photon.Pun;

namespace Photon.Pun.Demo.PunBasics
{
    /// <summary>
    /// Player manager
    /// ビームの入力判定と発射を行う。
    /// </summary>

    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
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
        private GameObject beams;
        // true の時はユーザーがビームを出しているとき。
        bool IsFiring;

        [Tooltip("The current Health of our player")]
        public float Health = 1.0f;


        #endregion

        #region MonoBegavior Callbacks
        /// <summary>
        /// 初期化時に呼び出される
        /// </summary>
        /// 

        void Awake()
        {
            if(beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }
        }


        void Start()
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
        }

        /// <summary>
        /// 毎フレームの処理
        /// </summary>
        void Update()
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


