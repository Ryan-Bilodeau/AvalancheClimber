using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//Attach to MainCamera
//Handles stuff to do with ending the game
public class EndGameManager : MonoBehaviour 
{
    [HideInInspector]
    public delegate void OnPlayerLostDel();

    //Gets called when player loses
    public static OnPlayerLostDel OnPlayerLost;

    public static bool PlayerWon;       //Set in RopeMover
    public static bool PlayerLost;
    public static Vector2 BodyDeathPos;
    public static bool DoneCentering = false;
    public static float TimeSinceStart;

    public float ShrinkSize;

    private Color DeathColor;
    private Vector3 CameraPos;
    private Vector3 BodyPos;
    private float LERP_TIME = 1f;
    private float XPos;
    private float Ypos;
    private float StartingCameraSize;
    private bool ShouldEnlarge = true;
    private int Timer;
    private float TimeModifier;
    private int Counter;

    private void Start()
    {
        PlayerWon = false;
        PlayerLost = false;
        DoneCentering = false;
        StartingCameraSize = Camera.main.orthographicSize;
        DeathColor = new Color();
        DeathColor.r = 142f / 255f;
        DeathColor.g = 169f / 255f;
        DeathColor.b = 231f / 255f;
        DeathColor.a = 1f;
        InvokeRepeating("UpdatePlayerLost", .5f, .5f);          //Stops calling when done
        InvokeRepeating("CallWhenPlayerWon", .1f, .1f);         //Stops calling when done
    }

    void Update ()
	{
        if(PlayerWon)
        {
            ManageCamera();
        }
        if (PlayerLost)
        {
            ManageCameraWhenDead();
            TurnPlayerBlue();
        }
    }

