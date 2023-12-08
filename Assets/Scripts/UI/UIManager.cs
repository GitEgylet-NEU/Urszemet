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
	public RectTransform abilityList;
	Dictionary<string, GameObject> abilityObjects;

	[Header("Info Panel")]
	[SerializeField] RectTransform infoPanel;
	Queue<InfoPanelData> infoPanelQueue = new();
	bool IsInfoPanelActive => infoPanel.gameObject.activeSelf;

	[Header("Strikes")]
	[SerializeField] RectTransform strikeHolder;
	[SerializeField] Sprite fullStrike, emptyStrike;

	bool exit = false;

	private void Start()
	{
		abilityObjects = new();
		abilityList.GetChild(0).gameObject.SetActive(false);
		foreach (var ability in GameManager.instance.gameSettings.modifierSettings.abilities)
		{
			GameObject obj = Instantiate(abilityList.GetChild(0), abilityList).gameObject;
			obj.name = ability.id;
			obj.transform.GetChild(0).GetComponent<Image>().sprite = ability.icon;
			obj.SetActive(true);
			abilityObjects.Add(ability.id, obj);
			obj.GetComponent<Button>().onClick.AddListener(() => ActivateAbility(ability.id));
		}

		UpdateAbilityObjects();
	}
	public void ActivateAbility(string id)
	{
		if (!InventoryManager.instance.SubtractItem(id, 1))
		{
			Debug.LogError($"Can't afford {id}, aborting");
			return;
		}
		abilityObjects[id].GetComponent<Image>().color = Color.green;
		abilityObjects[id].GetComponent<Button>().interactable = false;
		ModifierManager.instance.HandleModifierStart(id);
		float duration = GameManager.instance.gameSettings.modifierSettings.GetAbility(id).duration;
		UpdateAbilityObjects();
		StartCoroutine(DeactivateAbility(id, duration));
	}
	IEnumerator DeactivateAbility(string id, float t)
	{
		yield return new WaitForSeconds(t);
		abilityObjects[id].GetComponent<Image>().color = Color.white;
		abilityObjects[id].GetComponent<Button>().interactable = true;
		ModifierManager.instance.HandleModifierEnd(id);
		UpdateAbilityObjects();
		StartCoroutine(AbilityCooldown(id));
	}
	IEnumerator AbilityCooldown(string id)
	{
		if (InventoryManager.instance.GetItem(id) <= 0) yield break;
		abilityObjects[id].GetComponent<Button>().interactable = false;
		yield return new WaitForSeconds(GameManager.instance.gameSettings.modifierSettings.abilityCooldown);
		abilityObjects[id].GetComponent<Button>().interactable = true;
	}
	public void UpdateAbilityObjects()
	{
		foreach (var obj in abilityObjects)
		{
			int quantity = InventoryManager.instance.GetItem(obj.Key);
			obj.Value.transform.Find("QuantityCircle").GetChild(0).GetComponent<TextMeshProUGUI>().text = quantity.ToString();
			if (obj.Value.GetComponent<Image>().color == Color.green) continue;
			if (quantity <= 0)
			{
				obj.Value.GetComponent<Image>().color = Color.red;
				obj.Value.GetComponent<Button>().interactable = false;
			}
			else
			{
				obj.Value.GetComponent<Image>().color = Color.white;
				obj.Value.GetComponent<Button>().interactable = true;
			}
		}
	}

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
	public void StartAbility(Modifier modifier)
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
	public void AddInfoPanelToQueue(InfoPanelData infoPanelData, bool first = false)
	{
		if (first)
		{
			var q = infoPanelQueue.ToArray();
			infoPanelQueue.Clear();
			infoPanelQueue.Enqueue(infoPanelData);
			foreach (var item in q) infoPanelQueue.Enqueue(item);
		}
		else infoPanelQueue.Enqueue(infoPanelData);
		if (!IsInfoPanelActive) ShowNextInfoPanel();
	}
	public void ShowGameOver(bool strikes)
	{
		if (strikes == true)
        {
			AudioManager.instance.PlaySound("gameover");
        }
		string text = strikes ? "Sajnos, számodra véget ért a játék, mert háromszor is rosszul szelektáltál. Egyet se csüggedj, próbáld hát újra!" : "Megtelt az ûrhajód rakodótere, így kénytelen vagy hazatérni, hogy kiürítsd. Érdemes lenne megfontolni a tárolóegységed fejlesztését! (bolt)";
		Sprite icon = strikes ? fullStrike : null;
		infoPanelQueue.Clear();
		infoPanel.gameObject.SetActive(false);
		exit = true;
		AddInfoPanelToQueue("Játék vége!", text + $"\n\nA kör pontszáma: {GameManager.instance.counter}", icon, "Kilépés");
	}
	public void ShowPauseMenu()
	{
		infoPanel.gameObject.SetActive(false);
		exit = true;
		AddInfoPanelToQueue(new("Szüneteltetés", "Megállítottad a játékot. Most lehetõséged van visszatérni a bázisodra és eladni az eddig összegyûjtött szemetet.", null, "Fõmenü", true), true);
	}
	public void CloseInfoPanel(bool confirm)
	{
		if (exit && confirm) GameManager.instance.ReturnToMainMenu();
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
			//Debug.LogWarning("There are no entries in infoPanelQueue!");
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
		infoPanel.Find("ConfirmButton").GetChild(0).GetComponent<TextMeshProUGUI>().text = data.buttonText;
		infoPanel.Find("CloseButton").gameObject.SetActive(data.showCloseButton);

		infoPanel.gameObject.SetActive(true);
		GameManager.instance.paused = true;
	}

	public struct InfoPanelData
	{
		public string title;
		public string text;
		public Sprite image;
		public string buttonText;
		public bool showCloseButton;

		public InfoPanelData(string title, string text, Sprite image = null, string buttonText = "OK", bool showCloseButton = false)
		{
			this.title = title;
			this.text = text;
			this.image = image;
			this.buttonText = buttonText;
			this.showCloseButton = showCloseButton;
		}
	}
}