using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ToolType
{
    None,
    Sickle,
    Hammer
}

public class PlayerToolHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator viewModelAnimator;
    [SerializeField] private AudioSource audioSource;

    [Header("Current Tool")]
    public ToolType equippedTool = ToolType.None;

    [Header("Swing Settings")]
    [SerializeField] private float swingDistance = 3f;
    [SerializeField] private float hitDelay = 0.25f;
    [SerializeField] private float swingCooldown = 0.8f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("FeedBack")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private AudioClip swingSound;
    [SerializeField] private AudioClip hitSound;

    [Header("Held Tool Models")]
    [SerializeField] private GameObject sickleViewModel;
    [SerializeField] private GameObject hammerViewModel;

    private bool isSwinging = false;
    private bool readyToSwing = true;

    private void Start()
    {
        //start empty-handed
        EquipTool(ToolType.None);
    }

    public void EquipTool(ToolType newTool)
    {
        if (sickleViewModel != null)
            sickleViewModel.SetActive(newTool == ToolType.Sickle);

        //if(hammerViewModel != null)
        //    hammerViewModel.SetActive(newTool == ToolType.Hammer);
    }

    private void OnEnable()
    {
        InputManager.Actions.Game.UseTool.performed += OnUseTool;
    }

    private void OnDisable()
    {
        InputManager.Actions.Game.UseTool.performed += OnUseTool;
    }

    private void OnUseTool(InputAction.CallbackContext context)
    {
        //dont swing if empty-handed or already mid-swing
        if (equippedTool == ToolType.None || !readyToSwing || isSwinging)
            return;

        StartSwing();
    }

    private void StartSwing()
    {
        readyToSwing = false;
        isSwinging = true;

        if (audioSource != null && swingSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(swingSound);
        }

        if(viewModelAnimator != null)
        {
            viewModelAnimator.CrossFadeInFixedTime("Sickle_Swing", 0.1f);
        }

        Invoke(nameof(PerformRaycastHit), hitDelay);
        Invoke(nameof(ResetSwing), swingCooldown);
    }

    private void PerformRaycastHit()
    {
        if(playerCamera == null)
        {
            //Debug.Log("[ToolHandler] Player camera is not assigned in the inspector!");
            return;
        }

        // Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * swingDistance, Color.red, 2f);
        // Debug.Log("[ToolHandler] Casting ray...");

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, swingDistance, interactableLayer))
        {
            //  Debug.Log($"[ToolHandler] Ray HIT: {hit.collider.gameObject.name} (Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)})");

            BreakableObstacle obstacle = hit.collider.GetComponentInParent<BreakableObstacle>();
            if (obstacle != null)
            {
                bool didBreak = obstacle.TryBreak(equippedTool);
                if (didBreak)
                {
                    SpawnHitFeedback(hit.point);
                }
            }
        }
    }

    private void SpawnHitFeedback(Vector3 hitPoint)
    {
        if (audioSource != null && swingSound != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(hitSound);
        }

        if (hitEffect != null)
        {
            GameObject fx = Instantiate(hitEffect, hitPoint, Quaternion.identity);
            Destroy(fx, 5f);
        }
    }

    private void ResetSwing()
    {
        isSwinging = false;
        readyToSwing = true;

        if(viewModelAnimator != null )
        {
            viewModelAnimator.CrossFadeInFixedTime("Tool_Idle", 0.15f);
        }
    }
}
