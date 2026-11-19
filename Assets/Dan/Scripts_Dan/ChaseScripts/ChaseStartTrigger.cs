using UnityEngine;

public class ChaseStartTrigger : MonoBehaviour
{
    public GameObject chaserPrefab;
    public Transform[] spawnPoints;

    //Stops the ambush happening more than once
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            SpawnChasers();
        }
 
    }

    void SpawnChasers()
    {
        foreach(Transform point in spawnPoints)
        {
            Instantiate(chaserPrefab, point.position, point.rotation);
        }
    }
}
