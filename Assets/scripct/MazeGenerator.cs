using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public int width = 10;
    public int height = 10;
    public float cellSize = 4f;

    private int[,] maze;

    void Start()
    {
        GenerateMaze();
        BuildMaze();
    }

    void GenerateMaze()
    {
        maze = new int[width * 2 + 1, height * 2 + 1];

        // 전부 벽으로 채우기
        for (int x = 0; x < width * 2 + 1; x++)
            for (int y = 0; y < height * 2 + 1; y++)
                maze[x, y] = 1;

        // DFS로 미로 생성
        bool[,] visited = new bool[width, height];
        DFS(0, 0, visited);

        // 출구 만들기
        maze[width * 2 - 1, height * 2] = 0;
    }

    void DFS(int x, int y, bool[,] visited)
    {
        visited[x, y] = true;
        maze[x * 2 + 1, y * 2 + 1] = 0;

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        // 방향 섞기
        for (int i = 0; i < 4; i++)
        {
            int r = Random.Range(i, 4);
            int tmp = dx[i]; dx[i] = dx[r]; dx[r] = tmp;
            tmp = dy[i]; dy[i] = dy[r]; dy[r] = tmp;
        }

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && nx < width && ny >= 0 && ny < height && !visited[nx, ny])
            {
                maze[x * 2 + 1 + dx[i], y * 2 + 1 + dy[i]] = 0;
                DFS(nx, ny, visited);
            }
        }
    }

    void BuildMaze()
    {
        for (int x = 0; x < width * 2 + 1; x++)
        {
            for (int y = 0; y < height * 2 + 1; y++)
            {
                if (maze[x, y] == 1)
                {
                    Vector3 pos = new Vector3(x * cellSize * 0.5f, 1f, y * cellSize * 0.5f);
                    GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity);
                    wall.transform.localScale = new Vector3(cellSize * 0.5f, 2f, cellSize * 0.5f);
                    wall.transform.SetParent(transform);
                }
            }
        }
    }
}