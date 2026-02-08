using System.Collections.Generic;
using UnityEngine;

public static class EarClipping
{
    /// <summary>
    /// Triangulates a simple polygon (no holes) for Mesh generation.
    /// Input: polygon as List<Vector3> (z can be 0 for 2D)
    /// Output: flat list of triangle indices
    /// </summary>
    public static List<int> Triangulate(List<Vector3> vertices)
    {
        List<Vector3> verts = new List<Vector3>(vertices);
        List<int> indices = new List<int>();

        if (verts.Count < 3)
            return indices;

        List<int> vertIndices = new List<int>();
        for (int i = 0; i < verts.Count; i++)
            vertIndices.Add(i);

        int safety = 0; // prevent infinite loops

        while (vertIndices.Count > 3 && safety < 1000)
        {
            bool earFound = false;

            for (int i = 0; i < vertIndices.Count; i++)
            {
                int prevIndex = vertIndices[(i - 1 + vertIndices.Count) % vertIndices.Count];
                int currIndex = vertIndices[i];
                int nextIndex = vertIndices[(i + 1) % vertIndices.Count];

                Vector3 prev = verts[prevIndex];
                Vector3 curr = verts[currIndex];
                Vector3 next = verts[nextIndex];

                if (IsConvex(prev, curr, next))
                {
                    bool containsOther = false;

                    for (int j = 0; j < vertIndices.Count; j++)
                    {
                        int testIndex = vertIndices[j];
                        if (testIndex == prevIndex || testIndex == currIndex || testIndex == nextIndex)
                            continue;

                        if (PointInTriangle(verts[testIndex], prev, curr, next))
                        {
                            containsOther = true;
                            break;
                        }
                    }

                    if (!containsOther)
                    {
                        // Clip the ear
                        indices.Add(prevIndex);
                        indices.Add(currIndex);
                        indices.Add(nextIndex);
                        vertIndices.RemoveAt(i);
                        earFound = true;
                        break;
                    }
                }
            }

            if (!earFound)
            {
                Debug.LogWarning("Ear clipping failed — possible degenerate polygon");
                break;
            }

            safety++;
        }

        // Last remaining triangle
        if (vertIndices.Count == 3)
        {
            indices.Add(vertIndices[0]);
            indices.Add(vertIndices[1]);
            indices.Add(vertIndices[2]);
        }

        return indices;
    }

    private static bool IsConvex(Vector3 a, Vector3 b, Vector3 c)
    {
        // z-component of cross product (b - a) x (c - b)
        return ((b.x - a.x) * (c.y - b.y) - (b.y - a.y) * (c.x - b.x)) > 0;
    }

    private static bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        float denominator = ((b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y));
        float alpha = ((b.y - c.y) * (p.x - c.x) + (c.x - b.x) * (p.y - c.y)) / denominator;
        float beta = ((c.y - a.y) * (p.x - c.x) + (a.x - c.x) * (p.y - c.y)) / denominator;
        float gamma = 1f - alpha - beta;
        return alpha > 0f && beta > 0f && gamma > 0f;
    }
}