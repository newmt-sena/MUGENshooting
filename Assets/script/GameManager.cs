using UnityEngine;
using TMPro; // TextMeshProを使うために必要

public class GameManager : MonoBehaviour
{
    // シングルトンの実装
    public static GameManager instance { get; private set; }

    [Header("UI設定")]
    [SerializeField] private TextMeshProUGUI scoreText; // TextMeshProUGUIに変更

    private int killCount = 0;
    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // シーンを跨ぐ場合は DontDestroyOnLoad(gameObject); を追加
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// 敵が倒れた時に外部から呼ばれる
    /// </summary>
    public void AddKillCount()
    {
        if (isGameOver) return;

        killCount++;
        UpdateUI();
    }

    /// <summary>
    /// プレイヤーがやられた時に外部から呼ばれる
    /// </summary>
    public void AddDead()
    {
        isGameOver = true;
        scoreText.text = $"<color=red>GAME OVER</color>\nScore: {killCount}";
    }

    /// <summary>
    /// UIの表示を最新の状態に更新する
    /// </summary>
    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {killCount}";
        }
    }
}