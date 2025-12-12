using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawnerKaisho : MonoBehaviour
{
    public enum SpawnSideMode
    {
        Left,
        Right,
        Random
    }

    [Header("Enemy Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Side Mode")]
    [SerializeField] private SpawnSideMode sideMode = SpawnSideMode.Right;

    [Header("Vertical Range (Viewport) 0..1")]
    [SerializeField, Range(0f, 1f)] private float minViewportY = 0.1f;
    [SerializeField, Range(0f, 1f)] private float maxViewportY = 0.9f;

    [Header("Horizontal Offset (Viewport)")]
    [SerializeField, Tooltip("画面外に出すためのX方向オフセット(例:0.05)")]
    private float horizontalViewportOffset = 0.05f;

    [Header("Timing")]
    [SerializeField, Tooltip("ランダム間隔を使うか")]
    private bool useRandomInterval = false;
    [SerializeField, Min(0.01f), Tooltip("固定生成間隔(秒)")]
    private float interval = 2f;
    [SerializeField, Min(0.01f), Tooltip("最小ランダム間隔(秒)")]
    private float minInterval = 1f;
    [SerializeField, Min(0.01f), Tooltip("最大ランダム間隔(秒)")]
    private float maxInterval = 3f;

    [Header("Start Delay")]
    [SerializeField, Tooltip("游戏开始后延迟多少秒再开始生成")]
    private float startDelay = 0f;

    [Header("Auto Start")]
    [SerializeField, Tooltip("Start时に自動で生成ループ開始")]
    private bool autoStart = true;

    private Coroutine loop;

    void OnValidate()
    {
        if (maxViewportY < minViewportY) maxViewportY = minViewportY;
        if (useRandomInterval && maxInterval < minInterval) maxInterval = minInterval;
        horizontalViewportOffset = Mathf.Max(0f, horizontalViewportOffset);
        startDelay = Mathf.Max(0f, startDelay);
    }

    void Start()
    {
        if (autoStart) StartSpawning();
    }

    public void StartSpawning()
    {
        if (loop == null) loop = StartCoroutine(SpawnLoopWithDelay());
    }

    public void StopSpawning()
    {
        if (loop != null)
        {
            StopCoroutine(loop);
            loop = null;
        }
    }

    public void SpawnOnce()
    {
        DoSpawn();
    }

    private IEnumerator SpawnLoopWithDelay()
    {
        // 开局延迟
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        // 进入常规生成循环
        while (true)
        {
            DoSpawn();
            float wait = useRandomInterval
                ? Random.Range(minInterval, maxInterval)
                : interval;
            yield return new WaitForSeconds(wait);
        }
    }

    private void DoSpawn()
    {
        if (enemyPrefab == null) return;
        var cam = Camera.main;
        if (cam == null) return;

        // 选择生成侧并决定方向
        int sideDir = 0;
        switch (sideMode)   
        {
            case SpawnSideMode.Left: sideDir = 1; break;   // 左外→向右
            case SpawnSideMode.Right: sideDir = -1; break; // 右外→向左
            case SpawnSideMode.Random: sideDir = (Random.value < 0.5f) ? 1 : -1; break;
        }

        float vx = sideDir == 1
            ? -horizontalViewportOffset
            : 1f + horizontalViewportOffset;

        float vy = Random.Range(minViewportY, maxViewportY);

        Vector3 vp = new Vector3(vx, vy, Mathf.Abs(cam.transform.position.z));
        Vector3 worldPos = cam.ViewportToWorldPoint(vp);
        worldPos.z = 0f;

        var go = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

        // 传递方向（如果敌人支持 SetInitialDirection）
        var octo = go.GetComponent<tonderutekiタコ>();
        if (octo != null)
        {
            octo.SetInitialDirection(sideDir);
        }
        // 其他类型可根据需要扩展
    }
}