using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;

public class CurrentMode : MonoBehaviour
{
    public GroundController g = GroundController.Instance;
    public string textBox;
    public int mode2 = 0;
    public Text textBox2;

    void Start()
    {
        textBox = "mode not found";
        textBox2 = this.GetComponent<Text>();
        //textBox2 = new Text();
        g = GroundController.Instance;
        //text = g.currentMode;
        mode2 = (int) g.currentMode;
        textBox = ModeToString(mode2);
        textBox2.text = "Mode: " + textBox;
    }


    void Update()
    {

        if (mode2 != (int)g.currentMode)
        {
            mode2 = (int)g.currentMode;
            textBox = ModeToString(mode2);
            textBox2.text = "Mode: " + textBox;
        }

    }

    public string ModeToString(int m)
    {
        if (m==0)
        {
            return "x";
        } else if (m==1)
        {
            return "y";
        } else if (m==2)
        {
            return "z";
        }
        else if (m == 3)
        {
            return "size";
        }
        else if (m == 4)
        {
            return "move";
        }
        else if (m == 5)
        {
            return "merge";
        }
        else if (m == 6)
        {
            return "delete";
        }
        return "idk update the code";
    }

}
