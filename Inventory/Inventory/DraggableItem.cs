using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEditor;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public RawImage rawImage;
    public Image image;
    [HideInInspector]
    public Transform initialParent;
    public Transform parentAfterDrag;

    public InventorySlot inventorySlot;
    public EquipmentSlot equipmentSlot;

    public EquipmentSlot equipableSlot;

    private InventoryItem thisItem;


    private bool allowDrag = true;
    private bool allowEquip = false;
    private bool allowUnequip = false;

    private bool dragging = false;
    private Vector3 offset = Vector3.zero;


    private void ResetParam()
    {
        allowEquip = false;
        allowUnequip = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //image preveiw is now a raw image
        Debug.Log("selected");
        thisItem = GetComponent<InventoryItem>();
        thisItem.gameObject.GetComponentInParent<Toggle>().isOn = true; //this is how you select an item slot
        GameObject imagePreview = GameObject.Find("ImagePreview");
        Sprite i = imagePreview.GetComponent<Image>().sprite; //only 1 of these 

        GameObject g = GameObject.Find("Item(Clone)/Icon");
        GameObject stateGo = thisItem.gameObject.transform.Find("Icon").gameObject;
        Sprite i2 = stateGo.GetComponent<Image>().sprite;
        Debug.Log(i.name + " " + i2.name);
        imagePreview.GetComponent<Image>().sprite = i2;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        thisItem = GetComponent<InventoryItem>();
        if (thisItem.gameObject.GetComponentInParent<Toggle>().isOn != true)
        {
            allowDrag = false;
            return;
        }
        dragging = true;

        initialParent = transform.parent;
        parentAfterDrag = transform.parent;

        // Attempt to get the InventorySlot component
        inventorySlot = transform.parent.GetComponent<InventorySlot>();
        if (inventorySlot != null)
        {
            allowEquip = true;
            inventorySlot.isEmpty = true;
            InventoryManager.Instance.scrollRect.vertical = false;// Avoid moving inventory with mouse

            equipableSlot = InventoryManager.Instance.equipmentSlots.Single(i => i.type == thisItem.data.info.baseStat.type); //If the item is stackable, Single() will throw exception
           
        }

        // Attempt to get the EquipmentSlot component
        equipmentSlot = transform.parent.GetComponent<EquipmentSlot>();
        if (equipmentSlot != null)
        {
            allowUnequip = true;
            equipmentSlot.isEquip = false;
        }
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (!allowDrag) return;

        
        Vector3 mousePos = Input.mousePosition;
        
        //var viewportPosition = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
        //((RectTransform)transform).anchoredPosition = viewportPosition / canvas.scaleFactor * canvas.pixelRect.size;
        GameObject target = eventData.pointerCurrentRaycast.gameObject;  
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!allowDrag)
        {
            allowDrag = true;
            return;
        }
        dragging = false;
        thisItem.SetPosition(parentAfterDrag);
        InventoryManager.Instance.inventoryField.SetActive(false);
        InventoryManager.Instance.equipField.SetActive(false);
        InventoryManager.Instance.scrollRect.vertical = true;
        //thisItem.gameObject.GetComponentInParent<Toggle>().isOn = true;
        GameObject target = eventData.pointerEnter;

        if (target.CompareTag("InventoryField"))
        {
            InventoryManager.Instance.UnequipItem(thisItem);
        }
        if (target.CompareTag("EquipField"))
        {
            if (!equipableSlot.isEquip) // EquipItem
            {
                InventoryManager.Instance.EquipItem(thisItem, equipableSlot);
            }
            else
            {
                InventoryItem equippedItem = equipableSlot.GetComponentInChildren<InventoryItem>();
                InventoryManager.Instance.ReplaceItem(thisItem, equippedItem);
            }
        }    

        InventoryItem targetItem = target.GetComponent<InventoryItem>();
        if (targetItem != null)
        {
            InventoryManager.Instance.ReplaceItem(thisItem, targetItem);
            target.gameObject.GetComponentInParent<Toggle>().isOn = true;
        }
        if (initialParent == transform.parent)
        {
            inventorySlot = GetComponentInParent<InventorySlot>();
            if (inventorySlot != null)
            {
                inventorySlot.isEmpty = false;
            }
            equipmentSlot = GetComponentInParent<EquipmentSlot>();
            if (equipmentSlot != null)
            {
                equipmentSlot.isEquip = true;
            }
        }

        ResetParam();
        image.raycastTarget = true;
    }

}
