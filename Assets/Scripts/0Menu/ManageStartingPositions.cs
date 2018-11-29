using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Attach to ManageGameStart in 0Menu scene
//Manages placing player, clouds, and objects each time 1Main is loaded
public class ManageStartingPositions : MonoBehaviour
{
    public static ManageStartingPositions Instance;

    [HideInInspector]
    public bool PlayerSet;
    [HideInInspector]
    public bool ObjectsSet;
    [HideInInspector]
    public bool CloudsSet;
    [HideInInspector]
    public Vector2[] CloudPos;
    [HideInInspector]
    public Vector2[] ObjectPos;
    [HideInInspector]
    public float PlayerYPos;
    [HideInInspector]
    public Vector2 GravityAtGamestart;
    [HideInInspector]
    public float TimeScaleAtGamestart;
    [HideInInspector]
    public int AdCounter;

    private Vector2 TempPos;
    private Color PanelColor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GravityAtGamestart = Physics2D.gravity;
            TimeScaleAtGamestart = Time.timeScale;
            AdCounter = 0;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        PanelColor = Color.black;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        SceneManager.sceneUnloaded += OnLevelUnload;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        SceneManager.sceneUnloaded -= OnLevelUnload;
    }

    #region Methods
    private void SetObjectPos()
    {
        for (int i = 0; i < ObjectPos.Length; i++)
        {
            TempPos.y = PlayerYPos + ((Random.Range(2f, 8f)) * (Random.Range(0, 2) == 0 ? -1 : 1));
            TempPos.x = Random.Range(2f, 8f) * (Random.Range(0, 2) == 0 ? -1 : 1);
            ObjectPos[i] = TempPos;
        }
    }

    private void SetCloudPos()
    {
        for (int i = 0; i < CloudPos.Length; i++)
        {
            TempPos.y = PlayerYPos + ((Random.Range(0f, 12f)) * (Random.Range(0, 2) == 0 ? -1 : 1));
			TempPos.x = Random.Range(0f, 10f) * (Random.Range(0, 2) == 0 ? -1 : 1);
            CloudPos[i] = TempPos;
        }
    }
    #endregion

    #region Coroutines
    //Called in OnLevelFinishedLoading
    private IEnumerator FadeFromBlackAnimation()
    {
        yield return new WaitForSeconds(.4f);
        for (;;)
        {
            PanelColor.a -= .03f;

            if (SceneObjects.FadeCanvas.GetComponent<Image>().color.a > 0f)
                SceneObjects.FadeCanvas.GetComponent<Image>().color = PanelColor;
            else
            {
                PanelColor.a = 0f;
                SceneObjects.FadeCanvas.GetComponent<Image>().color = PanelColor;
                break;
            }

            yield return new WaitForSeconds(.02f);
        }
    }
    #endregion

    #region Delegates
    //Called when a level is loaded
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //Set up everything for 1Main scene here
        if (scene.buildIndex == 1)
        {
            Time.timeScale = TimeScaleAtGamestart;
            Physics2D.gravity = GravityAtGamestart;
            CameraManager.CameraFollowingOnY = true;

            PanelColor = Color.black;
            SceneObjects.FadeCanvas.GetComponent<Image>().color = Color.black;
            StartCoroutine(FadeFromBlackAnimation());

            SceneObjects.Ragdoll.transform.position = new Vector3(0f, PlayerYPos, 0f);
			PlayerSet = true;
			ObjectsSet = true;
			CloudsSet = true;
        }
    }

    private void OnLevelUnload(Scene scene)
    {
		StopAllCoroutines();

		CloudPos = new Vector2[Random.Range(2, 6)];
		ObjectPos = new Vector2[Random.Range(2, 4)];

		PlayerYPos = -Random.Range(30f, 50f);

		SetCloudPos();
		SetObjectPos();
    }
	#endregion
}
