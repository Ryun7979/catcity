using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ryu.catcityconnection
{

    public class PlayerUI : MonoBehaviour
    {


        #region Private Fields

        // プレイヤーUIの位置調整(pixel)
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

        [Tooltip("プレイヤーネームを表示するtxtオブジェクト")]
        [SerializeField]
        private Text playerNameText;

        [Tooltip("スライダーのオブジェクト")]
        [SerializeField]
        private Slider playerHealthSlider;

        PlayerManager target;

        float characterControllerHeight = 0f;

        Transform targetTransform;

        Vector3 targetPosition;


        #endregion


        #region MonoBehaviour CallBacks

        void Awake()
        {
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        }




        // Update is called once per frame
        void Update()
        {
            // ターゲットがnullの場合、自身を破壊します。Photonがネットワーク上のプレーヤーのインスタンスを破壊している場合、フェイルセーフです。
            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }

            // プレイヤーのhealthをスライダーに反映
            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = target.Health;
            }


        }

        #endregion

        #region Public Methods

        public void SetTarget(PlayerManager _target)
        {
            if(_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }

            // 効率化のためのキャッシュ参照
            target = _target;
            if(playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }

            CharacterController characterController = _target.GetComponent<CharacterController>();
            // このコンポーネントの存続期間中に変更されないデータをプレーヤーから取得します
            if (characterController != null)
            {
                characterControllerHeight = characterController.height;
            }

        }

        void LateUpdate()
        {
            // 画面上のターゲットGameObjectに従います。
            if (targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
            }
        }


        #endregion


    }

}

