using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int max = 3;
    [SerializeField] float iFrames = 1f;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float flashInterval = 0.1f;

    [Header("Shield Settings")]
    [SerializeField] private ParticleSystem shieldVisual;
    [SerializeField] private ParticleSystem shieldBreakEffect;

    public bool shielded;

    private int _current;
    private float _invulnerableUntil;
    private Coroutine _flashRoutine;

    private Material _material;
    private Color _originalColor;

    public int Current => _current;
    public int Max => max;

    void Awake()
    {
        _current = max;

        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        if (meshRenderer != null)
        {
            _material = meshRenderer.material;
            _originalColor = _material.color;
        }

        shieldVisual.Stop();
    }

    public void SetShield(bool active)
    {
        shielded = active;

        if (active) shieldVisual.Play();
        else
        {
            shieldVisual.Stop();
            shieldBreakEffect.Play();
        }
    }

    public bool TakeDamage(int amount)
    {
        if (_current <= 0 || Time.time < _invulnerableUntil) return false;
        _invulnerableUntil = Time.time + iFrames;

        if (shielded)
        {
            SetShield(false);
            return false;
        }

        _current -= amount;

        if (meshRenderer != null && _material != null)
        {
            if (_flashRoutine != null) StopCoroutine(_flashRoutine);
            _flashRoutine = StartCoroutine(WhiteBlinkEffect());
        }

        GameManager.Instance.UpdateUI();
        if (_current <= 0) GameManager.Instance.PlayerDied();

        return true;
    }

    public void Restore(float protection)
    {
        _current = max;
        _invulnerableUntil = Time.time + protection;

        if (_flashRoutine != null) StopCoroutine(_flashRoutine);

        if (meshRenderer != null)
            meshRenderer.enabled = true;

        if (_material != null)
            _material.color = _originalColor;

        GameManager.Instance.UpdateUI();
    }

    private IEnumerator WhiteBlinkEffect()
    {
        _material.color = Color.white;

        float elapsedTime = 0f;

        while (elapsedTime < iFrames)
        {
            meshRenderer.enabled = false;
            yield return new WaitForSeconds(flashInterval);
            meshRenderer.enabled = true;
            yield return new WaitForSeconds(flashInterval);

            elapsedTime += flashInterval * 2f;
        }

        meshRenderer.enabled = true;
        _material.color = _originalColor;
    }
}