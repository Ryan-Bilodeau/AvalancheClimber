using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Attach to CloudManager
//Manages moving and spawning clouds
public class CloudManager : MonoBehaviour 
{
    public static CloudManager Instance;

    [HideInInspector]
    public List<GameObject> DisabledClouds = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> EnabledPosYClouds = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> EnabledNegYClouds = new List<GameObject>();
    [HideInInspector]
    public bool Loading = true;
    [HideInInspector]
    public bool CanSpawn = true;

    public float XSpawnChance;            //Needs to be a decimal
    public float SpawnAboveChance;
    public float ParalaxSpeed;          //Needs to be a small decimal
    public float MoveSpeed;
    public float MinXSpawnDist;
    public float MinYSpawnDist;
    public float DisableDist;
    public float SpawnSpeed;

    private Object[] PrefabsClouds;
    private Vector3 ParentPos;
    private Vector3 RandCloudPos;
    private Vector2 RandCloudScale = Vector2.one;
    private Vector2 CameraPos;
	private int CloudsCounter;
	private int RandIndex;
    private int XMoveDir;
    private int LastPosYSpawn = 0;
    private int LastNegYSpawn = 0;
    private bool SpawnAbove;
    private bool DoneRestarting;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
			PrefabsClouds = Resources.LoadAll("Prefabs/Clouds", typeof(GameObject));
			InvokeRepeating("LoadClouds", .02f, .02f);          //Stops calling when done loading
			DontDestroyOnLoad(gameObject);
		}
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        SceneManager.sceneUnloaded += OnSceneUnload;
	}

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        SceneManager.sceneUnloaded -= OnSceneUnload;
    }

	#region Private Methods
	//Relies on ManageStartinPositions
	private void ManageCloudsAtStart()
	{
		for (int i = 0; i < ManageStartingPositions.Instance.CloudPos.Length; i++)
		{
			RandIndex = Random.Range(0, DisabledClouds.Count);
			XMoveDir = (Random.Range(0, 2) == 0 ? -1 : 1);
			RandCloudScale.x = Random.Range(0, 2) == 0 ? -1 : 1;
            RandCloudScale.y = Mathf.Sign(RandCloudScale.y) > 0 ? RandCloudScale.y * -1 : RandCloudScale.y;
			RandCloudPos = ManageStartingPositions.Instance.CloudPos[i];

			DisabledClouds[RandIndex].SetActive(true);
			DisabledClouds[RandIndex].transform.localScale = RandCloudScale;
			DisabledClouds[RandIndex].transform.position = RandCloudPos;
			DisabledClouds[RandIndex].GetComponent<CloudState>().SetDisableDist(DisableDist);
			DisabledClouds[RandIndex].GetComponent<CloudState>().SetMoveSpeed(MoveSpeed * -XMoveDir);
			DisabledClouds[RandIndex].GetComponent<CloudState>().SetCloudColor();
			DisabledClouds[RandIndex].GetComponent<CloudState>().StartCorotines();

			DisabledClouds.RemoveAt(RandIndex);
		}
	}

	private void SpawnOnX()
	{
		RandIndex = Random.Range(0, DisabledClouds.Count);
		XMoveDir = (Random.Range(0, 2) == 0 ? -1 : 1);
		RandCloudScale.x = Random.Range(0, 2) == 0 ? -1 : 1;
        RandCloudScale.y = Mathf.Sign(RandCloudScale.y) > 0 ? RandCloudScale.y * -1 : RandCloudScale.y;
		RandCloudPos.x = Camera.main.transform.position.x +
			((MinXSpawnDist + Random.Range(0f, 6f)) * XMoveDir);
		RandCloudPos.y = Camera.main.transform.position.y +
			(Random.Range(0f, 10f) * (Random.Range(0, 2) == 0 ? -1 : 1));
		RandCloudPos.z = transform.position.z;

        DisabledClouds[RandIndex].SetActive(true);
		DisabledClouds[RandIndex].transform.localScale = RandCloudScale;
		DisabledClouds[RandIndex].transform.position = RandCloudPos;
		DisabledClouds[RandIndex].GetComponent<CloudState>().SetDisableDist(DisableDist);
		DisabledClouds[RandIndex].GetComponent<CloudState>().SetMoveSpeed(MoveSpeed * -XMoveDir);
		DisabledClouds[RandIndex].GetComponent<CloudState>().SetCloudColor();
        DisabledClouds[RandIndex].GetComponent<CloudState>().StartCorotines();

		DisabledClouds.RemoveAt(RandIndex);
	}

	private void SpawnOnY()
	{
		RandIndex = Random.Range(0, DisabledClouds.Count);
		XMoveDir = (Random.Range(0, 2) == 0 ? -1 : 1);
		RandCloudScale.x = Random.Range(0, 2) == 0 ? -1 : 1;
        RandCloudScale.y = Mathf.Sign(RandCloudScale.y) > 0 ? RandCloudScale.y * -1 : RandCloudScale.y;

		SetYSpawnPosition();

        DisabledClouds[RandIndex].SetActive(true);
		DisabledClouds[RandIndex].transform.localScale = RandCloudScale;
		DisabledClouds[RandIndex].transform.position = RandCloudPos;
		DisabledClouds[RandIndex].GetComponent<CloudState>().SetDisableDist(DisableDist);
		DisabledClouds[RandIndex].GetComponent<CloudState>().SetMoveSpeed(MoveSpeed * -XMoveDir);
		DisabledClouds[RandIndex].GetComponent<CloudState>().SetCloudColor();
        DisabledClouds[RandIndex].GetComponent<CloudState>().StartCorotines();

		DisabledClouds.RemoveAt(RandIndex);
	}

	//Spawn clouds on y axis based on where the last cloud spawned
	private void SetYSpawnPosition()
	{
		LastPosYSpawn = EnabledPosYClouds.Count - 1;
		LastNegYSpawn = EnabledNegYClouds.Count - 1;
		CameraPos = Camera.main.transform.position;
		RandCloudPos = Vector3.zero;

		if (Random.Range(0f, 1f) < SpawnAboveChance)
			SpawnAbove = true;
		else
			SpawnAbove = false;

		//Positive
		if (SpawnAbove)
		{
			if (EnabledPosYClouds.Count > 0)
			{
				if (Mathf.Abs(((Vector2)EnabledPosYClouds[LastPosYSpawn].transform.position - CameraPos).magnitude)
					> MinYSpawnDist && EnabledPosYClouds[LastPosYSpawn].transform.position.y > CameraPos.y)
				{
					RandCloudPos.y = EnabledPosYClouds[LastPosYSpawn].transform.position.y + Random.Range(1f, 10f);
				}
				else
				{
					RandCloudPos.y = CameraPos.y + MinYSpawnDist + Random.Range(0f, 10f);
				}
			}
			else
			{
				RandCloudPos.y = CameraPos.y + MinYSpawnDist + Random.Range(0f, 10f);
			}

			EnabledPosYClouds.Add(DisabledClouds[RandIndex]);
		}
		//Negative
		else if (!SpawnAbove)
		{
			if (EnabledNegYClouds.Count > 0)
			{
				if (Mathf.Abs(((Vector2)EnabledNegYClouds[LastNegYSpawn].transform.position - CameraPos).magnitude)
					> MinYSpawnDist && EnabledNegYClouds[LastNegYSpawn].transform.position.y < CameraPos.y)
				{
					RandCloudPos.y = EnabledNegYClouds[LastNegYSpawn].transform.position.y - Random.Range(1f, 10f);
				}
				else
					RandCloudPos.y = CameraPos.y - MinYSpawnDist - Random.Range(0f, 10f);
			}
			else
				RandCloudPos.y = CameraPos.y - MinYSpawnDist - Random.Range(0f, 10f);

			EnabledNegYClouds.Add(DisabledClouds[RandIndex]);
		}

		RandCloudPos.x = CameraPos.x + (Random.Range(0f, 15f) * XMoveDir);
		RandCloudPos.z = transform.position.z;
	}
    #endregion

    #region Delegates
    private void OnSceneLoad(Scene scene, LoadSceneMode sceneMode)
	{
		if (scene.buildIndex == 1)
		{
			StartCoroutine(ManageRestarting1Main());
		}
	}

    private void OnSceneUnload(Scene scene)
    {
		StopAllCoroutines();

		//Reset lists and disable coroutines
		foreach (Transform child in transform)
		{
			child.GetComponent<CloudState>().On1MainUnload();
		}

		DoneRestarting = true;
    }
	#endregion

	#region Coroutines
	//Called when 1Main is loaded
	private IEnumerator ManageRestarting1Main()
	{
        while (DoneRestarting == false || ManageStartingPositions.Instance.CloudsSet == false 
               || Loading == true || UIManager.AltitudeSet == false)
			yield return null;

		//Set starting cloud positions
		ManageCloudsAtStart();
        ManageStartingPositions.Instance.CloudsSet = false;

		StartCoroutine(ManageCanSpawn());
		StartCoroutine(SpawnManager());
        StartCoroutine(UpdatePosition());
    }

	private IEnumerator SpawnManager()
	{
		for (;;)
		{
			if (CanSpawn && DisabledClouds.Count > 0)
			{
				if (Random.Range(0f, 1f) < XSpawnChance)
					SpawnOnX();
				else
					SpawnOnY();
			}

			yield return new WaitForSeconds(SpawnSpeed + Random.Range(0f, .5f));
		}
	}

	//Update CloudManager position
	private IEnumerator UpdatePosition()
	{
		for (;;)
		{
			if (!EndGameManager.PlayerWon && !CanvasButtonManger.Paused)
			{
				if (CameraManager.CameraFollowingOnY)
				{
					ParentPos.x = SceneObjects.Body.transform.position.x;
					ParentPos.y += SceneObjects.Body.GetComponent<Rigidbody2D>().velocity.y * ParalaxSpeed;
					ParentPos.z = transform.position.z;
					transform.position = ParentPos;
				}
				else
				{
					ParentPos.x = SceneObjects.Body.transform.position.x;
					transform.position = ParentPos;
				}
			}

			yield return null;
		}
	}

	private IEnumerator ManageCanSpawn()
	{
		while (true)
		{
			if (UIManager.Altitude > 50f)
				CanSpawn = false;
			else
				CanSpawn = true;

			yield return new WaitForSeconds(.5f);
		}
	}
	#endregion

	#region Call as InvokeRepeating
	private void LoadClouds()
	{
		if (CloudsCounter < PrefabsClouds.Length)
		{
			DisabledClouds.Add(Instantiate(PrefabsClouds[CloudsCounter], transform) as GameObject);
			DisabledClouds[CloudsCounter].AddComponent<CloudState>();
			DisabledClouds[CloudsCounter].SetActive(false);
			CloudsCounter++;
		}
		else
		{
			Loading = false;
			CancelInvoke("LoadClouds");
		}
	}
    #endregion
}

