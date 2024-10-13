using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static ItemInfo;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class FolderManager : MonoBehaviour
{
    public List<DirectoryInfo> folders = new List<DirectoryInfo>();
    public List<List<FileInfo>> items = new List<List<FileInfo>>();
    public List<FolderView> fold;
    public int index = 0;

    
    private void Awake()
    {
        
    }

    public void Start()
    {
        findFolders();
        GameObject big = GameObject.FindGameObjectWithTag("BigFolder");
        FolderView[] folds = big.GetComponentsInChildren<FolderView>();
        List<FolderView> fold = new List<FolderView>();
        foreach (FolderView f in folds)
        {
            fold.Add(f);
        }
    }

    public void Update()
    {
        
    
    
    }

    public void findFolders()
    {
        //FileInfo[] fileInfo = 
        //+Assets/Game System/Prefabs
        string filepath = Application.persistentDataPath;
        DirectoryInfo dir = new DirectoryInfo(filepath);
        DirectoryInfo[] info = dir.GetDirectories("*.*");
        foreach (DirectoryInfo d in info)
        {
            folders.Add(d);
            items.Add(new List<FileInfo>());
            display(index);
            index++;
            //Debug.Log(d.ToString());
        }
    }
    public void display(int i)
    {
        DirectoryInfo d = folders[i];
        Debug.Log(d.Name +": ");
        string str = d.Name;
        List<FileInfo> currentFolder = new List<FileInfo>();
        foreach (FileInfo f in d.EnumerateFiles())
        {
            if (!f.Name.EndsWith(".meta"))
            {
                items[i].Add(f);
                //Debug.Log(f.Name);
            }
        }
        foreach (DirectoryInfo dir in d.EnumerateDirectories())
        {

        }
        
    }

 
}
