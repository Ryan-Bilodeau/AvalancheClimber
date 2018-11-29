using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Attach to MenuCloudManager in 0Menu scene
//Manages spawning clouds in the main menu
public class MenuCloudManager : MonoBehaviour
{
    public static MenuCloudManager Instance;

    [HideInInspector]
    public List<GameObject> DisabledClouds = new List<GameObject>();

    private float CLOUD_MOVE_SPEED = .01f;

    private Object[] CloudPrefabs;
    private Vector3 RandPos = Vector3.zero;
    private Vector3 RandScale = Vector3.one;
    private bool Loading = true;
    private int Counter;
    private int RandIndex;
    private int MovementDir;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CloudPrefabs = Resources.LoadAll("Prefabs/Clouds", typeof(GameObject));
            InvokeRepeating("LoadClouds", .02f, .02f);          //Stops calling when done loading
			DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        SceneManager.sceneUnloaded += OnLevelUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        SceneManager.sceneUnloaded -= OnLevelUnloaded;
    }

    #region Delegates
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode sceneMode)
    {
        //Start coroutines if on 0Menu
        if (scene.buildIndex == 0)
        {
            StartCoroutine(WaitWhileLoading(SpawnClouds()));
        }
    }

    private void OnLevelUnloaded(Scene scene)
    {
        if (scene.buildIndex == 0)
        {
            //Reset MenuCloudStateManger on 0Menu load
            foreach (Transform child in transform)
            {
                child.GetComponent<MenuCloudStateManager>().CallOn0MenuLoad();
            }

			StopAllCoroutines();
        }
    }
    #endregion

    #region Coroutines
    //Randomly spawns clouds 
    private IEnumerator SpawnClouds()
    {
        for (;;)
        {
			RandIndex = Random.Range(0, DisabledClouds.Count);
			RandPos.y = Camera.main.transform.position.y + (Random.Range(0f, 10f) * (Random.Range(0, 2) == 0 ? -1 : 1));
			RandPos.x = Camera.main.transform.position.x +
				((17f + Random.Range(0f, 3f)) * (Random.Range(0, 2) == 0 ? -1 : 1));
			RandPos.z = 0f;
			RandScale.y = DisabledClouds[RandIndex].transform.localScale.y *
				(DisabledClouds[RandIndex].transform.localScale.y > 0 ? 1 : -1);
			RandScale.x = DisabledClouds[RandIndex].transform.localScale.x * (Random.Range(0, 2) == 0 ? -1 : 1);
            RandScale.y = Mathf.Sign(RandScale.y) > 0 ? RandScale.y * -1 : RandScale.y;
			MovementDir = -(int)Mathf.Sign(RandPos.x);

			DisabledClouds[RandIndex].SetActive(true);
			DisabledClouds[RandIndex].transform.localScale = RandScale;
			DisabledClouds[RandIndex].transform.position = RandPos;
			DisabledClouds[RandIndex].GetComponent<MenuCloudStateManager>().SetMovement(CLOUD_MOVE_SPEED, MovementDir);
			DisabledClouds[RandIndex].GetComponent<MenuCloudStateManager>().StartManageCloudState();
			DisabledClouds.RemoveAt(RandIndex);

            yield return new WaitForSeconds(Random.Range(4f, 6f));
        }
    }

    //Call in OnLevelFinishedLoading
    private IEnumerator WaitWhileLoading(IEnumerator routine)
    {
        while (Loading)
            yield return null;

        if (routine != null)
            StartCoroutine(routine);
    }
    #endregion

    #region Call as InvokeRepeating
    private void LoadClouds()
    {
        if (Counter < CloudPrefabs.Length)
        {
            DisabledClouds.Add(Instantiate(CloudPrefabs[Counter], transform) as GameObject);
            DisabledClouds[Counter].AddComponent<MenuCloudStateManager>();
            DisabledClouds[Counter].SetActive(false);
            Counter++;
        }
        else
        {
            Loading = false;
            CancelInvoke("LoadClouds");
        }
    }
    #endregion
}

//Gets attached to each cloud in MenuCloudManager
public class MenuCloudStateManager : MonoBehaviour
{
    private Vector2 NewPos;
    private float DisableDist = 25f;
    private float MovementSpeed = 1;

    private void Start()
    {
        NewPos = gameObject.transform.localPosition;
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            NewPos.x += MovementSpeed;
            gameObject.transform.localPosition = NewPos;
        }
    }

    //Called by MenuCloudManager
    public void CallOn0MenuLoad()
    {
        if (!MenuCloudManager.Instance.DisabledClouds.Contains(gameObject))
            MenuCloudManager.Instance.DisabledClouds.Add(gameObject);

		StopAllCoroutines();
		gameObject.SetActive(false);
    }

	//Called by MenuCloudManager at cloud spawn
	public void StartManageCloudState()
    {
        StartCoroutine(ManageCloudState());
    }

	//Called by MenuCloudManager at cloud spawn
	public void SetMovement(float speed, int dir)
	{
		MovementSpeed = speed;
		MovementSpeed *= dir;
	}

    private IEnumerator ManageCloudState()
    {
        for (;;)
        {
            if (Mathf.Abs(((Vector2)Camera.main.transform.position - (Vector2)transform.position).magnitude) > DisableDist)
            {
                MenuCloudManager.Instance.DisabledClouds.Add(gameObject);
                gameObject.SetActive(false);
                StopCoroutine(ManageCloudState());
            }

            yield return new WaitForSeconds(.5f);
        }
    }
}
