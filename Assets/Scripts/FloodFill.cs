using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloodFill
{
    // Ska dessa vara av float/vector3's? float[,]?
    private static Queue<Vector3> q = new Queue<Vector3>();
    private static List<Vector3> l = new List<Vector3>();
    
    

    public static void GenerateFloodFillData(Vector3[] vertsInChunk)
    {
        //Initialize
        q = new Queue<Vector3>();
        l = new List<Vector3>();


        Debug.Log("Flood fill data traversal:" + "*DATA HERE*");
    }
}
