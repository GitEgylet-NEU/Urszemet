using NohaSoftware.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
	public static ShopMenu instance;
	private void Awake()
	{
		instance = this;

		shopItemObjectPrefab = transform.GetChild(0).gameObject;
		shopItemObjectPrefab.SetActive(false);
	}

	GameObject shopItemObjectPrefab;
	ShopItemData[] items;
	List<ShopItemObject> shopItemObjects = new();

	private void OnEnable()
	{
		foreach (var obj in shopItemObjects)
		{
			Destroy(obj.gameObject);
		}
		shopItemObjects.Clear();

		items = GameManager.instance.gameSettings.shopItemData.Where(x => x.showInShop).ToArray();
		foreach (var item in items)
		{
			GameObject obj = Instantiate(shopItemObjectPrefab, transform);
			obj.name = item.name;
			ShopItemObject menuBuilding = obj.GetComponent<ShopItemObject>();
			menuBuilding.SetData(item);
			shopItemObjects.Add(menuBuilding);
		}

		CalculateHeight();
	}

	void CalculateHeight()
	{
		float height = 0;
		if (items.Length > 0)
		{
			height = 25f + Mathf.Ceil(shopItemObjects.Count / 2f) * (200f + 25f);
		}
		GetComponent<RectTransform>().SetHeight(height);
	}

	public void CloseMenu(GameObject menu)
	{
		if (ShopItemObject.previouslySelected != null)
		{
			ShopItemObject.previouslySelected.GetComponent<Image>().color = new Color(0, 0, 0, 0.2F);
			ShopItemObject.previouslySelected = null;
		}
		menu.SetActive(false);
	}
}