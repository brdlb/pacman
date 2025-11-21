using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Vector2 movement = Vector2.zero;
    [Header("Movement")][SerializeField]
    [Range(0, 500)]
    private float speed = 100f;

    [Header("References")][SerializeField]
    private Transform directionPrefab = null;

    [Header("Eyes Offset")][SerializeField]
    private Vector2 eyesRight = new Vector2(1.2f, 1f);
    [SerializeField]
    private Vector2 eyesLeft = new Vector2(0.7f, 1f);
    [SerializeField]
    private Vector2 eyesUp = new Vector2(1f, 1.1f);
    [SerializeField]
    private Vector2 eyesDown = new Vector2(1f, 0.8f);

    [Header("AI")][SerializeField]
    private float arrivalThreshold = 0.1f;
    [SerializeField]
    private float killDistanceThreshold = 0.3f;

    public int score = 0;

    [SerializeField]
    private Transform eyes = null;

    [SerializeField]
    private Rigidbody2D rb = null;
    private Vector3 mazePosition;
    private WallState[,] maze;
    public float[,] mazeHeat;

    private int eyesDirection = -1;

    public void Init(WallState[,] m)
    {
        maze = m;
        mazeHeat = new float[maze.GetLength(0), maze.GetLength(1)];
        GetRoute();
    }

    private Vector2 ChooseDirection(List<Vector2> possibleDirections)
    {
        if (possibleDirections.Count == 0)
        {
            return Vector2.zero;
        }

        if (possibleDirections.Count == 1)
        {
             return possibleDirections[0];
        }

        Vector2 invertedMovement = -movement;
        List<Vector2> preferredDirections = new List<Vector2>();
        foreach (var dir in possibleDirections)
        {
             if (dir != invertedMovement)
             {
                 preferredDirections.Add(dir);
             }
        }

        if (preferredDirections.Count > 0)
        {
            int choiceIndex = Random.Range(0, preferredDirections.Count);
            return preferredDirections[choiceIndex];
        }
        else
        {
            int choiceIndex = Random.Range(0, possibleDirections.Count);
            return possibleDirections[choiceIndex];
        }
    }

    public void GetRoute()
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        Vector3 position = GetMazePosition(transform.position);
        mazePosition = position;
        WallState state = maze[(int)position.x, (int)position.y];
        var possibleDirections = new List<Vector2>();
        Vector2 oldMovement = movement;

        if (!state.HasFlag(WallState.LEFT))
        {
            possibleDirections.Add(new Vector2(-1, 0));
        }
        if (!state.HasFlag(WallState.UP))
        {
            possibleDirections.Add(new Vector2(0, 1));
        }
        if (!state.HasFlag(WallState.RIGHT))
        {
            possibleDirections.Add(new Vector2(1, 0));
        }
        if (!state.HasFlag(WallState.DOWN))
        {
            possibleDirections.Add(new Vector2(0, -1));
        }

        movement = ChooseDirection(possibleDirections);

        if (oldMovement != movement)
        {
            transform.position = GetRenderPosition(position);
            if (movement.x > 0 && eyesDirection != 1)
            {
                eyesDirection = 1;
                eyes.localPosition = eyesRight;
            }
            if (movement.x < 0 && eyesDirection != 0)
            {
                eyesDirection = 0;
                eyes.localPosition = eyesLeft;
            }
            if (movement.y > 0 && eyesDirection != 2)
            {
                eyesDirection = 2;
                eyes.localPosition = eyesUp;
            }
            if (movement.y < 0 && eyesDirection != 3)
            {
                eyesDirection = 3;
                eyes.localPosition = eyesDown;
            }
        }
    }

    private Vector3 GetMazePosition(Vector3 worldPosition)
    {
        if (maze != null)
        {
            int width = maze.GetLength(0);
            int height = maze.GetLength(1);
            return new Vector3(Mathf.Round(worldPosition.x + width / 2.0f), Mathf.Round(worldPosition.y + height / 2.0f), 0);
        }
        return Vector3.zero;
    }

    private Vector3 GetRenderPosition(Vector3 mazePos)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        return new Vector3(-width / 2.0f + mazePos.x, -height / 2.0f + mazePos.y, 0);
    }

    void Update()
    {
        var mp = GetMazePosition(transform.position);
        var rp = GetRenderPosition(mp);
        var delta = transform.position - rp;
        if (delta.magnitude < arrivalThreshold && (mazePosition.x != mp.x || mazePosition.y != mp.y))
        {
            GetRoute();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = movement * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            //movement = new Vector2(0, 0);
            //StartCoroutine(wait());
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var delta = other.transform.position - transform.position;
            if (delta.magnitude < killDistanceThreshold)
            {
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.Kill();
                }
            }
        }
    }
}
