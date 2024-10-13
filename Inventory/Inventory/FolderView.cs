using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static UnityEditor.Progress;
using System;
/* This script is just used for grouping the objects
 * When I use "GetComponentsInChildren<FolderView>", I can easily get all views => It makes the code reusable. Like when I add new system
 */

public class FolderView : MonoBehaviour
{
    public DataInventory data;
    public List<FileInfo> items = new List<FileInfo>();
    public List<DirectoryInfo> folders = new List<DirectoryInfo>();
    public GameObject big;
    public FolderView[] folds;

    int index = 0;
    public bool active = false;

    public List<InventorySlot> slots;

    public void Awake()
    {
        //DataInventory.LoadData(data);
        //slots = new List<InventorySlot>();
        //slots.Add(generateSlot());
    }

    public void Start()
    {
        //FillFolder();
        //this.name
        //GameObject root = GameObject.Find("folder");
        //GameObject duplicate = Instantiate(root);
        big = GameObject.FindGameObjectWithTag("BigFolder");
        //FolderView[] folds = big.GetComponentsInChildren<FolderView>();
        slots = new List<InventorySlot>();
        generateSlot();
        slots.Add(generateSlot());
    }

    public void setActive()
    {
        Debug.Log(this.name + "is active");
        foreach (FolderView fv in big.transform)
        {
            fv.active = false;
        }
        active = true;
        //InventoryManager.Instance.RemoveAllItem();
        generateSlot();
        slots.Add(generateSlot());
        GameObject grid = GameObject.Find("Grid");
        foreach (Transform child in grid.transform)
            child.gameObject.SetActive(false);
        //GameObject[] gs = grid.GetComponentsInChildren<GameObject>();
        //foreach (GameObject g in gs)
        //{
        //    g.SetActive(false);
        //}
        foreach (InventorySlot s in slots)
        {
            s.gameObject.SetActive(true);
        }
    }

    public InventorySlot generateSlot()
    {
        GameObject grid = GameObject.Find("Grid");
        
        GameObject g = Instantiate(GameObject.FindGameObjectWithTag("copyThis"));
        
        InventorySlot s = g.GetComponent<InventorySlot>();
        s.isLocked = false; 
        Debug.Log(s.name);
        return s;
    }

    public void FillFolder()
    {
        string filepath = Application.persistentDataPath + this.name.ToString();
        Debug.Log(filepath);
        DirectoryInfo dir = new DirectoryInfo(filepath);
        DirectoryInfo[] info = dir.GetDirectories("*.*");
        foreach (DirectoryInfo d in info)
        {

            folders.Add(d);
            //GameObject Folder = Instantiate(GameObject.FindGameObjectWithTag("copyThis"), transform.Find("Folders"));
            //create folder
            Debug.Log(d.ToString());
        }
        foreach (FileInfo f in dir.EnumerateFiles())
        {
            Debug.Log(f);
            items.Add(f);
        }
    }

    public void Display()
    {
        Debug.Log("made it");
        foreach (FileInfo f in items)
        {
            Debug.Log(f.Name);
            
        }
        //int Id = data.inventoryData[6].ID;
        //data.RemoveInventoryData(Id);

    }


}

