using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector2 movement;

    public int score = 0;

    private int eyesDirection = 1;

    [Header("References")]
    [SerializeField]
    private Transform eyes = null;
    [SerializeField]
    private Rigidbody2D rb = null;
    [SerializeField]
    private Text scoreText = null;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 180f;
    [SerializeField]
    private float snapThreshold = 0.3f;

    [Header("Eyes Offset")]
    [SerializeField]
    private Vector2 eyesRight = new Vector2(1.2f, 1f);
    [SerializeField]
    private Vector2 eyesLeft = new Vector2(0.8f, 1f);
    [SerializeField]
    private Vector2 eyesUp = new Vector2(1f, 1.1f);
    [SerializeField]
    private Vector2 eyesDown = new Vector2(1f, 0.8f);

    [Header("Immortality")]
    [SerializeField]
    private float blinkDuration = 3f;
    [SerializeField]
    private float blinkInterval = 0.05f;
    [SerializeField]
    private float teleportDelay = 0.3f;

    private WallState[,] maze;
    private bool immortal = false;
    private bool killed = false;
    private SpriteRenderer spriteRenderer;

    public void init(WallState[,] m)
    {
        maze = m;
        setRandomPosition();
        StartCoroutine(Blink(blinkDuration));
    }

    void Awake()
    {
        if (scoreText == null)
        {
            Debug.LogError("Score Text is not assigned in the inspector!", this);
        }
        spriteRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
             Debug.LogError("SpriteRenderer not found on the first child object!", this);
        }
    }

    void setRandomPosition()
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        var pos = new Vector2(Mathf.Floor(Random.value * width), Mathf.Floor(Random.value * height));
        var position = new Vector3(-width / 2 + pos.x, -height / 2 + pos.y, 0);
        transform.position = position;
    }

    void Update()
    {
        var m = new Vector2();
        m.x = Input.GetAxisRaw("Horizontal");
        m.y = Input.GetAxisRaw("Vertical");
        if (m.x != movement.x)
        {
            movement.x = m.x;
            movement.y = 0;
            if (Mathf.Abs(transform.position.y - Mathf.Round(transform.position.y)) < snapThreshold)
            {
                transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), transform.position.z);
            }
        }
        if (m.y != movement.y)
        {
            movement.y = m.y;
            movement.x = 0;
            if (Mathf.Abs(transform.position.x - Mathf.Round(transform.position.x)) < snapThreshold)
            {
                transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, transform.position.z);
            }
        }
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

    void FixedUpdate()
    {
        if (!killed)
        {
            rb.velocity = movement * moveSpeed * Time.deltaTime;
        }
    }
    public void Kill()
    {
        if (!immortal)
        {
            killed = true;
            movement = Vector2.zero;
            score = 0;
            if (scoreText != null) scoreText.text = score.ToString();
            StartCoroutine(Blink(blinkDuration));
            StartCoroutine(Teleport(teleportDelay));
        }
    }

    public void AddScore()
    {
        score += 1;
        if (scoreText != null) scoreText.text = score.ToString();
    }
    IEnumerator Blink(float waitTime)
    {
        immortal = true;
        var endTime = Time.time + waitTime;
        if (spriteRenderer != null)
        {
             while (Time.time < endTime)
            {
                spriteRenderer.enabled = false;
                yield return new WaitForSeconds(blinkInterval);
                spriteRenderer.enabled = true;
                yield return new WaitForSeconds(blinkInterval);
            }
             spriteRenderer.enabled = true;
        }
        immortal = false;
    }
    IEnumerator Teleport(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        setRandomPosition();
        killed = false;
    }

}

