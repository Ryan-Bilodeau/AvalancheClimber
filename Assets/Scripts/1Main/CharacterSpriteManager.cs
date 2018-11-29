using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to Ragdoll
//Manages character sprites
public class CharacterSpriteManager : MonoBehaviour 
{
	public static Sprite DeathSprite;       //Called by EndGameManager
    private Sprite StartingSprite;
    private Sprite JumpingSprite;

    private void Start()
    {
        StopAllCoroutines();
		StartingSprite = SceneObjects.Body.GetComponent<SpriteRenderer>().sprite;
		JumpingSprite = GameObject.Find("JumpingSprite").GetComponent<SpriteRenderer>().sprite;
		DeathSprite = GameObject.Find("DeathSprite").GetComponent<SpriteRenderer>().sprite;
        StartCoroutine(SwitchIfJumping());
    }

    private IEnumerator SwitchIfJumping()
    {
        for (;;)
        {
			if (!EndGameManager.PlayerLost)
			{
				if (PlayerJumper.PlayerInAir)
					SceneObjects.Body.GetComponent<SpriteRenderer>().sprite = JumpingSprite;
				else if (!PlayerJumper.PlayerInAir)
					SceneObjects.Body.GetComponent<SpriteRenderer>().sprite = StartingSprite;
			}

			yield return new WaitForSeconds(.015f);
        }
    }
}
