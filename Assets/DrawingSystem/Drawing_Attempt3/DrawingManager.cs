using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    public Camera cam;
    public int totalXPixels = 1024;
    public int totalYPixels = 512;
    public int brushSize = 4;
    public Color brushColour;

    public Transform topLeftCorner;
    public Transform bottomRightCorner;
    public Transform point;
    
    public Material material;
    public Texture2D generatedTexture;
    Color[] colourMap;

    private int xPixel = 0;
    private int yPixel = 0;

    private float xMult;
    private float yMult;

    private void Start()
    {
        colourMap = new Color[totalXPixels * totalYPixels];
        generatedTexture = new Texture2D(totalYPixels, totalXPixels, TextureFormat.RGBA32, false);
        generatedTexture.filterMode = FilterMode.Point;
        material.SetTexture("_BaseMap", generatedTexture);

        ResetColour();

        xMult = totalXPixels / (bottomRightCorner.localPosition.x - topLeftCorner.localPosition.x);
        yMult = totalYPixels / (bottomRightCorner.localPosition.y - topLeftCorner.localPosition.y);
    }

    private void Update()
    {
        //if (InteractionManager.GetInstance().GetHeldItemData() != null)
        //{
        //    if (InteractionManager.GetInstance().GetHeldItemData().CompareTag("Pencil") && InputManager.GetInstance().GetPrimaryInteractPressed())
        //    {
        //        CalculatePixel();
        //    }
        //}

        if (InputManager.GetInstance().GetPrimaryInteractPressed())
        {
            CalculatePixel();
        }
    }

    private void CalculatePixel()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(InputManager.GetInstance().GetMousePos());
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 10f))
        {
            point.position = hit.point;

            xPixel = (int)((point.localPosition.x - topLeftCorner.localPosition.x) * xMult);
            yPixel = (int)((point.localPosition.y - topLeftCorner.localPosition.y) * yMult);

            ChangePixelsAroundPoint();
        }
    }

    private void ChangePixelsAroundPoint()
    {
        DrawBrush(xPixel , yPixel);
        SetTexture();
    }

    private void DrawBrush(int xPix, int yPix)
    {
        int i = xPix - brushSize + 1, j = yPix - brushSize + 1, maxi = xPix + brushSize - 1, maxj = yPix + brushSize - 1;

        if (i < 0) i = 0;
        if (j < 0) j = 0;

        if (maxi >= totalXPixels) maxi = totalXPixels - 1;
        if (maxj >= totalYPixels) maxj = totalYPixels - 1;

        for (int x = i; x <= maxi; x++)
        {
            for (int y = j; y <= maxj; y++)
            {
                if ((x - xPix) * (x - xPix) + (y - yPix) * (y - yPix) <= brushSize * brushSize) colourMap[x * totalYPixels + y] = brushColour;
            }
        }
    }

    private void SetTexture()
    {
        generatedTexture.SetPixels(colourMap);
        generatedTexture.Apply();
    }

    private void ResetColour()
    {
        for (int i = 0; i < colourMap.Length; i++) colourMap[i] = Color.white;
        SetTexture();
    }
}
