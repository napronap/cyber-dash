using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private string prefix = "SCORE: ";

    private void Awake()
    {
        if (text == null) text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (ScoreManager.Instance == null) return;
        text.text = prefix + ScoreManager.Instance.Score.ToString();
    }
}
