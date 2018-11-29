using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to Ragdoll 
//Manages aiming arms at destination during jumps
//Relies on PlayerJumer
public class ArmAimer : MonoBehaviour 
{
    public static bool Enabled;
    public float Torque;
    public float MoveTowardsVelocity;

    private Vector2 Target = Vector2.zero;
    private Vector2 RightArmAim = Vector2.zero;
    private Vector2 LeftArmAim = Vector2.zero;
    private Vector2 RotatingRelMousePos = Vector2.zero;
    private Vector2 BodyPosAtJump = Vector2.zero;
    private Vector2 RelMousePos;
    private float TargetAngle = 0f;
    private float RightArmAngle = 0f;
    private float LeftArmAngle = 0f;
    private int TargetDir = 1;
    private int RightArmDir = 1;
    private int LeftArmDir = 1;
    private int RightArmTorqueDir = 1;
    private int LeftArmTorqueDir = 1;
    private bool FirstExecution = true;
    private bool MouseButtonDown = false;
    private bool RightArmIsAiming = true;
    private bool LeftArmIsAiming = true;

    private void Start()
    {
        Enabled = true;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
            MouseButtonDown = true;
        else
            MouseButtonDown = false;

        if (Input.GetMouseButtonDown(0) && !PlayerJumper.PlayerInAir)
        {
            RotatingRelMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            BodyPosAtJump = SceneObjects.Body.transform.position;
        }

        if (EndGameManager.PlayerLost || EndGameManager.PlayerWon)
            enabled = false;
    }

    //Rotate Arms
    void FixedUpdate()
    {
        if (Enabled)
        {
            //If player is in air and they aren't aiming their arms
            if (PlayerJumper.PlayerInAir && !MouseButtonDown && (RightArmIsAiming || LeftArmIsAiming))
            {
                AimArms();
            }
            else if (!PlayerJumper.PlayerInAir)
            {
                FirstExecution = true;
                LeftArmIsAiming = true;
                RightArmIsAiming = true;
                SceneObjects.RightArm.GetComponent<Rigidbody2D>().freezeRotation = false;
                SceneObjects.LeftArm.GetComponent<Rigidbody2D>().freezeRotation = false;
            }
        }
    }

    //Move arms toward mouse when in air to enable moving towards objects
    private void LateUpdate()
    {
        if (Enabled)
        {
			if (PlayerJumper.PlayerInAir)
			{
				if (MouseButtonDown && PlayerJumper.JumpAreaClicked)
				{
					SceneObjects.RightArm.GetComponent<Rigidbody2D>().freezeRotation = false;
					SceneObjects.LeftArm.GetComponent<Rigidbody2D>().freezeRotation = false;
					RelMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    if (RelMousePos.y < SceneObjects.Body.transform.position.y)
                    {
						SceneObjects.RightHand.GetComponent<Rigidbody2D>().AddForce(
						(RelMousePos - (Vector2)SceneObjects.RightHand.transform.position).normalized * MoveTowardsVelocity);

						SceneObjects.LeftHand.GetComponent<Rigidbody2D>().AddForce(
							(RelMousePos - (Vector2)SceneObjects.LeftHand.transform.position).normalized * MoveTowardsVelocity);
                    }
                    else
                    {
						SceneObjects.RightHand.GetComponent<Rigidbody2D>().AddForce(
						(RelMousePos - (Vector2)SceneObjects.RightHand.transform.position).normalized *
							(MoveTowardsVelocity * .5f));

						SceneObjects.LeftHand.GetComponent<Rigidbody2D>().AddForce(
							(RelMousePos - (Vector2)SceneObjects.LeftHand.transform.position).normalized *
						(MoveTowardsVelocity * .5f));
                    }
                }
			}
        }
    }

    private void AimArms()
    {
        //Set variables that need to change with each frame
		RightArmAim = SceneObjects.RightHand.transform.position - SceneObjects.RightArm.transform.position;
		LeftArmAim = SceneObjects.LeftHand.transform.position - SceneObjects.LeftArm.transform.position;
		RightArmDir = (int)Mathf.Sign(Mathf.Abs(SceneObjects.RightArm.transform.position.y) -
									  Mathf.Abs(SceneObjects.RightHand.transform.position.y));
		LeftArmDir = (int)Mathf.Sign(Mathf.Abs(SceneObjects.LeftArm.transform.position.y) -
									 Mathf.Abs(SceneObjects.LeftHand.transform.position.y));
		if (RightArmDir < 0)
			RightArmAngle = 360 - Vector2.Angle(Vector2.right, RightArmAim);
		else
			RightArmAngle = Vector2.Angle(Vector2.right, RightArmAim);

        if (LeftArmDir < 0)
            LeftArmAngle = 360 - Vector2.Angle(Vector2.right, LeftArmAim);
        else
            LeftArmAngle = Vector2.Angle(Vector2.right, LeftArmAim);

        //Set variables when the player first jumps
		if (FirstExecution)
		{
            Target = RotatingRelMousePos - BodyPosAtJump;
			TargetDir = (int)Mathf.Sign(Mathf.Abs(BodyPosAtJump.y) - Mathf.Abs(RotatingRelMousePos.y));
			if (TargetDir < 0)
				TargetAngle = 360 - Vector2.Angle(Vector2.right, Target);
			else
				TargetAngle = Vector2.Angle(Vector2.right, Target);

            if (TargetAngle - 180 < RightArmAngle)
                RightArmTorqueDir = 1;
            else
                RightArmTorqueDir = -1;

            if (TargetAngle - 180 < LeftArmAngle)
                LeftArmTorqueDir = 1;
            else
                LeftArmTorqueDir = -1;

            SceneObjects.RightArm.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            SceneObjects.LeftArm.GetComponent<Rigidbody2D>().angularVelocity = 0f;
		}

        //Aim Arms
        if (RightArmAngle < TargetAngle + 20 && RightArmAngle > TargetAngle - 20)
        {
            RightArmIsAiming = false;
            SceneObjects.RightArm.GetComponent<Rigidbody2D>().freezeRotation = true;
        }
        else if (RightArmIsAiming)
            SceneObjects.RightArm.GetComponent<Rigidbody2D>().AddTorque(Torque * RightArmTorqueDir);

        if (LeftArmAngle < TargetAngle + 20 && LeftArmAngle > TargetAngle - 20)
        {
			LeftArmIsAiming = false;
            SceneObjects.LeftArm.GetComponent<Rigidbody2D>().freezeRotation = true;
        }
		else if (LeftArmIsAiming)
            SceneObjects.LeftArm.GetComponent<Rigidbody2D>().AddTorque(Torque * LeftArmTorqueDir);
        
        FirstExecution = false;
    }
}
