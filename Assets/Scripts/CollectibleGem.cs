using UnityEngine;

public class CollectibleGem : MonoBehaviour
{
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private float rotationSpeed = 90f;

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.AddScore(scoreValue);
        gameObject.SetActive(false);
    }
}
