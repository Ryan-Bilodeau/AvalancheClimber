using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to right hand
public class RightHandStateManager : MonoBehaviour 
{
    public static bool ArmTouchingObject;
    public static Rigidbody2D OtherRigidbody;
    //Used by ArmRotator
    public static int Cooldown;

    private void Start()
    {
        ArmTouchingObject = false;
    }

    private void OnCollisionEnter2D(Collision2D coll)
	{
        if (coll.gameObject.CompareTag("Object") || coll.gameObject.CompareTag("Debris") || coll.gameObject.CompareTag("Rope"))
		{
			ArmTouchingObject = true;
			OtherRigidbody = coll.gameObject.GetComponent<Rigidbody2D>();

            if (coll.gameObject.CompareTag("Rope"))
                EndGameManager.PlayerWon = true;
        }
	}

	private void OnCollisionExit2D(Collision2D coll)
	{
        if (coll.gameObject.CompareTag("Object") || coll.gameObject.CompareTag("Debris"))
		{
			ArmTouchingObject = false;
			OtherRigidbody = null;
		}
	}
}
