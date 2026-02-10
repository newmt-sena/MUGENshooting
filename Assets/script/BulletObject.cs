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
            if (hit.collider.CompareTag("enemy"))
            {
                EnemyObject enemy = hit.collider.GetComponent<EnemyObject>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1);
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