using UnityEngine;

public class FallRespawn : MonoBehaviour
{
    [SerializeField] private GyroStuff ballController;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float fallY = -5f;

    private void Update()
    {
        if (transform.position.y < fallY && ballController != null && spawnPoint != null)
        {
            ballController.ResetBall(spawnPoint.position);
        }
    }
}
