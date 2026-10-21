using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 4f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if(playerObj != null ) 
                player = playerObj.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;

        Vector3 targetPostion = new Vector3(player.position.x, transform.position.y, player.position.z);

        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        transform.LookAt(targetPostion);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
