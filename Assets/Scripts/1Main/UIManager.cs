using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Attach to Main Camera
//Handles UI stuff
public class UIManager : MonoBehaviour 
{
    public static float Altitude;
    public static bool AltitudeSet;
    public float ALT_MODIFIER;

    private void Start()
    {
        StartCoroutine(UpdateAltitude());
    }

    private void OnDisable()
    {
        AltitudeSet = false;
    }

    private IEnumerator UpdateAltitude()
    {
        for (;;)
        {
            if (!EndGameManager.PlayerLost)
            {
				Altitude = SceneObjects.Body.transform.position.y / ALT_MODIFIER;

				if (Altitude > 0)
					Altitude = 0;
				else
					Altitude = Mathf.Abs(Altitude);
				SceneObjects.Altitude.GetComponent<Text>().text = Mathf.RoundToInt(Altitude).ToString();
				SceneObjects.AltitudeBackground.GetComponent<Text>().text = Mathf.RoundToInt(Altitude).ToString();

                AltitudeSet = true;
            }

            yield return new WaitForSeconds(.2f);
        }
    }
}
