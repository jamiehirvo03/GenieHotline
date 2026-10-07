using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    [Tooltip("The RenderTexture to draw on")]
    public RenderTexture renderTexture;

    [Header("BrushSettings")]
    [Tooltip("Max distance for brush detection")]
    public float maxDistance = 0.2f;
    [Tooltip("Minimum distance between brush positions")]
    public float minBrushDistance = 2f;

    private Material brushMaterial;

    public Color backgroundColour;
    public float markerAlpha = 0.7f;

    [Header("Collider Type")]
    [Tooltip("Set false to use a MeshCollider for 3D objects")]
    public bool useBoxCollider = true;

    public bool showBrushRays = false;

    // define a brush class to hold properties for each brush
    [System.Serializable]
    public class BrushSettings
    {
        public Holdable brushHoldable; // ref for 'Holdable' script of brush item
        public Transform brushTransform; // transform of brush item pivot
        public Color colour = Color.black;
        public int sizeX = 20; // brush width in pixels
        public int sizeY = 20; // brush height in pixels

        public bool isEraser = false;

        public Vector2 lastPosition; // last drawn position
        public bool isFirstDraw = true; // flag for first draw
        public bool isDrawing = false; // whether the brush is in contact
    }

    [Header("Add Brushes")]
    public List<BrushSettings> brushes = new List<BrushSettings>(); // list to hold multiple brushes

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // initialize the brush material with a simple shader
        brushMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
        // set the RenderTexture as the main texture of the object's material
        GetComponent<Renderer>().material.mainTexture = renderTexture;
        // clear the RenderTexture at the start
        RenderTexture.active = renderTexture;
        // set the background colour
        GL.Clear(true, true, backgroundColour);

        // set the brush colour to match the backgroundColour if it is an eraser
        foreach (BrushSettings brush in brushes)
        {
            // set the alpha level of the markers
            brush.colour.a = markerAlpha;

            if (brush.isEraser)
            {
                brush.colour = backgroundColour;
            }
        }
        RenderTexture.active = null;
    }

    /*running in update gives a marker smear breakup effect.. in URP its recommended to run in RenderPipelineManager.endCameraRendering or
     RenderPipelineManager.endFrameRendering, these are call backs, this will give sharp lines that don't look as much like a marker. For SRP its recommended to run
     in OnPostRender(), the same result..you can play with that and try adding noise etc but I was not able to find something that looked better than just
     running in Update()...*/

    // Update is called once per frame
    void Update()
    {
        // ensure the RenderTexture is active for drawing
        RenderTexture.active = renderTexture;

        // draw each brush on the texture
        foreach (var brush in brushes)
        {
            // check if the brush is being held to only run functions for the brush being used
            if (brush.brushHoldable && brush.brushHoldable.GetHeldStatus())
            {
                DrawBrushOnTexture(brush);
            }
        }

        // deactivate the RenderTexture after drawing
        RenderTexture.active = null;
    }

    private void DrawBrushOnTexture(BrushSettings brush)
    {
        if (brush.brushTransform == null) return; // null check in case transform isn't assigned

        // raycast from the brush tip transform
        Ray ray = new Ray(brush.brushTransform.position, brush.brushTransform.forward);

        if (showBrushRays) Debug.DrawRay(brush.brushTransform.position, brush.brushTransform.forward, Color.orange);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            // if the raycast from the brush is hitting this gameObject which is the board/page
            if (hit.collider.gameObject == gameObject)
            {
                Vector2 uv;

                if (useBoxCollider)
                {
                    // using BoxCollider
                    BoxCollider boxCollider = GetComponent<BoxCollider>();
                    if (boxCollider == null)
                    {
                        Debug.LogError("No BoxColldier found on this GameObject");
                        return;
                    }

                    // calculate the local hit point and normalize to UV
                    Vector3 localHitPoint = transform.InverseTransformPoint(hit.point); // convert hit point to local space
                    uv = new Vector2(
                        (localHitPoint.x / boxCollider.size.x) + 0.5f,          // normalize X position
                        1.0f - ((localHitPoint.y / boxCollider.size.y) + 0.5f)  // normalize and flip Y position
                    );
                }
                else
                {
                    // using MeshCollider
                    uv = hit.textureCoord;
                    uv.y = 1.0f - uv.y; // flip Y-axis
                }

                // convert UV coordinates to texture space
                int x = (int)(uv.x * renderTexture.width);
                int y = (int)(uv.y * renderTexture.height);
                Vector2 currentPosition = new Vector2(x, y);

                // only draw when we need to by comparing current position to the left
                if (!brush.isDrawing)
                {
                    // reset when the brush starts drawing again
                    brush.isFirstDraw = true;
                    brush.isDrawing = true;
                }

                if (brush.isFirstDraw)
                {
                    DrawAtPosition(currentPosition, brush.colour, brush.sizeX, brush.sizeY, brush.brushTransform.rotation.eulerAngles.z);
                    brush.lastPosition = currentPosition;
                    brush.isFirstDraw = false;
                    return;
                }

                // check if the texture space coordinates wrap around the edges,
                // this is so if you are drawing on a 3D object if you move from the left to right and up and down across the textures mapped edge
                // without it, it will draw a line all the way across the texture

                float deltaX = Mathf.Abs(currentPosition.x - brush.lastPosition.x);
                float deltaY = Mathf.Abs(currentPosition.y - brush.lastPosition.y);

                bool crossesHorizontalEdge = deltaX > renderTexture.width / 16; // crosses left-right edge
                bool crossesVerticalEdge = deltaY > renderTexture.height / 16; // crosses top-bottom edge

                if (crossesHorizontalEdge || crossesVerticalEdge)
                {
                    // if crossing an edge, do not interpolate. Just draw at the current position
                    DrawAtPosition(currentPosition, brush.colour, brush.sizeX, brush.sizeY, brush.brushTransform.rotation.eulerAngles.z);
                }
                else
                {
                    // interpolate between the last position and the current position
                    float distance = Vector2.Distance(currentPosition, brush.lastPosition);
                    int steps = Mathf.CeilToInt(distance / minBrushDistance);
                    
                    for (int i = 1; i <= steps; i++)
                    {
                        Vector2 interpolatedPosition = Vector2.Lerp(brush.lastPosition, currentPosition, i / (float)steps);
                        DrawAtPosition(interpolatedPosition, brush.colour, brush.sizeX, brush.sizeY,
                            brush.brushTransform.rotation.eulerAngles.z);
                    }
                }

                brush.lastPosition = currentPosition; // update the last drawn position
            }
        }
        else
        {
            // stop drawing when the brush is no longer in contact
            brush.isDrawing = false;
        }
    }

    private void DrawAtPosition(Vector2 position, Color colour, float sizeX, float sizeY, float rotationAngle)
    {
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, renderTexture.width, renderTexture.height, 0);

        brushMaterial.SetPass(0);

        GL.Begin(GL.QUADS);
        GL.Color(colour);

        // convert rotation angle to radians
        float radians = rotationAngle * Mathf.Deg2Rad;

        // calculate the rotation matrix components
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        // define the local offset vertices of the rectangle relative to the center
        Vector2[] vertices = new Vector2[4];
        vertices[0] = new Vector2(-sizeX, -sizeY); // bottom - left
        vertices[1] = new Vector2(sizeX, -sizeY); // bottom - right
        vertices[2] = new Vector2(sizeX, sizeY); // top - right
        vertices[3] = new Vector2(-sizeX, sizeY); // top - left

        // rotate each vertex to match the brush rotation,
        // this is so you can have a brush that is wide and thin and the paint will match the rotation
        for (int i = 0;  i < vertices.Length; i++)
        {
            float rotatedX = vertices[i].x * cos + vertices[i].y * sin; // positive Y-axis rotation
            float rotatedY = -vertices[i].x * sin + vertices[i].y * cos; // inverted sine for clockwise rotation

            // add the position offset to align with the center
            GL.Vertex3(position.x + rotatedX, position.y + rotatedY, 0);
        }

        // populated the pixels
        GL.End();
        GL.PopMatrix();
    }
}
