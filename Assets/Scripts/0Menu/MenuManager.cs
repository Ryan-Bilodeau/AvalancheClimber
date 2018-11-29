using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Attach to main camera in 0Menu
//Manages menu buttons
public class MenuManager : MonoBehaviour
{
    public static bool PlayClicked;
    public static bool Muted;       //If muted, no audio will play

    public GameObject MuteButton;
    public Sprite IsMutedSprite;
    public Sprite IsntMutedSprite;

    private void Awake()
    {
		//Create PlayerPrefs if they aren't created already
		if (!PlayerPrefs.HasKey(PlayerPrefsKeys.TotalWins))
			PlayerPrefs.SetInt(PlayerPrefsKeys.TotalWins, 0);

		if (!PlayerPrefs.HasKey(PlayerPrefsKeys.QuickestWin))
			PlayerPrefs.SetFloat(PlayerPrefsKeys.QuickestWin, 0f);

		if (!PlayerPrefs.HasKey(PlayerPrefsKeys.Ropes))
			PlayerPrefs.SetInt(PlayerPrefsKeys.Ropes, 0);

        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.MuteState))
            PlayerPrefs.SetInt(PlayerPrefsKeys.MuteState, 1);

        Muted = PlayerPrefs.GetInt(PlayerPrefsKeys.MuteState) == 0 ? true : false;      //0 is off, 1 is on

        if (Muted)
            MuteButton.GetComponent<Image>().sprite = IsMutedSprite;
        else
            MuteButton.GetComponent<Image>().sprite = IsntMutedSprite;
    }

    #region Button calls
    public void OnPlayButtonDown()
    {
        PlayClicked = true;

        if (!StartingObjectSpawner.Instance.Loading && PlayClicked)
        {
            SceneManager.LoadScene(1);
        }
    }

    public void OnPlayButtonUp()
    {
        PlayClicked = false;
    }

    public void OnMuteButtonUp()
    {
        Muted = !Muted;

        if (Muted)
        {
            AudioListener.volume = 0f;
            MuteButton.GetComponent<Image>().sprite = IsMutedSprite;
        }
        else
        {
            AudioListener.volume = 1f;
            MuteButton.GetComponent<Image>().sprite = IsntMutedSprite;
        }

        PlayerPrefs.SetInt(PlayerPrefsKeys.MuteState, PlayerPrefs.GetInt(PlayerPrefsKeys.MuteState) == 0 ? 1 : 0);
    }

    public void OnButtonDownPlaySound()
    {
        AudioManager.Instance.JumpSource.Play();
    }

    public void OnLeaderboardButtonUp()
    {
        #if !UNITY_EDITOR
        if (LeaderboardManager.LoggedIn)
            Social.ShowLeaderboardUI();
		#endif
    }
    #endregion
}
