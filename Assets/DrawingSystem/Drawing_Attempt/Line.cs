using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Line : MonoBehaviour
{
    public LineRenderer LineRenderer;

    List<Vector3> points;

    void SetPoint(Vector3 point)
    {
        points.Add(point);

        LineRenderer.positionCount = points.Count;

        LineRenderer.SetPosition(points.Count - 1, point);
    }

    public void UpdateLine(Vector3 position)
    {
        if (points == null)
        {
            points = new List<Vector3>();
            SetPoint(position);
            return;
        }

        // if new point is far away enough to consider it a valid new point (prevent double ups)
        if (Vector3.Distance(points.Last(), position) > 0.1f)
        {
            SetPoint(position);
        }
    }
}
