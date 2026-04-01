using System.Collections.Generic;
using UnityEngine;

public static class ArcGenerator
{
    public static Vector2[] GenerateArc(Vector2 startPoint, Vector2 endPoint, float arcRadius, bool doesFlip, int pointCount)
    {
        if (pointCount < 2)
        {
            throw new System.ArgumentException("Must have at least 2 points");
        }

        float chordLength = Vector2.Distance(endPoint, startPoint);
        float halfChordLength = chordLength * 0.5f;

        // Circle can't go throw both chord points if the radius is too small 
        if (arcRadius < halfChordLength)
        {
            throw new System.ArgumentException("The radius is too small for the chord!");
        }

        Vector2 midPoint = (startPoint + endPoint) * 0.5f;

        // Get perpendicular direction (normalize start to end points)
        Vector2 chordDirection = (endPoint - startPoint).normalized;
        Vector2 normal = new Vector2(-chordDirection.y, chordDirection.x);

        // Distance between midpoint and center circle 
        float height = Mathf.Sqrt((arcRadius * arcRadius) - ((chordLength * chordLength) / 4f));
        Vector2 center;
        // Deciding which side of the chord the arc bulges towards
        if (doesFlip)
        {
            center = midPoint - (normal * height);
        }
        else
        {
            center = midPoint + (normal * height);
        }

        // Convert start and end points to an angle around a circle center
        float startAngle = Mathf.Atan2(startPoint.y - center.y, startPoint.x - center.x);
        float endAngle = Mathf.Atan2(endPoint.y - center.y, endPoint.x - center.x);
        float angularDir = endAngle - startAngle;

        // Adjust angular direction so interpolation stays on the same side of the chord as the chosen circle center
        while (angularDir > Mathf.PI)
        {
            angularDir -= Mathf.PI * 2f;
        }
        while(angularDir < Mathf.PI)
        {
            angularDir += Mathf.PI * 2f;
        }

        Vector2[] points = new Vector2[pointCount];

        // Even arc-length spacing on a circle
        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / ((float)pointCount - 1); 
            float currentAngle = startAngle + (angularDir * t);

            float x = center.x + Mathf.Cos(currentAngle) * arcRadius;
            float y = center.y + Mathf.Sin(currentAngle) * arcRadius;
            points[i] = new Vector2(x, y);
        }

        // Set start and end point
        points[0] = startPoint;
        points[pointCount - 1] = endPoint;

        return points;
    }

    public static Vector2[][] GenerateArcWithOffsets(Vector2 startPoint, Vector2 endPoint, 
                                        float arcRadius, bool doesFlip, int pointCount, 
                                        float offsetDistance, int copyCount)
    {
        if (pointCount < 2)
        {
            throw new System.ArgumentException("Must have at least 2 points");
        }

        Vector2[][] arcs = new Vector2[copyCount + 1][];

        for (int i = 0; i <= copyCount; i++)
        {
            float offsetRadius = arcRadius + (offsetDistance * i);

            arcs[i] = GenerateArc(startPoint, endPoint, offsetRadius, doesFlip, pointCount);
        }
        return arcs;
    }
}
