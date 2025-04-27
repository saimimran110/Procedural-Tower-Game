using UnityEngine;

public class BlockCollision : MonoBehaviour
{
    private bool canBeDestroyed = false;
    public static int blockCount = 0;
    public static int fallenBlocks = 0;
    public static int simultaneousFallenBlocks = 0;
    public static bool isGameOver = false;

    void Start()
    {
        blockCount++;

        if (blockCount > 1)
        {
            Invoke("AllowDestroy", 1f);
        }

     
    }

    void AllowDestroy()
    {
        canBeDestroyed = true;
    }
    void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Ground"))
    {
        

        if (canBeDestroyed)
        {
            fallenBlocks++;
            BlockSpawner.score--;
            // BlockSpawner.UpdateScoreUI();
            //  BlockSpawner spawner = FindObjectOfType<BlockSpawner>(); // Find the BlockSpawner in the scene
            // spawner.UpdateScoreUI();
            simultaneousFallenBlocks++;

            if (simultaneousFallenBlocks >= 3)
            {
                Debug.Log("Game Over: 3 blocks fell at the same time!");
                isGameOver = true;
                ResetFallenBlocks();
            }
            else if (fallenBlocks >= 3)
            {
                Debug.Log("Game Over: 3 blocks fell one by one!");
                isGameOver = true;
                ResetFallenBlocks();
            }

            Destroy(gameObject);
        }
    }
}

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            simultaneousFallenBlocks = 0;
        }
    }

    private void ResetFallenBlocks()
    {
        fallenBlocks = 0;
        simultaneousFallenBlocks = 0;
    }

    public static void ResetCounters()
    {
        blockCount = 0;
        fallenBlocks = 0;
        simultaneousFallenBlocks = 0;
        isGameOver = false;
    }

   
}
