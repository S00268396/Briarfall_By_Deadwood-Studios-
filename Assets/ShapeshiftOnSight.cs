using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class ShapeshiftOnSight : MonoBehaviour
{
    public GameObject trueFormObject;
    public float maxVisionDistance = 15f;
    public LayerMask obstacleMask;

    //Cached references
    private Transform playerCamera;
    private MeshRenderer disguisedMesh;
    private EnemyFollow followScript;
    private Rigidbody rb;

    //Tracks current state to prevent running transformation code every frame
    private bool isCurrentlySeen = false;

    void Start()
    {
        //Store component references once at startup to save performance
        playerCamera = Camera.main.transform;
        disguisedMesh = GetComponent<MeshRenderer>();
        followScript = GetComponent<EnemyFollow>();
        rb = GetComponent<Rigidbody>();

        ChangeToTrueForm();
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        bool seesMe = CheckIfPlayerSeesMe();

        //only trigger the transformation exactly when the vision state changes
        if (seesMe && !isCurrentlySeen)
        {
            isCurrentlySeen = true;
            ChangeToDisguise();
        }
        else if (!seesMe && isCurrentlySeen)
        {
            isCurrentlySeen = false;
            ChangeToTrueForm();
        }
    }

    bool CheckIfPlayerSeesMe()
    {
        // 1. Target chest/head height (1.8m up) so looking at the tall demon's face registers
        Vector3 targetPoint = transform.position + (Vector3.up * 0.5f);
        Vector3 dirToEnemy = targetPoint - playerCamera.position;

        // 2. Distance check
        if (dirToEnemy.magnitude > maxVisionDistance) return false;

        // 3. Angle check (35 degrees cleanly covers screen edges)
        if (Vector3.Angle(playerCamera.forward, dirToEnemy) > 40f) return false;

        // 4. Raycast check (starts 0.2m forward so it doesn't hit the player's own face)
        Vector3 rayOrigin = playerCamera.position + (playerCamera.forward * 0.1f);

        if (Physics.Raycast(rayOrigin, dirToEnemy.normalized, out RaycastHit hit, maxVisionDistance, ~obstacleMask))
        {
            return hit.transform.root == transform.root;
        }

        return false;
    }

    void ChangeToDisguise()
    {
        //Freeze movement and show the disguise prop
        if (disguisedMesh != null) disguisedMesh.enabled = true;
        if (trueFormObject != null) trueFormObject.SetActive(false);
        if (followScript != null) followScript.enabled = false;

        if(rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    void ChangeToTrueForm()
    {
        //Start chasing and show the true monster form 
        if (disguisedMesh != null) disguisedMesh.enabled = false;
        if (trueFormObject != null) trueFormObject.SetActive(true);
        if (followScript != null) followScript.enabled = true;

        if(rb !=null)
            rb.isKinematic = false;
    }
}
