using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyObject : MonoBehaviour
{
    private Action _onDisable;  // 非アクティブ化するためのコールバック
    private GameObject target;
    public float speed;


    public void Initialize(Action onDisable)
    {
        _onDisable = onDisable;

        speed = 0.01f;
        target = GameObject.Find("Player");
    }

    private void Update()
    {
        transform.LookAt(target.transform);
        transform.position += transform.forward * speed;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("当たったよ");
            _onDisable?.Invoke();
            gameObject.SetActive(false);
           

        }
    }
}