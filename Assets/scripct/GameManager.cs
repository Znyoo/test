using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("레퍼런스")]
    public GameObject player;
    public GameObject exitPrefab;
    public MazeGenerator mazeGenerator;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI clearText;

    private float timer = 0f;
    private bool isGameOver = false;
    private bool isClear = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 플레이어 시작 위치 설정
        float cellSize = mazeGenerator.cellSize;
        player.transform.position = new Vector3(cellSize * 0.5f, 1f, cellSize * 0.5f);

        // 출구 생성
        float exitX = (mazeGenerator.width * 2 - 1) * cellSize * 0.5f;
        float exitZ = (mazeGenerator.height * 2 - 1) * cellSize * 0.5f;
        Instantiate(exitPrefab, new Vector3(exitX, 0.5f, exitZ), Quaternion.identity);
    }

    void Update()
    {
        if (isGameOver || isClear) return;

        timer += Time.deltaTime;
        if (timerText != null)
            timerText.text = "시간: " + Mathf.FloorToInt(timer) + "초";
    }

    public void GameClear()
    {
        isClear = true;
        if (clearText != null)
            clearText.text = "탈출 성공!\n시간: " + Mathf.FloorToInt(timer) + "초";
    }
}