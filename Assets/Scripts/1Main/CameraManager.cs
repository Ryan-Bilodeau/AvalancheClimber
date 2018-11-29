using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to main camera
//Manages the main camera
//Relies on BuildingsMover
public class CameraManager : MonoBehaviour 
{
	public static bool CameraFollowingOnY;       //Set in BuildingsMover
	public static float Y_OFFSET = 5f;      //Where the player is on the camera
    private Vector3 PlayerPos;

    private void Awake()
    {
        InvokeRepeating("DisableWhenPlayerLoses", .5f, .5f);
    }

    private void Update ()
	{
        PlayerPos = SceneObjects.Body.transform.position;
        ManageCameraFollowing();
	}

    private void ManageCameraFollowing()
    {
        if (CameraFollowingOnY)
        {
            transform.position = new Vector3(PlayerPos.x, PlayerPos.y + Y_OFFSET, transform.position.z);
		}
        else
        {
            transform.position = new Vector3(PlayerPos.x, transform.position.y, transform.position.z);
        }
    }

	private void DisableWhenPlayerLoses()
	{
		if (EndGameManager.PlayerLost)
		{
			this.enabled = false;
			CancelInvoke();
		}
	}
}
