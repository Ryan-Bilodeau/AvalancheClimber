using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Attach to MenuObjectSpawner in 0Menu scene
//Manages spawning objects in the main menu
public class MenuObjectSpawner : MonoBehaviour
{
    public static MenuObjectSpawner Instance;

    [HideInInspector]
    public List<GameObject> DisabledObjects = new List<GameObject>();

    private float MIN_SPAWN_INTERVAL = .5f;
    private float MIN_SPAWN_Y = 20f;
    private float MAX_SPAWN_X = 9f;

    private bool Loading = true;

    private Object[] ObjectsPrefabs;
    private Vector2 RandPos;
    private float RandRot;
    private float AngularVel;
    private int RandIndex;
    private int Counter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ObjectsPrefabs = Resources.LoadAll("Prefabs/Debris", typeof(GameObject));
            InvokeRepeating("LoadObjects", .02f, .02f);         //Stops calling when done
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
		if (scene.buildIndex == 0)
		{
            StartCoroutine(WaitWhileLoading(SpawnObject()));
		}
	}

    private void OnLevelUnloaded(Scene scene)
    {
        if (scene.buildIndex == 0)
        {
			foreach (Transform child in transform)
			{
				child.GetComponent<MenuObjectStateManager>().CallOn0MenuUnload();
			}

            StopAllCoroutines();
        }
    }
	#endregion

	#region Coroutines
	private IEnumerator WaitWhileLoading(IEnumerator routine)
	{
		while (Loading)
			yield return null;

		if (routine != null)
			StartCoroutine(routine);
	}

	private IEnumerator SpawnObject()
    {
        for (;;)
        {
            if (!Loading)
            {
                RandIndex = Random.Range(0, DisabledObjects.Count);
                RandPos.y = Random.Range(MIN_SPAWN_Y, MIN_SPAWN_Y + 5f);
                RandPos.x = Random.Range(0f, MAX_SPAWN_X) * (Random.Range(0, 2) == 0 ? -1 : 1);
                RandRot = Random.Range(0f, 180f) * (Random.Range(0, 2) == 0 ? -1 : 1);
                AngularVel = Random.Range(10f, 80f) * (Random.Range(0, 2) == 0 ? -1 : 1);

                DisabledObjects[RandIndex].SetActive(true);
                DisabledObjects[RandIndex].transform.position = RandPos;
                DisabledObjects[RandIndex].transform.rotation = Quaternion.Euler(0f, 0f, RandRot);
                DisabledObjects[RandIndex].GetComponent<Rigidbody2D>().angularVelocity = AngularVel;
                DisabledObjects[RandIndex].GetComponent<MenuObjectStateManager>().CallManageState();
                DisabledObjects.RemoveAt(RandIndex);
            }

            yield return new WaitForSeconds(Random.Range(MIN_SPAWN_INTERVAL, 3f));
        }
    }
    #endregion

    #region Call as InvokeRepeating
    private void LoadObjects()
    {
        if (Counter < ObjectsPrefabs.Length)
        {
            DisabledObjects.Add(Instantiate(ObjectsPrefabs[Counter], transform) as GameObject);
            DisabledObjects[Counter].AddComponent<MenuObjectStateManager>();
            DisabledObjects[Counter].SetActive(false);
            Counter++;
        }
        else
        {
            Loading = false;
            CancelInvoke("LoadObjects");
        }
    }
    #endregion
}

//Gets attached to each object in 0Menu scene
public class MenuObjectStateManager : MonoBehaviour
{
    private float DisableDist = 25f;

	//Called by MenuObjectSpawner
	public void CallOn0MenuUnload()
	{
		if (!MenuObjectSpawner.Instance.DisabledObjects.Contains(gameObject))
			MenuObjectSpawner.Instance.DisabledObjects.Add(gameObject);

		StopAllCoroutines();
		gameObject.SetActive(false);
	}

    //Called by MenuObjectSpawner
    public void CallManageState()
    {
        StartCoroutine(ManageState());
    }

    private IEnumerator ManageState()
    {
        for (;;)
        {
			if (Mathf.Abs(((Vector2)Camera.main.transform.position - (Vector2)transform.position).magnitude) > DisableDist)
			{
				MenuObjectSpawner.Instance.DisabledObjects.Add(gameObject);
				gameObject.SetActive(false);
                break;
			}

			yield return new WaitForSeconds(.5f);
        }
    }
}
