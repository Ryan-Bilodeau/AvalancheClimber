using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.UI;

//Attach to main camera
//Manages handling UI buttons/states
//Used by ArmRotator 
public class CanvasButtonManger : MonoBehaviour
{
    public static bool Paused;

	public GameObject MuteButton;
	public Sprite IsMutedSprite;
	public Sprite IsntMutedSprite;

    private bool Muted;
    private float TimeScale;

    private void Awake()
    {
		Muted = PlayerPrefs.GetInt(PlayerPrefsKeys.MuteState) == 0 ? true : false;      //0 is off, 1 is on

		if (Muted)
			MuteButton.GetComponent<Image>().sprite = IsMutedSprite;
		else
			MuteButton.GetComponent<Image>().sprite = IsntMutedSprite;
    }

    public void OnRightButtonDown()
    {
        ArmRotator.RightButtonDown = true;
    }

    public void OnLeftButtonDown()
    {
        ArmRotator.LeftButtonDown = true;
    }

    public void OnRightButtonUp()
    {
        ArmRotator.RightButtonDown = false;
    }

    public void OnLeftButtonUp()
    {
        ArmRotator.LeftButtonDown = false;
    }

    public void OnJumpButtonDown()
    {
        PlayerJumper.JumpAreaClicked = true;
    }

    public void OnJumpButtonUp()
    {
        PlayerJumper.JumpAreaClicked = false;
    }

    public void OnRestartButtonDown()
    {
        StartCoroutine(ManageShowingAds(1));
    }

    public void OnExitButtonDown()
    {
        StartCoroutine(ManageShowingAds(0));
    }

    public void OnPauseButtonDown()
    {
        Paused = true;
        TimeScale = Time.timeScale;
        Time.timeScale = 0f;

        SceneObjects.PauseCanvas.SetActive(true);
        SceneObjects.UICanvas.SetActive(false);
    }

    public void OnResumeButtonDown()
    {
        Paused = false;
        Time.timeScale = TimeScale;

        SceneObjects.PauseCanvas.SetActive(false);
        SceneObjects.UICanvas.SetActive(true);
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

    private IEnumerator ManageShowingAds(int sceneBuildIndex)
    {
        Paused = false;
        AudioManager.Instance.StopAllSoundEffects();

        if (Advertisement.IsReady() && ManageStartingPositions.Instance.AdCounter % 2 == 0)
        {
            Advertisement.Show();
            SceneObjects.AdCanvas.SetActive(true);
        }

        while (Advertisement.isShowing)
            yield return null;

        SceneObjects.AdCanvas.SetActive(false);
        Time.timeScale = TimeScale;
        SceneManager.LoadScene(sceneBuildIndex);
    }
}
