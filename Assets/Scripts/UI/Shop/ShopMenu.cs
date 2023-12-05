using NohaSoftware.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
	public static ShopMenu instance;
	private void Awake()
	{
		instance = this;
	}

	public string selectedID = null;
	public GameSettings gameSettings;

	ShopItemObject[] itemObjects;

	[Header("UI")]
	[SerializeField] TMP_Dropdown itemTypeSelector;
	[SerializeField] RectTransform itemGrid;
	GameObject itemObjectPrefab;
	[Space]
	[SerializeField] RectTransform itemDetailPanel;
	[SerializeField] TextMeshProUGUI itemNameText;
	[SerializeField] TextMeshProUGUI itemTypeText;
	[SerializeField] TextMeshProUGUI itemPriceText;
	[SerializeField] TextMeshProUGUI itemDurationText;
	[SerializeField] TextMeshProUGUI itemDescriptionText;
	[SerializeField] TextMeshProUGUI itemQuantityText;
	[SerializeField] Image itemIconImage;

	private void Start()
	{
		itemObjectPrefab = itemGrid.GetChild(0).gameObject;
		itemObjectPrefab.SetActive(false);

		Initialise();
	}

	void Initialise()
	{
		var items = gameSettings.shopItemData.Where(i => i.showInShop).ToArray();
		itemObjects = new ShopItemObject[items.Length];
		for (int i = 0; i < items.Length; i++)
		{
			var obj = Instantiate(itemObjectPrefab, itemGrid);
			itemObjects[i] = obj.GetComponent<ShopItemObject>();
			itemObjects[i].SetData(items[i].id);
			obj.SetActive(true);
		}
		itemDetailPanel.gameObject.SetActive(false);
		itemGrid.SetHeight(CalculateHeight());
	}

	public void Select(string id)
	{
		if (!string.IsNullOrEmpty(selectedID))
		{
			itemObjects.Where(x => x.itemDataID == selectedID).FirstOrDefault().Deselect();
		}

		selectedID = id;
		
		var data = gameSettings.GetShopItemData(id);
		if (data == null)
		{
			Debug.LogWarning($"Can't retrieve ShopItemData {id} from GameSettings");
			return;
		}

		itemNameText.text = data.displayName;
		itemTypeText.text = $"<b>Típus</b>: {data.GetLocalisedType()}";
		itemPriceText.text = $"<b>Ár</b>: {data.price}";
		itemDurationText.text = string.Empty; //TODO
		itemDescriptionText.text = data.description;
		itemQuantityText.text = string.Empty; // TODO
		itemIconImage.sprite = data.icon;

		itemDetailPanel.gameObject.SetActive(true);
	}
	public void Deselect()
	{
		selectedID = null;
		itemDetailPanel.gameObject.SetActive(false);
	}

	public void Buy()
	{
		throw new NotImplementedException("you lazy bitch");
	}

	float CalculateHeight()
	{
		float height = 25f;
		height += Mathf.CeilToInt(itemObjects.Length / 2f) * 225f;
		return height;
	}
}