using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to ragdoll object
//Manages the player's arm movement when grounded
//Relies on LeftHandStateManger and RightHandStateManager and PlayerJumper and CanvasButtonManager
public class ArmRotator : MonoBehaviour 
{
    public static bool Enabled;
	public static FixedJoint2D RightFixedJoint;
	public static FixedJoint2D LeftFixedJoint;
    public static bool RightButtonDown = false;
    public static bool LeftButtonDown = false;

    public float Torque;
    public float ConnectedRotationMultiplier;
    private const float DEAD_ZONE = .1f;
    private const int COOLDOWN_THRESHOLD = 10;
    private bool FirstContact;

    private void Start()
    {
        Enabled = true;
        RightFixedJoint = SceneObjects.RightHand.AddComponent<FixedJoint2D>() as FixedJoint2D;
        RightFixedJoint.enabled = false;
        RightFixedJoint.enableCollision = true;
        LeftFixedJoint = SceneObjects.LeftHand.AddComponent<FixedJoint2D>() as FixedJoint2D;
        LeftFixedJoint.enabled = false;
        LeftFixedJoint.enableCollision = true;
    }

    private void Update()
    {
        if (EndGameManager.PlayerLost)
        {
			RightFixedJoint.enabled = false;
			LeftFixedJoint.enabled = false;
			enabled = false;
        }
        else if(EndGameManager.PlayerWon)
        {
            if (RightHandStateManager.OtherRigidbody != null)
            {
				if (RightHandStateManager.OtherRigidbody.CompareTag("Rope"))
				{
					RightFixedJoint.connectedBody = RightHandStateManager.OtherRigidbody;
					RightFixedJoint.enabled = true;
				}
            }

            if (LeftHandStateManager.OtherRigidbody != null)
            {
				if (LeftHandStateManager.OtherRigidbody.CompareTag("Rope"))
				{
					LeftFixedJoint.connectedBody = LeftHandStateManager.OtherRigidbody;
					LeftFixedJoint.enabled = true;
				}
            }

            enabled = false;
        }
    }

    //CHECK - Might need to be changed to Update
    private void FixedUpdate ()
    {
        if (Enabled)
        {
			//If both fixedjoints are disabled, the one that touches an object first gets enabled
			if (RightHandStateManager.ArmTouchingObject || LeftHandStateManager.ArmTouchingObject)
			{
				if (!RightFixedJoint.enabled && !LeftFixedJoint.enabled && PlayerJumper.JumpTimer < 0)
				{
					if (RightHandStateManager.ArmTouchingObject)
						RightHandStateManager.Cooldown = COOLDOWN_THRESHOLD + 1;
					if (LeftHandStateManager.ArmTouchingObject)
						LeftHandStateManager.Cooldown = COOLDOWN_THRESHOLD + 1;

					FirstContact = true;
				}

				PlayerJumper.PlayerInAir = false;
			}

			if (!RightFixedJoint.enabled && !LeftFixedJoint.enabled)
				PlayerJumper.PlayerInAir = true;

			//If mouse is pressed or the player isn't attached to an object
			if ((Input.GetMouseButton(0) || FirstContact) && (!PlayerJumper.PlayerInAir || FirstContact))
			{
				SceneObjects.RightArm.GetComponent<Rigidbody2D>().freezeRotation = false;
				SceneObjects.LeftArm.GetComponent<Rigidbody2D>().freezeRotation = false;

				float dir = TorqueDirection();
				float xScale;
				//If dir is 0, don't change body direction
				if (dir < 1 && dir > -1)
					xScale = SceneObjects.Body.transform.localScale.x;
				else
					xScale = -dir;

				SceneObjects.Body.transform.localScale =
					new Vector3(xScale, SceneObjects.Body.transform.localScale.y, SceneObjects.Body.transform.localScale.z);

				ManageRightArmMovement(dir);

				ManageLeftArmMovement(dir);

				if (RightFixedJoint.enabled)
					LeftHandStateManager.Cooldown++;
				if (LeftFixedJoint.enabled)
					RightHandStateManager.Cooldown++;

				FirstContact = false;
			}
        }
    }

    //Gets direction arms should move in
    private float TorqueDirection()
    {
        float dir = 0;

        if (RightButtonDown)
            dir = 1;
        if (LeftButtonDown)
            dir = -1;

        return dir;
    }

    //Moves right arm
    private void ManageRightArmMovement(float dir)
    {
        if (RightHandStateManager.ArmTouchingObject || (RightButtonDown && RightFixedJoint.enabled))
		{
			if (RightHandStateManager.Cooldown > COOLDOWN_THRESHOLD && !RightFixedJoint.enabled)
			{
				SceneObjects.RightHand.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

				RightFixedJoint.connectedBody = RightHandStateManager.OtherRigidbody;
				RightFixedJoint.enabled = true;
				LeftFixedJoint.enabled = false;
				LeftHandStateManager.Cooldown = 0;
			}

			if (RightFixedJoint.enabled == true)
				SceneObjects.RightArm.GetComponent<Rigidbody2D>().AddTorque(Torque * ConnectedRotationMultiplier * dir);
		}
		else
		{
			SceneObjects.RightArm.GetComponent<Rigidbody2D>().AddTorque(Torque * dir);
		}
    }

    //Moves left arm
    private void ManageLeftArmMovement(float dir)
    {
		if (LeftHandStateManager.ArmTouchingObject || (LeftButtonDown && LeftFixedJoint.enabled))
		{
			if (LeftHandStateManager.Cooldown > COOLDOWN_THRESHOLD && !LeftFixedJoint.enabled)
			{
				SceneObjects.LeftHand.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

				LeftFixedJoint.connectedBody = LeftHandStateManager.OtherRigidbody;
				LeftFixedJoint.enabled = true;
                RightFixedJoint.enabled = false;
				RightHandStateManager.Cooldown = 0;
			}

			if (LeftFixedJoint.enabled == true)
				SceneObjects.LeftArm.GetComponent<Rigidbody2D>().AddTorque(Torque * ConnectedRotationMultiplier * dir);
		}
		else
		{
			SceneObjects.LeftArm.GetComponent<Rigidbody2D>().AddTorque(Torque * dir);
		}
    }
}
