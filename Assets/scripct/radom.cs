using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI를 제어하기 위해 필수!
using TMPro; // 만약 TextMeshPro를 쓴다면 필요

public class random : MonoBehaviour
{
    [Header("프리팹 설정")]
    public GameObject floorPrefab; 
    public GameObject wallPrefab;  

    [Header("미로 크기 (숫자가 클수록 미로가 커집니다)")]
    public int width = 25;  // 기존 15에서 25로 크게 키움!
    public int height = 25; // 기존 15에서 25로 크게 키움!

    [Header("플레이어 설정")]
    public GameObject player; // 하이어라키의 Jammo_Player를 여기에 드래그!

    [Header("UI 설정")]
    public TextMeshProUGUI goalText; // 화면에 띄울 TextMeshPro - Text 오브젝트 연결

    private int[,] maze; // 0: 길, 1: 벽
    private float calculatedBlockSize = 2.0f;
    private float calculatedWallYOffset = 0.0f;

    private Vector3 startPosition;
    private Vector3 exitPosition;
    private bool isGoalReached = false;

    void Start()
    {
        if (width % 2 == 0) width++;
        if (height % 2 == 0) height++;

        // Goal 문구 처음엔 안 보이게 숨기기
        if (goalText != null) goalText.gameObject.SetActive(false);

        CalculatePrefabDimensions();
        GenerateMazeData();
        SpawnMaze();
        PositionPlayerAtStart();
    }

    void Update()
    {
        // 매 프레임마다 플레이어가 끝 지점에 도달했는지 체크
        if (!isGoalReached && player != null)
        {
            // 플레이어와 출구 사이의 거리가 블록 크기보다 가까워지면 승리!
            if (Vector3.Distance(player.transform.position, exitPosition) < calculatedBlockSize * 0.7f)
            {
                isGoalReached = true;
                if (goalText != null)
                {
                    goalText.text = "Goal!";
                    goalText.gameObject.SetActive(true); // 문구 띄우기
                }
                Debug.Log("★ 축하합니다! 미로를 탈출했습니다! ★");
            }
        }
    }

    void CalculatePrefabDimensions()
    {
        if (wallPrefab != null)
        {
            Collider wallCollider = wallPrefab.GetComponent<Collider>();
            MeshFilter meshFilter = wallPrefab.GetComponentInChildren<MeshFilter>();
            Vector3 size = Vector3.one * 5f;

            if (wallCollider != null) size = wallCollider.bounds.size;
            else if (meshFilter != null && meshFilter.sharedMesh != null) size = meshFilter.sharedMesh.bounds.size;

            calculatedBlockSize = size.x;
            calculatedWallYOffset = size.y / 2f;
        }
    }

    void GenerateMazeData()
    {
        maze = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) maze[x, y] = 1;
        }

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int start = new Vector2Int(1, 1);
        maze[start.x, start.y] = 0;
        stack.Push(start);

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = new List<Vector2Int>();

            foreach (var dir in dirs)
            {
                Vector2Int next = current + dir * 2;
                if (next.x > 0 && next.x < width - 1 && next.y > 0 && next.y < height - 1)
                {
                    if (maze[next.x, next.y] == 1) neighbors.Add(dir);
                }
            }

            if (neighbors.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, neighbors.Count);
                Vector2Int chosenDir = neighbors[randomIndex];
                maze[current.x + chosenDir.x, current.y + chosenDir.y] = 0;
                maze[current.x + chosenDir.x * 2, current.y + chosenDir.y * 2] = 0;
                stack.Push(current + chosenDir * 2);
            }
            else stack.Pop();
        }

        // 입구(시작)와 출구(끝) 좌표 계산 및 데이터 뚫기
        maze[1, 0] = 0; 
        maze[width - 2, height - 1] = 0; 

        // 실제 3D 세계관 좌표로 변환하여 기억해두기
        startPosition = new Vector3(1 * calculatedBlockSize, 0.5f, 0 * calculatedBlockSize);
        exitPosition = new Vector3((width - 2) * calculatedBlockSize, 0.5f, (height - 1) * calculatedBlockSize);
    }

    void SpawnMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * calculatedBlockSize, 0, y * calculatedBlockSize);

                if (floorPrefab != null)
                {
                    GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity);
                    floor.transform.SetParent(this.transform);
                    FixCeilingAndScale(floor, false);
                }

                if (maze[x, y] == 1 && wallPrefab != null)
                {
                    Vector3 wallPosition = new Vector3(position.x, calculatedWallYOffset, position.z);
                    GameObject wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity);
                    wall.transform.SetParent(this.transform);
                    FixCeilingAndScale(wall, true);
                }
            }
        }
    }

    void PositionPlayerAtStart()
    {
        if (player != null)
        {
            // CharacterController가 켜져 있으면 좌표 강제 이동이 안 씹히도록 잠시 껐다 켬
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = startPosition;

            if (cc != null) cc.enabled = true;
            Debug.Log($"[미로] 플레이어를 시작 지점({startPosition})으로 순간이동 시켰습니다.");
        }
    }

    void FixCeilingAndScale(GameObject target, bool isWall)
    {
        Transform[] allChildren = target.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            string name = child.name.ToLower();
            if (name.Contains("ceiling") || name.Contains("top") || name.Contains("roof"))
            {
                Destroy(child.gameObject);
            }
        }
        if (isWall) target.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}