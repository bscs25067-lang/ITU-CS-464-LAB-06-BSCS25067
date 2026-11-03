using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public enum CoinType
    {
        Default,
        Blue,
        Red
    }

    public CoinType Type => coinType;

    [SerializeField] private CoinType coinType = CoinType.Default;
    [SerializeField] private int value = 1;
    [SerializeField] private float rotSpeed = 2;

    [Header("Pop In")]
    [SerializeField] private float popDuration = 0.35f;
    [SerializeField] private float maxStartDelay = 0.2f;

    private Vector3 _targetScale;
    private Collider _collider;
    private bool _cached;

    private void Awake()
    {
        _targetScale = transform.localScale;
        _collider = GetComponent<Collider>();
        _cached = true;
    }

    private void OnEnable()
    {
        if (!_cached) return;
        StartCoroutine(PopIn());
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
    }

    private IEnumerator PopIn()
    {
        transform.localScale = Vector3.zero;
        if (_collider != null) _collider.enabled = false;

        float delay = Random.Range(0f, maxStartDelay);
        if (delay > 0f) yield return new WaitForSeconds(delay);

        if (_collider != null) _collider.enabled = true;

        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / popDuration);
            transform.localScale = _targetScale * EaseOutBack(p);
            yield return null;
        }

        transform.localScale = _targetScale;
    }

    private static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        float t = x - 1f;
        return 1f + c3 * t * t * t + c1 * t * t;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (coinType)
        {
            case CoinType.Default:
                GameManager.Instance.AddScore(value);
                break;

            case CoinType.Red:
                GameManager.Instance.SpawnBlueCoins();
                break;

            case CoinType.Blue:
                GameManager.Instance.AddBonusScore(value);
                break;
        }

        Destroy(gameObject);
    }
}