//Gets attached to each cloud
public class CloudState : MonoBehaviour
{
    private float XMoveSpeed;
    private Vector3 MoveSpeed;
    private float DisableDist;
    private Color AltitudeColor = Color.white;

    Vector2 tempPos;

    //Called by CloudManager at 1MainUnload
    public void On1MainUnload()
    {
        if (!CloudManager.Instance.DisabledClouds.Contains(gameObject))
        {
            CloudManager.Instance.DisabledClouds.Add(gameObject);
            gameObject.SetActive(false);
        }

		if (CloudManager.Instance.EnabledPosYClouds.Contains(gameObject))
			CloudManager.Instance.EnabledPosYClouds.Remove(gameObject);
		else if (CloudManager.Instance.EnabledNegYClouds.Contains(gameObject))
			CloudManager.Instance.EnabledNegYClouds.Remove(gameObject);

        StopAllCoroutines();
    }

    public void StartCorotines()
    {
        StartCoroutine(UpdatePosition());
        StartCoroutine(UpdateState());
    }

    private IEnumerator UpdatePosition()
    {
        for (;;)
        {
            if (gameObject.activeSelf)
            {
                if (!CanvasButtonManger.Paused)
                {
                    MoveSpeed.x = XMoveSpeed - (SceneObjects.Body.GetComponent<Rigidbody2D>().velocity.x / 200f);
                    transform.localPosition += MoveSpeed;
                }
            }
            else
                break;

            yield return null;
        }
    }

