using UnityEngine;
using System.Collections;

public class NewEnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct EnemySpawnData
    {
        public GameObject prefab;
        public float minY;
        public float maxY;
    }

    [Header("Enemies")]
    [SerializeField] private EnemySpawnData[] enemies;

    [Header("Spawn Rules")]
    [SerializeField] private int maxAlive = 3;
    [SerializeField] private float spawnInterval = 1.0f;

    [Header("Spawn Position")]
    [SerializeField] private float spawnXOffset = 1.5f;

    private int aliveCount = 0;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (enemies != null && enemies.Length > 0 && aliveCount < maxAlive)
            {
                SpawnOne();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOne()
    {
        var data = enemies[Random.Range(0, enemies.Length)];
        if (data.prefab == null) return;

        float x = GetRightEdgeWorldX() + spawnXOffset;
        float y = Random.Range(data.minY, data.maxY);

        Vector3 pos = new Vector3(x, y, 0f);

        GameObject enemy = Instantiate(data.prefab, pos, Quaternion.identity);
        aliveCount++;

        enemy.AddComponent<AutoDespawnNotify>().Init(this);
    }

    private float GetRightEdgeWorldX()
    {
        var cam = Camera.main;
        if (cam == null) return transform.position.x;

        Vector3 rightEdge = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
        return rightEdge.x;
    }

    public void NotifyEnemyDestroyed()
    {
        aliveCount = Mathf.Max(0, aliveCount - 1);
    }

    private class AutoDespawnNotify : MonoBehaviour
    {
        private NewEnemySpawner spawner;

        public void Init(NewEnemySpawner s) => spawner = s;

        private void OnDestroy()
        {
            if (spawner != null)
                spawner.NotifyEnemyDestroyed();
        }
    }
}
