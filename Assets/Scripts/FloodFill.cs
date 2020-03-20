using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloodFill
{
    private static Queue<Vector3> q = new Queue<Vector3>();
    private static List<Vector3> l = new List<Vector3>();

    private static void CheckIfQueueNeighbor(int index, float limit, Vector3[] allVertices, Vector3 currentNode)
    {
        if (Mathf.Abs(allVertices[index].y - currentNode.y) <= limit)
        {
            if (l.Contains(allVertices[index]) == false)
            {
                l.Add(allVertices[index]);
                q.Enqueue(allVertices[index]);
            }
            else
            {
                // Do nothing if already in list
            }
        }
    }

    public static float GenerateFloodFillData(float heightThresholdValue, MeshFilter meshFilter, Material debugMaterial, bool debug = false)
    {
        Vector3[] verts = meshFilter.sharedMesh.vertices;
        Vector3 cNode;
        Transform debugContainer = meshFilter.transform.GetChild(0);

        //Initialize
        int width = (int)meshFilter.sharedMesh.bounds.size.x;
        q = new Queue<Vector3>();
        l = new List<Vector3>();

        //Start at index 0, maybe be any corner or vertex in mesh. Investigate more.
        q.Enqueue(verts[0]);

        while (q.Count > 0)
        {
            cNode = q.Dequeue();

            //Add traversable nodes: Left, right, top & bottom
            if ((Array.IndexOf(verts, cNode) - 1) >= 0)
            {
                CheckIfQueueNeighbor(Array.IndexOf(verts, cNode) - 1, heightThresholdValue, verts, cNode);
            }
            if ((Array.IndexOf(verts, cNode) + 1) < verts.Length)
            {
                CheckIfQueueNeighbor(Array.IndexOf(verts, cNode) + 1, heightThresholdValue, verts, cNode);
            }
            if ((Array.IndexOf(verts, cNode) - width) >= 0)
            {
                CheckIfQueueNeighbor(Array.IndexOf(verts, cNode) - width, heightThresholdValue, verts, cNode);
            }
            if (Array.IndexOf(verts, cNode) + width < verts.Length)
            {
                CheckIfQueueNeighbor(Array.IndexOf(verts, cNode) + width, heightThresholdValue, verts, cNode);
            }
        }

        float averageTraversal = (float)l.Count / (float)verts.Length;

        CheckIfDestroyOldDebugArea(debugContainer);
        if (debug)
        {
            DebugFloodFillArea(debugContainer, debugMaterial, l);
        }
        return averageTraversal;
    }

    private static void CheckIfDestroyOldDebugArea(Transform parent)
    {
        if (parent.childCount > 0)
        {
            for (int i = parent.childCount; i > 0; --i)
            {
                GameObject.DestroyImmediate(parent.GetChild(0).gameObject);
            }
        }
    }

    private static void DebugFloodFillArea(Transform parent, Material debugMaterial, List<Vector3> traversableVertices)
    {
        // [*Old must be deleted either before this function or in here at top*]

        // Spawn new 
        foreach (Vector3 vertex in traversableVertices)
        {
            GameObject debugVertex = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugVertex.transform.parent = parent;
            debugVertex.transform.position = vertex;
            GameObject.DestroyImmediate(debugVertex.GetComponent<SphereCollider>());
            debugVertex.GetComponent<Renderer>().sharedMaterial = debugMaterial;
        }
    }
}
