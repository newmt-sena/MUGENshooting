using UnityEngine;
using System.Collections;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("スポーン設定")]
    [SerializeField] private float spawnInterval = 3f; // 何秒ごとにスポーンするか
    [SerializeField] private int enemiesPerSpawn = 2;  // 一度に何体出すか

    [Header("出現エリア設定")]
    [SerializeField] private Vector3 spawnAreaCenter = new Vector3(0, 0, 10);
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(20, 0, 10);

    private void Start()
    {
        // スポーンのループを開始
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            // 指定秒数待機
            yield return new WaitForSeconds(spawnInterval);

            // 敵を〇体生成
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        // ランダムな位置を計算
        Vector3 randomPos = new Vector3(
            Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2),
            spawnAreaCenter.y,
            Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2)
        );

        // プールから敵を取得して配置
        if (EnemyObjectPool.Instance != null)
        {
            EnemyObjectPool.Instance.SpawnEnemy(randomPos);
        }
    }

    // Unityエディタ上で出現範囲を見えるようにする（デバッグ用）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}