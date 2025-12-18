using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    public float moveSpeed = 20f;
    
    // 回転の滑らかさを調整するための変数
    public float rotationSpeed = 100f;

    private Vector3 latestPos; //前回のPosition

    [Header("タゲッティング設定")]
    [Tooltip("敵を探す半径")]
    public float detectionRange = 15f;
    [Tooltip("敵のTag名")]
    public string enemyTag = "enemy";

    private Transform targetEnemy; // 現在ロックオンしている敵のTransform


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        latestPos = transform.position;
    }



    // Update is called once per frame
    void Update()
    {
        FindTargetEnemy();

        Vector3 diff = transform.position - latestPos;   // 前回からどこに進んだかをベクトルで取得
        latestPos = transform.position;  // 前回のPositionの更新

        Vector3 lookDirection;

        // A. 敵がロックオンされているか、かつプレイヤーが動いていない場合
        if (targetEnemy != null)
        {
            // 敵の方向を向く
            lookDirection = targetEnemy.position - transform.position;
        }
        // B. 敵がロックオンされておらず、プレイヤーが移動している場合 (既存のロジック)
        else if (diff.magnitude > 0.01f)
        {
            // 移動方向を向く
            lookDirection = diff;
        }
        // C. どちらでもない場合（回転処理をスキップ）
        else
        {
            return; // 移動もせず、敵もいないので、回転処理は不要
        }

        // 以下の回転ロジックは既存のものを活かします

        // Y軸の回転のみを適用するため、Y軸成分を0にする
        lookDirection.y = 0;

        if (lookDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            // Slerpを使って滑らかに回転
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // 物理演算の更新はFixedUpdateで行うのが推奨されます
    void FixedUpdate()
    {
        // 1. 入力を取得
        var _h = Input.GetAxis("Horizontal");
        var _v = Input.GetAxis("Vertical");

        // 2. 移動方向ベクトルを作成（Y軸は0）
        var _movement = new Vector3(_h, 0, _v);

        // 3. 移動ベクトルを正規化し、速度を乗算
        //    正規化 (Normalize) することで、斜め移動の速度が速くなりすぎるのを防ぎます。
        //    moveSpeedをかけることで、望みの速度になります。
        if (_movement.magnitude > 1) // 念のため正規化
        {
            _movement.Normalize();
        }

        // 4. 新しい速度ベクトルを計算
        //    XとZ軸の速度は入力とmoveSpeedで設定しますが、
        //    Y軸の速度は既存の速度 (重力など) を維持します。
        var _newVelocity = new Vector3(
            _movement.x * moveSpeed, // X速度を設定
            rb.velocity.y,           // Y速度はそのまま維持
            _movement.z * moveSpeed  // Z速度を設定
        );

        // 5. Rigidbodyの速度を直接上書き
        rb.velocity = _newVelocity;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("当たったよ");
        if (collider.gameObject.CompareTag("enemy"))
        {
           Destroy(this.gameObject);
            GameManager.instance.AddDead();
        }


    }

    private void FindTargetEnemy()
    {
        // 索敵範囲内のすべてのColliderを取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);

        // 最も近い敵と、その距離を追跡するための変数
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        // 取得したColliderを一つずつチェック
        foreach (var hitCollider in hitColliders)
        {
            // 敵のTag（"Enemy"など）が付いているか確認
            if (hitCollider.CompareTag(enemyTag))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);

                // 現在見つけた敵が、これまでの最も近い敵よりも近ければ更新
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hitCollider.transform;
                }
            }
        }

        // 索敵範囲内に敵がいなければ targetEnemy は null になる
        targetEnemy = closestEnemy;
    }
}
