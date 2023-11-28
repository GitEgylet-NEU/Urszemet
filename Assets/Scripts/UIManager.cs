using System.Text;
using TMPro;
using UnityEngine;
using NohaSoftware.Utilities;
using System.Collections;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
	public static UIManager instance;
	private void Awake()
	{
		instance = this;
	}

	public TextMeshProUGUI counterText;
	public BinSwapper swapper;
	public RectTransform modifierList;

	void Update()
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < GameManager.instance.counter.Length; i++)
		{
			var data = GameManager.instance.gameData.debrisTypeData.GetData((Debris.DebrisType)i);
			if (data == null) continue;
			if (!data.shouldCount) continue;
			sb.AppendLine($"<b><color=#{ColorUtility.ToHtmlStringRGB(data.color)}>{data.displayName}</color></b>: {GameManager.instance.counter[i]}");
		}
		counterText.text = sb.ToString();
		counterText.RecalculateClipping();
		var rect = counterText.GetComponent<RectTransform>();
		counterText.transform.parent.GetComponent<RectTransform>().SetHeight(counterText.GetPreferredValues(sb.ToString()).y - rect.offsetMax.y + rect.offsetMin.y);
	}

	public void AddModifier(ModifierManager.Modifier modifier)
	{
		GameObject obj = Instantiate(modifierList.GetChild(0), modifierList).gameObject;
		obj.name = modifier.id;
		obj.GetComponent<Image>().color = modifier.GetColor();
		obj.transform.GetChild(0).GetComponent<Image>().sprite = modifier.icon;
		obj.SetActive(true);
		StartCoroutine(RemoveModifier(obj, modifier.duration));
	}
	IEnumerator RemoveModifier(GameObject obj, float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(obj);
	}
}