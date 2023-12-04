using NohaSoftware.Utilities;
using System.Collections;
using System.Collections.Generic;
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
	public TextMeshProUGUI capacityText;
	public BinSwapper swapper;
	public RectTransform modifierList;

	[Header("Info Panel")]
	[SerializeField] RectTransform infoPanel;
	Queue<InfoPanelData> infoPanelQueue = new();
	bool IsInfoPanelActive => infoPanel.gameObject.activeSelf;

	[Header("Strikes")]
	[SerializeField] RectTransform strikeHolder;
	[SerializeField] Sprite fullStrike, emptyStrike;

	void Update()
	{
		//counter text
		counterText.text = $"{GameManager.instance.counter} ({GameManager.instance.points})";
		var values = counterText.GetPreferredValues(counterText.text);
		var rect = counterText.transform.parent.GetComponent<RectTransform>();
		rect.SetHeight(values.y + 50f);
		rect.SetWidth(values.x + 50f);

		//capacity text
		capacityText.text = $"{GameManager.instance.binFilled} / {GameManager.instance.binCapacity}";
		capacityText.color = GameManager.instance.binFilled >= GameManager.instance.binCapacity * .85f ? Color.red : Color.black;
		values = capacityText.GetPreferredValues(capacityText.text);
		rect = capacityText.transform.parent.GetComponent<RectTransform>();
		rect.SetHeight(values.y + 50f);
		rect.SetWidth(values.x + 50f);

		//strike icons
		for (int i = 0; i < 3; i++)
		{
			strikeHolder.GetChild(i).GetComponent<Image>().sprite = i >= GameManager.instance.strikes ? emptyStrike : fullStrike;
		}
	}

	public void AddModifier(Modifier modifier)
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

		StartCoroutine(RemoveModifier(obj, modifier.duration, modifier.id));
	}
	IEnumerator RemoveModifier(GameObject obj, float time, string id)
	{
		yield return new WaitForSeconds(time);
		Destroy(obj);
		ModifierManager.instance.HandleModifierEnd(id);
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
	public void ShowGameOver(bool strikes)
	{
		string text = strikes ? "Sajnos, számodra véget ért a játék, mert háromszor is rosszul szelektáltál. Egyet se csüggedj, próbáld hát újra!" : "Megtelt az ûrhajód rakodótere, így kénytelen vagy hazatérni, hogy kiürítsd. Érdemes lenne megfontolni a tárolóegységed fejlesztését! (bolt)";
		Sprite icon = strikes ? fullStrike : null;
		infoPanelQueue.Clear();
		infoPanel.gameObject.SetActive(false);
		infoPanel.Find("ConfirmButton").GetComponent<Button>().onClick.AddListener(() => GameManager.instance.ExitGame());
		AddInfoPanelToQueue("Játék vége!", text + $"\n\nA kör pontszáma: {GameManager.instance.counter}", icon, "Kilépés");
	}

	public void CloseInfoPanel()
	{
		infoPanel.gameObject.SetActive(false);
		GameManager.instance.paused = false;
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
			GameManager.instance.paused = false;
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
		GameManager.instance.paused = true;
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