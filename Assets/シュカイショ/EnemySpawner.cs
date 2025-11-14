using System.Collections;
using UnityEngine;

/// <summary>
/// 可配置的敌人生成器（在视口外生成并可调节频率）
/// SpawnSide: Left / Right
/// 可设置固定间隔或随机间隔
/// 自动把方向传给带有 SetInitialDirection 的敵人脚本（使用反射）
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    public enum SpawnSide { Left, Right }

    [Header("Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Area (viewport 0..1)")]
    [SerializeField, Range(0f, 1f)] private float minViewportY = 0.1f;
    [SerializeField, Range(0f, 1f)] private float maxViewportY = 0.9f;
    [SerializeField, Tooltip("从视口外多少单位开始生成（以视口坐标为基准，通常 0.05-0.5 足够）")]
    private float viewportOffset = 0.05f;

    [Header("Spawn Timing")]
    [SerializeField] private bool useRandomInterval = false;
    [SerializeField, Min(0f)] private float interval = 2f;
    [SerializeField, Min(0f)] private float minInterval = 1f;
    [SerializeField, Min(0f)] private float maxInterval = 3f;

    [Header("Spawn Side")]
    [SerializeField] private SpawnSide spawnSide = SpawnSide.Right;

    [Header("Runtime")]
    [SerializeField, Tooltip("自动开始生成")]
    private bool spawnOnStart = true;

    private Coroutine spawnRoutine;

    void Start()
    {
        Debug.Log("EnemySpawner started.");
        SpawnOnce();
        //if (spawnOnStart) StartSpawning();
    }

    public void StartSpawning()
    {
        if (spawnRoutine == null) spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        //while (true)
        //{
            SpawnOnce();
            float wait = useRandomInterval ? Random.Range(minInterval, maxInterval) : interval;
            yield return new WaitForSeconds(wait);
        //}
    }

    private void SpawnOnce()
    {
        if (enemyPrefab == null) return;
        var cam = Camera.main;
        if (cam == null) return;

        Debug.Log("Spawning enemy...");

        float vx = (spawnSide == SpawnSide.Left) ? -viewportOffset : 1f + viewportOffset;
        float vy = Random.Range(minViewportY, maxViewportY);

        Vector3 vp = new Vector3(vx, vy, Mathf.Abs(cam.transform.position.z));
        Vector3 worldPos = cam.ViewportToWorldPoint(vp);
        worldPos.z = 0f;

        var go = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

        // 左側生成 -> 右移動 (dir=+1)、右側生成 -> 左移動 (dir=-1)
        float dir = (spawnSide == SpawnSide.Left) ? 1f : -1f;

        // 使用反射在任意组件上寻找并调用 SetInitialDirection(float)
        var comps = go.GetComponentsInChildren<MonoBehaviour>();
        foreach (var c in comps)
        {
            var method = c.GetType().GetMethod("SetInitialDirection", new System.Type[] { typeof(float) });
            if (method != null)
            {
                method.Invoke(c, new object[] { dir });
                break;
            }
        }

        var viewportPos = cam.WorldToViewportPoint(transform.position);
        if (viewportPos.z > 0f && (viewportPos.x < 0f || viewportPos.x > 1f || viewportPos.y < 0f || viewportPos.y > 1f))
        {
            Destroy(gameObject);
        }
    }
}