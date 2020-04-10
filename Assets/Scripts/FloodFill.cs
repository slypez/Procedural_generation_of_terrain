using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FloodFill
{
    private static Queue<Vector3> q = new Queue<Vector3>();
    private static List<Vector3> l = new List<Vector3>();
    private static List<Vector3> l_Jump = new List<Vector3>();
    private static Queue<Vector3> q_Jump = new Queue<Vector3>();


    private static void CheckIfQueueNeighbor(int index, float limit, Vector3[] allVertices, Vector3 currentNode, ref List<Vector3> list, ref Queue<Vector3> queue)
    {
        if (Mathf.Abs(allVertices[index].y - currentNode.y) <= limit)
        {
            if (list.Contains(allVertices[index]) == false)
            {
                list.Add(allVertices[index]);
                queue.Enqueue(allVertices[index]);
            }
            else
            {
                // Do nothing if already in list
            }
        }
    }

    public static float GenerateFloodFillData(float heightThresholdValue, MeshFilter meshFilter, Transform parent,  Material debugMaterialWalking, Material debugMaterialJumping, Material debugMaterialNotReachable, bool checkForJumpTraversity, float jumpHeightThresholdValue, bool debugTraversability, bool debugNonTraversability)
    {
        Vector3[] verts = meshFilter.sharedMesh.vertices;
        Vector3 cNode;
        

        //Initialize
        int width = (int)meshFilter.sharedMesh.bounds.size.x;
        q = new Queue<Vector3>();
        l = new List<Vector3>();
        q_Jump = new Queue<Vector3>();
        l_Jump = new List<Vector3>();

        //Start with anything and be sure to bruteforce through all to get the real traversability
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        foreach (Vector3 vertex in verts)
        {
            int index = Array.IndexOf(verts, vertex);
            if (l.Contains(verts[index]))
            {
                continue;
            }

            q.Enqueue(verts[index]);

            while (q.Count > 0)
            {
                cNode = q.Dequeue();

                //Add traversable nodes: Left, right, top & bottom
                if ((Array.IndexOf(verts, cNode) - 1) >= 0)
                {
                    CheckIfQueueNeighbor(Array.IndexOf(verts, cNode) - 1, heightThresholdValue, verts, cNode, ref l, ref q);
                }
                if ((Array.IndexOf(verts, cNode) + 1) < verts.Length)
                {
                    CheckIfQueueNeighbor(Array.IndexOf(verts, cNode) + 1, heightThresholdValue, verts, cNode, ref l, ref q);
                }
                if ((Array.IndexOf(verts, cNode) - width) >= 0)
                {
                    CheckIfQueueNeighbor(Array.IndexOf(verts, cNode) - width, heightThresholdValue, verts, cNode, ref l, ref q);
                }
                if (Array.IndexOf(verts, cNode) + width < verts.Length)
                {
                    CheckIfQueueNeighbor(Array.IndexOf(verts, cNode) + width, heightThresholdValue, verts, cNode, ref l, ref q);
                }
            }
        }
        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds);

        if (checkForJumpTraversity)
        {
           GenerateFloodFillJumpData(verts, l, jumpHeightThresholdValue, meshFilter, debugMaterialJumping, debugTraversability);
        }

        float averageTraversal = ((float)l.Count + (float)l_Jump.Count) / (float)verts.Length;
        CheckIfDestroyOldDebugArea(parent);

        if (debugTraversability)
        {
            DebugFloodFillArea(parent, debugMaterialWalking, l);

            if (checkForJumpTraversity)
            {
                DebugFloodFillArea(parent, debugMaterialJumping, l_Jump);
            }
            if (debugNonTraversability)
            {
                List<Vector3> nonReachableVerts = verts.ToList();
                nonReachableVerts.RemoveAll(vertex => l.Contains(vertex));
                nonReachableVerts.RemoveAll(vertex => l_Jump.Contains(vertex));
                DebugFloodFillArea(parent, debugMaterialNotReachable, nonReachableVerts);
            }
        }

        return averageTraversal;
    }

    private static void GenerateFloodFillJumpData(Vector3[] verts, List<Vector3> traversableVerts, float jumpHeightThresholdValue, MeshFilter meshFilter, Material debugMaterial, bool debug)
    {
        //Remove already traversable nodes
        List<Vector3> temp = verts.ToList();
        temp.RemoveAll(vertex => traversableVerts.Contains(vertex));
        Vector3[] remainingVerts = temp.ToArray();
        Vector3 cNode;
        Transform debugContainer = meshFilter.transform.GetChild(0);
        int width = (int)meshFilter.sharedMesh.bounds.size.x;

        foreach (Vector3 vertex in remainingVerts)
        {
            int index = Array.IndexOf(remainingVerts, vertex);
            if (l_Jump.Contains(remainingVerts[index]))
            {
                continue;
            }

            q_Jump.Enqueue(verts[index]);

            while (q_Jump.Count > 0)
            {
                cNode = q_Jump.Dequeue();

                if ((Array.IndexOf(remainingVerts, cNode) - 1) >= 0)
                {
                    CheckIfQueueNeighbor(Array.IndexOf(remainingVerts, cNode) - 1, jumpHeightThresholdValue, remainingVerts, cNode, ref l_Jump, ref q_Jump);
                }
                if ((Array.IndexOf(remainingVerts, cNode) + 1) < remainingVerts.Length)
                {
                    CheckIfQueueNeighbor(Array.IndexOf(remainingVerts, cNode) + 1, jumpHeightThresholdValue, remainingVerts, cNode, ref l_Jump, ref q_Jump);
                }
                if ((Array.IndexOf(remainingVerts, cNode) - width) >= 0)
                {
                    CheckIfQueueNeighbor(Array.IndexOf(remainingVerts, cNode) - width, jumpHeightThresholdValue, remainingVerts, cNode, ref l_Jump, ref q_Jump);
                }
                if (Array.IndexOf(remainingVerts, cNode) + width < remainingVerts.Length)
                {
                    CheckIfQueueNeighbor(Array.IndexOf(remainingVerts, cNode) + width, jumpHeightThresholdValue, remainingVerts, cNode, ref l_Jump, ref q_Jump);
                }
            }
        }

    }

    public static void CheckIfDestroyOldDebugArea(Transform parent)
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
