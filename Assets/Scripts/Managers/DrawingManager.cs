using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    private static DrawingManager instance;

    [SerializeField] private Holdable pencilParent;
    [SerializeField] private Transform pencilTipOrigin;

    [SerializeField] private Holdable eraserParent;
    [SerializeField] private Transform eraserTipOrigin;

    [SerializeField] private Holdable approvedStampParent;
    [SerializeField] private Transform approvedStampOrigin;

    [SerializeField] private Holdable deniedStampParent;
    [SerializeField] private Transform deniedStampOrigin;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one DrawingManager in the scene.");
        }

        instance = this;
    }

    public static DrawingManager GetInstance()
    {
        return instance;
    }

    public (Holdable pencilHoldable, Transform pencilTip, Holdable eraserHoldable, Transform eraserTip, Holdable approvedStampHoldable, Transform approvedStampTip, Holdable deniedStampHoldable, Transform deniedStampTip) GetBrushes()
    {
        return (pencilParent, pencilTipOrigin, eraserParent, eraserTipOrigin, approvedStampParent, approvedStampOrigin, deniedStampParent, deniedStampOrigin);
    }
}
