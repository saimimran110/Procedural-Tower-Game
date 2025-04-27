using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockPrefab;
    public Transform stackTracker;
    public Material[] blockMaterials; // drag and drop in Inspector


    public  Text scoreText;
    public Text gameOverText;
    public Button restartButton;

    private GameObject currentBlock;
    private bool isDropping = false;
    private float spawnHeightOffset = 2f;
    private float maxTiltAngle = 30f;
    private bool hasPlayedSound = false;

    public AudioClip placeSound; // Sound for placing blocks
    public AudioClip gameOverSound; // Sound for Game Over
    private AudioSource audioSource;
    public static int score = 0;

    void Start()

    
    {
        Camera.main.backgroundColor = new Color(0.5f, 0.8f, 1f); // Soft sky blue

        restartButton.onClick.AddListener(RestartGame);

        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        SpawnNewBlock();
    }

    void Update()
    {
        if (BlockCollision.isGameOver)
        {
            ShowGameOverUI();
            return;
        }

        if (currentBlock != null && !isDropping)
        {
            float move = Mathf.PingPong(Time.time * 3, 3) - 1.5f;
            currentBlock.transform.position = new Vector3(move, currentBlock.transform.position.y, 0);
        }
// Handle touch input for dropping the block
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began && !isDropping)
        {
            DropBlock();
        }
    }
        if (Mathf.Abs(stackTracker.eulerAngles.z) > maxTiltAngle && Mathf.Abs(stackTracker.eulerAngles.z) < 360 - maxTiltAngle)
        {
            CollapseTower();
        }
    }

   void SpawnNewBlock()
{
    Vector3 spawnPos = new Vector3(0, stackTracker.position.y + spawnHeightOffset, 0);
    currentBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);

    Rigidbody rb = currentBlock.GetComponent<Rigidbody>();
    rb.isKinematic = true;
    rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ
                   | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    if (currentBlock.GetComponent<BlockCollision>() == null)
    {
        currentBlock.AddComponent<BlockCollision>();
    }

    // 🌟 Randomly apply material here
    if (blockMaterials.Length > 0)
    {
        int randomIndex = Random.Range(0, blockMaterials.Length);
        Renderer renderer = currentBlock.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = blockMaterials[randomIndex];
        }
    }
}


    void DropBlock()
    {
        Rigidbody rb = currentBlock.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        isDropping = true;
        StartCoroutine(WaitForLanding());
    }

    IEnumerator WaitForLanding()
    {
        yield return new WaitForSeconds(1.2f);

        if (currentBlock != null)
        {
            float blockTop = currentBlock.transform.position.y + 0.5f;
            if (blockTop > stackTracker.position.y)
            {
                stackTracker.position = new Vector3(0, blockTop, 0);
            }

            isDropping = false;
            SpawnNewBlock();

            score++;
            UpdateScoreUI();
        }
    }

    void CollapseTower()
    {
        Debug.Log("Tower collapsed!");
        GameObject[] allBlocks = GameObject.FindGameObjectsWithTag("Block");

        foreach (GameObject block in allBlocks)
        {
            Rigidbody rb = block.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.None;
            }
        }

        BlockCollision.isGameOver = true; // Important
    }

    public void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    void ShowGameOverUI()
    {
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);

        // Play Game Over sound
        if (gameOverSound != null && audioSource != null)
        {
            audioSource.clip = gameOverSound;
            audioSource.Play();
        }
    }

    public void RestartGame()
    {
        // Reset static variables
        BlockCollision.ResetCounters();

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    // Detect collision with ground and play the sound
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!hasPlayedSound && placeSound != null && audioSource != null)
            {
                audioSource.clip = placeSound;
                audioSource.Play();
                hasPlayedSound = true; // Ensure sound is played only once
            }
        }
    }

    // Reset sound played state when starting a new block
    void OnBlockSpawn()
    {
        hasPlayedSound = false;
    }
}