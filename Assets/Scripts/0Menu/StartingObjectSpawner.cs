using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Attach to ObjectSpawner in starting scene
//Manages spawning objects and populating clouds list
public class StartingObjectSpawner : MonoBehaviour
{
    public static StartingObjectSpawner Instance;

    [HideInInspector]
    public List<GameObject> DisabledDebris = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> EnabledPosYObjects = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> EnabledNegYObjects = new List<GameObject>();
    [HideInInspector]
    public float StartingLinearDrag = 2.3f;
    [HideInInspector]
    public bool CanSpawnStuff;
    [HideInInspector]
    public bool Loading = true;

    public float SpawnYDist;        //Minimum spawn y distance
    public float SpawnInterval;         //Minimum spawn interval
    public float XMaxOffset;            //Maximum x offset
    public float XMaxVel;
    public float SpawnAboveChance;          //Needs to be a decimal

    private Object[] PrefabsDebris;
    private bool LoadingDebris = true;
    private bool SpawnAbove;
    private bool DoneRestarting;

    int DebrisCounter;
    int RandIndex;
    Vector3 RandPos;
    Vector3 RandScale;
    Vector2 Velocity;
    Vector2 CameraPos;
    Vector2 BodyPos;
    Vector2 TempPos;
    Vector2[] ObjectStartingPos;
    int LastPosYSpawn;
    int LastNegYSpawn;
    float AngularVel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PrefabsDebris = Resources.LoadAll("Prefabs/Debris", typeof(GameObject));
            InvokeRepeating("LoadDebris", .02f, .02f);          //Stops calling when done loading
            InvokeRepeating("CheckIfDoneLoading", .1f, .1f);        //Stops calling when done
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
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

