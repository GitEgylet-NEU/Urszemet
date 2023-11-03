using System.Text;
using TMPro;
using UnityEngine;
using NohaSoftware.Utilities;

public sealed class UIManager : MonoBehaviour
{
	public TextMeshProUGUI counterText;
	public BinSwapper swapper;

	void Update()
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < GameManager.instance.counter.Length; i++)
		{
			var data = GameManager.instance.gameData.debrisTypeData.GetData((Debris.DebrisType)i);
			if (!data.shouldCount) continue;
			sb.AppendLine($"<b><color=#{ColorUtility.ToHtmlStringRGB(data.color)}>{data.displayName}</color></b>: {GameManager.instance.counter[i]}");
		}
		counterText.text = sb.ToString();
		counterText.RecalculateClipping();
		var rect = counterText.GetComponent<RectTransform>();
		counterText.transform.parent.GetComponent<RectTransform>().SetHeight(counterText.GetPreferredValues(sb.ToString()).y - rect.offsetMax.y + rect.offsetMin.y);
	}
}