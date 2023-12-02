using NohaSoftware.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
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

	[Header("Info Panel")]
	[SerializeField] RectTransform infoPanel;
	Queue<InfoPanelData> infoPanelQueue = new();
	bool IsInfoPanelActive => infoPanel.gameObject.activeSelf;

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

		Time.timeScale = IsInfoPanelActive ? 0f : 1f;
	}

	public void AddModifier(ModifierManager.Modifier modifier)
	{
		GameObject obj = Instantiate(modifierList.GetChild(0), modifierList).gameObject;
		obj.name = modifier.id;
		obj.GetComponent<Image>().color = modifier.GetColor();
		obj.transform.GetChild(0).GetComponent<Image>().sprite = modifier.icon;
		obj.SetActive(true);

		//display InfoPanel if new modifier
		if (!ModifierManager.instance.hasSeenModifier[modifier.id])
		{
			AddInfoPanelToQueue(modifier.name, "<b>You just found a new modifier!</b>\n\n" + modifier.description, modifier.icon);
			ModifierManager.instance.hasSeenModifier[modifier.id] = true;
		}

		StartCoroutine(RemoveModifier(obj, modifier.duration));
	}
	IEnumerator RemoveModifier(GameObject obj, float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(obj);
	}

	public void AddInfoPanelToQueue(string title, string text, Sprite image = null, string buttonText = "OK")
	{
		AddInfoPanelToQueue(new InfoPanelData(title, text, image, buttonText));
	}
	public void AddInfoPanelToQueue(InfoPanelData infoPanelData)
	{
		infoPanelQueue.Enqueue(infoPanelData);
		if (!IsInfoPanelActive) ShowNextInfoPanel();
	}

	public void CloseInfoPanel()
	{
		infoPanel.gameObject.SetActive(false);
		StartCoroutine(ShowNextInfoPanel(.4f));
	}
	IEnumerator ShowNextInfoPanel(float delay)
	{
		yield return new WaitForSeconds(delay);
		ShowNextInfoPanel();
	}
	void ShowNextInfoPanel()
	{
		InfoPanelData data;
		try
		{
			data = infoPanelQueue.Dequeue();
		}
		catch (System.Exception)
		{
			Debug.LogWarning("There are no entries in infoPanelQueue!");
			infoPanel.gameObject.SetActive(false);
			return;
		}

		infoPanel.Find("InfoTitle").GetComponent<TextMeshProUGUI>().text = data.title;
		infoPanel.Find("InfoText").GetComponent<TextMeshProUGUI>().text = data.text;
		if (data.image == null) infoPanel.Find("InfoImage").gameObject.SetActive(false);
		else
		{
			var img = infoPanel.Find("InfoImage");
			img.GetComponent<Image>().sprite = data.image;
			img.gameObject.SetActive(true);
		}

		infoPanel.gameObject.SetActive(true);
	}

	public struct InfoPanelData
	{
		public string title;
		public string text;
		public Sprite image;
		public string buttonText;

		public InfoPanelData(string title, string text, Sprite image = null, string buttonText = "OK")
		{
			this.title = title;
			this.text = text;
			this.image = image;
			this.buttonText = buttonText;
		}
	}
}