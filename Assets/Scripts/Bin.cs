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
		UpdateUI();
	}

	public int counter = 0;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.TryGetComponent(out Debris debris))
		{
			if (type != Debris.DebrisType.Communal)
			{
				if (debris.type == type)
				{
					counter++;
				}
				else
				{
					counter -= 5;
				}
				UpdateUI();
			}
		}
		Destroy(collision.gameObject);
	}

	void UpdateUI()
	{
		if (counterText == null) return;

		string countText = $"<b>{counter}</b>";
		if (counter < 0) countText = "<color=red>" + countText;
		counterText.text = $"{type}: {countText}";
	}
}