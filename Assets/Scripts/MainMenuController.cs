using UnityEngine;
using UnityEngine.SceneManagement;

public class UIcontroller : MonoBehaviour
{
	// Start is called before the first frame update
	public bool HoloActive = false;
	public AnimationClip InClip;
	public int ActiveTab = 0;
	public Animator HoloAnimator;
	public GameObject HoloObject;
	public AnimationClip clip;
	public GameObject[] UI;

	void Start()
	{
		ActiveTab = -1;
		//HoloAnimator = HoloObject.GetComponent<Animator>();
		//AnimationCurve curve = new AnimationCurve();
		//curve.AddKey(0.5f, 1f);

		//curve.CopyFrom(AnimationUtility.GetEditorCurve(InClip, "", typeof(RectTransform), "Position.x"));
		//curve.CopyFrom(AnimationUtility.GetAllCurves(InClip));
		//AnimationUtility.GetAllCurves(InClip);

		//clip = EditorGUILayout.ObjectField("Clip",clip,typeof(AnimationClip), false) as AnimationClip;
		//curve.CopyFrom(AnimationUtility.GetObjectReferenceCurve(InClip,));
		//Debug.Log(curve.keys.Length);
		//foreach (AnimationCurve key in AnimationUtility.GetCurveBindings(InClip))
		//{
		//    Debug.Log(key);
		//}

		//InClip.SetCurve("",typeof(RectTransform), "Position.x", curve);
	}

	void Update()
	{
	}
	public void PlayButton()
	{
		SceneManager.LoadScene("SampleScene");
	}
	public void Changed()
	{

	}

	public void ManualButtonFunc()
	{
		ActiveTab = 1;
		Debug.Log("Manual");
	}

	public void ShopButtonFunc()
	{
		//if (ActiveTab == 2)
		//{
		//    HoloAnimator.SetBool("open", false);
		//    ActiveTab = 0;
		//    Debug.Log("Close");
		//    HoloActive = false;
		//}
		//else 
		//{

		//    if (HoloActive == false)
		//    {
		//        HoloAnimator.SetBool("open", true);
		//        HoloActive = true;
		//    }

		//    ActiveTab = 2;
		//    Debug.Log("Shop");
		//}
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