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
        if (allVertices[index].y - currentNode.y <= limit)
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

    public static float GenerateFloodFillData(float heightThresholdValue, MeshFilter meshFilter)
    {
        Vector3[] verts = meshFilter.sharedMesh.vertices;
        Vector3 cNode;

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
        //Debug.Log("FF average: " + (averageTraversal *100f) + "%" + " list.Count: " + l.Count + " and all vert.Length is: " + verts.Length);
        return averageTraversal;
    }

}
