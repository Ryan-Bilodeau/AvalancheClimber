using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to main camera
//Holds references to different objects in the scene
//TO ONLY BE USED BY OTHER SCRIPTS, DON'T RUN UPDATE/FIXED UPDATE IN HUR
public class SceneObjects : MonoBehaviour 
{
    public static GameObject Ragdoll;
    public static GameObject Body;
    public static GameObject RightArm;
    public static GameObject LeftArm;
    public static GameObject RightHand;
    public static GameObject LeftHand;

    public static GameObject Altitude;
    public static GameObject AltitudeBackground;
    public static GameObject House;
    public static GameObject Arrow;
    public static GameObject JumpArea;
    public static GameObject Sky;
    public static GameObject StarsManager;
    public static GameObject FadeCanvas;
    public static GameObject Ropes;
    public static GameObject RestartCanvas;
    public static GameObject UICanvas;
    public static GameObject PauseCanvas;
    public static GameObject AdCanvas;

    public GameObject AltitudeReference;
    public GameObject AltitudeBackgroundReference;
    public GameObject HouseReference;
    public GameObject ArrowReference;
    public GameObject JumpAreaReference;
    public GameObject SkyReference;
    public GameObject StarsManagerReference;
    public GameObject FadeCanvasReference;
    public GameObject RopesReference;
    public GameObject RestartCanvasReference;
    public GameObject UICanvasReference;
    public GameObject PauseCanvasReference;
    public GameObject AdCanvasReference;

    public void Awake()
    {
        Ragdoll = GameObject.FindWithTag("Ragdoll");
        Body = GameObject.FindWithTag("Body");
        RightArm = GameObject.FindWithTag("RightArm");
        LeftArm = GameObject.FindWithTag("LeftArm");
        RightHand = GameObject.FindWithTag("RightHand");
        LeftHand = GameObject.FindWithTag("LeftHand");

        Altitude = AltitudeReference;
        AltitudeBackground = AltitudeBackgroundReference;
        House = HouseReference;
        Arrow = ArrowReference;
        JumpArea = JumpAreaReference;
        Sky = SkyReference;
        StarsManager = StarsManagerReference;
        FadeCanvas = FadeCanvasReference;
        Ropes = RopesReference;
        RestartCanvas = RestartCanvasReference;
        UICanvas = UICanvasReference;
        PauseCanvas = PauseCanvasReference;
        AdCanvas = AdCanvasReference;
    }
}
