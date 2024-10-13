using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Linq;
using System.IO;
using UnityEngine.Rendering;


[System.Serializable]
public class Item : MonoBehaviour
{
    [SerializeField] public GameObject go;
    [SerializeField] public Texture2D preview;
    public GameObject icon;
    public Mesh m;
        
    public void Start()
    {
        Mesh m = null;
        MeshCollider mc = go.GetComponent<MeshCollider>();
        if (mc)
        {
            m = go.GetComponent<MeshCollider>().sharedMesh;
        } else { 
            m = go.GetComponent<MeshFilter>().sharedMesh;
        }
        MeshPreview mp = new MeshPreview(m);
        preview = mp.RenderStaticPreview(256, 256);

        GameObject icon = FindChild(this.transform.parent.gameObject, "Icon");
        icon  = FindChild(this.gameObject, "Icon");
        icon.GetComponent<RawImage>().texture = preview;
    }

    public string textureToPNG(Texture2D image)
    {
        byte[] bytes = image.EncodeToPNG();
        var dirPath = Application.dataPath + "/SaveImages2/";
        Debug.Log(dirPath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        string s = dirPath + go.name + ".png";
        File.WriteAllBytes(s, bytes);
        return s;
    }

    public GameObject FindChild(GameObject fromGameObject, string withName)
    {
        var allKids = fromGameObject.GetComponentsInChildren<Transform>();
        var kid = allKids.FirstOrDefault(k => k.gameObject.name == withName);
        if (kid == null)
        {
            Debug.Log("search fail");
            return null;
        } 
        return kid.gameObject;
    }

    public void SetPosition(Transform slotPos)
    {
        transform.SetParent(slotPos);
        transform.SetSiblingIndex(4); //If it is the last => count text will be faded??? do i need this?
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        
    }


}

