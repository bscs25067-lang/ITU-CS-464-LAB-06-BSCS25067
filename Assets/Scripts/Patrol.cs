using UnityEngine;

public class Patrol : MonoBehaviour
{
    enum State
    {
        Patrol,
        Chase,
        Return
    }

    enum PatrolMode
    {
        Loop,
        PingPong,
        Random
    }

    [Header("Patrol settings")]
    [SerializeField] private PatrolMode currMode = PatrolMode.Loop;
    [SerializeField] private Transform[] points;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float turnSpeed = 360f;

    [Header("FOV Settings")]
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewDistance;

    private int _currIndex;
    private float _waitUntil;
    private int _step = 1;

    private State _currState = State.Patrol;
    private Transform _player;
    private Vector3 _lastPlayerPos;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null) _player = playerObj.transform;
        else
            Debug.LogWarning("No GameObject with tag 'Player' found in the scene!");
    }

    private void Update()
    {
        if (_player == null) return;

        switch (_currState)
        {
            case State.Patrol:
                HandlePatrol();
                if (CanSee()) _currState = State.Chase;
                break;

            case State.Chase:
                HandleChase();
                if (!CanSee()) _currState = State.Return;
                break;

            case State.Return:
                if (HandleReturn()) _currState = State.Patrol;
                break;
        }
    }

    private bool CanSee()
    {
        Vector3 dirToPlayer = (_player.position - transform.position).normalized;

        float dot = Vector3.Dot(transform.forward, dirToPlayer);
        float distance = Vector3.Distance(transform.position, _player.position);

        if (distance > viewDistance) return false;
        if (dot < Mathf.Cos(viewAngle * Mathf.Deg2Rad))
            return false;

        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Vector3 dirToPlayerCenter = (_player.position - rayOrigin).normalized;

        if (Physics.Raycast(rayOrigin, dirToPlayerCenter, out RaycastHit hit, viewDistance))
        {
            if (hit.collider.CompareTag(_player.tag))
            {
                _lastPlayerPos = _player.position;
                return true;
            }
        }

        return false;
    }

    private bool HandleReturn()
    {
        transform.position = Vector3.MoveTowards(transform.position, _lastPlayerPos, speed * Time.deltaTime);

        Vector3 toTarget = _lastPlayerPos - transform.position;
        if (toTarget != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(toTarget), turnSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _lastPlayerPos) < 0.1f)
        {
            return true;
        }
        return false;
    }

    private void HandleChase()
    {
        transform.position = Vector3.MoveTowards(transform.position, _player.position, speed * Time.deltaTime);

        Vector3 toTarget = _player.position - transform.position;
        if (toTarget != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(toTarget), turnSpeed * Time.deltaTime);
    }

    private void HandlePatrol()
    {
        if (_waitUntil > Time.time) return;
        if (points == null || points.Length == 0) return;

        Vector3 target = points[_currIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        Vector3 toTarget = target - transform.position;
        if (toTarget != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(toTarget), turnSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            _currIndex = NextIndex();
            _waitUntil = Time.time + waitTime;
        }
    }

    private int NextIndex()
    {
        if (points == null || points.Length == 0) return 0;

        switch (currMode)
        {
            case PatrolMode.Loop:
                return (_currIndex + 1) % points.Length;

            case PatrolMode.PingPong:
                if (_currIndex + _step < 0 || _currIndex + _step >= points.Length)
                    _step = -_step;
                return _currIndex + _step;

            case PatrolMode.Random:
                int next = Random.Range(0, points.Length);
                return next == _currIndex ? (next + 1) % points.Length : next;

            default:
                return (_currIndex + 1) % points.Length;
        }
    }
}