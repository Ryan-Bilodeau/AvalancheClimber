using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartMenuManager : MonoBehaviour
{
    public static bool ShowRestartMenu;

    private Color TempColor;
    private int StartingRopeFontSize;

    private void Awake()
    {
        CancelInvoke();
        StopAllCoroutines();

        SetStartingColors();

        transform.GetChild(2).GetChild(2).GetComponent<Text>().text = PlayerPrefs.GetInt(PlayerPrefsKeys.Ropes).ToString();
        StartingRopeFontSize = transform.GetChild(2).GetChild(2).GetComponent<Text>().fontSize;

        InvokeRepeating("UpdateRestartMenuState", .2f, .2f);
        ShowRestartMenu = false;
        gameObject.SetActive(false);
    }

    #region Methods
    //Call at start of awake
    private void SetStartingColors()
    {
		foreach (Transform child in transform)
		{
            if (child.GetComponent<Image>() != null || child.GetComponent<Text>() != null)
            {
                if (child.GetComponent<Image>() != null)
                    TempColor = child.GetComponent<Image>().color;
                else
                    TempColor = child.GetComponent<Text>().color;

                child.gameObject.AddComponent<ManageRestartMenuObjectTransperncy>().SetStartingColor(TempColor);
            }

			if (child.childCount > 0)
			{
				foreach (Transform kid in child)
				{
					if (kid.GetComponent<Image>() != null || kid.GetComponent<Text>() != null)
					{
						if (kid.GetComponent<Image>() != null)
							TempColor = kid.GetComponent<Image>().color;
						else
							TempColor = kid.GetComponent<Text>().color;

						kid.gameObject.AddComponent<ManageRestartMenuObjectTransperncy>().SetStartingColor(TempColor);
					}
				}
			}
		}
    }

	//Gets called in CallFadeInAfterDelay
	private void FadeInFromTransparent()
	{
		foreach (Transform child in transform)
		{
			if (child.childCount > 0)
			{
				foreach (Transform kid in child)
				{
					if (kid.GetComponent<ManageRestartMenuObjectTransperncy>() != null)
						kid.GetComponent<ManageRestartMenuObjectTransperncy>().ManageFadeIn();
				}
			}

			if (child.GetComponent<ManageRestartMenuObjectTransperncy>() != null)
				child.GetComponent<ManageRestartMenuObjectTransperncy>().ManageFadeIn();
		}
	}
    #endregion

    #region Coroutines
    //Called in in UpdateRestartMenuState
    private IEnumerator CallFadeInAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);

        FadeInFromTransparent();
    }

	//Called in in UpdateRestartMenuState
	private IEnumerator IncreaseRopeCount()
    {
        yield return new WaitForSeconds(3.5f);

        transform.GetChild(2).GetChild(2).GetComponent<Text>().text = PlayerPrefs.GetInt(PlayerPrefsKeys.Ropes).ToString();
        transform.GetChild(2).GetChild(2).GetComponent<Text>().fontSize = StartingRopeFontSize + 20;

        for (;;)
        {
            if (transform.GetChild(2).GetChild(2).GetComponent<Text>().fontSize > StartingRopeFontSize)
            {
                transform.GetChild(2).GetChild(2).GetComponent<Text>().fontSize -= 1;
            }
            else
                break;

            yield return new WaitForSeconds(.02f);
        }
    }
    #endregion

    #region InvokeRepeating
    //Needs to be called each time 1Main is loaded
    private void UpdateRestartMenuState()
    {
        if (ShowRestartMenu)
        {
            gameObject.SetActive(true);

            if (EndGameManager.PlayerWon)
            {
				StartCoroutine(IncreaseRopeCount());
                transform.GetChild(1).GetComponent<Text>().text = "You Won";
            }
            else if (EndGameManager.PlayerLost)
            {
                transform.GetChild(1).GetComponent<Text>().text = "You Died";
            }

            StartCoroutine(CallFadeInAfterDelay());

            TempColor = Color.white;
            TempColor.a = 0f;

			foreach (Transform child in transform)
			{
				if (child.GetComponent<Image>() != null || child.GetComponent<Text>() != null)
				{
					if (child.GetComponent<Image>() != null)
						child.GetComponent<Image>().color = TempColor;
					else
						child.GetComponent<Text>().color = TempColor;
				}

				if (child.childCount > 0)
				{
					foreach (Transform kid in child)
					{
						if (kid.GetComponent<Image>() != null || kid.GetComponent<Text>() != null)
						{
							if (kid.GetComponent<Image>() != null)
								kid.GetComponent<Image>().color = TempColor;
							else
								kid.GetComponent<Text>().color = TempColor;
						}
					}
				}
			}

            CancelInvoke("UpdateRestartMenuState");
        }
        else
            gameObject.SetActive(false);
    }
    #endregion
}

//Gets attached to each child in the restart menu with an Image or Text component
public class ManageRestartMenuObjectTransperncy : MonoBehaviour
{
    private Color StartingColor;
    private Color TempColor;
    private float IncreaseAmount;

    public void ManageFadeIn()
    {
        if (gameObject.GetComponent<Image>() != null)
            StartCoroutine(FadeInImage());
        else
            StartCoroutine(FadeInText());
    }

    public void SetStartingColor(Color color)
    {
        StartingColor = color;
        TempColor = color;
        IncreaseAmount = color.a / 75f;
    }

    private IEnumerator FadeInImage()
    {
        TempColor.a = 0f;
        gameObject.GetComponent<Image>().color = TempColor;

        for (;;)
        {
            if (gameObject.GetComponent<Image>().color.a < StartingColor.a)
            {
                TempColor.a += IncreaseAmount;
                gameObject.GetComponent<Image>().color = TempColor;
            }
            else
                break;

            yield return new WaitForSeconds(.02f);
        }
    }

	private IEnumerator FadeInText()
	{
		TempColor.a = 0f;
		gameObject.GetComponent<Text>().color = TempColor;

		for (;;)
		{
			if (gameObject.GetComponent<Text>().color.a < StartingColor.a)
			{
				TempColor.a += IncreaseAmount;
				gameObject.GetComponent<Text>().color = TempColor;
			}
			else
				break;

			yield return new WaitForSeconds(.02f);
		}
	}
}
