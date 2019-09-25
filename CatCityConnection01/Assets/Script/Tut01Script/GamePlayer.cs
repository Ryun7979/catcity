using Photon.Pun;
using UnityEngine;

public class GamePlayer : MonoBehaviourPunCallbacks
{
    private ProjectileManager projectileManager;

    private void Awake()
    {
        projectileManager = GameObject.FindWithTag("ProjectileManager").GetComponent<ProjectileManager>();
    }


    private void Update()
    {
        if (photonView.IsMine)
        {
            // 入力方向（ベクトル）を正規化する
            var direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            // 移動速度を時間に依存させて、移動量を求める
            var dv = 6f * Time.deltaTime * direction;
            transform.Translate(dv.x, dv.y, 0f);


            // 左クリックでカーソルの方向に弾を発射する処理を行う
            if (Input.GetMouseButtonDown(0))
            {
                var playerWorldPosition = transform.position;
                var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var dp = mouseWorldPosition - playerWorldPosition;
                float angle = Mathf.Atan2(dp.y, dp.x);

                // FireProjectile(angle)をRPCで実行する
                photonView.RPC(nameof(FireProjectile), RpcTarget.All, angle);
            }
        }
    }

    // 弾を発射するメソッド
    // [PunRPC]属性をつけると、RPCでの実行が有効になる
    [PunRPC]
    private void FireProjectile(float angle)
    {
        projectileManager.Fire(transform.position, angle);
    }
}
