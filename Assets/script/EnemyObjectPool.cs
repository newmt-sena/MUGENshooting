using UnityEngine;
using UnityEngine.Pool;

public class EnemyObjectPool : MonoBehaviour
{
    private static EnemyObjectPool _instance;
    public static EnemyObjectPool Instance => _instance;

    [SerializeField] private EnemyObject _enemyPrefab;
    [SerializeField] private int _defaultCapacity = 5;
    [SerializeField] private int _maxSize = 15;

    private ObjectPool<EnemyObject> _enemyPool;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // プールの初期化
        _enemyPool = new ObjectPool<EnemyObject>(
            createFunc: OnCreateObject,
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject,
            collectionCheck: true,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
        );
    }

    /// <summary>
    /// 敵を取得して指定した位置に配置します
    /// </summary>
    public EnemyObject SpawnEnemy(Vector3 position)
    {
        EnemyObject enemy = _enemyPool.Get();
        enemy.transform.position = position;
        return enemy;
    }

    public void ClearEnemy() => _enemyPool.Clear();

    // --- ObjectPool 内部処理 ---

    private EnemyObject OnCreateObject()
    {
        EnemyObject enemy = Instantiate(_enemyPrefab, transform);
        // 返却用のActionを一度だけ登録
        enemy.Initialize(ReleaseEnemy);
        return enemy;
    }

    private void OnGetObject(EnemyObject enemy)
    {
        enemy.gameObject.SetActive(true);
    }

    private void OnReleaseObject(EnemyObject enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void OnDestroyObject(EnemyObject enemy)
    {
        Destroy(enemy.gameObject);
    }

    // EnemyObjectから呼ばれる返却メソッド
    private void ReleaseEnemy(EnemyObject enemy)
    {
        _enemyPool.Release(enemy);
    }
}