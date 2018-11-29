using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to StarsManager
public class StarsManager : MonoBehaviour 
{
    public static Object[] StarSprites;
    public static bool CanTwinkle;
    private Vector2 NewPos;
    private Vector2 StartingPos;

    private void Awake()
    {
        StartingPos = transform.position;
        StarSprites = Resources.LoadAll("Sprites/Background/Stars", typeof(Sprite));
    }

    private void Start () 
	{
		NewPos = transform.position;
        foreach (Transform child in transform)
        {
            child.gameObject.AddComponent<TwinkleStar>();
        }
    }

    private void Update ()
	{
        if (UIManager.Altitude > 70)
        {
            NewPos.x = SceneObjects.Body.transform.position.x;

            if (EndGameManager.PlayerLost)
                FollowPlayerYPos();
            
            transform.position = NewPos;
            CanTwinkle = true;
        }
        else
            CanTwinkle = false;

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
//Gets attached to each star
public class TwinkleStar : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(ManageTwinkle());
    }

    private IEnumerator ManageTwinkle()
    {
        for (;;)
        {
            StartCoroutine(ChangeSprite());
            yield return new WaitForSeconds(Random.Range(2f, 8f));
        }
    }

    private IEnumerator ChangeSprite()
    {
        bool Incrementing = true;

        if (StarsManager.CanTwinkle)
        {
			for (int i = 0; i > -1;)
			{
				if (i == 3)
					Incrementing = false;

				if (Incrementing)
					i++;
				else if (!Incrementing)
					i--;

				GetComponent<SpriteRenderer>().sprite = StarsManager.StarSprites[i] as Sprite;
				yield return new WaitForSeconds(.05f);
				if (i < 1)
					i = -1;
			}
        }
    }

    private IEnumerator DelayTwinkle()
    {
        yield return new WaitForSeconds(Random.Range(2f, 8f));
    }
}
