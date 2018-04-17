using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OctreeIndex
{
    UpperLeftFront = 0, //000
    UpperRightFront = 2, //010
    UpperRightBack = 3, // 011
    UpperLeftBack = 1, //001
    
    LowerLeftFront = 4, //100
    LowerRightFront = 6, //110
    LowerRightBack = 7, //111
    LowerLeftBack = 5 //101
}


public class Octree<TType>
{
    private OctreeNode<TType> node;
    private int depth;

    private class OctreeNode<TType>
    {
        Vector3 position; //pos of the cube
        float size; //width/height/depth etc
        OctreeNode<TType>[] subNodes = new OctreeNode<TType>[8];
        List<TType> values; //leaf
    }
     private int GetIndexOfPosition(Vector3 lookupPosition, Vector3 nodePosition)
    {
        int index = 0;
        index |= lookupPosition.y > nodePosition.y ? 0 : 4;
        index |= lookupPosition.x > nodePosition.x ? 0 : 2;
        index |= lookupPosition.z > nodePosition.z ? 0 : 1;
        return index;
    }
}