	#region Methods called when PlayerLost
	private void ManageCameraWhenDead()
	{
		if (!Camera.main.GetComponent<CameraManager>().enabled && !DoneCentering)
		{
			TimeModifier = Time.deltaTime * 60f;
			CameraPos = Camera.main.transform.position;
			BodyPos = SceneObjects.Body.transform.position;
			Camera.main.transform.position = new Vector3(BodyPos.x,
														BodyPos.y + (5f * (1f - (Counter / 180f))),
														 CameraPos.z);
			Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, ShrinkSize + 2f, TimeModifier * .02f);
			if (Counter < 180)
				Counter++;

			if ((Counter > 179 && Camera.main.orthographicSize <= ShrinkSize + 2.2f) ||
				SceneObjects.Body.transform.position.y - 15f < SkyManager.EndOfSky.transform.position.y)
			{
				DoneCentering = true;
				SceneObjects.Sky.transform.SetParent(Camera.main.transform);
				SceneObjects.StarsManager.transform.SetParent(Camera.main.transform);
			}
		}
	}

	private void TurnPlayerBlue()
	{
		foreach (Transform child in SceneObjects.Ragdoll.transform)
		{
			if (child.GetComponent<SpriteRenderer>() != null)
			{
				child.GetComponent<SpriteRenderer>().color =
					Color.Lerp(child.GetComponent<SpriteRenderer>().color, DeathColor, TimeModifier * .02f);
			}

			if (child.childCount > 0)
			{
				if (child.GetChild(0).GetComponent<SpriteRenderer>() != null)
				{
					child.GetChild(0).GetComponent<SpriteRenderer>().color =
					Color.Lerp(child.GetChild(0).GetComponent<SpriteRenderer>().color, DeathColor, TimeModifier * .02f);
				}
			}
		}
	}
    #endregion

    #region Methods called when PlayerWon
    private void ManageCamera()
	{
		Camera.main.GetComponent<CameraManager>().enabled = false;

		if (ShouldEnlarge)
			EnlargeCamera();
		else
			ShrinkCamera();
	}

    //Only called by ManageCamera method
	private void ShrinkCamera()
	{
		XPos =
			Mathf.Lerp(Camera.main.transform.position.x, SceneObjects.Body.transform.position.x, Time.deltaTime * LERP_TIME);
		Ypos =
			Mathf.Lerp(Camera.main.transform.position.y, SceneObjects.Body.transform.position.y, Time.deltaTime * LERP_TIME);

		Camera.main.transform.position = new Vector3(XPos, Ypos, Camera.main.transform.position.z);
		Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, ShrinkSize, Time.deltaTime * LERP_TIME);
	}

    //Only called by ManageCamera method
	private void EnlargeCamera()
	{
		Camera.main.orthographicSize =
			Mathf.Lerp(Camera.main.orthographicSize, StartingCameraSize + .5f, Time.deltaTime * LERP_TIME * 10f);

		if (Timer < 20)
			Timer++;
		else
			ShouldEnlarge = false;
	}
    #endregion

    #region Call as InvokeRepeating
    private void UpdatePlayerLost()
	{
		if (UIManager.Altitude > 100 && !PlayerLost)
		{
			PlayerLost = true;
            OnPlayerLost();

			AudioManager.Instance.PlayDeathClip();
			BodyDeathPos = SceneObjects.Body.transform.position;
			SceneObjects.Arrow.SetActive(false);            //Called in here instead of ArrowManager for efficency
            SceneObjects.UICanvas.SetActive(false);
			SceneObjects.JumpArea.SetActive(false);
			SceneObjects.Body.GetComponent<SpriteRenderer>().sprite
						= CharacterSpriteManager.DeathSprite;           //Called in here for efficency
			Physics2D.gravity = new Vector2(0f, 0f);
            RestartMenuManager.ShowRestartMenu = true;
            ManageStartingPositions.Instance.AdCounter += 1;

			foreach (Transform child in SceneObjects.Ragdoll.transform)
			{
				if (child.gameObject.GetComponent<Rigidbody2D>() != null)
				{
					child.gameObject.GetComponent<Rigidbody2D>().drag = 0f;
				}
				if (child.childCount > 0)
				{
					if (child.GetChild(0).GetComponent<Rigidbody2D>() != null)
					{
						child.GetChild(0).GetComponent<Rigidbody2D>().drag = 0f;
					}
				}
			}

			SceneObjects.Body.GetComponent<Rigidbody2D>().AddTorque(
				Mathf.Sign(SceneObjects.Body.GetComponent<Rigidbody2D>().angularVelocity) * 10f);
			SceneObjects.RightArm.GetComponent<Rigidbody2D>().AddTorque(
				Mathf.Sign(SceneObjects.RightArm.GetComponent<Rigidbody2D>().angularVelocity) * 10f);
			SceneObjects.LeftArm.GetComponent<Rigidbody2D>().AddTorque(
				Mathf.Sign(SceneObjects.LeftArm.GetComponent<Rigidbody2D>().angularVelocity) * 10f);

			CancelInvoke("UpdatePlayerLost");
		}
	}

	private void CallWhenPlayerWon()
	{
        if (PlayerWon)
        {
            TimeSinceStart = (float)Math.Round(Time.timeSinceLevelLoad, 2);
            PlayerPrefs.SetInt(PlayerPrefsKeys.TotalWins, PlayerPrefs.GetInt(PlayerPrefsKeys.TotalWins) + 1);
            PlayerPrefs.SetFloat(PlayerPrefsKeys.QuickestWin, TimeSinceStart);
            PlayerPrefs.SetInt(PlayerPrefsKeys.Ropes, PlayerPrefs.GetInt(PlayerPrefsKeys.Ropes) + 1);
            ManageStartingPositions.Instance.AdCounter += 1;

            AudioManager.Instance.PlayWinClip();

            SceneObjects.UICanvas.SetActive(false);
            RestartMenuManager.ShowRestartMenu = true;

            CancelInvoke("CallWhenPlayerWon");
        }
    }
    #endregion
}
