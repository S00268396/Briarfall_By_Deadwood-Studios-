using UnityEngine;

public class ChaseEndTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            DespawnChasers();
        }
    }

    void DespawnChasers()
    {
        GameObject[] activerChasers = GameObject.FindGameObjectsWithTag("Chaser");

        foreach(GameObject chaser in activerChasers)
        {
            Destroy(chaser);
        }
    }
}
