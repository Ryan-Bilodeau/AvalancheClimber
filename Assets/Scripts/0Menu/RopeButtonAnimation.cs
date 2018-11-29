using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Attach to FreeRopeButtonAnimation
//Makes rope button look like its flashing
public class RopeButtonAnimation : MonoBehaviour 
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    //Delegate
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.buildIndex == 0)
        {
            StartCoroutine(FlashingAnimation());
        }
        else
            StopAllCoroutines();
    }

    private IEnumerator FlashingAnimation()
    {
        while (true)
        {
			if (gameObject.GetComponent<Image>().IsActive())
				gameObject.GetComponent<Image>().enabled = false;
			else
				gameObject.GetComponent<Image>().enabled = true;

			yield return new WaitForSeconds(.5f);
        }
    }
}
