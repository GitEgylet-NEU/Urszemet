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

	//TODO:
	public string[] prerequisites;

	public enum ItemType
	{
		Default = 0,
		PowerUp = 1,
		Ability = 2
	}
}