using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bin : MonoBehaviour
{
	public Debris.DebrisType type;
	bool shouldCount = true;

	private void Start()
	{
		//UpdateUI();
		var data = GameManager.instance.gameSettings.debrisTypeData.GetData(type);
		shouldCount = data.shouldCount;
		GetComponent<SpriteRenderer>().color = data.color;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.TryGetComponent(out Debris debris) && !debris.IsSpecial)
		{
			if (shouldCount)
			{
				if (debris.type == type)
				{
					GameManager.instance.counter++;
				}
				else
				{
					//GameManager.instance.counter--;
					GameManager.instance.strikes--;
					if (GameManager.instance.strikes > 0) AudioManager.instance.PlaySound("strike");
				}
			}
			GameManager.instance.binFilled++;
		}
		Destroy(collision.gameObject);
	}
}