    private IEnumerator UpdateState()
    {
        for (;;)
        {
            if (gameObject.activeSelf)
            {
                tempPos = SceneObjects.Body.transform.position;
                tempPos.y += CameraManager.Y_OFFSET;
                if (Mathf.Abs(((Vector2)transform.position - (Vector2)tempPos).magnitude) > DisableDist)
                {
                    CloudManager.Instance.DisabledClouds.Add(gameObject);

                    if (CloudManager.Instance.EnabledPosYClouds.Contains(gameObject))
                        CloudManager.Instance.EnabledPosYClouds.Remove(gameObject);
                    else if (CloudManager.Instance.EnabledNegYClouds.Contains(gameObject))
                        CloudManager.Instance.EnabledNegYClouds.Remove(gameObject);

                    StopAllCoroutines();
                    gameObject.SetActive(false);
                }
            }
            else
                break;

            yield return new WaitForSeconds(.2f);
        }
    }

    public void SetDisableDist(float dist)
    {
        DisableDist = dist;
    }

    public void SetMoveSpeed(float xSpeed)
    {
        XMoveSpeed = xSpeed;
	}

	public void SetCloudColor()
	{
		AltitudeColor.r = 1 - (UIManager.Altitude / 200f);
		AltitudeColor.g = 1 - (UIManager.Altitude / 200f);
		AltitudeColor.b = 1 - (UIManager.Altitude / 200f);
        transform.GetComponent<SpriteRenderer>().color = AltitudeColor;
	}
}
