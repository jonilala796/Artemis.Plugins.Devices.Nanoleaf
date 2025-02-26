using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Helper;

public static class ShapeHelper
{
    public static string GetTriangleSvgPath(float height)
    {
        // Calculate the base of the equilateral triangle
        float baseLength = (float)(2 * height / Math.Sqrt(3));

        // Define the points of the triangle
        var points = new List<PointF>
        {
            new(0, 0),
            new(baseLength / 2, height),
            new(baseLength, 0)
        };
        
        // Scale the points to 25%
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = new PointF(points[i].X * 0.05f, points[i].Y * 0.05f);
        }

        // Create the SVG path
        var path = new StringBuilder();
        path.AppendFormat(CultureInfo.InvariantCulture, $"M {points[0].X.ToString(CultureInfo.InvariantCulture)},{points[0].Y.ToString(CultureInfo.InvariantCulture)} ");
        path.AppendFormat(CultureInfo.InvariantCulture, $"L {points[1].X.ToString(CultureInfo.InvariantCulture)},{points[1].Y.ToString(CultureInfo.InvariantCulture)} ");
        path.AppendFormat(CultureInfo.InvariantCulture, $"L {points[2].X.ToString(CultureInfo.InvariantCulture)},{points[2].Y.ToString(CultureInfo.InvariantCulture)} ");
        path.Append("Z");

        return path.ToString();
    }

    public static string GetHexagonSvgPath(float sideLength)
    {
        // Calculate the height of the hexagon
        float height = (float)(Math.Sqrt(3) * sideLength);

        // Define the points of the hexagon
        var points = new List<PointF>
        {
            new PointF(0, height / 2),
            new PointF(sideLength / 2, 0),
            new PointF(1.5f * sideLength, 0),
            new PointF(2 * sideLength, height / 2),
            new PointF(1.5f * sideLength, height),
            new PointF(sideLength / 2, height)
        };
        
        // Scale the points to 25%
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = new PointF(points[i].X * 0.05f, points[i].Y * 0.05f);
        }

        // Create the SVG path
        var path = new StringBuilder();
        path.AppendFormat(CultureInfo.InvariantCulture, $"M {points[0].X.ToString(CultureInfo.InvariantCulture)},{points[0].Y.ToString(CultureInfo.InvariantCulture)} ");
        foreach (var point in points.Skip(1))
        {
            path.AppendFormat(CultureInfo.InvariantCulture, $"L {point.X.ToString(CultureInfo.InvariantCulture)},{point.Y.ToString(CultureInfo.InvariantCulture)} ");
        }

        path.Append("Z");

        return path.ToString();
    }
}