using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ShopItemObject : MonoBehaviour
{
	[SerializeField] Image itemIcon;
	[SerializeField] TextMeshProUGUI itemNameText;
	public ShopItemData shopItemData { get; private set; }

	static public ShopItemData selectedItem;
	static public GameObject previouslySelected;
	Color orange = new Color(1, 0.75F, 0.015F, 0.5F);
	Color defaultColor = new Color(0, 0, 0, 0.2F);

	private void OnEnable()
	{
		UpdateInfo();
		//PlayerController.instance.GetCurrentPlayer().onResourceChange?.RemoveListener(() => UpdateInfo());
		//PlayerController.instance.GetCurrentPlayer().onResourceChange?.AddListener(() => UpdateInfo());
	}

	public void SelectItem()
	{
		selectedItem = shopItemData;
	}
	void DeselectItem()
	{
		//if(selectedItem == shopItemData)
		//{
  //          RaycasterScript.wantsToConstruct = false;
  //          Debug.Log("piss and cum");
  //          gameObject.GetComponent<Image>().color = defaultColor;
  //          if (RaycasterScript.previouslyHit != null) RaycasterScript.previouslyHit.GetComponent<MeshRenderer>().material.color = RaycasterScript.previousColor;
  //          RaycasterScript.currentlyHit = null;
  //          RaycasterScript.previouslyHit = null;
  //          previouslySelected = null;
  //      }
	}

	public void SetData(ShopItemData data)
	{
		this.shopItemData = data;
		UpdateInfo();
	}

	public void UpdateInfo()
	{
		if (shopItemData == null) return;

		if (shopItemData.icon != null)
		{
			itemIcon.sprite = shopItemData.icon;
			itemIcon.gameObject.SetActive(true);
		}
		else
		{
			itemIcon.gameObject.SetActive(false);
		}

		itemNameText.text = shopItemData.name;

		//if (!shopItemData.CanBuild(PlayerController.instance.GetCurrentPlayer()))
		//{
		//	buildingIcon.color = Color.red;
		//	GetComponent<Button>().interactable = false;

		//	DeselectItem();
		//}
		//else
		//{
			itemIcon.color = Color.white;
			GetComponent<Button>().interactable = true;
		//}
	}
}