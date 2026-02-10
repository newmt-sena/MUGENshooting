using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float rotationSpeed = 10f; // Slerp用に少し小さめの値が使いやすいです

    [Header("タゲッティング設定")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private string enemyTag = "enemy";

    [Header("射撃設定")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;

    private Rigidbody rb;
    private Transform targetEnemy;
    private Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. 入力取得 (Updateで行うのが定石)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(h, 0, v).normalized;

        // 2. 索敵
        FindTargetEnemy();

        // 3. 回転
        UpdateRotation();

        // 4. 射撃
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        // 物理移動
        rb.velocity = new Vector3(moveInput.x * moveSpeed, rb.velocity.y, moveInput.z * moveSpeed);
    }

    private void UpdateRotation()
    {
        Vector3 lookDirection = Vector3.zero;

        if (targetEnemy != null)
        {
            // 敵がいるなら敵の方向
            lookDirection = targetEnemy.position - transform.position;
        }
        else if (moveInput.magnitude > 0.1f)
        {
            // 敵がいなくて移動中なら移動方向
            lookDirection = moveInput;
        }

        if (lookDirection != Vector3.zero)
        {
            lookDirection.y = 0;
            Quaternion targetRot = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void FindTargetEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(enemyTag))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hitCollider.transform;
                }
            }
        }
        targetEnemy = closestEnemy;
    }

    void Shoot()
    {
        var bullet = BulletObjectPool.Instance.GetBullet();
        if (bullet != null)
        {
            // 発射方向の決定
            Vector3 shootDir = targetEnemy != null
                ? (targetEnemy.position - firePoint.position).normalized
                : transform.forward;

            bullet.Fire(firePoint.position, firePoint.rotation, shootDir, bulletSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag))
        {
            // プレイヤー消滅時の処理（GameManager側で管理するのが綺麗です）
            GameManager.instance.AddDead();
            Destroy(gameObject);
        }
    }

    // 索敵範囲をSceneビューで可視化（デバッグ用）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}