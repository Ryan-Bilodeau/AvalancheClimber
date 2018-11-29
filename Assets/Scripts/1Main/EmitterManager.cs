using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to falling emitter child of body
//Manages the emitter
public class EmitterManager : MonoBehaviour 
{
    private float ENABLED_SPEED = 2f;
    private float X_MULTIPLIER = 4f;
    private bool IsPlaying;
    private ParticleSystem.MainModule NewMain;
    private ParticleSystem.EmissionModule NewEmitter;
    private int YVelDir;
    private float XVel;

    private void Awake()
    {
        NewEmitter = transform.GetComponent<ParticleSystem>().emission;
        InvokeRepeating("SetRotation", .5f, .5f);
        InvokeRepeating("UpdateEmissionRate", .2f, .2f);
        InvokeRepeating("DisableWhenPlayerLoses", .5f, .5f);
    }

    void Update ()
	{
        SetOnOrOff();
	}

    private void SetOnOrOff()
    {
        if (Mathf.Abs(transform.GetComponentInParent<Rigidbody2D>().velocity.y) > ENABLED_SPEED && !IsPlaying)
        {
            transform.GetComponent<ParticleSystem>().Play();
            IsPlaying = true;
        }
        else if (Mathf.Abs(transform.GetComponentInParent<Rigidbody2D>().velocity.y) < ENABLED_SPEED && IsPlaying)
        {
            transform.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
            IsPlaying = false;
        }
    }

    private void SetRotation()
    {
        if (IsPlaying)
        {
			NewMain = transform.GetComponent<ParticleSystem>().main;
			YVelDir = transform.GetComponentInParent<Rigidbody2D>().velocity.y > 0 ? 1 : -1;
			XVel = transform.GetComponentInParent<Rigidbody2D>().velocity.x;
			NewMain.startRotation = XVel * YVelDir * X_MULTIPLIER * Mathf.Deg2Rad;
        }
    }

    private void UpdateEmissionRate()
    {
        NewEmitter.rateOverDistance = new ParticleSystem.MinMaxCurve(0f, 3f - ((UIManager.Altitude / 100f) * 3f));
    }

	private void DisableWhenPlayerLoses()
	{
		if (EndGameManager.PlayerLost)
		{
			this.enabled = false;
			CancelInvoke();
		}
	}
}
