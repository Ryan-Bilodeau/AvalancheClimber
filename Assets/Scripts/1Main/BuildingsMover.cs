using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to Buildings
//Manages bulidings parallaxing
//Relies on UIManager
public class BuildingsMover : MonoBehaviour 
{
    public float Y_OFFSET_MULTIPLIER;

    private Vector3 BodyStartingPos;
    private Vector3 BodyPos;
    private float DistFromOrigin;

    private void Start()
    {
        BodyStartingPos = new Vector3(0f, -5f, 0f);
    }

    private void Update()
    {
        if (UIManager.Altitude < 25f && !EndGameManager.PlayerWon)
        {
			BodyPos = SceneObjects.Body.transform.position;
			DistFromOrigin = Mathf.Abs(Mathf.Abs(BodyStartingPos.y) + BodyPos.y);
			MoveBuildings();
        }
    }

    private void MoveBuildings()
    {
        if (BodyPos.y < BodyStartingPos.y)
        {
            transform.position = new Vector3(SceneObjects.Body.transform.position.x,
                (SceneObjects.Body.transform.position.y - BodyStartingPos.y) + (DistFromOrigin * Y_OFFSET_MULTIPLIER),
                                         SceneObjects.Body.transform.position.z);

            CameraManager.CameraFollowingOnY = true;
        }
        else
        {
            transform.position = new Vector3(BodyPos.x, transform.position.y, transform.position.z);
            CameraManager.CameraFollowingOnY = false;
        }
    }
}
