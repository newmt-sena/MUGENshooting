using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletObject : MonoBehaviour
{
    private Action _onDisable;  // 非アクティブ化するためのコールバック
   
    public void Initialize(Action onDisable)
    {
        _onDisable = onDisable;
        

    }

    private void Update()
    {
     
    }

    public void Fire(Vector3 position, Quaternion rotation, Vector3 direction, float speed)
    {
        // 1. 位置と向きを設定
        transform.position = position;
        transform.rotation = rotation;

        // 2. 移動処理
        Rigidbody rb = GetComponent<Rigidbody>();

        // 既存の力が残っている可能性があるので一度リセット
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 3. 衝撃力を加える（FireBullet.csから移動したロジック）
        rb.AddForce(direction * speed, ForceMode.Impulse);

        // 4. プールへの自動返却を開始
        // 0.8秒後に ReleaseBullet メソッドを実行するコルーチンを開始
        StartCoroutine(ReleaseAfterDelay(0.8f));

    }

    // 衝突時の処理（EnemyObject.cs との連携）
    void OnTriggerEnter(Collider other)
    {
        // 例: 敵に当たったらすぐにプールに戻す
        if (other.CompareTag("enemy"))
        {
            ReleaseBullet();
        }
        // (弾が破壊されずにそのまま進む場合は、この処理は不要)
    }

    // プールへオブジェクトを返却する
    private void ReleaseBullet()
    {
        // 移動を止め、非アクティブにし、プールに戻す
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.SetActive(false);
        _onDisable?.Invoke(); // プールに返却（ObjectPool.Releaseが呼ばれる）
    }

    // 指定時間後に弾をプールに戻すコルーチン
    private System.Collections.IEnumerator ReleaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReleaseBullet();
    }
}