using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [Header("Maze Dimensions")][SerializeField]
    [Range(1, 100)]
    private int width = 10;
    [SerializeField]
    [Range(1, 100)]
    private int height = 10;

    [Header("Maze Generation")][SerializeField]
    [Range(0, 5000)]
    private int seed = 0;

    [SerializeField]
    [Range(0, 100)]
    private int fullness = 100;

    [Header("Prefabs")][SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    [SerializeField]
    private Transform playerPrefab = null;

    [SerializeField]
    private Transform coinPrefab = null;

    [SerializeField]
    private Transform enemyPrefab = null;

    [Header("Object Counts")][SerializeField]
    [Range(1, 1000)]
    private int coinsCount = 10;

    [SerializeField]
    [Range(1, 400)]
    private int enemyCount = 3;

    private const float CellSize = 1f;
    private const float CoinOffset = 0.5f;
    private const float InitialCameraZ = -40f;

    void Start()
    {
        if (wallPrefab == null || playerPrefab == null || coinPrefab == null || enemyPrefab == null)
        {
             Debug.LogError("One or more prefabs are not assigned in the inspector!", this);
             return;
        }

        var maze = MazeGenerator.Generate(width, height, seed, fullness);
        DrawMaze(maze);
        PlaceCoins();
        PlaceEnemies(maze);

        var player = Instantiate(playerPrefab, transform);
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.init(maze);
        }
        else
        {
            Debug.LogError("PlayerController component not found on player prefab!", player);
        }

        var cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController != null)
        {
            cameraController.SetTarget(player);
            if (cameraController.enabled)
            {
                var pos = new Vector3(player.position.x, player.position.y, InitialCameraZ);
                Camera.main.transform.position = pos;
            }
        }
         else
        {
            Debug.LogError("CameraController component not found on Main Camera!", Camera.main);
        }
    }

    private Transform[] coins;
    public void SetCoinRandomPosition(Transform coin)
    {
        coin.tag = "Untagged";
        Vector2 randomPosition = new Vector2(Random.Range(0, width), Random.Range(0, height));
        Vector3 position = new Vector3(-width / 2.0f + randomPosition.x + CoinOffset, -height / 2.0f + randomPosition.y + CoinOffset, 0);
        coin.position = position;

        var existingCoins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (var existingCoin in existingCoins)
        {
             if (existingCoin != coin.gameObject && Vector3.Distance(existingCoin.transform.position, coin.position) < 0.1f)
             {
                 SetCoinRandomPosition(coin);
                 return;
             }
        }
        coin.tag = "Coin";
    }

    private void PlaceEnemies(WallState[,] maze)
    {
        for (int i = 0; i < enemyCount; ++i)
        {
            var enemy = Instantiate(enemyPrefab, transform);
            Vector2 randomPosition = new Vector2(Random.Range(0, width), Random.Range(0, height));
            Vector3 position = new Vector3(-width / 2.0f + randomPosition.x, -height / 2.0f + randomPosition.y, 0);
            enemy.position = position;

            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                 enemyController.Init(maze);
            }
            else
            {
                 Debug.LogError("EnemyController component not found on enemy prefab!", enemy);
            }
        }
    }
    private void PlaceCoins()
    {
        for (int i = 0; i < coinsCount; ++i)
        {
            var coin = Instantiate(coinPrefab, transform);
            SetCoinRandomPosition(coin);
        }
    }

    private void PlacePlayer(Vector2 position)
    {
        var player = Instantiate(playerPrefab, transform);
        var pos = new Vector3(-width / 2.0f + position.x + CellSize / 2.0f, -height / 2.0f + position.y + CellSize / 2.0f, 0);
        player.position = pos;

        var cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController != null)
        {
             cameraController.SetTarget(player);
        }
         else
        {
            Debug.LogError("CameraController component not found on Main Camera!", Camera.main);
        }
    }

    private void DrawMaze(WallState[,] maze)
    {
        float halfWidth = width / 2.0f;
        float halfHeight = height / 2.0f;

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-halfWidth + i, -halfHeight + j, 0);

                if (cell.HasFlag(WallState.UP))
                {
                    var topWall = Instantiate(wallPrefab, transform);
                    topWall.position = position + new Vector3(CellSize / 2, CellSize, 0);
                    topWall.localScale = new Vector3(CellSize, topWall.localScale.y, topWall.localScale.z);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, transform);
                    leftWall.position = position + new Vector3(0, CellSize / 2, 0);
                    leftWall.eulerAngles = new Vector3(0, 0, 90);
                    leftWall.localScale = new Vector3(CellSize, leftWall.localScale.y, leftWall.localScale.z);
                }

                if (i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, transform);
                        rightWall.position = position + new Vector3(CellSize, CellSize / 2, 0);
                        rightWall.eulerAngles = new Vector3(0, 0, 90);
                        rightWall.localScale = new Vector3(CellSize, rightWall.localScale.y, rightWall.localScale.z);
                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab, transform);
                        bottomWall.position = position + new Vector3(CellSize / 2, 0, 0);
                        bottomWall.localScale = new Vector3(CellSize, bottomWall.localScale.y, bottomWall.localScale.z);
                    }
                }
            }
        }
    }
}
