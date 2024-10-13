using System.Collections.Generic;
//using System.Drawing;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;

using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;

static class Boolean
{
    // Tolerance used by `splitPolygon()` to decide if a point is on the plane.
    internal const float k_Epsilon = 0.00001f;

    public static Model Union(GameObject lhs, GameObject rhs)
    {
        Model csg_model_a = new Model(lhs);
        Model csg_model_b = new Model(rhs);

        Node a = new Node(csg_model_a.ToPolygons());
        Node b = new Node(csg_model_b.ToPolygons());

        List<Polygon> polygons = Node.Union(a, b).AllPolygons();

        return new Model(polygons);
    }


    public static Model Subtract(GameObject lhs, GameObject rhs)
    {
        Model csg_model_a = new Model(lhs);
        Model csg_model_b = new Model(rhs);

        Node a = new Node(csg_model_a.ToPolygons());
        Node b = new Node(csg_model_b.ToPolygons());

        List<Polygon> polygons = Node.Subtract(a, b).AllPolygons();

        return new Model(polygons);
    }


    public static Model Intersect(GameObject lhs, GameObject rhs)
    {
        Model csg_model_a = new Model(lhs);
        Model csg_model_b = new Model(rhs);

        Node a = new Node(csg_model_a.ToPolygons());
        Node b = new Node(csg_model_b.ToPolygons());

        List<Polygon> polygons = Node.Intersect(a, b).AllPolygons();

        return new Model(polygons);
    }

    public static GameObject ModelToObject(Model result)
    {
        var materials = result.materials.ToArray();
        ProBuilderMesh pb = ProBuilderMesh.Create();
        pb.GetComponent<MeshFilter>().sharedMesh = (Mesh)result;
        pb.GetComponent<MeshRenderer>().sharedMaterials = materials;     
        MeshImporter importer = new MeshImporter(pb.gameObject);
        importer.Import(new MeshImportSettings() { quads = true, smoothing = true, smoothingAngle = 1f });
        pb.CenterPivot(null);
        MeshCollider mc = pb.gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = (Mesh)result;
        return pb.gameObject;       
    }




}