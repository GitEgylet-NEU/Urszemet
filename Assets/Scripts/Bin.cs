using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bin : MonoBehaviour
{
	public Debris.DebrisType type;
	public TextMeshProUGUI counterText;

	Collider2D c;
	private void Awake()
	{
		c = GetComponent<Collider2D>();
	}

	private void Start()
	{
		//UpdateUI();
		var data = GameManager.instance.gameData.debrisTypeData.GetData(type);
		shouldCount = data.shouldCount;
		GetComponent<SpriteRenderer>().color = data.color;
	}

	bool shouldCount = true;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (shouldCount && collision.gameObject.TryGetComponent(out Debris debris))
		{
			if (debris.type == type)
			{
				GameManager.instance.counter[(int)type]++;
			}
			else
			{
				GameManager.instance.counter[(int)type] -= 5;
			}
			//UpdateUI();
		}
		Destroy(collision.gameObject);
	}

	void UpdateUI()
	{
		if (counterText == null) return;

		string countText = $"<b>{GameManager.instance.counter[(int)type]}</b>";
		if (GameManager.instance.counter[(int)type] < 0) countText = "<color=red>" + countText;
		counterText.text = $"{type}: {countText}";
	}
}