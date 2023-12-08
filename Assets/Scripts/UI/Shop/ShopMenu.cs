using NohaSoftware.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
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
	[SerializeField] TextMeshProUGUI currencyText;
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
	[SerializeField] Button buyButton;

	private void Start()
	{
		itemObjectPrefab = itemGrid.GetChild(0).gameObject;
		itemObjectPrefab.SetActive(false);

		Initialise();
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			InventoryManager.instance.AddCurrency(25f);
			UpdateUI();
		}
	}

	void Initialise()
	{
		var items = gameSettings.shopItemData.Where(i => i.showInShop).ToList();
		items.Add(gameSettings.capacityUpgradeItem);
		itemObjects = new ShopItemObject[items.Count];
		for (int i = 0; i < items.Count - 1; i++)
		{
			var obj = Instantiate(itemObjectPrefab, itemGrid);
			itemObjects[i] = obj.GetComponent<ShopItemObject>();
			itemObjects[i].SetData(items[i].id);
			obj.SetActive(true);
		}
		//cap upgrade
		var cap = Instantiate(itemObjectPrefab, itemGrid);
		itemObjects[^1] = cap.GetComponent<ShopItemObject>();
		itemObjects[^1].SetData(gameSettings.capacityUpgradeItem);
		cap.SetActive(true);

		itemDetailPanel.gameObject.SetActive(false);
		itemGrid.SetHeight(CalculateHeight());
		if (currencyText != null) currencyText.text = InventoryManager.instance.Currency.ToString("# ##0.0");
		Select(items.First().id);
		itemObjects.First().OnClick();
	}

	public void Select(string id)
	{
		if (!string.IsNullOrEmpty(selectedID))
		{
			itemObjects.Where(x => x.itemDataID == selectedID).FirstOrDefault().Deselect();
		}

		selectedID = id;

		UpdateUI();

		itemDetailPanel.gameObject.SetActive(true);
	}
	public void Deselect()
	{
		selectedID = null;
		itemDetailPanel.gameObject.SetActive(false);
	}

	public void Buy()
	{
		InventoryManager.instance.SubtractCurrency(gameSettings.GetShopItemData(selectedID).price);
		InventoryManager.instance.AddItem(selectedID, 1);
		UpdateUI();
	}

	void UpdateUI()
	{
		var data = gameSettings.GetShopItemData(selectedID);
		if (data == null)
		{
			Debug.LogWarning($"Can't retrieve ShopItemData {selectedID} from GameSettings");
		}
		else
		{
			itemNameText.text = data.displayName;
			itemTypeText.text = $"<b>Típus</b>: {data.GetLocalisedType()}";
			itemPriceText.text = $"<b>Ár</b>: {data.price}";
			itemDurationText.text = string.Empty; //TODO
			itemDescriptionText.text = data.description;
			itemQuantityText.text = $"<b>{InventoryManager.instance.GetItem(selectedID)}db</b> a raktárban";
			itemIconImage.sprite = data.icon;
			itemIconImage.enabled = data.icon != null;

			buyButton.interactable = InventoryManager.instance.CanAfford(data.price);

			if (data.id == "capacity_upgrade")
			{
				int szint = InventoryManager.instance.GetItem("capacity_upgrade") + 1;
				bool buyable = szint < gameSettings.capacityUpgradeLevels.Length;
				buyButton.interactable = buyable;
				itemDurationText.text = $"<b>Szint</b>: {szint} / {gameSettings.capacityUpgradeLevels.Length}";
				if (buyable) itemDescriptionText.text += "\n\nKövetkezõ kapacitás: " + gameSettings.capacityUpgradeLevels[szint - 1];
				itemQuantityText.text = string.Empty;
			}
		}

		if (currencyText != null) currencyText.text = InventoryManager.instance.Currency.ToString("# ##0.0");
	}
	float CalculateHeight()
	{
		float height = 25f;
		height += Mathf.CeilToInt(itemObjects.Length / 2f) * 200f;
		return height;
	}
}