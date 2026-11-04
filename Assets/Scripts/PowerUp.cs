using UnityEngine;

public class PowerUp : MonoBehaviour
{
    enum PowerUpType
    {
        Speed,
        Shield
    }

    [SerializeField] PowerUpType type = PowerUpType.Speed;
    [SerializeField] float multiplier = 1.6f;
    [SerializeField] float duration = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (type == PowerUpType.Speed)
            other.GetComponent<PlayerController>().BoostSpeed();
        else
            other.GetComponent<Health>().shielded = true;

        Destroy(gameObject);
    }
}
