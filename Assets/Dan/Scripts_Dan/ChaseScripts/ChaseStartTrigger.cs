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
        for(int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject chaser = Instantiate(chaserPrefab,spawnPoints[i].position, spawnPoints[i].rotation);

            // Give each chaser a slightly different speed so they spread out instead of overlapping
            EnemyFollow follow = chaser.GetComponent<EnemyFollow>();
            if (follow != null)
            {
                follow.moveSpeed += i * 0.4f;
            }
        }
    }
}
