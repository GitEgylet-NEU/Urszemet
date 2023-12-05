using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Shop Item")]
public class ShopItemData : ScriptableObject
{
	public string id;
	public string displayName;
	public string description;
	public Sprite icon;
	public ItemType type;

	public bool showInShop = true;
	public float price;

	//TODO:
	public string[] prerequisites;

	public enum ItemType
	{
		Default = 0,
		PowerUp = 1,
		Ability = 2
	}

	public string GetLocalisedType()
	{
		switch (type)
		{
			case ItemType.Default: return "Általános";
			case ItemType.PowerUp: return "Powerup";
			case ItemType.Ability: return "Képesség";
			default: return type.ToString();
		}
	}
}