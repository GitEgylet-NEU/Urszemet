using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	public int ActiveTab = 0;
	public Animator HoloAnimator;
	public GameObject[] UI;
	public TextMeshProUGUI versionText;

	void Start()
	{
		ActiveTab = -1;
		versionText.text = 'v' + PlayerSettings.bundleVersion;
	}

	
	public void PlayButton()
	{
		SceneManager.LoadScene("SampleScene");
	}
	public void OpenURLInBrowser(string url)
	{
		Application.OpenURL(url);
	}

	//gombonyaskor meghiv int button sorszammal
	public void HoloMove(int pressed)
	{
		if (pressed != ActiveTab)
		{
			//megnyitni animalva, nincs meg
			if (ActiveTab == -1)
			{

				//animalas
				//HoloObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, Screen.width, 0);
				HoloAnimator.SetBool("open", true);
			}
			else
			{
				UI[ActiveTab].SetActive(false);
			}
			// active ui window state megvaltoztatasa

			UI[pressed].SetActive(true);
			ActiveTab = pressed;

		}
		else
		{
			//ujra megnyomja: bezarodik
			HoloAnimator.SetBool("open", false);
			UI[ActiveTab].SetActive(false);
			ActiveTab = -1;
		}

	}

}