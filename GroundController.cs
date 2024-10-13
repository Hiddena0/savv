using System;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using System.Drawing;
using UnityEditor.PackageManager.UI;
using System.Collections.Generic;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Text;
using System.IO;
using System.Xml.Linq;
using static UnityEngine.Networking.UnityWebRequest;
using UnityEngine.Assertions.Must;

//using UnityEngine.WSA;

[System.Serializable]
public class GroundController : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> ObjectList;
    public GameObject CurrentObject;
    string testing;
    Vector3 realPos = Vector3.zero;
    Vector3 offset = Vector3.zero;

    public static GroundController Instance { get; private set; }
    public float targetTime;
    Vector3 depth = new Vector3(0, 0, 1);
    bool ended = true;
    bool ended2 = true;
    bool ended3 = true;
    bool deleted = false;
    bool moving = false;
    bool stretch = false;


    public mode currentMode = mode.x;
    public float mouseScroll = 0;
    Vector3 rot = new Vector3(0, 0, 0);
    GameObject obj;
    GameObject a;
    GameObject b;
    GameObject c;
    GameObject g;
    GameObject merged;
    GameObject cut;
    GameObject intersect;
    bool shift;
    Data d;
    bool loop = true;
    public int load = 0;
    public int count = 0;
    List<GameObject> gs;
    public List<Data> list = new List<Data>();
    public List<string> names = new List<string>();
    public string filePath;
    StringBuilder sb = new StringBuilder();
    string result;
    public string folderName = "Objects";

    public void UpdateTimer(string clock)
    {
        targetTime -= Time.deltaTime;

        if (targetTime <= 0.0f)
        {
            if(clock == "click")
            {
                ended = true;
            }
            else if (clock == "ctrl")
            {
                ended2 = true;
            }
            else if (clock == "move")
            {
                ended3 = true;
            }

        }
    }
    public void StartTimer(string clock)
    {
        if (clock == "click")
        {
            ended = false;
            targetTime = .3f;
        }
        else if (clock == "ctrl")
        {
            ended2 = false;
            targetTime = 1.8f;
        }
        else if (clock == "move")
        {
            ended3 = false;
            targetTime = .3f;
        }
    }

    private float mouseWheelRotation;
    private int currentPrefabIndex = -1;
    public enum mode
    {
        x, y, z, size, move, merge, delete, subtract, intersect //
    };

    public void Start()
    {
        Awake();
        list = new List<Data>();
        filePath = Application.persistentDataPath + "/objects.json";
        result = File.ReadAllText(filePath);
        //Debug.Log("result: " + result);
        gs = CreatePrefab.LoadAllPrefabs("Assets/Prefabs/Objects/");
        if (gs.Count > 0)
        {
            foreach (GameObject g in gs)
            {
                Instantiate(g);
            }
        }
        if (result.Length > 1)
        {
            string[] words = result.Substring(1).Split('{');
            for (int i = 0; i < words.Length; i++)
            {
                //Debug.Log("{" + words[i]);
                d = JsonUtility.FromJson<Data>("{" + words[i]);
                list.Add(d);
                names.Add(d.name);
            }
        }
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0.5f, 0);
        CreatePrefab.Save(cube);
    }

    public void DeleteAll()
    {
        //deleted all objects in object file
        File.WriteAllText(filePath, "");

        CreatePrefab.DeleteAllFilesInFolder();
        foreach (string n in names)
        {
            Destroy(GameObject.Find(n));
        }
        names = new List<string>();
        foreach (GameObject g in gs)
        {
            Destroy(g);
        }
    }
    public void Undo()
    {
        //use UndoUtility class
    }
    
        
    public void Update()
    {

        HandleNewObjectHotkey();
        UpdateTimer("click");
        UpdateTimer("ctrl");
        UpdateTimer("move");
        CheckMode();

        selectObjects();
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DeleteAll();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && ended2)
        {
            StartTimer("ctrl");
        }
        if (Input.GetKeyDown(KeyCode.Delete) && !ended2)
        {
            DeleteAll();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            File.AppendAllText(filePath, sb.ToString());
            foreach (string n in names)
            {
                //Debug.Log(n);
            }          
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (CurrentObject != null)
        {
            MoveCurrentObjectToMouse();
            if (stretch)
            {
                ResizeFromMouseWheel();
            }else
            {
                RotateFromMouseWheel();
            }
            

            if (Input.GetMouseButtonDown(0) && ended)
            {
                ReleaseIfClicked();
                StartTimer("click");
            }
        }       
    }
    public void ResizeFromMouseWheel()
    {
        mouseWheelRotation += Input.mouseScrollDelta.y;
        mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 bound = CurrentObject.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        if (currentMode == mode.x)
        {
            mouseScroll += Input.mouseScrollDelta.y;
            bound.x = bound.x + mouseScroll;
        }
        else if (currentMode == mode.y)
        {
            mouseScroll += Input.mouseScrollDelta.y;
            bound.y = bound.y + mouseScroll;
        }
        else if (currentMode == mode.z)
        {
            mouseScroll += Input.mouseScrollDelta.y;
            bound.z = bound.z + mouseScroll;
        }
        else if (bound == null) //nonsense for now
        {
            UnityEngine.Color c = CurrentObject.GetComponent<Material>().color;
            mouseScroll += Input.mouseScrollDelta.y;
            c.r = c.r + mouseScroll;
            c.g = c.g + mouseScroll;
            c.b = c.b + mouseScroll;
        }
    }

    void OnApplicationQuit()
    {
        //Debug.Log(File.ReadAllText(filePath));
    }

    public void PlaceObject()
    {
        
        updateOutline(CurrentObject);
        CurrentObject.layer = 9;
        Debug.Log("placed");

        CreatePrefab.Save(CurrentObject);
        CurrentObject = null;
        if (currentMode == mode.merge)
        {
            //merged = null;
        }
    }

    public List<GameObject> GetList()
    {
        return ObjectList;
    }

    public void selectObjects()
    {
        if (Input.GetMouseButtonDown(0) && ended)
        {
            if (currentMode == mode.move)
            {
                if (!moving)
                {
                    StartTimer("click");
                }
                if (CurrentObject != null)
                {
                    moving = true;
                }
                else
                {
                    moving = false;
                }
                Move();
            }
            else if (currentMode == mode.delete)
            {
                Delete();
                StartTimer("click");
            }
            else if (currentMode == mode.merge)
            {
                Merge();
            }
            else if (currentMode == mode.subtract)
            {
                Cut();
            }
            else if (currentMode == mode.intersect)
            {
                Overlap();
            }
        }
    }


    public void Move()
    {
        if (!moving)
        {
            Ray ray;
            RaycastHit hit;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("found " + hit.collider.gameObject.name + " at distance: " + hit.distance);
                if (hit.collider.gameObject.name != "RealGroundPlane") {
                    CurrentObject = hit.collider.gameObject;
                    CurrentObject.layer = 2;
                    realPos = CurrentObject.transform.position;
                    updateOutline(CurrentObject);
                    moving = true;
                    StartTimer("move");
                }
            }                           
        }
    }
    public void ReleaseIfClicked()
    {
        if (currentMode == mode.move)
        {
            if (ended3 && moving)
            {
                PlaceObject();
            }
        }
        else
        {
            PlaceObject();
        }        
    }

    public void Delete()
    {
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject target = hit.collider.gameObject;
            CreatePrefab.Delete(target);
            Destroy(hit.collider.gameObject);
            //deleted = true;
        }                                    
    }

    public void Merge() {
        if (merged != null)
        {
            StartTimer("click");
        }
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            obj = hit.collider.gameObject;
            if (obj.name != "RealGroundPlane")
            {
                updateOutline(obj);
                if (a == null)
                {
                    a = obj;
                }
                else if (b == null)
                {
                    b = obj;

                    Model m = Boolean.Union(a, b);
                    GameObject merged = Boolean.ModelToObject(m);

                    updateOutline(a);
                    updateOutline(b);

                    merged.name = a.name + b.name;
                    merged.layer = 9;

                    CreatePrefab.Save(merged);
                    //CurrentObject = merged;
                    Destroy(a);
                    Destroy(b);
                }               
            }
        }
    }
    public void Cut()
    {
        if (cut != null)
        {
            StartTimer("click");
        }
        if (a == null)
        {
            a = CurrentObject;
            updateOutline(a);
        }
        else if (b == null)
        {
            b = CurrentObject;
            updateOutline(a);
            Model m = Boolean.Subtract(a, b);
            GameObject cut = Boolean.ModelToObject(m);
            cut.name = a.name + " (cut)";
            cut.layer = 9;
            CreatePrefab.Save(cut);
            Destroy(a);
            Destroy(b);              
        }
    }
    public void Overlap()
    {
        if (intersect != null)
        {
            StartTimer("click");
        }
        if (a == null)
        {
            a = CurrentObject;
            updateOutline(a);
        }
        else if (b == null)
        {
            b = CurrentObject;
            updateOutline(a);
            Model m = Boolean.Intersect(a, b);
            GameObject intersect = Boolean.ModelToObject(m);

            intersect.name = a.name + " (intersect)";
            intersect.layer = 9;

            CreatePrefab.Save(intersect);
            CurrentObject = intersect;
            Destroy(a);
            Destroy(b);
        }
    }
    public void CheckMode()
    {
        mode prev = currentMode;
        if (Input.GetKeyDown(KeyCode.P))
        {
            currentMode = mode.x;
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            currentMode = mode.y;         
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            currentMode = mode.z;            
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            currentMode = mode.size;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            currentMode = mode.move;
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            currentMode = mode.merge;    
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            currentMode = mode.subtract;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            currentMode = mode.intersect;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            currentMode = mode.delete;
            deleted = false;
        }
        if (currentMode != prev)
        {
            if (prev == mode.merge)
            {
                merged = null;
                a = null;
                b = null;
            }
        }
    }

    private void HandleNewObjectHotkey()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + 1 + i))
            {
                if (PressedKeyOfCurrentPrefab(i))
                {
                    Debug.Log("destroyed");
                    Destroy(CurrentObject);
                    currentPrefabIndex = -1;
                }
                else
                {
                    if (CurrentObject != null)
                    {
                        Destroy(CurrentObject);
                    }
                    CurrentObject = Instantiate(ObjectList[i]);
                    //CurrentObject = Instantiate(ObjectList[i]);
                    CurrentObject.layer = 2;
                    ObjectList.Add(CurrentObject);
                    addOutline(CurrentObject);
                    currentPrefabIndex = i;
                }
                break;
            }
        }
    }

    private bool PressedKeyOfCurrentPrefab(int i)
    {
        return CurrentObject != null && currentPrefabIndex == i;
    }

    private void MoveCurrentObjectToMouse()
    {
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            rot = CurrentObject.transform.eulerAngles;
            if (currentMode == mode.move)
            {
                CurrentObject.transform.position = hit.point;

            } else
            {
                CurrentObject.transform.position = hit.point;
            }
            CurrentObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            CurrentObject.transform.rotation = Quaternion.Euler(rot);
        }
    }

    public void RotateFromMouseWheel()
    {
        mouseWheelRotation += Input.mouseScrollDelta.y;
        mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (currentMode == mode.x)
        {
            mouseScroll += Input.mouseScrollDelta.y;
            CurrentObject.transform.Rotate(Vector3.up, mouseScroll * 10f);
        }
        else if (currentMode == mode.y)
        {
            mouseScroll += Input.mouseScrollDelta.y;
            CurrentObject.transform.Rotate(Vector3.right, mouseScroll * 10f);
        }
        else if (currentMode == mode.z)
        {
            mouseScroll += Input.mouseScrollDelta.y;
            CurrentObject.transform.Rotate(depth, mouseScroll * 10f);
        }
        else if (currentMode == mode.size)
        {
            mouseScroll += Input.mouseScrollDelta.y;
            CurrentObject.transform.localScale = new Vector3(mouseScroll *.01f, mouseScroll * .01f, mouseScroll * .01f);
        }
    }

    public void addOutline(GameObject g)
    {
        Outline o = g.AddComponent<Outline>();
        o.OutlineColor = UnityEngine.Color.green;
        o.OutlineWidth = 5;
        o.OutlineMode = Outline.Mode.OutlineAll;
        o.enabled = true;
    }

    public void updateOutline(GameObject g)
    {
        Outline o = g.GetComponent<Outline>();
        if (o == null)
        {
            addOutline(g);
        }
        if (o.enabled)
        {
            o.enabled = false;
        }
        else
        {
            o.enabled = true;
        }
    }
    public void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }
}