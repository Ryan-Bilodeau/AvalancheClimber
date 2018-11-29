using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to PlayButton
//Manages play button animation
public class PlayButtonPulse : MonoBehaviour 
{
    public float EnlargedScale;
    public float StartingScale;
    public float ChangeRate;

    private bool Increasing = true;

    private Vector3 NewSize;
	
	void Update ()
	{
        NewSize = transform.localScale;

        if (Increasing)
            IncreaseSize();
        else
            DecreaseSize();

        transform.localScale = NewSize;
	}

    private void IncreaseSize()
    {
        if (transform.localScale.x < EnlargedScale)
        {
            NewSize.x += ChangeRate;
            NewSize.y += ChangeRate;
        }
        else
            Increasing = false;
    }

    private void DecreaseSize()
    {
        if (transform.localScale.x > StartingScale)
        {
            NewSize.x -= ChangeRate;
            NewSize.y -= ChangeRate;
        }
        else
            Increasing = true;
    }
}
