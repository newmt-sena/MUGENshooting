using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;

public class BulletObjectPool : MonoBehaviour
{
    // アクセスしやすいようにシングルトン化
    private static BulletObjectPool _instance;
    public static BulletObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BulletObjectPool>();
            }

            return _instance;
        }
    }

    [SerializeField] private BulletObject _BulletPrefab;  // オブジェクトプールで管理するオブジェクト
    private ObjectPool<BulletObject> _BulletPool;  // オブジェクトプール本体

   

    private void Start()
    {
        _BulletPool = new ObjectPool<BulletObject>(
            createFunc: () => OnCreateObject(),
            actionOnGet: (obj) => OnGetObject(obj),
            actionOnRelease: (obj) => OnReleaseObject(obj),
            actionOnDestroy: (obj) => OnDestroyObject(obj),
            collectionCheck: true,
            defaultCapacity: 3,
            maxSize: 15
        );

        StartCoroutine(delete());

    }

    private void Update()
    {
        

    }

    private IEnumerator delete()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            ClearBullet();

        }
    }

    // プールからオブジェクトを取得する
    public BulletObject GetBullet()
    {
        return _BulletPool.Get();
    }

    // プールの中身を空にする
    public void ClearBullet()
    {
        _BulletPool.Clear();
    }

    // プールに入れるインスタンスを新しく生成する際に行う処理
    private BulletObject OnCreateObject()
    {
        return Instantiate(_BulletPrefab, transform);
    }

    // プールからインスタンスを取得した際に行う処理
    private void OnGetObject(BulletObject BulletObject)
    {
        BulletObject.Initialize(() => _BulletPool.Release(BulletObject));
        BulletObject.gameObject.SetActive(true);
    }

    // プールにインスタンスを返却した際に行う処理
    private void OnReleaseObject(BulletObject BulletObject)
    {
        Debug.Log("Release");  // BulletObject側で非アクティブにするのでログ出力のみ。ここで非アクティブにするパターンもある。
    }

    // プールから削除される際に行う処理
    private void OnDestroyObject(BulletObject BulletObject)
    {
        Destroy(BulletObject.gameObject);
    }
}