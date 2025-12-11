using UnityEngine;
using UnityEngine.Pool;

public class EnemyObjectPool : MonoBehaviour
{
    // アクセスしやすいようにシングルトン化
    private static EnemyObjectPool _instance;
    public static EnemyObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnemyObjectPool>();
            }

            return _instance;
        }
    }

    [SerializeField] private EnemyObject _enemyPrefab;  // オブジェクトプールで管理するオブジェクト
    private ObjectPool<EnemyObject> _enemyPool;  // オブジェクトプール本体

    private void Start()
    {
        _enemyPool = new ObjectPool<EnemyObject>(
            createFunc: () => OnCreateObject(),
            actionOnGet: (obj) => OnGetObject(obj),
            actionOnRelease: (obj) => OnReleaseObject(obj),
            actionOnDestroy: (obj) => OnDestroyObject(obj),
            collectionCheck: true,
            defaultCapacity: 3,
            maxSize: 10
        );
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            GetEnemy();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            ClearEnemy();
        }
    }
    // プールからオブジェクトを取得する
    public EnemyObject GetEnemy()
    {
        
            return _enemyPool.Get();
        
        
    }

    // プールの中身を空にする
    public void ClearEnemy()
    {
        _enemyPool.Clear();
    }

    // プールに入れるインスタンスを新しく生成する際に行う処理
    private EnemyObject OnCreateObject()
    {
        return Instantiate(_enemyPrefab, transform);
    }

    // プールからインスタンスを取得した際に行う処理
    private void OnGetObject(EnemyObject enemyObject)
    {
        enemyObject.transform.position = new Vector3(Random.Range(-10, 10), 2, Random.Range(7, 16));
        enemyObject.Initialize(() => _enemyPool.Release(enemyObject));
        enemyObject.gameObject.SetActive(true);
    }

    // プールにインスタンスを返却した際に行う処理
    private void OnReleaseObject(EnemyObject enemyObject)
    {
        Debug.Log("Release");  // EnemyObject側で非アクティブにするのでログ出力のみ。ここで非アクティブにするパターンもある。
    }

    // プールから削除される際に行う処理
    private void OnDestroyObject(EnemyObject enemyObject)
    {
        Destroy(enemyObject.gameObject);
    }
}