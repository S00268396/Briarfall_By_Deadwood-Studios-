using UnityEngine;

public class BreakableObstacle : MonoBehaviour
{
    [SerializeField]
    private ToolType requiredTool = ToolType.Sickle;
    [SerializeField]
    private GameObject brokenPrefab;

    public bool TryBreak(ToolType usedTool)
    {
        if(usedTool != requiredTool)
        {
            //Wrong tool (e.g. hitting wood with a sickle)
            return false;
        }

        if(brokenPrefab != null)
        {
            Instantiate(brokenPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
        return true;
    }


}
