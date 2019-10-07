using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ryu.catcityconnection
{
    public class CameraWork : MonoBehaviour
    {

        #region Private Fields

        [Tooltip("The distance in the local x-z plane to the target")]
        [SerializeField]
        private float distance = 7.0f;

        [Tooltip("The height we want the camera to be above the target")]
        [SerializeField]
        private float height = 3.0f;
        
        [Tooltip("The Smooth time lag for the height of the camera.")]
        [SerializeField]
        private float heightSmoothLag = 0.3f;

        [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
        [SerializeField]
        private Vector3 centerOffset = Vector3.zero;

        [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
        [SerializeField]
        private bool followOnStart = false;

        // ターゲット変換のキャッシュ
        Transform cameraTransform;

        // ターゲットを見失ったときなどのフラグ
        bool isFollowing;

        // 現在の速度
        private float heightVelocity;

        // 到達しようとする位置
        private float targetHeight = 100000.0f;


        #endregion

        #region MonoBegavior CallBacks

        // Start is called before the first frame update
        void Start()
        {
            // ターゲットフォローを始める
            if (followOnStart)
            {
                OnStartFollowing();
            }

        }

        void LateUpdate()
        {
            if(cameraTransform == null && isFollowing)
            {
                OnStartFollowing();
            }

            if (isFollowing)
            {
                Apply();
            }

        }

        #endregion

        #region Public Methods

        public void OnStartFollowing()
        {
            cameraTransform = Camera.main.transform;
            isFollowing = true;
            // 何も平滑化せず、正しいカメラショットに直行します
            Cut();
        }


        #endregion

        #region Private Methods

        void Apply()
        {
            Vector3 targetCenter = transform.position + centerOffset;
            // Calculate the current & target rotation angles
            float originalTargetAngle = transform.eulerAngles.y;
            float currentAngle = cameraTransform.eulerAngles.y;
            // Adjust real target angle when camera is locked
            float targetAngle = originalTargetAngle;
            currentAngle = targetAngle;
            targetHeight = targetCenter.y + height;


            // Damp the height
            float currentHeight = cameraTransform.position.y;
            currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightVelocity, heightSmoothLag);
            // Convert the angle into a rotation, by which we then reposition the camera
            Quaternion currentRotation = Quaternion.Euler(0, currentAngle, 0);
            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            cameraTransform.position = targetCenter;
            cameraTransform.position += currentRotation * Vector3.back * distance;
            // Set the height of the camera
            cameraTransform.position = new Vector3(cameraTransform.position.x, currentHeight, cameraTransform.position.z);
            // Always look at the target
            SetUpRotation(targetCenter);
        }


        /// 指定したターゲットと中心にカメラを直接配置します。
        /// 
        void Cut()
        {
            float oldHeightSmooth = heightSmoothLag;
            heightSmoothLag = 0.001f;
            Apply();
            heightSmoothLag = oldHeightSmooth;
        }

        // カメラの回転を常にターゲットの後ろに設定します

        void SetUpRotation(Vector3 centerPos)
        {
            Vector3 cameraPos = cameraTransform.position;
            Vector3 offsetToCenter = centerPos - cameraPos;
            // y軸を中心としたベース回転のみを生成
            Quaternion yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));
            Vector3 relativeOffset = Vector3.forward * distance + Vector3.down * height;
            cameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);

        }


        #endregion



    }

}

