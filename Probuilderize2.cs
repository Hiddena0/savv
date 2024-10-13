using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;

public class Probuilderize2 
{
    /**
    class SmoothGroupData : IDisposable
    {

        bool m_Disposed;
        public bool isVisible;
        public Dictionary<int, List<Face>> groups;
        public Dictionary<int, Color> groupColors;
        public HashSet<int> selected;
        public Mesh previewMesh;
        public Mesh normalsMesh;

        public SmoothGroupData(ProBuilderMesh pb)
        {
            groups = new Dictionary<int, List<Face>>();
            selected = new HashSet<int>();
            groupColors = new Dictionary<int, Color>();
            isVisible = true;

            previewMesh = new Mesh()
            {
                hideFlags = HideFlags.HideAndDontSave,
                name = pb.name + "_SmoothingPreview"
            };

            normalsMesh = new Mesh()
            {
                hideFlags = HideFlags.HideAndDontSave,
                name = pb.name + "_SmoothingNormals"
            };

            Rebuild(pb);
        }

        public void Rebuild(ProBuilderMesh pb)
        {
            CacheGroups(pb);
            CacheSelected(pb);
            RebuildPreviewMesh(pb);
            RebuildNormalsMesh(pb);
        }

        public void CacheGroups(ProBuilderMesh pb)
        {
            groups.Clear();

            foreach (Face face in pb.faces)
            {
                List<Face> affected;

                if (!groups.TryGetValue(face.smoothingGroup, out affected))
                    groups.Add(face.smoothingGroup, new List<Face>() { face });
                else
                    affected.Add(face);
            }
        }

        public void CacheSelected(ProBuilderMesh pb)
        {
            selected.Clear();
            foreach (Face face in pb.faces)
                selected.Add(face.smoothingGroup);
        }

        void RebuildPreviewMesh(ProBuilderMesh pb)
        {
            List<int> indexes = new List<int>();
            Color32[] colors = new Color32[pb.vertexCount];
            groupColors.Clear();

            foreach (KeyValuePair<int, List<Face>> smoothGroup in groups)
            {
                if (smoothGroup.Key > 0) //assuming Smoothing.smoothingGroupNone = 0
                {
                    Color32 color = GetDistinctColor(smoothGroup.Key);
                    groupColors.Add(smoothGroup.Key, color);
                    var groupIndexes = smoothGroup.Value.SelectMany(y => y.indexes);
                    indexes.AddRange(groupIndexes);
                    foreach (int i in groupIndexes)
                        colors[i] = color;
                }
            }

            previewMesh.Clear();

            previewMesh.vertices = pb.positions.ToArray();
            previewMesh.colors32 = colors;
            previewMesh.triangles = indexes.ToArray();
        }

        public void RebuildNormalsMesh(ProBuilderMesh pb)
        {
            normalsMesh.Clear();

            Vector3[] srcPositions = pb.mesh.verticies;
            Vector3[] srcNormals = pb.mesh.normals;
            int vertexCount = System.Math.Min(ushort.MaxValue / 2, pb.vertexCount);
            Vector3[] positions = new Vector3[vertexCount * 2];
            Vector4[] tangents = new Vector4[vertexCount * 2];
            int[] indexes = new int[vertexCount * 2];
            for (int i = 0; i < vertexCount; i++)
            {
                int a = i * 2, b = i * 2 + 1;

                positions[a] = srcPositions[i];
                positions[b] = srcPositions[i];
                tangents[a] = new Vector4(srcNormals[i].x, srcNormals[i].y, srcNormals[i].z, 0f);
                tangents[b] = new Vector4(srcNormals[i].x, srcNormals[i].y, srcNormals[i].z, 1f);
                indexes[a] = a;
                indexes[b] = b;
            }
            normalsMesh.vertices = positions;
            normalsMesh.tangents = tangents;
            normalsMesh.subMeshCount = 1;
            normalsMesh.SetIndices(indexes, MeshTopology.Lines, 0);
        }

    }

    static Material s_FaceMaterial = null;
    static Material smoothPreviewMaterial
    {
        get
        {
            if (s_FaceMaterial == null)
            {
                s_FaceMaterial = new Material(Shader.Find("Hidden/ProBuilder/SmoothingPreview"));
                s_FaceMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            return s_FaceMaterial;
        }
    }

    static Material s_NormalPreviewMaterial = null;
    static Material normalPreviewMaterial
    {
        get
        {
            if (s_NormalPreviewMaterial == null)
                s_NormalPreviewMaterial = new Material(Shader.Find("Hidden/ProBuilder/NormalPreview"));
            return s_NormalPreviewMaterial;
        }
    }

    //List<Vertex> vlist = m1.GetVertices().ToList(); why does this work? Is it assuming mesh is a probuildermesh bc pbm extends mesh?
    public static void realMeshToPro(List<MeshFilter> m)
    {
        foreach (MeshFilter mf in m)
        {
            if (mf.sharedMesh == null)
                continue;

            GameObject go = mf.gameObject;
            Mesh sourceMesh = mf.sharedMesh;
            Material[] sourceMaterials = go.GetComponent<MeshRenderer>()?.sharedMaterials;

            try
            {
                ProBuilderMesh destination = Undo.AddComponent<ProBuilderMesh>(go);
                var meshImporter = new MeshImporter(sourceMesh, sourceMaterials, destination);
                //meshImporter.Import(settings); 

                SmoothGroupData.Rebuild(destination);
                destination.Optimize();

                i++;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Failed ProBuilderizing: " + go.name + "\n" + e.ToString());
            }

            UnityEditor.EditorUtility.DisplayProgressBar("ProBuilderizing", mf.gameObject.name, i / count);
        }
    }

    public static ProBuilderMesh MeshToPro(Mesh m1)
    {
        ProBuilderMesh m = new ProBuilderMesh();
        List<Vector3> list = m1.vertices.ToList();
        List<Vertex> vlist = new List<Vertex>();
        foreach (Vector3 l in list)
        {
            Vertex v = new Vertex();
            v.position = l;
            vlist.Add(v);
        }
        m.SetVertices(vlist);
        List<int> indicies = m1.GetIndices(m1.subMeshCount).ToList();

        int[] indy = m1.GetIndices(m1.subMeshCount);
        //m.
        //m.indicies(indy);
        List<Face> faces = new List<Face>(m.faces);
        foreach (Face f in faces)
        {
            List<int> f1 = f.indexes.ToList();
            //f1.
        }

        var indices = new List<int>();
        m.faces.ToList()
                        .ForEach(o =>
                        {
                            indices.AddRange(o.indexes.ToList());
                        });
        m1.SetIndices(indices, MeshTopology.Triangles, 0);
        m1.RecalculateBounds();
        m1.RecalculateNormals();
        return m;
    }
    */
    /**
    public static Mesh ProToMesh(ProBuilderMesh m)
    {
        Mesh mesh = new Mesh();
        float test = m.GetVertices()[0].position.x;
        mesh.SetVertices(m.GetVertices().Select(o => new Vector3()
        {
            x = o.position.x,
            y = o.position.y,
            z = o.position.z,
        }).ToList());
        var indices = new List<int>();
        m.faces.ToList()
                        .ForEach(o =>
                        {
                            indices.AddRange(o.indexes.ToList());
                        });
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }
    */
}
