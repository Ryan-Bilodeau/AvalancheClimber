using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to AudioManager in 0Menu scene
//Manages game soundtrack
public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;

    [HideInInspector]
    public AudioSource[] SoundTrackSource;
    [HideInInspector]
    public AudioSource JumpSource;
    [HideInInspector]
    public AudioSource ButtonClickSource;
    [HideInInspector]
    public AudioSource WonSource;
    [HideInInspector]
    public AudioSource LostSource;

    public AudioClip JumpClip;
    public AudioClip ButtonClickClip;
    public AudioClip WonClip;
    public AudioClip LostClip;
    public AudioClip[] SoundTrackClip = new AudioClip[4];

    private bool GameStart;
    private int ClipIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameStart = true;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        //If game is just starting, load audioclips and start soundtrack
        if (GameStart)
        {
            SoundTrackSource = new AudioSource[SoundTrackClip.Length];
            JumpSource = Instance.gameObject.AddComponent<AudioSource>();
            ButtonClickSource = Instance.gameObject.AddComponent<AudioSource>();
            WonSource = Instance.gameObject.AddComponent<AudioSource>();
            LostSource = Instance.gameObject.AddComponent<AudioSource>();

			JumpSource.clip = JumpClip;
			ButtonClickSource.clip = ButtonClickClip;
			WonSource.clip = WonClip;
			LostSource.clip = LostClip;

            for (int i = 0; i < SoundTrackClip.Length; i++)
            {
                SoundTrackSource[i] = Instance.gameObject.AddComponent<AudioSource>();
                SoundTrackSource[i].clip = SoundTrackClip[i];
            }

            SetVolumeLevels();
            StartSoundTrack();

            GameStart = false;
        }
    }

    private void Start()
    {
		if (MenuManager.Muted)
			AudioListener.volume = 0f;
		else
			AudioListener.volume = 1f;

		StartCoroutine(PlayNextSoundTrack());
    }

	#region Methods
    public void StopAllSoundEffects()
    {
        WonSource.Stop();
        LostSource.Stop(); 
    }

    public void PlayJumpClip()
    {
        JumpSource.Play();
    }

    public void PlayDeathClip()
    {
        LostSource.Play();
    }

    public void PlayWinClip()
    {
        WonSource.Play();
        StartCoroutine(LowerVolumeOverTime(WonSource));
    }

	//Call in Awake 
	private void StartSoundTrack()
	{
		ClipIndex = Random.Range(0, SoundTrackSource.Length);
		SoundTrackSource[ClipIndex].Play();
	}

    //Call in Awake
    private void SetVolumeLevels()
    {
        JumpSource.volume = .75f;
        ButtonClickSource.volume = .75f;
        WonSource.volume = .75f;
        LostSource.volume = .75f;

        for (int i = 0; i < SoundTrackSource.Length; i++)
        {
            SoundTrackSource[i].volume = .5f;
        }
    }
    #endregion

    #region Coroutines
    //Call in Start
    private IEnumerator PlayNextSoundTrack()
	{
		for (;;)
		{
			if (!SoundTrackSource[ClipIndex].isPlaying)
			{
				if (ClipIndex + 1 >= SoundTrackSource.Length)
					ClipIndex = -1;
				ClipIndex++;
				SoundTrackSource[ClipIndex].Play();
			}

			yield return new WaitForSeconds(.2f);
		}
	}

    private IEnumerator LowerVolumeOverTime(AudioSource source)
    {
        float startingAudioLevel = source.volume;
        float volumeLowerTimer = 0f;

        for (;;)
        {
            volumeLowerTimer += Time.deltaTime;

            if (volumeLowerTimer > 2f)
                source.volume -= .007f;

            if (source.volume <= 0f)
                break;
            
			yield return new WaitForSeconds(.017f);
        }

        source.Stop();
        source.volume = startingAudioLevel;
    }
    #endregion
}
