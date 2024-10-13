using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLabels : MonoBehaviour
{

    public GroundController g;
    public Text[] list1;
    private List<GameObject> objList;
    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        g = GroundController.Instance;
        objList = g.ObjectList;

        list1 =  this.GetComponentsInChildren<Text>();
        //Debug.Log(list1.Length + " " + g.ObjectList.Length);
        
        foreach (GameObject g in objList)
        {
            if (objList[count] != null)
            {
                list1[count].text = objList[count].name;
            }
            count++;
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        //update with new hotbar items
    }
}
