﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    [Rename("河道宽度")]
    private float riverWide;
    [SerializeField]
    [Rename("玩家移动速度")]
	private float playerMoveSpeed;
	public float tilt;

	public Rigidbody WebPole_L;
    public Rigidbody WebPole_R;
	public GameObject Web;

    public bool skillInput;
    public static float dis;

    public delegate void SkillInput();
    public static event SkillInput SkillInputHandler;

    [SerializeField]
    Transform PlayerModel_L;
    [SerializeField]
    Transform PlayerModel_R;
    public Text debug;

    //if move to boarder then clamp it
    void ClampMove(){
        if (WebPole_L.transform.position.x < -riverWide){
            WebPole_L.transform.position = new Vector3(-riverWide + 0.05f,WebPole_L.transform.position.y,WebPole_L.transform.position.z);
            //WebPole_L.velocity = Vector3.zero;
        }
            
        if (WebPole_R.transform.position.x > riverWide)
            WebPole_R.transform.position = new Vector3(riverWide - 0.05f, WebPole_R.transform.position.y, WebPole_R.transform.position.z);
            //WebPole_R.velocity = Vector3.zero;
    }

    bool killMovement = false;

    void FixedUpdate ()
	{
        FixPlayerModelPosition();
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.UpArrow)&&!killMovement)
        {
            skillInput = true;
            SkillInputHandler();
        }
        else
            skillInput = false;


        
        float moveHorizontalL = killMovement ? 0: Input.GetAxis("HorizontalL");
        float moveHorizontalR = killMovement ? 0: Input.GetAxis("HorizontalR");

        Vector3 movementL = new Vector3(moveHorizontalL, 0.0f, 0.0f);
        Vector3 movementR = new Vector3(moveHorizontalR, 0.0f, 0.0f);


        WebPole_L.velocity = movementL * playerMoveSpeed;
        WebPole_R.velocity = movementR * playerMoveSpeed;

        ClampMove();
#endif

#if UNITY_ANDROID
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.UpArrow))
        {
            skillInput = true;
            SkillInputHandler();
        }
        else
            skillInput = false;

        Vector3 rotationVelocity = Input.gyro.attitude.eulerAngles;
        float l = Input.gyro.attitude.eulerAngles.x;
        float threshold = 5f;
        l = l > 180f ? l - 360f : l;
        if (l > threshold)
            l = 1.0f;
        else if (l < -threshold)
            l = -1.0f;
        else
            l = 0;
        Debug.Log(l);
        Vector3 movementL = new Vector3(l, 0.0f, 0.0f);

        int r = Random.Range(0, 3) - 1;
        Vector3 movementR = new Vector3(0f, 0.0f, 0.0f);
        WebPole_R.velocity = movementL * playerMoveSpeed;
        WebPole_R.velocity = movementR * playerMoveSpeed;
#endif
        //Player_L.position = new Vector3(Mathf.Clamp (Player_L.position.x, boundary.xMin, Player_R.position.x), Player_L.position.y, Player_L.position.z);
        //Player_R.position = new Vector3(Mathf.Clamp (Player_R.position.x, Player_L.position.x, boundary.xMax), Player_R.position.y, Player_R.position.z);

        //Player_R.rotation = Quaternion.Euler (0.0f, Player_R.velocity.x * tilt, 0.0f);
		//Player_L.rotation = Quaternion.Euler (0.0f, Player_L.velocity.x * tilt, 0.0f);

       // Web.transform.rotation = Quaternion.Euler(60.0f, 0.0f, 0.0f);
        Web.transform.position =  new Vector3((WebPole_R.position.x + WebPole_L.position.x) / 2,Web.transform.position.y,Web.transform.position.z);
        Web.transform.localScale = new Vector3((WebPole_R.position - WebPole_L.position).x-1.5f,9f,1.5f);
        dis = WebPole_R.position.x - WebPole_L.position.x;
    }

    void KillMovement() {
        killMovement = true;
    }


    private void Awake()
    {
        GameManager.MGameOverHandler += KillMovement;
    }

    private void OnDestroy()
    {
        GameManager.MGameOverHandler -= KillMovement;
    }

    void Start ()
	{
#if UNITY_ANDROID
        Input.gyro.enabled = true;
#endif

        //Application.targetFrameRate = 30;
        //playerL = transform.parent.Find("PlayerL");
        //playerR = transform.parent.Find("PlayerR");
    }

    void FixPlayerModelPosition() {
        PlayerModel_L.transform.position = new Vector3(WebPole_L.transform.position.x - 1.0f, PlayerModel_L.transform.position.y, PlayerModel_L.transform.position.z);
        PlayerModel_R.transform.position = new Vector3(WebPole_R.transform.position.x + 1.0f, PlayerModel_R.transform.position.y, PlayerModel_R.transform.position.z);
    }
}