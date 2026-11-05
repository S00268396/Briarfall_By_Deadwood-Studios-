using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    [Header("CheckPoint Settings: ")]
    [SerializeField]
    private MessageTriggerData Triggerdata;
    private bool IsTriggered = false;
    private float Delay = 3f;

    [SerializeField]
    private TMP_Text CheckPointText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerCheckpoint playerCheckpoint = other.gameObject.GetComponent<PlayerCheckpoint>();

            if (!IsTriggered)
            {
                CheckPointText.SetText($"{Triggerdata.message}");
                IsTriggered = true;

                if (playerCheckpoint != null)
                {
                    playerCheckpoint.SetNewCheckpoint(transform);
                }

                StartCoroutine(ClearTextAfterDelay(Delay));
            }
        }
    }

    private IEnumerator ClearTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
}
