using UnityEngine;
using System.Collections;

public class TentacleProFanMovement : MonoBehaviour
{
    [Header("=== Movimiento BASE: Abanico ===")]
    public float minAngle = -45f;   // 下側の角度
    public float maxAngle = 45f;    // 上側の角度
    public float baseRotationSpeed = 2f;

    [Header("Movimiento suave tipo onda (optional)")]
    public bool useSineWave = true;
    public float sineMultiplier = 1f;

    [Header("Movimiento tipo martillazo (optional)")]
    public bool useHammerStrike = false;
    public float strikeDownSpeed = 6f;   // 下降時の速度
    public float strikeUpSpeed = 2f;     // 上昇時の速度
    public float strikeThreshold = 0.8f; // 攻撃動作を行うサイクルの部分

    [Header("Flash de aviso al atacar (optional)")]
    public bool useFlash = false;
    public SpriteRenderer spriteRenderer;
    public Color flashColor = Color.white;
    public float flashDuration = 0.1f;

    [Header("Delay aleatorio entre ciclos (optional)")]
    public bool useRandomDelay = false;
    public float minDelay = 0.1f;
    public float maxDelay = 0.5f;

    private float cycleValue = 0f;  // 0 → 1 → 0 cycle
    private bool forward = true;
    private bool inDelay = false;
    private float delayTimer = 0f;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        // Si est? en delay, espera
        if (inDelay)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0)
            {
                inDelay = false;
            }
            else
            {
                return;
            }
        }

        // --- 基本動作：扇形のスイング ---
        if (forward)
            cycleValue += baseRotationSpeed * Time.deltaTime;
        else
            cycleValue -= baseRotationSpeed * Time.deltaTime;

        // サイクルの限界値
        if (cycleValue >= 1f)
        {
            cycleValue = 1f;
            forward = false;
            TryRandomDelay();
        }
        else if (cycleValue <= 0f)
        {
            cycleValue = 0f;
            forward = true;
            TryRandomDelay();
        }

        // 基本角度の計算
        float angle = Mathf.Lerp(minAngle, maxAngle, cycleValue);

        // --- サイン波モーション（オプション）---
        if (useSineWave)
        {
            angle += Mathf.Sin(Time.time * baseRotationSpeed) * sineMultiplier;
        }

        // --- ハンマー叩きモーション（オプション）---
        if (useHammerStrike)
        {
            float speed = (cycleValue > strikeThreshold) ? strikeDownSpeed : strikeUpSpeed;
            cycleValue += (forward ? 1 : -1) * speed * Time.deltaTime;
        }

        // --- 攻撃フラッシュ（オプション）---
        if (useFlash && cycleValue > strikeThreshold && spriteRenderer != null)
        {
            StartCoroutine(Flash());
        }

        // 最終的な回転を適用する
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
    void TryRandomDelay()
    {
        if (!useRandomDelay) return;

        inDelay = true;
        delayTimer = Random.Range(minDelay, maxDelay);
    }

    IEnumerator Flash()
    {
        Color original = spriteRenderer.color;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = original;
    }
}