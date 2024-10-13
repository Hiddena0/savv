using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEditor;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{

    [HideInInspector]
    public InventorySlot inventorySlot;
    public Item g;

    public void Start()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        g = this.GetComponent<Item>();
        this.gameObject.GetComponentInParent<Toggle>().isOn = true; //this is how you select an item slot
      
        GameObject imagePreview = GameObject.Find("ImagePreview");
        //Texture t = imagePreview.GetComponent<RawImage>().texture;
        imagePreview.GetComponent<RawImage>().texture = g.preview;

        GameObject icon = g.FindChild(g.transform.gameObject, "Icon");
        icon.GetComponent<RawImage>().texture = g.preview;
        //rawImage.texture = g.preview;


        //GameObject target = GameObject.FindGameObjectWithTag("ImagePreview");
        //target.GetComponent<RawImage>().texture = g.preview;
    }

    //get orig slot
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Attempt to get the InventorySlot component
        inventorySlot = transform.parent.GetComponent<InventorySlot>();
        if (inventorySlot != null)
        {
            inventorySlot.isEmpty = true;
        }

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        //rawImage.raycastTarget = false;
    }


    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject target = eventData.pointerEnter;
        InventorySlot slot = target.GetComponentInParent<InventorySlot>();       
        Item targetItem = slot.gameObject.GetComponentInChildren<Item>();

        if (!slot.isEmpty)
        {
            Debug.Log("swap");
            targetItem.SetPosition(inventorySlot.transform);
            targetItem.transform.SetParent(inventorySlot.transform);
            inventorySlot.isEmpty = false;
            g.transform.SetParent(slot.transform);
            g.SetPosition(slot.transform);
            slot.isEmpty = false;
        } else
        {
            Debug.Log("moved");
            g.transform.SetParent(slot.transform);
            g.SetPosition(slot.transform);
            slot.isEmpty = false;
        }

        //rawImage.raycastTarget = true;
    }

}
