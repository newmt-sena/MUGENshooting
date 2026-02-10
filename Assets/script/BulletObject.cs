using System;
using UnityEngine;

public class BulletObject : MonoBehaviour
{
    private Action<BulletObject> _onDisable;
    private Vector3 _direction;
    private float _speed;
    private bool _isActive = false;

    [SerializeField] private float lifeTime = 2.0f; // 弾の射程時間
    private float _timer;

    public void Initialize(Action<BulletObject> onDisable)
    {
        _onDisable = onDisable;
    }

    public void Fire(Vector3 position, Quaternion rotation, Vector3 direction, float speed)
    {
        transform.position = position;
        transform.rotation = rotation;
        _direction = direction.normalized;
        _speed = speed;

        _timer = 0;
        _isActive = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!_isActive) return;

        _timer += Time.deltaTime;
        if (_timer >= lifeTime) { ReleaseBullet(); return; }

        float moveDistance = _speed * Time.deltaTime;

        // 進行方向に何かあるかチェック
        if (Physics.Raycast(transform.position, _direction, out RaycastHit hit, moveDistance))
        {
            // --- ここが重要！ ---
            // 当たった相手が敵（enemyタグ）だった場合
            if (hit.collider.CompareTag("enemy"))
            {
                // 相手のEnemyObjectスクリプトを取得して、直接「ReleaseEnemy」を呼ぶ
                // ※GetComponentは少し重いですが、当たった瞬間だけなので許容範囲です
                EnemyObject enemy = hit.collider.GetComponent<EnemyObject>();
                if (enemy != null)
                {
                    // GameManagerのスコア加算もここで行うと確実です
                    if (GameManager.instance != null) GameManager.instance.AddKillCount();

                    // 敵をプールに戻すメソッドを呼ぶ（外部から呼べるようにEnemyObject側の修正が必要）
                    enemy.SendMessage("ReleaseEnemy", SendMessageOptions.DontRequireReceiver);
                }
            }

            transform.position = hit.point;
            ReleaseBullet();
        }
        else
        {
            transform.position += _direction * moveDistance;
        }
    }

    private void ReleaseBullet()
    {
        if (!_isActive) return;

        _isActive = false;
        gameObject.SetActive(false);
        _onDisable?.Invoke(this); // プールに返却
    }
}