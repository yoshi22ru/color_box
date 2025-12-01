using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] prefabObjects;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float spawnInterval;
    [SerializeField] GameManager gameManager;
    [SerializeField] CountDown countDown;
    private bool shouldSpawn = false;
    private float timer;
    
    void Start()
    {
        // カウントダウンからのイベントを購読
        countDown.OnGameStart.AddListener(() => shouldSpawn = true);
    }

    void Update()
    {
        if (!shouldSpawn || countDown.isFinished) return; 

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPrefab();
            timer = 0;
        }
    }

    void SpawnPrefab()
    {
        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
        int randomSpawnNum = Random.Range(0, prefabObjects.Length);

        Transform randomSpawnPoint = spawnPoints[randomSpawnIndex];
        GameObject randomSpawnCube = prefabObjects[randomSpawnNum];

        GameObject spawnedObject = Instantiate(randomSpawnCube, randomSpawnPoint.position, randomSpawnPoint.rotation);
        MoveCube moveScript = spawnedObject.GetComponent<MoveCube>();
        
        int randomMaterialIndex = Random.Range(0, gameManager.cubeMaterials.Length);

        if (moveScript != null)
        {
            // --- 修正箇所：soundPlayを渡さない ---
            moveScript.Initialize(gameManager, randomMaterialIndex);
            moveScript.StartMove();
        }
        else
        {
            Debug.Log("生成されたプレハブにMoveCubeコンポーネントがアタッチされていません");
        }
    }
}