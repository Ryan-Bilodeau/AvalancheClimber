using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to ragdoll object
//Manages jumping the player
//Relies on ArmRotator
public class PlayerJumper : MonoBehaviour 
{
    public static bool Enabled;
    public static float JumpTimer;
    public static bool JumpAreaClicked;
	public static bool PlayerInAir;        //Set to false in ArmRotator
	public static Vector2 RelMousePos;

    public float JumpCooldown;
	public float VELOCITY_MULTIPLIER;
    private bool SingleClick;
    private bool CanLaunch;

    private void Start()
    {
        Enabled = true;
    }

    private void Update ()
	{
        if (Enabled)
        {
			if (Input.GetMouseButtonDown(0) && (ArmRotator.RightFixedJoint.enabled || ArmRotator.LeftFixedJoint.enabled))
				SingleClick = true;

			if (JumpTimer >= 0)
				JumpTimer -= Time.deltaTime;
        }

        if (EndGameManager.PlayerWon || EndGameManager.PlayerLost)
            enabled = false;
    }

    private void FixedUpdate()
    {
        if (Enabled)
        {
			if (ArmRotator.RightFixedJoint.enabled || ArmRotator.LeftFixedJoint.enabled)
				CanLaunch = true;
			else
				CanLaunch = false;

			if (JumpAreaClicked && SingleClick && CanLaunch && JumpTimer < 0)
			{
                AudioManager.Instance.PlayJumpClip();

				PlayerInAir = true;
				ArmRotator.RightFixedJoint.enabled = false;
				RightHandStateManager.Cooldown = 0;
				ArmRotator.LeftFixedJoint.enabled = false;
				LeftHandStateManager.Cooldown = 0;

				RelMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				//Flip body to face jumping direction
				float dir = RelMousePos.x - SceneObjects.Body.transform.position.x;
				SceneObjects.Body.transform.localScale = new Vector3
					(Mathf.Sign(dir), SceneObjects.Body.transform.localScale.y, SceneObjects.Body.transform.localScale.z);

				//Set every part of the ragdoll to 0 velocity before applying more velocity
				foreach (Transform child in SceneObjects.Ragdoll.transform)
				{
                    if (!child.GetComponent<Rigidbody2D>().Equals(null))
                        child.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				}

				//Determine the force applied to the ragdoll
				Vector2 force = (RelMousePos - (Vector2)SceneObjects.Body.transform.position).normalized;
				force *= VELOCITY_MULTIPLIER;

				//Apply force to each part of the ragdoll
				foreach (Transform child in SceneObjects.Ragdoll.transform)
				{
                    if (!child.GetComponent<Rigidbody2D>().Equals(null))
					    child.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Force);
				}

				SingleClick = false;
				JumpTimer = JumpCooldown;
			}
        }
    }
}
