using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to Arrow
//Manages arrow size, position, and rotation
public class ArrowManager : MonoBehaviour 
{
    private float DISAPPEAR_DIST = 15f;
    private float APPEAR_DIST = 25f;
    private float NORMAL_SIZE_DIST = 40f;
    private float SIZE_MODIFIER = .3f;
    private float DistMultiplier;              //Y axis multiplier
    private float StartingAlpha = 1f;               //Alpha value at start
    private float Angle;
    private float Sin;
    private float Cos;
    private Vector3 Target;
    private Vector3 ArrowPos;
    private Color SpriteColor;
    private Vector3 ArrowRot;
    private Vector3 StartingSize;
    private Vector3 NewSize;

    private void Start()
    {
        DistMultiplier = (13.6f / 2f) - 1f;
        ArrowPos = transform.position;
        StartingSize = new Vector3(.8f, .8f, 1f);
        NewSize = StartingSize;
    }

    private void Update()
    {
		Target = SceneObjects.Ropes.transform.position - SceneObjects.Body.transform.position;

		CheckIfShouldDisappear();
		SetPosition();
		SetRotation();
		SetSize();
    }

    private void SetPosition()
    {
		Angle = Vector2.Angle(Vector2.right, Target);
		Sin = Mathf.Sin(Angle * Mathf.Deg2Rad);
		Cos = Mathf.Cos(Angle * Mathf.Deg2Rad);

		ArrowPos.x = (Cos * DistMultiplier) + SceneObjects.Body.transform.position.x;
		ArrowPos.y = (Sin * DistMultiplier * 2.5f) + SceneObjects.Body.transform.position.y;
        
        transform.position = ArrowPos;
    }

    private void CheckIfShouldDisappear()
    {
        if (Target.magnitude < APPEAR_DIST)
            ManageAlpha(true);
        else
            ManageAlpha(false);
    }

    private void ManageAlpha(bool decrease)
    {
        SpriteColor = gameObject.GetComponent<SpriteRenderer>().color;

        if (decrease)
        {
            SpriteColor.a = ((Target.magnitude - DISAPPEAR_DIST) / (APPEAR_DIST - DISAPPEAR_DIST)) * StartingAlpha;
            if (SpriteColor.a < 0f)
                SpriteColor.a = 0f;
        }
        else
            SpriteColor.a = StartingAlpha;

        gameObject.GetComponent<SpriteRenderer>().color = SpriteColor;
    }

    private void SetRotation()
    {
        ArrowRot.z = Vector2.Angle(Vector2.up, Target);
        ArrowRot.z *= SceneObjects.Body.transform.position.x > SceneObjects.Ropes.transform.position.x ? 1 : -1;
        transform.eulerAngles = ArrowRot;
    }

    private void SetSize()
    {
        if (Target.magnitude > APPEAR_DIST && Target.magnitude < NORMAL_SIZE_DIST)
            NewSize.x = ((1 - ((Target.magnitude - APPEAR_DIST) / (NORMAL_SIZE_DIST - APPEAR_DIST))) * SIZE_MODIFIER) + 1;
        
        else if (Target.magnitude < APPEAR_DIST)
            NewSize.x = 1 + SIZE_MODIFIER;
        
        else
            NewSize = Vector3.one;

        NewSize.y = NewSize.x;
        transform.localScale = NewSize;
    }
}
