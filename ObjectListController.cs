using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectListController : MonoBehaviour
{
    //public GameObject[] ObjList2;
    public List<GameObject> ObjList; 
    public GroundController g;
    int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        g = GroundController.Instance;
        //ObjList2 = g.ObjectList;
        foreach (GameObject o in g.ObjectList)
        {
            ObjList.Add(o);
            count++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        g = GroundController.Instance;
        ObjList = g.ObjectList;
        while (g.ObjectList.Count()>count)
        {
            if (g.ObjectList[count] != null)
            {
               
            }
            count++;
            //Debug.Log(count + "PLS MAKE IT");
        }

    }
}
