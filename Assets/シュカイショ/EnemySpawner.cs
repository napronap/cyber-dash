using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : MonoBehaviour
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

    [Header("Auto Start")]
    [SerializeField, Tooltip("Start時に自動で生成ループ開始")]
    private bool autoStart = true;

    private Coroutine loop;

    void OnValidate()
    {
        if (maxViewportY < minViewportY) maxViewportY = minViewportY;
        if (useRandomInterval && maxInterval < minInterval) maxInterval = minInterval;
        horizontalViewportOffset = Mathf.Max(0f, horizontalViewportOffset);
    }

    void Start()
    {
        if (autoStart) StartSpawning();
    }

    public void StartSpawning()
    {
        if (loop == null) loop = StartCoroutine(SpawnLoop());
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

    private IEnumerator SpawnLoop()
    {
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

        // 生成側決定
        int sideDir = 0;
        switch (sideMode)
        {
            case SpawnSideMode.Left: sideDir = 1; break;   // 左から右へ
            case SpawnSideMode.Right: sideDir = -1; break; // 右から左へ
            case SpawnSideMode.Random: sideDir = (Random.value < 0.5f) ? 1 : -1; break;
        }

        // Viewport X (外側)
        float vx = sideDir == 1
            ? -horizontalViewportOffset                 // 左側外
            : 1f + horizontalViewportOffset;            // 右側外

        // 垂直位置
        float vy = Random.Range(minViewportY, maxViewportY);

        // Viewport -> World
        Vector3 vp = new Vector3(vx, vy, Mathf.Abs(cam.transform.position.z));
        Vector3 worldPos = cam.ViewportToWorldPoint(vp);
        worldPos.z = 0f;

        var go = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

        // 方向を渡す (tonderutekiタコ に限る簡易処理)
        var octo = go.GetComponent<tonderutekiタコ>();
        if (octo != null)
        {
            octo.SetInitialDirection(sideDir);
        }
        else
        {
            // 他の敵にも対応したい場合は共通IF実装か基底クラス拡張を検討
        }
    }
}