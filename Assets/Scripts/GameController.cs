using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Material[] materials;
    [SerializeField] private GameObject leftEdge;
    [SerializeField] private GameObject rightEdge;
    [SerializeField] private GameObject lineTriger;
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private GameObject pauseWindow;
    [SerializeField] private Button pauseBtn;

    public static GameController Instance;
    private int[,] graphList;
    private Ball[,] graphListBall;

    private Vector3 leftSidePos;
    private Vector3 rightSidePos;

    private Vector3 startPositionPlayerBalls = new Vector3(0, -3.4f, -0.5f);

    private int[,] directions = new int[,]
    {
        { 0, -1 },   // left
        //{ -1, -1 },  // top-left
        { -1, 0 },   // top
        //{ -1, 1 },   // top-right
        { 0, 1 }     // right
    };
    // === Only for balls ===

    private Vector3 startPositionBalls = new Vector3(-2.55f, 8.7f, -0.5f);
    private float startsPosOffSetXFirstRow = 0.3f;//0,45
    private float offSetX = 0.5f;
    private float offSetY = 0.5f;

    private int currentRow = 1;
    private int maxRows = 16;
    private int score = 0;
    private int currentCountBalls = 0;

    private int countMaxBallsFirstRow;

    private void Awake()
    {
        pauseBtn.onClick.AddListener(() =>
        {
            Audio.Instance.PlaySFX(4);
            pauseWindow.gameObject.SetActive(true);
        });
        if (Instance == null)
        {
            Instance = this;
        }
        leftSidePos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        rightSidePos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, Camera.main.nearClipPlane));

        float screenWidth = (leftSidePos.x - rightSidePos.x) * -1f;
        //Debug.Log("leftSideX: " + leftSidePos.x);
        countMaxBallsFirstRow = Mathf.FloorToInt(screenWidth / offSetX);

        Vector3 scale = lineTriger.transform.localScale;
        scale.x = Screen.width;
        transform.localScale = scale;

        startPositionBalls.x = leftSidePos.x;
    }
    private void Start()
    {
        SetCorrectBorders();
        SpawnBalls();
        SpawnPlayerBalls();
    }

    private void DecreaseCountOfBalls()
    {
        currentCountBalls--;
        if (currentCountBalls == 0)
        {
            ResultWindow.Instance.Show(GetScore(), true);
        }
    }

    public void AddPointsToScore(int value)
    {
        score += value;
        textScore.text = score.ToString();
    }
    public int GetScore()
    {
        return score;
    }
    private void SetCorrectBorders()
    {
        Vector3 pos = leftEdge.transform.position;
        pos.x = leftSidePos.x;
        leftEdge.transform.position = pos;

        Vector3 pos1 = rightEdge.transform.position;
        pos1.x = rightSidePos.x;
        rightEdge.transform.position = pos1;
    }

    private void SpawnPlayerBalls()
    {
        int maxPlayerBalls = 25;
        SpawnBallForPlayer.Instance.CreateObjectsInPool(maxPlayerBalls);
        
        for (int i = 0; i < 2; i++)
        {
            GameObject gameObject = SpawnBallForPlayer.Instance.GetPoolObject();
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            PlayerBall playerBall = gameObject.GetComponent<PlayerBall>();
            Ball Ball = gameObject.GetComponent<Ball>();

            int index = Random.Range(0, 5);

            Ball.IndexColor = index;
            playerBall.SetWaiting(false);
            Ball.SetGoingDown(false);

            Debug.Log("index " + index);

            meshRenderer.material = materials[index];

            gameObject.transform.position = new Vector3(startPositionPlayerBalls.x, startPositionPlayerBalls.y - i * 4, startPositionPlayerBalls.z);
            gameObject.SetActive(true);

            if (i == 1)
            {
                playerBall.SetWaiting(true);
            }
        }
    }

    public void PutNextPlayerBall()
    {
        //int indexPlayerBall, int ballIndex, int rowBall, int rowIndex, int columnIndex
        GameObject gameObject = SpawnBallForPlayer.Instance.GetPlayerFromQueryObject();
        Ball Ball = gameObject.GetComponent<Ball>();
        gameObject.transform.position = new Vector3(startPositionPlayerBalls.x, startPositionPlayerBalls.y, startPositionPlayerBalls.z);

        PlayerBall playerBall = gameObject.GetComponent<PlayerBall>();
        playerBall.SetWaiting(false);
        playerBall.ReleaseFromScene();
        Ball.SetGoingDown(false);

        gameObject.SetActive(true);

        GetReadyNextPlayerball();
    }

    private void GetReadyNextPlayerball()
    {
        GameObject gameObject = SpawnBallForPlayer.Instance.GetPoolObject();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        PlayerBall playerBall = gameObject.GetComponent<PlayerBall>();
        Ball Ball = gameObject.GetComponent<Ball>();

        int index = Random.Range(0, 5);
        Ball.IndexColor = index;
        playerBall.SetWaiting(true);
        Ball.SetGoingDown(false);

        Ball.SetRow(currentRow);
        Ball.SetRowAndColumnIndex(-1, -1);

        Rigidbody rigidbody = playerBall.GetComponent<Rigidbody>();
        //Debug.Log("rigi PLyaer Ball: " + rigidbody.useGravity);
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;


        meshRenderer.material = materials[index];

        gameObject.transform.position = new Vector3(startPositionPlayerBalls.x, startPositionPlayerBalls.y - 1, startPositionPlayerBalls.z - 4.0f);
        gameObject.SetActive(true);
    }

    private void SpawnBalls()
    {
        int currentRowInLoop = 0;

        graphList = new int[maxRows + 6, countMaxBallsFirstRow];
        graphListBall = new Ball[maxRows + 6, countMaxBallsFirstRow];
        for (int i = 0; i < graphList.GetLength(0); i++)
        {
            for (int j = 0; j < graphList.GetLength(1); j++)
            {
                graphList[i,j] = -1;
            }
        }

        CustomPool.Instance.CreateObjectsInPool(graphList.GetLength(0) * graphList.GetLength(1));

        for (int i = 0; i < maxRows + 0; i++)
        {
            Vector3 startPositionBall = startPositionBalls;

            int maxColumns = countMaxBallsFirstRow;

            startPositionBall.x += startsPosOffSetXFirstRow;
            //Debug.Log("maxColumns: " + maxColumns + " startPosOffSetX: " + startPosOffSetX + " startPositionBall: " + startPositionBall);
            for (int j = 0; j < maxColumns; j++)
            {
                GameObject gameobject = CustomPool.Instance.GetPoolObject();
                if (gameobject != null)
                {
                    int index = Random.Range(0, 5);

                    MeshRenderer meshRenderer = gameobject.GetComponent<MeshRenderer>();
                    meshRenderer.material = materials[index];

                    Ball ball = gameobject.GetComponent<Ball>();
                    ball.IndexColor = index;
                    ball.SetRow(currentRow);
                    ball.SetRowAndColumnIndex(i,j);
                    gameobject.transform.position = startPositionBall;
                    gameobject.SetActive(true);

                    graphListBall[currentRowInLoop, j] = ball;

                    graphList[currentRowInLoop, j] = index;
                    //Debug.Log("row: " + currentRowInLoop + " column: " + j + " index: " + index);

                    currentCountBalls++;
                }
                startPositionBall.x += offSetX;
                if ((j == maxColumns - 1) && currentRow == 2)
                {
                    if (rightSidePos.x - startPositionBall.x > 0.4f)
                    {
                        maxColumns++;
                    }
                    //Debug.Log("rightSide.x - startPositionBall.x: " + (rightSide.x - startPositionBall.x));
                }
            }

            if (currentRow == 1)
            {
                currentRow++;
            }
            else
            {
                currentRow--;
            }
            startPositionBalls.y -= offSetY;
            currentRowInLoop++;
        }
        //Debug.Log("count: " + currentCountBalls);
        //Debug.Log("строк: " + graphList.GetLength(0) + " столбцов: " + graphList.GetLength(1));
        for (int i = 0; i < graphList.GetLength(0); i++)
        {
            string row = "";
            for (int j = 0; j < graphList.GetLength(1); j++)
            {
                row += graphList[i, j] + "\t";
            }
            Debug.Log(row);
        }
    }

    public void FindConnectedSameValues(int startRow, int startCol)
    {
        int rows = graphList.GetLength(0);
        int cols = graphList.GetLength(1);
        int targetValue = graphList[startRow, startCol];

        bool[,] visited = new bool[rows, cols];
        List<Vector2Int> result = new List<Vector2Int>();
        List<Vector2Int> ballsBelowRemoved = new List<Vector2Int>();

        DFS(startRow, startCol);

        void DFS(int row, int col)
        {
            if (row < 0 || row >= rows || col < 0 || col >= cols) return;
            if (visited[row, col]) return;
            if (graphList[row, col] != targetValue) return;

            visited[row, col] = true;
            result.Add(new Vector2Int(row, col));

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int newRow = row + directions[i, 0];
                int newCol = col + directions[i, 1];
                DFS(newRow, newCol);
            }
        }

        if (result.Count > 2)
        {
            foreach (Vector2Int i in result)
            {
                //Debug.Log("i.x: " + i.x + " i.y: " + i.y);
                graphListBall[i.x, i.y].gameObject.GetComponent<Ball>().ReleaseFromScene();
                SetBalltoGraph(i.x, i.y, -1, null);
                AddPointsToScore(10);
                DecreaseCountOfBalls();
            }

            foreach (Vector2Int removedPos in result)
            {
                for (int row = removedPos.x + 1; row < rows; row++)
                {
                    if (graphList[row, removedPos.y] != -1)
                    {
                        Vector2Int pos = new Vector2Int(row, removedPos.y);
                        if (!ballsBelowRemoved.Contains(pos))
                        {
                            ballsBelowRemoved.Add(pos);
                        }
                    }
                }
            }

            if (ballsBelowRemoved.Count > 0)
            {
                DropBallsBelow(ballsBelowRemoved);
            }
        }
    }

    private void DropBallsBelow(List<Vector2Int> list)
    {
        foreach (var pos in list)
        {
            //Debug.Log($"Ball below removed: x={pos.x}, y={pos.y}");

            Rigidbody rigidbody = graphListBall[pos.x, pos.y].gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;

            Vector3 posBall = graphListBall[pos.x, pos.y].gameObject.transform.position;
            posBall.z = -2;
            graphListBall[pos.x, pos.y].gameObject.transform.position = posBall;

            SetBalltoGraph(pos.x, pos.y, -1, null);
            //Debug.Log($"Ball below removed: x={pos.x}, y={pos.y}");
            AddPointsToScore(10);
            DecreaseCountOfBalls();
        }
    }

    public int[] GetFreeSlotsAroundBall(int row, int column)
    {
        Debug.Log("row: " + row + ", " + column);
        int[] mas = new int[3];

        if (column - 1 >= 0)
            mas[0] = graphList[row, column - 1];
        else
            mas[0] = -2;

        if (column + 1 < graphList.GetLength(1))
            mas[1] = graphList[row, column + 1];
        else
            mas[1] = -2;

        if (row + 1 < graphList.GetLength(0))
            mas[2] = graphList[row + 1, column];
        else
            mas[2] = -2;

        return mas;
    }

    public void SetBalltoGraph(int row, int column, int indexColor, Ball ball)
    {
        //Debug.Log($"SetBalltoGraph row {row} column {column} index {indexColor}");
        graphList[row, column] = indexColor;
        graphListBall[row, column] = ball;
        Debug.Log("");
        for (int i = 0; i < graphList.GetLength(0); i++)
        {
            string view = "";
            for (int j = 0; j < graphList.GetLength(1); j++)
            {
                view += graphList[i, j] + "\t";
            }
            Debug.Log(view);
        }
    }
}
