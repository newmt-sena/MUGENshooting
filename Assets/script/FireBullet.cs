using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : MonoBehaviour
{
    [SerializeField]
    [Tooltip("弾の発射場所")]
    private GameObject firingPoint;
   
    [SerializeField]
    [Tooltip("弾の速さ")]
    private float speed = 30f;
    // Update is called once per frame

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // スペースキーが押されたかを判定
        if (Input.GetKeyDown(KeyCode.Space))
        {

            BulletObject newBulletObject = BulletObjectPool.Instance.GetBullet();
            // 弾を発射する場所を取得
            Vector3 bulletPosition = firingPoint.transform.position;
            Vector3 direction = this.transform.forward;

            newBulletObject.Fire(bulletPosition, this.transform.rotation, direction, speed);
        }
    }
}
