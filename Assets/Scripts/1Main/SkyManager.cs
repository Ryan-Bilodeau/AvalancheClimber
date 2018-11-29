using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to sky
//Manages sky position and stuff
public class SkyManager : MonoBehaviour 
{
    public static GameObject EndOfSky;
    private Vector2 NewPos;
    private Vector2 StartingPos;

    private void Start()
    {
        NewPos = transform.position;
        StartingPos = transform.position;
        EndOfSky = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        NewPos.x = SceneObjects.Body.transform.position.x;

        if (EndGameManager.PlayerLost)
            FollowPlayerYPos();
        
        transform.position = NewPos;

        if (EndGameManager.DoneCentering)
            enabled = false;
    }

	private void FollowPlayerYPos()
	{
		if (!EndGameManager.DoneCentering)
		{
            if (SceneObjects.Body.transform.position.y < EndGameManager.BodyDeathPos.y)
            {
				NewPos.y = StartingPos.y - ((Mathf.Abs(SceneObjects.Body.transform.position.y) -
									Mathf.Abs(EndGameManager.BodyDeathPos.y)) * .8f);
            }
        }
	}
}
