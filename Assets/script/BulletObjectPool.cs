using UnityEngine;
using UnityEngine.Pool;

public class BulletObjectPool : MonoBehaviour
{
    private static BulletObjectPool _instance;
    public static BulletObjectPool Instance => _instance;

    [SerializeField] private BulletObject _bulletPrefab;
    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxSize = 20;

    private ObjectPool<BulletObject> _bulletPool;

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

        // ObjectPoolの初期化
        _bulletPool = new ObjectPool<BulletObject>(
            createFunc: OnCreateObject,
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject,
            collectionCheck: true,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
        );
    }

    // プールから取得
    public BulletObject GetBullet()
    {
        return _bulletPool.Get();
    }

    // --- ObjectPool 内部処理 ---

    private BulletObject OnCreateObject()
    {
        BulletObject obj = Instantiate(_bulletPrefab, transform);
        // 生成時に「どうやって返却するか」を一度だけ教えておく
        obj.Initialize(ReleaseBullet);
        return obj;
    }

    private void OnGetObject(BulletObject obj)
    {
        // 弾をアクティブにする処理はBulletObject.Fireで行うので、
        // ここでは基本的な表示切り替えだけでOK
        obj.gameObject.SetActive(true);
    }

    private void OnReleaseObject(BulletObject obj)
    {
        // 返却時は確実に非アクティブにする
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyObject(BulletObject obj)
    {
        Destroy(obj.gameObject);
    }

    // BulletObjectから呼ばれる返却用メソッド
    private void ReleaseBullet(BulletObject obj)
    {
        _bulletPool.Release(obj);
    }
}