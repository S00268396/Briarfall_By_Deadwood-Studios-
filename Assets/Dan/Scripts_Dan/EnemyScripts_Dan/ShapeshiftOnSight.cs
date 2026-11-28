using UnityEngine;

public class ShapeshiftOnSight : MonoBehaviour
{
    [Header("Vision Settings")]
    public float maxVisionDistance = 15f;
    public LayerMask obstacleMask;

    [Header("Visual Forms")]
    [SerializeField] private GameObject disguiseVisual; // Demon for now, tree later
    [SerializeField] private GameObject monsterVisual;  // Skinwalker_Animated

    [Header("Tree Spacing")]
    public float minTreeDistance = 1.6f; // Minimum meters away from another tree before disguising
    public float stepAwaySpeed = 4.5f;   // How fast he shifts away from a tree to find space
    public LayerMask treeLayer;          // Set this to your "Tree" layer in the Inspector

    // Cached references
    private Transform playerCamera;
    private EnemyFollow followScript;
    private Rigidbody rb;

    // Tracks current state to prevent running transformation code every frame
    private bool isCurrentlySeen = false;

    void Start()
    {
        playerCamera = Camera.main.transform;
        followScript = GetComponent<EnemyFollow>();
        rb = GetComponent<Rigidbody>();

        ChangeToTrueForm();
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        bool seesMe = CheckIfPlayerSeesMe();

        if (seesMe)
        {
            if (!isCurrentlySeen)
            {
                Collider[] nearbyTrees = GetNearbyTrees();

                if (nearbyTrees.Length == 0)
                {
                    // Enough space from other trees: freeze and transform now
                    isCurrentlySeen = true;
                    ChangeToDisguise();
                }
                else
                {
                    // Too close to a tree: step into clear space first before transforming
                    StepAwayFromTrees(nearbyTrees);
                }
            }
        }
        else if (isCurrentlySeen || (followScript != null && !followScript.enabled))
        {
            // Player looked away: return to monster form and resume chasing
            isCurrentlySeen = false;
            ChangeToTrueForm();
        }
    }

    Collider[] GetNearbyTrees()
    {
        return Physics.OverlapSphere(transform.position, minTreeDistance, treeLayer);
    }

    void StepAwayFromTrees(Collider[] nearbyTrees)
    {
        // Stop normal chase movement so he doesn't run toward the player while seen
        if (followScript != null)
            followScript.enabled = false;

        // Combine directions away from all nearby tree trunks so he moves toward open ground
        Vector3 pushDirection = Vector3.zero;
        foreach (Collider tree in nearbyTrees)
        {
            Vector3 awayFromTrunk = transform.position - tree.ClosestPoint(transform.position);
            awayFromTrunk.y = 0f;

            if (awayFromTrunk.sqrMagnitude > 0.001f)
                pushDirection += awayFromTrunk.normalized;
        }

        // Fallback sideways step if perfectly pinched between two opposite trees
        if (pushDirection.sqrMagnitude < 0.001f)
            pushDirection = transform.right;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = pushDirection.normalized * stepAwaySpeed;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minTreeDistance);
    }

    bool CheckIfPlayerSeesMe()
    {
        Vector3 targetPoint = transform.position + (Vector3.up * 0.5f);
        Vector3 dirToEnemy = targetPoint - playerCamera.position;

        if (dirToEnemy.magnitude > maxVisionDistance) return false;
        if (Vector3.Angle(playerCamera.forward, dirToEnemy) > 40f) return false;

        Vector3 rayOrigin = playerCamera.position + (playerCamera.forward * 0.1f);

        if (Physics.Raycast(rayOrigin, dirToEnemy.normalized, out RaycastHit hit, maxVisionDistance, ~obstacleMask))
        {
            return hit.transform.root == transform.root;
        }

        return false;
    }

    void ChangeToDisguise()
    {
        if (disguiseVisual != null) disguiseVisual.SetActive(true);
        if (monsterVisual != null) monsterVisual.SetActive(false);

        if (followScript != null) followScript.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    void ChangeToTrueForm()
    {
        if (disguiseVisual != null) disguiseVisual.SetActive(false);
        if (monsterVisual != null) monsterVisual.SetActive(true);

        if (followScript != null) followScript.enabled = true;

        if (rb != null)
            rb.isKinematic = false;
    }
}