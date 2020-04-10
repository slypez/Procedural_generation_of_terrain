using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MeshSettings : UpdatableData
{
    public const int numSupportedLODs = 5;
    public const int numSupportedChunkSizesPerlinNoise = 8;
    public const int numSupportedChunkSizesDiamonsSquare = 3;
    public const int numSupportedFlatshadedChunkSizes = 3;
    //Dont go over mesh max-verts (256x256 verts)
    public static readonly int[] supportedChunkSizes = { 28, 60, 124, 144, 168, 192, 216, 240 }; //  28, 60, 124 works fine with DS: (2^n) - 1 - 5 )
    // Vertex-counten i praktiken utan multiple chunks blir (x + 3), ex. 28 + 3 = 31 x 31

    [Header("Parameters")]
    public bool useDiamondSquareCompatibleSizes; // Lite knackigt med en extra bool för DS, ja vet.
    public bool useFlatShading;
    public float meshScale = 1f;

    [Range(0, numSupportedChunkSizesPerlinNoise - 1)] public int chunkSizeIndexPerlinNoise;
    [Range(0, numSupportedChunkSizesDiamonsSquare - 1)] public int chunkSizeIndexDiamondSquare;
    [Range(0, numSupportedFlatshadedChunkSizes - 1)] public int flatshadedChunkSizeIndex;

    //Num of vertices per row of mesh rendered at LOD = 0. Includes the 2 extra verts that are excluded from final mesh, but used for calculating normals
    public int numVertsPerRow
    {
        get
        {
            return supportedChunkSizes[(useFlatShading) ? flatshadedChunkSizeIndex : (useDiamondSquareCompatibleSizes) ? chunkSizeIndexDiamondSquare : chunkSizeIndexPerlinNoise] + 5;
        }
    }

    public float meshWorldSize
    {
        get
        {
            return (numVertsPerRow - 3) * meshScale;
        }
    }
}
