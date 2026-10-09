using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    private static DrawingManager instance;

    [SerializeField] private Holdable pencilParent;
    [SerializeField] private Transform pencilTipOrigin;
    [SerializeField] private Holdable eraserParent;
    [SerializeField] private Transform eraserTipOrigin;

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

    public (Holdable pencilHoldable, Transform pencilTip, Holdable eraserHoldable, Transform eraserTip) GetBrushes()
    {
        return (pencilParent, pencilTipOrigin, eraserParent, eraserTipOrigin);
    }
}
