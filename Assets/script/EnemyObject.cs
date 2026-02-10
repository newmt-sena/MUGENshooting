using System;
using UnityEngine;

public class EnemyObject : MonoBehaviour
{
    private Action<EnemyObject> _onDisable;
    private Transform _playerTransform; // 毎回FindしないようにTransformで保持
    private Rigidbody _rb;

    [Header("移動設定")]
    [SerializeField] private float speed = 5f; // インスペクターから調整可能な速度


    [SerializeField] private int maxHP = 1;
    private int currentHP;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Action<EnemyObject> onDisable)
    {
        _onDisable = onDisable;

        // プレイヤーを一度だけ探す（すでに見つけていればスキップ）
        if (_playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) _playerTransform = player.transform;
        }
    }

    private void FixedUpdate()
    {
        if (_playerTransform == null) return;

        Vector3 direction = (_playerTransform.position - transform.position);
        direction.y = 0f;

        float distance = direction.magnitude;
        if (distance < 1.5f) return;

        direction.Normalize();

        _rb.MovePosition(_rb.position + direction * speed * Time.fixedDeltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, 5f * Time.fixedDeltaTime));
    }

    public void ReleaseEnemy()
    {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(false);
        _onDisable?.Invoke(this); // プールに返却
    }

    private void OnEnable()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (GameManager.instance != null)
            GameManager.instance.AddKillCount();

        ReleaseEnemy();
    }
}