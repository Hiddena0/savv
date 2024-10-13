using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.IO;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public bool isEmpty;
    public bool isLocked;
    public Toggle Toggle;
    public FileInfo file;
    public FolderView fv;


    //public TextMeshProUGUI countText;
    private void Start()
    {
        fv = GameObject.Find("Folder").GetComponent<FolderView>();
        if (Toggle)
        {
            Toggle.group = GetComponentInParent<ToggleGroup>();
        }
    }


    public InventoryItem GetItemType()
    {
        return GetComponentInChildren<InventoryItem>();
    }

    public void DestroyItem()
    {
        Destroy(GetItemType().gameObject);
        isEmpty = true;
    }


    public void OnDrop(PointerEventData eventData)
    {
        if (isEmpty)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            if (draggableItem != null)
            {
                draggableItem.parentAfterDrag = transform;
                draggableItem.GetComponent<InventoryItem>().UpdateItemImage();
                isEmpty = false;
            }
        }
    }
}
