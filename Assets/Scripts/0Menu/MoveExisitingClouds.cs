using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to each existing cloud in 0Menu scene
//Manages moving these clouds and their starting position
public class MoveExisitingClouds : MonoBehaviour 
{
    private Vector2 RandPos;
    private float DisableDist = 25f;
    private float MoveSpeed = .01f;
    private int MoveDir;

    private void Awake()
    {
        RandPos.y = Camera.main.transform.position.y + (Random.Range(0f, 15f) * (Random.Range(0, 2) == 0 ? -1 : 1));
        RandPos.x = Camera.main.transform.position.x + (Random.Range(0f, 10f) * (Random.Range(0, 2) == 0 ? -1 : 1));
        transform.position = RandPos;
        MoveDir = Random.Range(0, 2) == 0 ? -1 : 1;
        StartCoroutine(ManageState());
    }

    private void Update()
    {
        MoveCloud();
    }

    //Call in update, move cloud each frame
    private void MoveCloud()
    {
        RandPos.x += MoveSpeed * MoveDir;
        transform.position = RandPos;
    }

    //Call in awake, disables clouds when too far away from main camera
    private IEnumerator ManageState()
    {
        for (;;)
        {
			if (Mathf.Abs(((Vector2)Camera.main.transform.position - (Vector2)transform.position).magnitude) > DisableDist)
			{
				gameObject.SetActive(false);
				StopCoroutine(ManageState());
			}

			yield return new WaitForSeconds(.5f);
        }
    }
}
