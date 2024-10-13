using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using System.IO;
using System;
using System.Text;

using System.Reflection;
using static UnityEngine.Rendering.PostProcessing.HistogramMonitor;
using System.Data;
using UnityEditor;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;

public class SerializeObject
{
    int count = 0;
    StringBuilder sb = new StringBuilder();
    GameObject g1;
    int mat = 0;

    public void SerializeGameObject(GameObject g)
    {
        g1 = g;
        Material m = null;
        Material[] ms = null;
        mat = g.GetComponent<MeshRenderer>().materials.Length;
        if (mat == 1)
        {
            m = g.GetComponent<MeshRenderer>().material;
        } else if (mat > 1)
        {
            ms = g.GetComponent<MeshRenderer>().materials;
        }
            
        Component[] list = g.GetComponents(typeof(Component));
        //Component c = new Component();
        
        List<Type> typeList = new List<Type>();
        
        foreach (Component c in list)
        {
            typeList.Add(GetTypeFromString(c.GetType().Name));
            Debug.Log(c.GetType().Name);
            
            Serial(c, typeList[count]);
            count++;
        }

    }
    public void SerialMaterial(Material m)
    {
        var material = new
        {
            name = m.name,
            color = m.color,
            shader = m.shader,
            texture = m.mainTexture,
            renderQueue = m.renderQueue,

            doubleSidedGlobal = m.doubleSidedGI,

        };
    }
    public void Serial(Component c1, Type type)
    {
        if (type.ToString().Equals("Transform"))
        {
            Transform c = (Transform)c1;
            var transform = new
            {
                name = c.name,
                rotX = c.rotation.eulerAngles.x,
                roY = c.rotation.eulerAngles.y,
                rotZ = c.rotation.eulerAngles.z,
                posX = c.position.x,
                posY = c.position.y,
                posZ = c.position.z,
                scaleX = c.localScale.x,
                scaleY = c.localScale.y,
                scaleZ = c.localScale.z,
            };

            float hidsfs = c.position.y;
        }
        else if (type.ToString().Equals("Mesh Renderer"))
        {
            MeshRenderer c = (MeshRenderer)c1;
            var meshRenderer = new
            {
                name = c.name,
                materials = "c.materials is a list of material",
                recieveShadows = c.receiveShadows,
                shadowCastingMode = (int)c.shadowCastingMode,
                contributeGlobalIllumination = "null(need StaticEditorFlags.ContributeGI in flags)",
                recieveGlobal = (int)c.receiveGI,

                lightProbeMode = (int)c.lightProbeUsage,
                reflectionProbeMode = (int)c.reflectionProbeUsage,
                anchorOverride = c.probeAnchor, //transform (usually null)

                motionVectorMode = (int)c.motionVectorGenerationMode,
                dynamicOcclusion = c.allowOcclusionWhenDynamic,
            };
        }
        else if (type.ToString().Equals("Mesh Collider"))
        {
            MeshCollider c = (MeshCollider)c1;
            var meshCollider = new
            {
                name = c.name,
                convex = c.convex,
                cookingOptions = (int)c.cookingOptions,
                material = "none (Physic Material)",
                sharedMesh = c.sharedMesh, //mesh
            };
        }
        else if (type.ToString().Equals("Mesh Filter"))
        {
            MeshFilter c = (MeshFilter)c1;
            var meshFilter = new
            {
                name = c.name,
                mesh = c.mesh, //its a mesh 
            };
        }

    }

    public static Type GetTypeFromString(string TypeName)
    {
        var type = Type.GetType(TypeName);

        if (type != null)
            return type;

        // Get the name of the assembly (Assumption is that we are using  fully-qualified type names)
        var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

        // Attempt to load the indicated Assembly
        var assembly = Assembly.Load(assemblyName);
        if (assembly == null)
            return null;

        // Ask that assembly to return the proper Type
        return assembly.GetType(TypeName);
    }

    public void SerialMaterial(Material[] ms )
    {
        foreach (Material m in ms)
        {

        }
    }


}
