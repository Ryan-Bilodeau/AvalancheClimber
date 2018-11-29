using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to HouseGrassRope
//Manages moving the grass
public class GrassManager : MonoBehaviour
{
    public float X_MOVE_DIST;
    public GameObject One;
    public GameObject Two;
    public GameObject Three;
    public GameObject Four;
    private GameObject[] Grasses;

    private void Start()
    {
        StopAllCoroutines();
        Grasses = new GameObject[] {One, Two, Three, Four};
        StartCoroutine(MoveGrasses());
    }

    private IEnumerator MoveGrasses()
    {
        for (;;)
        {
            yield return new WaitForSeconds(.5f);
            if (UIManager.Altitude < 30)
            {
				MoveGrassRight();
				MoveGrassLeft();
            }
        }
    }

    private void MoveGrassRight()
    {
        for (int i = 0; i < Grasses.Length; i++)
        {
            if (Grasses[i].transform.position.x + X_MOVE_DIST * 2f < SceneObjects.Body.transform.position.x)
            {
                Grasses[i].transform.position = new Vector3(Grasses[i].transform.position.x + X_MOVE_DIST * 4f,
                                                            Grasses[i].transform.position.y, Grasses[i].transform.position.z);
            }
        }
    }

    private void MoveGrassLeft()
    {
		for (int i = 0; i < Grasses.Length; i++)
		{
			if (Grasses[i].transform.position.x - X_MOVE_DIST * 2f > SceneObjects.Body.transform.position.x)
			{
				Grasses[i].transform.position = new Vector3(Grasses[i].transform.position.x - X_MOVE_DIST * 4f,
															Grasses[i].transform.position.y, Grasses[i].transform.position.z);
			}
		}
    }
}
