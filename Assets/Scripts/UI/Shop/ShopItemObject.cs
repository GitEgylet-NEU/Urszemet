using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemObject : MonoBehaviour
{
	public TextMeshProUGUI nameText;
	public Image iconImage;
	public string itemDataID;
	ShopItemData itemData;

	bool isSelected;
	Color original;
	Image img;
	Button button;

	private void Awake()
	{
		img = GetComponent<Image>();
		original = img.color;
		button = GetComponent<Button>();
	}

	public void SetData(string id)
	{
		SetData(ShopMenu.instance.gameSettings.GetShopItemData(id));
	}
	public void SetData(ShopItemData itemData)
	{
		this.itemData = itemData;
		itemDataID = itemData.id;
		nameText.text = itemData.displayName;
		iconImage.sprite = itemData.icon;
	}

	public void OnClick()
	{
		if (isSelected)
		{
			ShopMenu.instance.Deselect();
			Deselect();
		}
		else
		{
			ShopMenu.instance.Select(itemData.id);
			img.color = Color.green;
			isSelected = true;
		}
		
	}

	public void Deselect()
	{
		img.color = original;
		EventSystem.current.SetSelectedGameObject(null);
		isSelected = false;
	}
}