    #region Delegates
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.buildIndex == 1)
        {
            StartCoroutine(ManageRestarting1Main());
        }
    }

    private void OnLevelUnload(Scene scene)
    {
		StopAllCoroutines();

		foreach (Transform child in transform)
		{
			child.GetComponent<GameObjectEnableManager>().On1MainUnload();
		}

		DoneRestarting = true;
    }
    #endregion

    #region Coroutines
    private IEnumerator ManageRestarting1Main()
    {
        while (ManageStartingPositions.Instance.ObjectsSet == false || DoneRestarting == false || Loading == true)
            yield return null;

        ManageObjectsAtStart();

        DoneRestarting = false;
        ManageStartingPositions.Instance.ObjectsSet = false;

        StartCoroutine(ManageSpawningDistance());
        StartCoroutine(SpawnManager());
        StartCoroutine(SpawnGameObjectOnX());
    }

    //Manages how the gameobject is spawned
    private IEnumerator SpawnManager()
    {
        for (;;)
        {
            if (CanSpawnStuff && DisabledDebris.Count > 0)
            {
                SpawnGameObject();
            }

            yield return new WaitForSeconds(SpawnInterval + Random.Range(0f, .5f));
        }
    }

    private IEnumerator SpawnGameObjectOnX()
    {
        for (;;)
        {
            CameraPos = Camera.main.transform.position;
            BodyPos = SceneObjects.Body.transform.position;

            RandIndex = Random.Range(0, DisabledDebris.Count);
            RandScale = DisabledDebris[RandIndex].transform.localScale;
            RandScale.x *= Random.Range(0, 2) == 0 ? -1 : 1;
            RandScale.y *= Random.Range(0, 2) == 0 ? -1 : 1;
            RandPos.x = BodyPos.x + (
                Mathf.Sign(SceneObjects.Body.GetComponent<Rigidbody2D>().velocity.x) * (Random.Range(15f, 20f)));
            RandPos.y = BodyPos.y + (Random.Range(0f, 8f) * (Random.Range(0, 2) == 0 ? -1 : 1));
            AngularVel = Random.Range(20f, 100f) * (Random.Range(0, 2) == 0 ? -1 : 1);

            DisabledDebris[RandIndex].SetActive(true);

            if (DisabledDebris[RandIndex].GetComponent<ManageObjectSprite>() != null)
                DisabledDebris[RandIndex].GetComponent<ManageObjectSprite>().ChangeSprite();

            DisabledDebris[RandIndex].transform.position = RandPos;
            DisabledDebris[RandIndex].transform.localScale = RandScale;
            DisabledDebris[RandIndex].GetComponent<Rigidbody2D>().angularVelocity = AngularVel;
            DisabledDebris[RandIndex].GetComponent<GameObjectEnableManager>().StartCallingCoroutines();
            DisabledDebris.RemoveAt(RandIndex);

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

	//Stop spawning objects when close to the house
	private IEnumerator ManageSpawningDistance()
	{
		while (true)
		{
			if (Mathf.Abs(((Vector2)SceneObjects.House.transform.position - (Vector2)Camera.main.transform.position).magnitude)
				< 15f)
				CanSpawnStuff = false;
			else
				CanSpawnStuff = true;

			yield return null;
		}
	}
	#endregion

	#region Private Methods
    //Called in ManageObjectsAtStart
	private void SetObjectPos()
	{
        ObjectStartingPos = new Vector2[Random.Range(2, 4)];

		for (int i = 0; i < ObjectStartingPos.Length; i++)
		{
			//TempPos.y = PlayerYPos + ((Random.Range(2f, 8f)) * (Random.Range(0, 2) == 0 ? -1 : 1));
			TempPos.y = ManageStartingPositions.Instance.PlayerYPos + 1;
			TempPos.x = Random.Range(2f, 8f) * (Random.Range(0, 2) == 0 ? -1 : 1);
			ObjectStartingPos[i] = TempPos;
		}
	}

    //Relies on ManageStartingPositions 
    private void ManageObjectsAtStart()
    {
        //SetObjectPos();

        for (int i = 0; i < ManageStartingPositions.Instance.ObjectPos.Length; i++)
        {
            RandIndex = Random.Range(0, DisabledDebris.Count);
            AngularVel = Random.Range(0f, 60f) * (Random.Range(0, 2) == 0 ? -1 : 1);
            RandScale = DisabledDebris[RandIndex].transform.localScale;
            RandScale.x *= Random.Range(0, 2) == 0 ? -1 : 1;
            RandScale.y *= Random.Range(0, 2) == 0 ? -1 : 1;
            RandPos = ManageStartingPositions.Instance.ObjectPos[i];

            DisabledDebris[RandIndex].SetActive(true);

            DisabledDebris[RandIndex].transform.position = RandPos;
            DisabledDebris[RandIndex].GetComponent<Rigidbody2D>().angularVelocity = AngularVel;
            DisabledDebris[RandIndex].GetComponent<GameObjectEnableManager>().StartCallingCoroutines();
            DisabledDebris[RandIndex].transform.localScale = RandScale;

            if (DisabledDebris[RandIndex].GetComponent<ManageObjectSprite>() != null)
                DisabledDebris[RandIndex].GetComponent<ManageObjectSprite>().ChangeSprite();

            DisabledDebris.RemoveAt(RandIndex);
        }
    }

    //Spawn an object
    private void SpawnGameObject()
    {
        RandIndex = Random.Range(0, DisabledDebris.Count);
        RandScale = DisabledDebris[RandIndex].transform.localScale;

        GetNewPosition();

        RandScale.x *= Random.Range(0, 2) == 0 ? -1 : 1;
        RandScale.y *= Random.Range(0, 2) == 0 ? -1 : 1;
        AngularVel = Random.Range(20f, 100f) * (Random.Range(0, 2) == 0 ? -1 : 1);
        Velocity.x = Random.Range(0f, XMaxVel) * (Random.Range(0, 2) == 0 ? -1 : 1);

        DisabledDebris[RandIndex].SetActive(true);

        if (DisabledDebris[RandIndex].GetComponent<ManageObjectSprite>() != null)
            DisabledDebris[RandIndex].GetComponent<ManageObjectSprite>().ChangeSprite();

        DisabledDebris[RandIndex].transform.position = RandPos;
        DisabledDebris[RandIndex].transform.localScale = RandScale;
        DisabledDebris[RandIndex].GetComponent<Rigidbody2D>().angularVelocity = AngularVel;
        DisabledDebris[RandIndex].GetComponent<Rigidbody2D>().velocity = Velocity;
        DisabledDebris[RandIndex].GetComponent<GameObjectEnableManager>().StartCallingCoroutines();

        DisabledDebris.RemoveAt(RandIndex);
    }

    private void GetNewPosition()
    {
        LastPosYSpawn = EnabledPosYObjects.Count - 1;
        LastNegYSpawn = EnabledNegYObjects.Count - 1;
        CameraPos = Camera.main.transform.position;
        RandPos = Vector3.zero;

        if (Random.Range(0f, 1f) < SpawnAboveChance)
            SpawnAbove = true;
        else
            SpawnAbove = false;

        //Positive
        if (SpawnAbove)
        {
            if (EnabledPosYObjects.Count > 0)
            {
                if (Mathf.Abs(((Vector2)EnabledPosYObjects[LastPosYSpawn].transform.position - CameraPos).magnitude)
                    > SpawnYDist && Mathf.Abs(((Vector2)EnabledPosYObjects[LastPosYSpawn].transform.position -
                                               CameraPos).magnitude) < 35f &&
                    EnabledPosYObjects[LastPosYSpawn].transform.position.y > CameraPos.y)
                {
                    RandPos.y = EnabledPosYObjects[LastPosYSpawn].transform.position.y + Random.Range(4f, 8f);
                }
                else
                {
                    RandPos.y = CameraPos.y + SpawnYDist + Random.Range(0f, 5f);
                }
            }
            else
            {
                RandPos.y = CameraPos.y + SpawnYDist + Random.Range(0f, 5f);
            }

            EnabledPosYObjects.Add(DisabledDebris[RandIndex]);
        }
        //Negative
        else if (!SpawnAbove)
        {
            if (EnabledNegYObjects.Count > 0)
            {
                if (Mathf.Abs(((Vector2)EnabledNegYObjects[LastNegYSpawn].transform.position - CameraPos).magnitude)
                    > SpawnYDist && Mathf.Abs(((Vector2)EnabledNegYObjects[LastNegYSpawn].transform.position -
                                               CameraPos).magnitude) < 35f &&
                    EnabledNegYObjects[LastNegYSpawn].transform.position.y < CameraPos.y)
                {
                    RandPos.y = EnabledNegYObjects[LastNegYSpawn].transform.position.y - Random.Range(4f, 8f);
                }
                else
                {
                    RandPos.y = CameraPos.y - SpawnYDist - Random.Range(0f, 5f);
                }
            }
            else
            {
                RandPos.y = CameraPos.y - SpawnYDist - Random.Range(0f, 5f);
            }

            EnabledNegYObjects.Add(DisabledDebris[RandIndex]);
        }

        RandPos.x = CameraPos.x + (Random.Range(0, XMaxOffset) * (Random.Range(0, 2) == 0 ? -1 : 1));
        RandPos.z = SceneObjects.Body.transform.position.z;
    }
	#endregion

	#region Call as InvokeRepeating
	//Loads one per frame to avoid lag
	private void LoadDebris()
	{
		if (DebrisCounter < PrefabsDebris.Length)
		{
			DisabledDebris.Add(Instantiate(PrefabsDebris[DebrisCounter], transform) as GameObject);

			if (DisabledDebris[DebrisCounter].CompareTag("Object"))
				DisabledDebris[DebrisCounter].AddComponent<ManageObjectSprite>();

			DisabledDebris[DebrisCounter].AddComponent<GameObjectEnableManager>();
			DisabledDebris[DebrisCounter].SetActive(false);
			DebrisCounter++;
		}
		else
		{
			LoadingDebris = false;
			CancelInvoke("LoadDebris");
		}
	}

	private void CheckIfDoneLoading()
	{
		if (!LoadingDebris && !CloudManager.Instance.Loading)
		{
			Loading = false;
			CancelInvoke("CheckIfDoneLoading");
		}
	}
    #endregion
}

//Gets attached to each falling object
public class GameObjectEnableManager : MonoBehaviour
{
    private float DisableDistance = 40f;
    private ParticleSystem.EmissionModule NewEmitter;

    Vector2 tempPos;

    private void Awake()
    {
        NewEmitter = transform.GetComponentInChildren<ParticleSystem>().emission;
        EndGameManager.OnPlayerLost += CallOnPlayerLost;
    }

	#region Add to Delegates
	//Add to EndGameManager.OnPlayerLost
	private void CallOnPlayerLost()
	{
		gameObject.GetComponent<Rigidbody2D>().drag = 0f;
	}
    #endregion

    #region Public Methods
    //Called by StartingObjectSpawner at 1Main load
    public void On1MainUnload()
    {
        if (!StartingObjectSpawner.Instance.DisabledDebris.Contains(gameObject))
        {
            StartingObjectSpawner.Instance.DisabledDebris.Add(gameObject);
            gameObject.SetActive(false);
        }

        if (StartingObjectSpawner.Instance.EnabledPosYObjects.Contains(gameObject))
            StartingObjectSpawner.Instance.EnabledPosYObjects.Remove(gameObject);
        else if (StartingObjectSpawner.Instance.EnabledNegYObjects.Contains(gameObject))
            StartingObjectSpawner.Instance.EnabledNegYObjects.Remove(gameObject);
        
        gameObject.GetComponent<Rigidbody2D>().drag = StartingObjectSpawner.Instance.StartingLinearDrag;
        StopAllCoroutines();
    }

    //Called by StartingObjectSpawner on 1Main load
    public void StartCallingCoroutines()
    {
        StartCoroutine(UpdateEmissionRate());
        StartCoroutine(UpdateLinearDrag());
        StartCoroutine(UpdateState());
    }
	#endregion

    #region Coroutines
    private IEnumerator UpdateState()
    {
        for (;;)
		{
            if (gameObject.activeSelf)
            {
                tempPos = SceneObjects.Body.transform.position;
                tempPos.y += CameraManager.Y_OFFSET;
                if (Mathf.Abs(((Vector2)transform.position - (Vector2)tempPos).magnitude) > DisableDistance)
                {
                    StartingObjectSpawner.Instance.DisabledDebris.Add(gameObject);

                    if (StartingObjectSpawner.Instance.EnabledPosYObjects.Contains(gameObject))
                        StartingObjectSpawner.Instance.EnabledPosYObjects.Remove(gameObject);
                    else if (StartingObjectSpawner.Instance.EnabledNegYObjects.Contains(gameObject))
                        StartingObjectSpawner.Instance.EnabledNegYObjects.Remove(gameObject);

                    StopAllCoroutines();
                    gameObject.SetActive(false);
                }
            }
            else
                break;

            yield return new WaitForSeconds(.2f);
		}
    }

    private IEnumerator UpdateEmissionRate()
    {
        for (;;)
        {
            if (gameObject.activeSelf)
            {
                NewEmitter.rateOverTime = new ParticleSystem.MinMaxCurve(0f, 15f - ((UIManager.Altitude / 100f) * 15f));
            }
            else
                break;

            yield return new WaitForSeconds(.2f); 
        }
    }

    private IEnumerator UpdateLinearDrag()
    {
        for (;;)
        {
            if (gameObject.activeSelf)
            {
                if (transform.position.y < Camera.main.transform.position.y - 15f)
                    gameObject.GetComponent<Rigidbody2D>().drag = StartingObjectSpawner.Instance.StartingLinearDrag * 1.5f;
                else
                    gameObject.GetComponent<Rigidbody2D>().drag = StartingObjectSpawner.Instance.StartingLinearDrag;
            }
            else
                break;

            yield return new WaitForSeconds(.2f);
        }
    }
    #endregion
}

//Only gets attached to objects with multiple sprites
public class ManageObjectSprite : MonoBehaviour
{
    private Object[] Sprites;
    private string TrimmedName;

    private void Awake()
    {
        TrimmedName = gameObject.name;

        //Removes the last 7 characters from the name
        for (int i = 0; i < 7; i++)
        {
            TrimmedName = TrimmedName.Remove(TrimmedName.Length - 1);
        }

        Sprites = Resources.LoadAll(string.Format("Sprites/Objects/{0}", TrimmedName), typeof(Sprite));
    }

    //Called by StartingObjectSpawner when gameObject is enabled
    public void ChangeSprite()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Sprites[Random.Range(0, Sprites.Length)] as Sprite;
    }
}
