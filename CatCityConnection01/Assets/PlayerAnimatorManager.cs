using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ryu.catcityconnection
{
    public class PlayerAnimatorManager : MonoBehaviour
    {
        #region MonoBehaviour Callbacks


        private Animator animator;

        // Use this for initializationいつもコードを書くべきです。手間ですが、長い目で見るとそのほうがずっといいです。

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (!animator)
            {
                return;
            }
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (v < 0)
            {
                v = 0;
            }
            animator.SetFloat("Speed", h * h + v * v);

        }

        #endregion

    }

}

