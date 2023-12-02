using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIcontroller : MonoBehaviour
{
    // Start is called before the first frame update
    public bool HoloActive = false;
    public AnimationClip InClip;
    public int ActiveTab = 0;
    public Animator HoloAnimator;
    public GameObject HoloObject;

    void Start()
    {
        ActiveTab = 0;
        HoloAnimator = HoloObject.GetComponent<Animator>();  
      // InClip.SetCurve("",typeof(RectTransform), "Position.x", curve);
    }

    // Update is called once per frame
    void Update()
    {
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
        if (ActiveTab == 2)
        {
            HoloAnimator.SetBool("open", false);
            ActiveTab = 0;
            Debug.Log("Close");
            HoloActive = false;
        }
        else 
        {

            if (HoloActive == false)
            {
                HoloAnimator.SetBool("open", true);
                HoloActive = true;
            }

            ActiveTab = 2;
            Debug.Log("Shop");
        }
    }

    public void PlayButtonFunc()
    {
        ActiveTab = 3;
        Debug.Log("Play");
        SceneManager.LoadScene("SampleScene");
    }

    public void SettingsButtonFunc()
    {
        ActiveTab = 5;
        Debug.Log("Settings");
    }

    public void CreditsButtonFunc()
    {
        ActiveTab = 3;
        Debug.Log("Credits");
    }

}
