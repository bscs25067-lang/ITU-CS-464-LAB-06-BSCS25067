using System.Collections;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 6f;
    [SerializeField] float restTime = 1f, warnTime = 0.7f, strikeTime = 0.4f;
    [SerializeField] GameObject warning;
    [SerializeField] GameObject active;
    [SerializeField] private ParticleSystem fx;

    [Header("Stomp Animation Settings")]
    [SerializeField] private Transform stomper;
    [SerializeField] private float dropDistance = 4f;
    [SerializeField] private float recoverTime = 1f;
    [SerializeField] private AnimationCurve stompEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector3 _startLocalPos;
    private Vector3 _slammedLocalPos;

    void Awake()
    {
        if (stomper == null)
            stomper = transform;

        _startLocalPos = stomper.localPosition;
        _slammedLocalPos = _startLocalPos - new Vector3(0f, dropDistance, 0f);

        if (stompEase.length <= 2)
        {
            stompEase = new AnimationCurve(
                new Keyframe(0f, 0f, 0f, 0f),
                new Keyframe(1f, 1f, 3f, 0f)
            );
        }
    }

    IEnumerator Start()
    {
        if (warning != null) warning.SetActive(false);
        if (active != null) active.SetActive(false);

        Collider zone = GetComponent<Collider>();

        while (true)
        {
            zone.enabled = false;
            if (active != null) active.SetActive(false);

            if (restTime > 0f)
            {
                StartCoroutine(MoveStomper(_slammedLocalPos, _startLocalPos, recoverTime, false));
                yield return new WaitForSeconds(restTime);
            }
            else
            {
                stomper.localPosition = _startLocalPos;
            }

            if (warning != null) warning.SetActive(true);
            yield return new WaitForSeconds(warnTime);
            if (warning != null) warning.SetActive(false);

            zone.enabled = true;
            if (active != null) active.SetActive(true);

            yield return StartCoroutine(MoveStomper(_startLocalPos, _slammedLocalPos, strikeTime, true, true));
        }
    }

    private IEnumerator MoveStomper(Vector3 from, Vector3 to, float duration, bool useStompEase, bool play = false)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float curvedT = useStompEase ? stompEase.Evaluate(t) : Mathf.SmoothStep(0f, 1f, t);
            stomper.localPosition = Vector3.LerpUnclamped(from, to, curvedT);

            yield return null;
        }

        if(play) fx.Play();
        stomper.localPosition = to;
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out Health health) || !health.TakeDamage(damage)) return;

        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;

        Vector3 away = body.position - transform.position;
        away.y = 0f;

        if (away.sqrMagnitude < 0.001f)
            away = Vector3.back;

        body.AddForce(away.normalized * knockback, ForceMode.Impulse);
    }
}