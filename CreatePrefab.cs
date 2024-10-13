using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
//using Newtonsoft.Json;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using static UnityEngine.GraphicsBuffer;

public class CreatePrefab
{
    //public string localPath2 = "Assets/Prefabs/Objects/";

    //returns the unique name of the created object
    public static string Save(GameObject g)
    {
        /**
        if (!Directory.Exists("Assets/Prefabs/Objects")) {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Objects");
        }
        */

        string localPath = "Assets/Prefabs/Objects/" + g.name + ".prefab";

        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        int len = localPath.IndexOf(".prefab") - (localPath.IndexOf("cts/") + 4);
        string n1 = localPath.Substring(localPath.IndexOf("cts/") + 4, len);

        bool success;
        PrefabUtility.SaveAsPrefabAssetAndConnect(g, localPath, InteractionMode.UserAction, out success);
        if (success == true)
            return n1;
        else
            return "you failed27836";      
    }
    

    public static string Save(GameObject g, string file)
    {
        string localPath = "Assets/Prefabs/" +file+ "/" + g.name + ".prefab";

        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        char last = file[file.Length - 1];
        char last2 = file[file.Length - 2];
        char last3 = file[file.Length - 3];
        string end = last3.ToString() + last2.ToString() + last.ToString();
        int len = localPath.IndexOf(".prefab") - (localPath.IndexOf(end) + 4);
        string n1 = localPath.Substring(localPath.IndexOf(end) + 4, len);

        bool success;
        PrefabUtility.SaveAsPrefabAssetAndConnect(g, localPath, InteractionMode.UserAction, out success);
        if (success == true)
            return n1;
        else
            return "you failed27836";
    }
    public static void Delete(GameObject g)
    {
        string localPath = "Assets/Prefabs/Objects/" + g.name + ".prefab";
        AssetDatabase.DeleteAsset(localPath);
    }
    public static void DeleteAllFilesInFolder()
    {
        //folder should already exist 
        Debug.Log("made");
        string target = "Assets/Prefabs/Objects";
        AssetDatabase.DeleteAsset(target);
        AssetDatabase.CreateFolder("Assets/Prefabs", "Objects");
    }
    public static List<GameObject> LoadAllPrefabs(string path)
    {
        string[] guids = AssetDatabase.FindAssets("t:prefab", new string[] { path});
        List<GameObject> gs = new List<GameObject>();
        foreach (string g in guids)
        {
            string p = AssetDatabase.GUIDToAssetPath(g);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(p);
            gs.Add(go);
        }
        return gs;
    }

    public static GameObject LoadPrefab(string name, Vector3 pos)
    {
        //Load a text file (Assets/Resources/Text/textFile01.txt) = Resources.Load<TextAsset>("Text/textFile01");
        GameObject g = Resources.Load<GameObject>(name);
        g.transform.position = pos;
        return g;
    }
    public static GameObject Load(string name)
    {
        GameObject g = Resources.Load<GameObject>(name);
        return g;
    }


    static bool ValidateCreatePrefab()
    {
        return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
    }
}