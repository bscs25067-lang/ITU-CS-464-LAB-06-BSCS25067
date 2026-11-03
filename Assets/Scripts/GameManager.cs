using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform player;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI bonusScore;
    [SerializeField] private GameObject bonusScorePopup;
    [SerializeField] private GameObject winUI;
    [SerializeField] private ParticleSystem collectFX;
    [SerializeField] private int targetScore;

    [Header("Win UI")]
    [SerializeField] private float winPopDuration = 0.5f;
    [SerializeField] private float winFadeDuration = 0.08f;
    [SerializeField] private float winOvershoot = 3f;

    private CanvasGroup _winGroup;
    private bool _hasWon;

    [Header("Bonus Popup")]
    [SerializeField] private float popInDuration = 0.15f;
    [SerializeField] private float popupHoldTime = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float popScale = 1.2f;

    private int _currScore;
    private int _bonusCoins;
    private Spawner _spawner;
    private CanvasGroup _popupGroup;
    private Coroutine _popupRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _spawner = GetComponentInChildren<Spawner>();

        _popupGroup = bonusScorePopup.GetComponent<CanvasGroup>();
        if (_popupGroup == null)
            _popupGroup = bonusScorePopup.AddComponent<CanvasGroup>();

        _winGroup = winUI.GetComponent<CanvasGroup>();
        if (_winGroup == null)
            _winGroup = winUI.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        UpdateHUD();
        winUI.SetActive(false);
        bonusScorePopup.SetActive(false);
    }

    private void Update() => FollowPlayer();

    private void UpdateHUD()
    {
        currentScore.text = $"Score: {_currScore} / {targetScore}";
    }

    private void ShowPopup()
    {
        bonusScore.text = $"Bonus Coins Collected: {_bonusCoins} / 50";

        if (_popupRoutine != null) StopCoroutine(_popupRoutine);
        _popupRoutine = StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        Transform t = bonusScorePopup.transform;
        bonusScorePopup.SetActive(true);
        _popupGroup.alpha = 1f;

        float elapsed = 0f;
        while (elapsed < popInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(elapsed / popInDuration);
            t.localScale = Vector3.one * Mathf.Lerp(0.6f, popScale, EaseOut(p));
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < popInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(elapsed / popInDuration);
            t.localScale = Vector3.one * Mathf.Lerp(popScale, 1f, p);
            yield return null;
        }
        t.localScale = Vector3.one;

        yield return new WaitForSecondsRealtime(popupHoldTime);

        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _popupGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeOutDuration);
            yield return null;
        }

        _popupGroup.alpha = 0f;
        bonusScorePopup.SetActive(false);
        _popupRoutine = null;
    }

    private static float EaseOut(float x) => 1f - (1f - x) * (1f - x);

    private void FollowPlayer()
    {
        collectFX.transform.position = player.position;
    }

    public void AddScore(int amount = 1)
    {
        _currScore += amount;
        collectFX.Play();
        UpdateHUD();

        if (_currScore >= targetScore) ShowWinUI();
    }

    public void AddBonusScore(int amount = 1)
    {
        _bonusCoins += amount;
        collectFX.Play();
        ShowPopup();
    }

    public void SpawnBlueCoins() => _spawner.SpawnBlueCoins(50);

    private void ShowWinUI()
    {
        if (_hasWon) return;
        _hasWon = true;

        Time.timeScale = 0f;
        StartCoroutine(WinPopRoutine());
    }

    private IEnumerator WinPopRoutine()
    {
        Transform t = winUI.transform;
        winUI.SetActive(true);
        _winGroup.alpha = 0f;
        t.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < winPopDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(elapsed / winPopDuration);

            _winGroup.alpha = Mathf.Clamp01(elapsed / winFadeDuration);
            t.localScale = Vector3.one * EaseOutBack(p, winOvershoot);
            yield return null;
        }

        _winGroup.alpha = 1f;
        t.localScale = Vector3.one;
    }

    private static float EaseOutBack(float x, float overshoot = 1.70158f)
    {
        float c3 = overshoot + 1f;
        float t = x - 1f;
        return 1f + c3 * t * t * t + overshoot * t * t;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}