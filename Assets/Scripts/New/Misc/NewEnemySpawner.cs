using UnityEngine;
using System.Collections;

public class NewEnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawn Rules")]
    [SerializeField] private int maxAlive = 3;
    [SerializeField] private float spawnInterval = 1.0f;

    [Header("Spawn Position")]
    [SerializeField] private float spawnXOffset = 1.5f;
    [SerializeField] private float spawnY = -3.5f;

    private int aliveCount = 0;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (enemyPrefabs != null && enemyPrefabs.Length > 0 && aliveCount < maxAlive)
            {
                SpawnOne();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOne()
    {
        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        if (prefab == null) return;

        float x = GetRightEdgeWorldX() + spawnXOffset;
        Vector3 pos = new Vector3(x, spawnY, 0f);

        GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
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
