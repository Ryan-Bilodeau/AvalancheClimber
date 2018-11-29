using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

//Attach to FreeRopeButton
//Manages showing rewarded ads
public class RopeButtonManager : MonoBehaviour
{
    private bool PlayingAnimation;
    private int StartingRopeFontSize;

    private Color TempColor;

    private void Start()
    {
        StopAllCoroutines();
    }

    public void OnGetFreeRopeButtonClick()
    {
        if (!PlayingAnimation)
        {
            //PausedCanvas
            transform.parent.GetChild(5).gameObject.SetActive(true);
            ShowAd("rewardedVideo");
        }
    }

	private void ShowAd(string zone = "")
	{
		if (string.Equals(zone, ""))
			zone = null;

		ShowOptions options = new ShowOptions();
		options.resultCallback = AdCallbackhandler;

		if (Advertisement.IsReady(zone))
			Advertisement.Show(zone, options);

        if (Advertisement.isShowing)
            StartCoroutine(PauseGameWhileShowing());
	}

	private void AdCallbackhandler(ShowResult result)
	{
		switch (result)
		{
			case ShowResult.Finished:
                StartCoroutine(IncreaseRopeCount());
                PlayingAnimation = true;
				break;
		}
	}

	private IEnumerator PauseGameWhileShowing()
	{
		float currentTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		yield return null;

		while (Advertisement.isShowing)
            yield return null;

        //PausedCanvas
        transform.parent.GetChild(5).gameObject.SetActive(false);
		Time.timeScale = currentTimeScale;
	}

    //Called when rewarded ad has been watched fully
    private IEnumerator IncreaseRopeCount()
    {
        transform.parent.GetChild(3).gameObject.SetActive(true);
        StartingRopeFontSize = transform.parent.GetChild(3).GetChild(2).GetComponent<Text>().fontSize;

		transform.parent.GetChild(3).GetChild(2).GetComponent<Text>().text =
			PlayerPrefs.GetInt(PlayerPrefsKeys.Ropes).ToString();
        PlayerPrefs.SetInt(PlayerPrefsKeys.Ropes, PlayerPrefs.GetInt(PlayerPrefsKeys.Ropes) + 1);

		for (int i = 0; i < transform.parent.GetChild(3).childCount; i++)
		{
			if (transform.parent.GetChild(3).GetChild(i).GetComponent<Text>() != null)
			{
				TempColor = transform.parent.GetChild(3).GetChild(i).GetComponent<Text>().color;
				TempColor.a = 1f;
				transform.parent.GetChild(3).GetChild(i).GetComponent<Text>().color = TempColor;
			}
			else
			{
				TempColor = transform.parent.GetChild(3).GetChild(i).GetComponent<Image>().color;
				TempColor.a = 1f;
				transform.parent.GetChild(3).GetChild(i).GetComponent<Image>().color = TempColor;
			}
		}

        yield return new WaitForSeconds(1f);

		transform.parent.GetChild(3).GetChild(2).GetComponent<Text>().text =
			PlayerPrefs.GetInt(PlayerPrefsKeys.Ropes).ToString();
        transform.parent.GetChild(3).GetChild(2).GetComponent<Text>().fontSize += 20;

        for (;;)
        {
            if (transform.parent.GetChild(3).GetChild(2).GetComponent<Text>().fontSize > StartingRopeFontSize)
            {
                transform.parent.GetChild(3).GetChild(2).GetComponent<Text>().fontSize -= 1;
            }
            else
                break;

            yield return new WaitForSeconds(.02f);
        }

        StartCoroutine(FadeRopeContainer());
    }

    //Called in IncreaseRopeCount
    private IEnumerator FadeRopeContainer()
    {
        float Timer = 0;

        for (;;)
        {
            if (Timer <= 2)
            {
                for (int i = 0; i < transform.parent.GetChild(3).childCount; i++)
                {
                    if (transform.parent.GetChild(3).GetChild(i).GetComponent<Text>() != null)
                    {
                        TempColor = transform.parent.GetChild(3).GetChild(i).GetComponent<Text>().color;
                        TempColor.a -= .025f;
                        transform.parent.GetChild(3).GetChild(i).GetComponent<Text>().color = TempColor;
                    }
                    else
                    {
                        TempColor = transform.parent.GetChild(3).GetChild(i).GetComponent<Image>().color;
                        TempColor.a -= .025f;
                        transform.parent.GetChild(3).GetChild(i).GetComponent<Image>().color = TempColor;
                    }
                }
            }
            else
                break;
            
            Timer += .05f;

            yield return new WaitForSeconds(.05f);
        }

        PlayingAnimation = false;
        transform.parent.GetChild(3).gameObject.SetActive(false);
    }
}
