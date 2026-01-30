using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private PlayerHealth player;

    [Header("UI")]
    [SerializeField] private Transform heartsRoot;
    [SerializeField] private GameObject heartPrefab;

    private readonly List<GameObject> _hearts = new List<GameObject>();

    private void OnEnable()
    {
        if (player != null)
        {
            player.OnHealthChanged.AddListener(OnHealthChanged);
        }
    }

    private void Start()
    {
        if (player != null)
        {
            Rebuild(player.currentHP, player.maxHP);
        }
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.OnHealthChanged.RemoveListener(OnHealthChanged);
        }
    }

    private void OnHealthChanged(int current, int max)
    {
        Rebuild(current, max);
    }

    private void Rebuild(int current, int max)
    {
        if (heartsRoot == null || heartPrefab == null) return;

        // 現在HPに一致する数だけ表示（足りなければ生成、余れば末尾から破棄）
        while (_hearts.Count > current)
        {
            var last = _hearts[_hearts.Count - 1];
            _hearts.RemoveAt(_hearts.Count - 1);
            if (last != null) Destroy(last);
        }

        while (_hearts.Count < current)
        {
            var go = Instantiate(heartPrefab, heartsRoot);
            _hearts.Add(go);
        }
    }
}