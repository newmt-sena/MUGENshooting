using UnityEngine;
using UnityEngine.UI; // UIを操作するために必要

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // どこからでもアクセスできるようにする（シングルトン）
    public Text scoreText; // 画面に表示するTextコンポーネント
    private int killCount = 0; // 討伐数

    void Awake()
    {
        // 他のスクリプトから「GameManager.instance」で呼べるようにする
        if (instance == null) { instance = this; }
    }

    private void Start()
    {
        scoreText.text = "討伐数: " + killCount; // 表示を更新
    }

    // 敵が倒れた時に呼ぶメソッド
    public void AddKillCount()
    {
        killCount++;
        scoreText.text = "討伐数: " + killCount; // 表示を更新
    }
    // 自機が倒れた時に呼ぶメソッド
    public void AddDead()
    {
       scoreText.text = "やられてしまった！\n討伐数: " + killCount; // 表示を更新
    }
}