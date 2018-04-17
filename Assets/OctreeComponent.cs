using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeComponent : MonoBehaviour {

    public float size;
    public int depth;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        Octree<int> octree = new Octree<int>(this.transform.position, size, depth);
        
        DrawNode(octree.GetRoot());
    }

    private Color minColor = new Color(1, 1.0f, 0, 1f);
    private Color maxColor = new Color(0, 1.0f, 0.0f, 1.0f);
    void DrawNode(OctreeNode<int> node, int nodeDepth = 0)
    {
        if (!node.IsLeaf())
        {
            foreach (var subnode in node.Nodes)
            {
                DrawNode(subnode, nodeDepth + 1);
            }
        }
        Gizmos.color = Color.Lerp(new Color(1, 1.0f, 0, 1f), new Color(0, 1.0f, 0.0f, 1.0f), nodeDepth / (float)depth);
        Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);


    }
}
