using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to Ropes
//Manages moving the ropes
public class RopeMover : MonoBehaviour 
{
    public float WindForce;
    private Transform[] Ropes;
    private int Timer;

    private void Awake()
    {
        Ropes = new Transform[transform.childCount];

        for (int i = 0; i < Ropes.Length; i++)
        {
            Ropes[i] = transform.GetChild(i);
        }
    }
    private void FixedUpdate()
    {
        if (UIManager.Altitude < 30 && !EndGameManager.PlayerWon)
        {
            MoveRope();
        }
    }

    private void MoveRope()
    {
        if (Timer > 0)
            Timer++;
        else
            Timer--;

        if (Timer > 100)
            Timer = -1;
        else if (Timer < -100)
            Timer = 1;

        for (int i = 0; i < transform.childCount; i++)
        {
            Ropes[i].GetComponent<Rigidbody2D>().AddForce(
                new Vector2(WindForce * (Timer / 100) * (transform.childCount / (i + 1)), 0f));
        }
    }
}
