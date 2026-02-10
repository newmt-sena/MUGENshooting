using System;
using UnityEngine;

public class EnemyObject : MonoBehaviour
{
    private Action<EnemyObject> _onDisable;
    private Transform _playerTransform; // 毎回FindしないようにTransformで保持

    [Header("移動設定")]
    [SerializeField] private float speed = 5f; // インスペクターから調整可能な速度

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

    private void Update()
    {
        if (_playerTransform == null) return;

        // プレイヤーの方を向いて移動
        transform.LookAt(_playerTransform);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 何かが触れたら必ずログを出す
        Debug.Log($"敵に何かが当たりました: {other.gameObject.name} (Tag: {other.tag})");

        if (other.CompareTag("Bullet"))
        {
            Debug.Log("弾が当たったと判定されました！");
            if (GameManager.instance != null) GameManager.instance.AddKillCount();
            ReleaseEnemy();
        }
    }

    public void ReleaseEnemy()
    {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(false);
        _onDisable?.Invoke(this); // プールに返却
    }
}