using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=TtuOX3pNMDE
public class MeshCombiner : MonoBehaviour
{
    public static MeshCombiner Instance { get; private set; }

    public void Start()
    {
        Awake();
    }
    public void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    public GameObject CombineObjects(List<MeshFilter> childMesh, GameObject parent)
    {
        var combine = new CombineInstance[childMesh.Count];

        for (var i = 0; i < childMesh.Count; i++)
        {
            combine[i].mesh = childMesh[i].sharedMesh;
            combine[i].transform = childMesh[i].transform.localToWorldMatrix;
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        Debug.Log(mesh.vertexCount);
        Debug.Log(mesh.name);
        parent.GetComponent<MeshFilter>().mesh = mesh;
        return parent;
    }
}