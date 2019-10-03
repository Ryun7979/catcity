using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Photon.Pun.Demo.PunBasics
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {

        #region Private Fields

        [SerializeField]
        private float directionDampTime = 0.25f;
        private Animator animator;

        #endregion


        #region MonoBehaviour Callbacks

        // Use this for initializationいつもコードを書くべきです。手間ですが、長い目で見るとそのほうがずっといいです。
        void Start()
        {
            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }

        }

        void Update()
        {
            if(photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            // アニメーターが無い時はエラーだから戻るぞ。
            if (!animator)
            {
                return;
            }

            // 前進、回転

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (v < 0)
            {
                v = 0;
            }

            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);


            // ジャンプするぞ。
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // うごいてるときしかジャンプできないようにする
            if(stateInfo.IsName("Base Layer.Run"))
            {
                // Fire2をおしたら、ジャンプアニメ再生。
                if (Input.GetButtonDown("Fire2"))
                {
                    animator.SetTrigger("Jump");
                }

            }

        }

        #endregion

    }

}

