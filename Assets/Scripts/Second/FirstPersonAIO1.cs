﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class FirstPersonAIO1 : MonoBehaviour {

    public static FirstPersonAIO1 _instance {
        get; private set;
    }

    #region Variables

    #region Look Settings
    [Header("Look Settings")]
    [Space(8)]
    [Tooltip("Determines whether the player can move camera or not.")]
    public bool enableCameraMovement=true;
    [Tooltip("For Relative Motion Mode, Leave Default.")]
    public Vector2 rotationRange = new Vector2(170, Mathf.Infinity);
    [Tooltip("Determines how sensitive the mouse is.")] [Range(0.01f, 100)]
    public float mouseSensitivity = 10f;
    [Tooltip("Mouse Smoothness.")] [Range(0.01f, 100)]
    public float dampingTime = 0.05f;
    [Tooltip("For Debuging or if You don't plan on having a pause menu or quit button.")]
    public bool lockAndHideMouse = true;
    [Tooltip("Camera that you wish to rotate.")]
    public Transform playerCamera;


   /* [SerializeField]*/ [Tooltip("Automatically Create Crosshair")] private bool autoCrosshair;
    /*public*/ Sprite Crosshair;
    /*public*/ static GameObject qui;
    private Vector3 targetAngles;
    private Vector3 followAngles;
    private Vector3 followVelocity;
    private Vector3 originalRotation;
    [Space(15)]

    #endregion

    #region Movement Settings
    [Header("Movement Settings")]
    [Space(8)]
    [Tooltip ("Determines whether the player can move.")]//Tooltip是注释哦
    [HideInInspector]  public bool playerCanMove = false;
    [Tooltip("If true; Left shift = Sprint. If false; Left Shift = Walk.")]
   /* [SerializeField] */private bool walkByDefault = true;
    [Tooltip("Determines how fast Player walks.")]
    [HideInInspector] [Range(0.1f, 100)] /*public*/ float walkSpeed = 4f;
    [Tooltip("Determines how fast Player Sprints.")]
    [HideInInspector] [Range(0.1f, 100)] /*public*/ float sprintSpeed = 8f;
    [Tooltip("Determines how fast Player Strafes.")]
    [HideInInspector] [Range(0.1f, 100)] /*public*/ float strafeSpeed = 4f;
    [Tooltip("Determines how high Player Jumps.")]
    [HideInInspector] [Range(0.1f, 1000)] /*public*/ float jumpPower = 5f;
    [Tooltip("Determines whether to use Stamina or not.")]
   /* [SerializeField]*/ private bool useStamina = true;
    [Tooltip("Determines how quickly the players stamina runs out")]
   /* [SerializeField]*/ [Range(0.1f, 9)] private float staminaDepletionMultiplier = 2f;
    [Tooltip("Determines how much stamina the player has")]
    /*[SerializeField]*/ [Range(0, 100)] private float Stamina = 50;
    [HideInInspector] /*public*/ float speed;
    private float backgroundStamina;
    [System.Serializable]
    public class DynamicSpeedCurve
    {
        [Tooltip("Determines Whether to use a Dynamic Speed Curve or not")] public bool useDynamicSpeedCurve;
        [Range(1, 100)] public float curveTime;
        [Range(1, 10)] public float speedMultiplier;
        internal float inrSpeedMultiplier;


    }
    /*public*/ DynamicSpeedCurve dynamicSpeedCurve = new DynamicSpeedCurve();
    /*[SerializeField]*/ [Tooltip("Determines whether to use Crouch or not.")] private bool useCrouch = true;
    [System.Serializable]
    public class CrouchModifiers {
        [SerializeField] [Tooltip("Name of the Input Axis you wish to use for crouching, this must be set up in the InputManager.")]public string crouchInputAxis;
        [SerializeField] [Range(0.1f, 100)] internal float walkSpeedWhileCrouching;
        [SerializeField] [Range(0.1f, 100)] internal float sprintSpeedWhileCrouching;
        [SerializeField] [Range(0.1f, 100)] internal float strafeSpeedWhileCrouching;
        [SerializeField] [Range(0.1f, 100)] internal float jumpPowerWhileCrouching;
        [SerializeField] [Tooltip("Determines whether some public values (ie. walkSpeed_External) effect functionality")] internal bool useExternalModifier = false;
        internal float defaultWalkSpeed;
        internal float defaultSprintSpeed;
        internal float defaultStrafeSpeed;
        internal float defaultJumpPower;
        [HideInInspector] public float walkSpeed_External;
        [HideInInspector] public float sprintSpeed_External;
        [HideInInspector] public float strafeSpeed_External;
        [HideInInspector] public float jumpPower_External;
    }
    /*public*/ CrouchModifiers _crouchModifiers = new CrouchModifiers();
    [System.Serializable]
    public class FOV_Kick
    {
        [SerializeField] internal bool useFOVKick = false;
        [SerializeField] [Range(0, 10)] internal float FOVKickAmount = 4;
        [SerializeField] [Range(0.01f, 5)] internal float changeTime;
        [SerializeField] internal AnimationCurve KickCurve;
        internal bool fovKickReady = true;
        internal float fovStart;
    }
    /*public*/ FOV_Kick fOVKick = new FOV_Kick();
    [System.Serializable]
    public class AdvancedSettings {
        [Tooltip("Determines how fast Player falls.")] [Range(0.1f, 100)] public float gravityMultiplier = 1.0f;
        public PhysicMaterial zeroFrictionMaterial;
        public PhysicMaterial highFrictionMaterial;
    }
   /* [SerializeField]*/ private AdvancedSettings advanced = new AdvancedSettings();
    private CapsuleCollider capsule;
    private const float jumpRayLength = 0.7f;
    /*public*/protected bool IsGrounded { get; private set; }
    Vector2 inputXY;
    /*public*/    protected bool isCrouching { get; private set; }
    bool isSprinting = false;

    [HideInInspector] public Rigidbody fps_Rigidbody;
    [Space(15)]

    #endregion

    #region Headbobbing Settings
    [Header("Headbobbing Settings")]
    [Space(8)]

    [Tooltip("Determines Whether to use headbobing or not")] /*public*/ bool useHeadbob = true;
    [SerializeField] [Tooltip("Parent Of Player Camera")] private Transform head;
    [Tooltip("Overall Speed of Headbob")] [Range(0.1f, 10)] /*public*/ float headbobFrequency = 1.5f;
    [Tooltip("Headbob Sway Angle")] /*public*/ float headbobSwayAngle = 0.5f;
    [Tooltip("Headbob Height")] /*public*/ float headbobHeight = 0.3f;
    [Tooltip("Headbob Side Movement")] /*public*/ float headbobSideMovement = 0.5f;
    [Tooltip("Speeds up Headbobbing while keeping it not so jerky")] /*public*/ float headbobSpeedMultiplier = 0.3f;
    [Tooltip("Headbob Stride Speed Length")] /*public*/ float bobStrideSpeedLength = 0.3f;
    [Tooltip("Determines how much the head jotts when Jumping")] /*public*/ float jumpLandMove = 0.3f;
    [Tooltip("Determines how much the head jotts when landing")] /*public*/ float jumpLandTilt = 60;
   /* [SerializeField]*/ [Tooltip("Determines Whether to use movement Sounds.")] private bool _useFootStepSounds = false;
   /* [SerializeField]*/ [Tooltip("Volume to play the Footsteps with.")] [Range(0,10)] private float Volume = 5f;
    /*[SerializeField]*/ [Tooltip("Foot step Sounds. Will also act as a fall back for the Dynamic Foot Steps.")] private AudioClip[] footStepSounds;
    /*[SerializeField] */[Tooltip("The Sound made when jumping. Not Used in Dynamic Foot Steps mode.")] private AudioClip jumpSound;
   /* [SerializeField]*/ [Tooltip("The Sound made when landing from a jump or a fall. Not Used in Dynamic Foot Steps mode.")] private AudioClip landSound;
    [System.Serializable]
    public class DynamicFootStep
    {

        //Not Easily changeable atm
        [Tooltip("Should the controller use dynamic footsteps? For this to work properly, A physics material must be assigned to both this scipt and the collider you wish give sound fx to. I.e: To use the grass fx, assign a physics material to the 'Grass' slot below, as well as the collider you wish to act as a grassy area")]public bool useDynamicFootStepSettings;
        public PhysicMaterial _Wood;
        public PhysicMaterial _metalAndGlass;
        public PhysicMaterial _Grass;
        public PhysicMaterial _dirtAndGravle;
        public PhysicMaterial _rockAndConcrete;
        public PhysicMaterial _Mud;
        public PhysicMaterial _CustomMaterial;
        internal AudioClip[] qikAC;

        [Tooltip("Audio clips to be played while walking on the Wood physics material")] public AudioClip[] _woodFootsteps;
        [Tooltip("Audio clips to be played while walking on the Metal or Glass physics material")] public AudioClip[] _metalAndGlassFootsteps;
        [Tooltip("Audio clips to be played while walking on the Grass physics material")] public AudioClip[] _GrassFootsteps;
        [Tooltip("Audio clips to be played while walking on the Dirt or Gravelphysics material")] public AudioClip[] _dirtAndGravelFootsteps;
        [Tooltip("Audio clips to be played while walking on the Rock or Concrete physics material")] public AudioClip[] _rockAndConcreteFootsteps;
        [Tooltip("Audio clips to be played while walking on the Mud physics material")] public AudioClip[] _MudFootsteps;
        [Tooltip("Audio clips to be played while walking on the Custom physics material")] public AudioClip[] _CustomMaterialFoorsteps;

    }
    /*public*/ DynamicFootStep dynamicFootstep = new DynamicFootStep();
    [HideInInspector] public Vector3 gunBobPassThrough_POS;
    [HideInInspector] public Quaternion gunBobPassThrough_QUA;
    private Vector3 originalLocalPosition;
    private float nextStepTime = 0.5f;
    private float headbobCycle = 0.0f;
    private float headbobFade = 0.0f;
    private float springPosition = 0.0f;
    private float springVelocity = 0.0f;
    private float springElastic = 1.1f;
    private float springDampen = 0.8f;
    private float springVelocityThreshold = 0.05f;
    private float springPositionThreshold = 0.05f;
    Vector3 previousPosition;
    Vector3 previousVelocity = Vector3.zero;
    bool previousGrounded;
    AudioSource audioSource;

    #endregion     

    #region Shoot Settings
    [Header("Shoot Settings")]
    [Space(8)]
    [HideInInspector] public bool canShoot = true;
    //input time constriant
    public float timer=2f;
    float tictock = 0;
    //clip
    AudioClip wallClip;
    AudioClip monsterClip;
    AudioClip flyClip;
    //raycast
    Vector3 p1;
    LayerMask monsterMask;
    LayerMask wallMask ;
    public bool canTransfer = false;
    Vector3 targetPos;
    public float transferSpeed=5;
    public float qteTransferSpeed = 1;
    float moveSpeed;
    private  float maxDistanceToWall = 0.1f;
    public Transform imageEffectCube;
    private GameObject targetEffect;
    GameObject effectGo;
    [HideInInspector]public  bool gameOver = false;
    //qte
    bool hitMonster = false;
    bool hitWall = false;
    bool hitBattery = false;
    bool hitCenterBall = false;
    [HideInInspector] public bool hasQte = false;
    [HideInInspector] public bool qteWin = false;
    int endIndex = -1;
    //Camera Movement
    public Camera fpsCamera;
    public Camera tpsCamera;
    public Transform[] camTargets;
    public float camMoveSpeed = 2;
    int index = 0;

    public float distance = 20f;
    public  bool attackDragon = false;
    #endregion

    #endregion

    private void Awake()
    {
        _instance = this;

        #region Look Settings - Awake
        originalRotation = transform.localRotation.eulerAngles;

        #endregion 

        #region Movement Settings - Awake
        capsule = GetComponent<CapsuleCollider>();
        IsGrounded = true;
        isCrouching = false;
        fps_Rigidbody = GetComponent<Rigidbody>();
        _crouchModifiers.defaultWalkSpeed = walkSpeed;
        _crouchModifiers.defaultSprintSpeed = sprintSpeed;
        _crouchModifiers.defaultStrafeSpeed = strafeSpeed;
        _crouchModifiers.defaultJumpPower = jumpPower;
        #endregion

        #region Headbobbing Settings - Awake

        #endregion

        #region BETA_SETTINGS - Awake
    
#endregion

    }
    
    private void Start()
    {
        #region Look Settings - Start
        enableCameraMovement = true;

        autoCrosshair = true;
        if (autoCrosshair)
        {

            //qui = new GameObject("AutoCrosshair");
            //qui.AddComponent<RectTransform>();
            //qui.AddComponent<Canvas>();
            //qui.AddComponent<CanvasScaler>();
            //qui.AddComponent<GraphicRaycaster>();
            //qui.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            //GameObject quic = new GameObject("Crosshair");
            //quic.AddComponent<Image>().sprite = Crosshair;

            //qui.transform.SetParent(this.transform);
            //qui.transform.position = Vector3.zero;
            //quic.transform.SetParent(qui.transform);
            //quic.transform.position = Vector3.zero;
        }
        lockAndHideMouse = true;
        if (lockAndHideMouse) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        #endregion

        #region Movement Settings - Start
        backgroundStamina = Stamina;

        #endregion

        #region Headbobbing Settings - Start
        originalLocalPosition = head.localPosition;
        if(GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }
        previousPosition = fps_Rigidbody.position;
        audioSource = GetComponent<AudioSource>();
        #endregion

        #region Shoot Settings - Start        
        wallClip= Resources.Load<AudioClip>("Sound/Effect/oops");
        monsterClip = Resources.Load<AudioClip>("Sound/Effect/refract");
        flyClip = Resources.Load<AudioClip>("Sound/Effect/fly");
        targetEffect = Resources.Load<GameObject>("Prefabs/targetCicleEffect");
        imageEffectCube.gameObject.SetActive(false);
        tictock = timer;
        tpsCamera.enabled = false; tpsCamera.gameObject.GetComponent<AudioListener>().enabled = false;
        tpsCamera.transform.localPosition=camTargets[0].transform.localPosition;
        fpsCamera.enabled = true;
        #endregion
        #region BETA_SETTINGS - Start
        fOVKick.fovStart = playerCamera.GetComponent<Camera>().fieldOfView;
        #endregion
    }

    private void Update()
    {
        #region Look Settings - Update

        if(enableCameraMovement)
        {
            float mouseXInput;            float mouseYInput;
            mouseXInput = Input.GetAxis("Mouse Y");
            mouseYInput = Input.GetAxis("Mouse X");
            if (targetAngles.y > 180) { targetAngles.y -= 360; followAngles.y -= 360; } else if (targetAngles.y < -180) { targetAngles.y += 360; followAngles.y += 360; }
            if (targetAngles.x > 180) { targetAngles.x -= 360; followAngles.x -= 360; } else if (targetAngles.x < -180) { targetAngles.x += 360; followAngles.x += 360; }
            targetAngles.y += mouseYInput * mouseSensitivity;
            targetAngles.x += mouseXInput * mouseSensitivity;
            targetAngles.y = Mathf.Clamp(targetAngles.y, -0.5f * rotationRange.y, 0.5f * rotationRange.y);
            targetAngles.x = Mathf.Clamp(targetAngles.x, -0.5f * rotationRange.x, 0.5f * rotationRange.x);

        }
            followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, dampingTime);
            playerCamera.localRotation = Quaternion.Euler(-followAngles.x + originalRotation.x,0,0);
            transform.localRotation =  Quaternion.Euler(0, followAngles.y+originalRotation.y, 0);

        #endregion

        #region Shoot Settings - Update        
        if (!gameOver&&canShoot)
        {
            if (Camera.main == null) return;
            //发射一条从屏幕中点到摄像机方向的射线
            p1 = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 2));
            Ray monsterRay = new Ray(p1, Camera.main.transform.forward);
            Debug.DrawRay(p1, Camera.main.transform.forward, Color.green);
            wallMask = 1 << LayerMask.NameToLayer("monster") | 1 << LayerMask.NameToLayer("wall") | 1 << LayerMask.NameToLayer("centerBall") | 1 << LayerMask.NameToLayer("battery");//开启monster和wall
            //计时器限制，2s后才能发射下一次
            tictock += Time.deltaTime;
            if (tictock > timer)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    //每一次发射前可以cleanUp变量
                    Shoot(monsterRay);//发射
                    tictock = 0;
                }
            }

            if (canTransfer)
            {
                Transfer();//因为是持续运动，所以需要在Update里执行
                if (hasQte) CameraTransfer();//第三人称的相机运动
            }
        }     

        #endregion

        #region Movement Settings - Update

        #endregion

        #region Headbobbing Settings - Update

        #endregion

        #region BETA_SETTINGS - Update

        #endregion
    }
    void CameraTransfer()
    {
        if (Dragon._instance.attackDragon) return;
        while(index<camTargets.Length)//0,1
        {

            tpsCamera.transform.localPosition = Vector3.Lerp(tpsCamera.transform.localPosition, camTargets[index].localPosition, camMoveSpeed*Time.deltaTime);          
            if (Vector3.Distance(tpsCamera.transform.localPosition, camTargets[index].localPosition) < 0.01f)
            {
                index++;
            }
        } 
    }
    void Transfer()//持续改变玩家位置，把玩家送到end
    {
        if (hasQte) moveSpeed = qteTransferSpeed;
        else moveSpeed = transferSpeed;

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
        if (Vector3.Distance(transform.position, targetPos) < maxDistanceToWall)
        {
            index = 0;           
            imageEffectCube.gameObject.SetActive(false);
            ShootCleanUp();
            ChangeFpsCamera();//切换到第一人称相机
            enableCameraMovement = true;
            canTransfer = false;
        }
    }
    void ShootCleanUp()
    {
        hitWall = false; 
        hitMonster = false;
        hitCenterBall = false;
        hitBattery = false;
        qteWin = false;
    }
    void Shoot(Ray monsterRay)
    {
        //射线检测--数组
        RaycastHit[] monsterHits = Physics.RaycastAll(monsterRay, 3000, wallMask);
        if (monsterHits.Length == 1)
        {
            switch (monsterHits[0].collider.gameObject.layer)
            {
                case 18: AudioManager._instance.PlayEffect("gun"); GameManager2._instance.canChange2Battery = true; break;//hit monster
                case 19: ShootTransfer(monsterHits[0]); GameManager2._instance.canChange2Battery = true; break;
                case 25: hitBattery = true; ShootTransfer(monsterHits[0]); break;//射中也是一样移动过去
            }
        }
        else if (monsterHits.Length > 1)//打中1个以上的物体：墙和障碍物
        {
            GameManager2._instance.canChange2Battery = true;
            for (int i = 0; i < monsterHits.Length; i++)//检测射线射中的物体中是否有 指定物体
            {
                switch (monsterHits[i].collider.gameObject.layer)
                {
                    case 25: hitBattery = true; endIndex = i; break;
                    case 19: hitWall = true; endIndex = i; break;                   
                    case 18: hitMonster = true; break;
                    case 23: hitCenterBall = true; break;                  
                }
            }
            if (hitWall && hitMonster)
            {
                if (hitCenterBall)
                {
                    hasQte = true;//只是告诉乌贼，可以检测qte了
                    StartCoroutine(WuZei._instance.Qte(WuZei._instance.qteTime));
                }               
            }
            if(hitWall||hitBattery)  ShootTransfer(monsterHits[endIndex]);//不管有没有射中，tranfer总是要进行的。player只管移动。移动中发生的事情由乌贼的控制
        }
    }

    void ShootTransfer(RaycastHit wallHit)
    {
        AudioManager._instance.PlayEffect("fly");
        if (!wallHit.collider.gameObject.tag.Equals("electric"))
        {
            effectGo = Instantiate(targetEffect, wallHit.point + Camera.main.transform.forward * (-2f), Quaternion.identity);        //落地特效
            Destroy(effectGo, 0.5f);
            if (hitBattery)
            {
                effectGo.transform.forward = Vector3.up;//设置特效朝向            
            }
            else
            {
                effectGo.transform.forward = wallHit.collider.transform.right;//设置特效朝向
            }
        }       

        canTransfer = true;        //开启移动开关
        targetPos = wallHit.point - distance*transform.forward;//设置落地地点
        imageEffectCube.gameObject.SetActive(true);//设置屏幕特效
        ChangeTpsCamera();//切换到第三人称相机
    }
    void ChangeTpsCamera()
    {
        enableCameraMovement = false;//禁止相机运动
        fpsCamera.enabled = false;
        fpsCamera.gameObject.GetComponent<AudioListener>().enabled = false;

        tpsCamera.enabled = true;
        tpsCamera.gameObject.GetComponent<AudioListener>().enabled = true;        
    }
    void ChangeFpsCamera()
    {
        enableCameraMovement = true;//禁止相机运动
        fpsCamera.enabled = true;
        fpsCamera.gameObject.GetComponent<AudioListener>().enabled = true;
        tpsCamera.enabled = false;
        tpsCamera.gameObject.GetComponent<AudioListener>().enabled = false;
        tpsCamera.transform.localPosition =camTargets[0].transform.localPosition;
    }
    public void GameOver()
    {
        gameOver = true;
        enableCameraMovement = false;
        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
    }
    private void FixedUpdate()
    {
        #region Look Settings - FixedUpdate

        #endregion

        #region Movement Settings - FixedUpdate
        
        //bool wasWalking = !isSprinting;
        //if(useStamina) {
        //    if(backgroundStamina > 0) { if(!isCrouching) { isSprinting = Input.GetKey(KeyCode.LeftShift); } }
        //    if(isSprinting == true && backgroundStamina > 0) { backgroundStamina -= staminaDepletionMultiplier; } else if(backgroundStamina < Stamina && !Input.GetKey(KeyCode.LeftShift)) { backgroundStamina += staminaDepletionMultiplier / 2; }
        //} else { isSprinting = Input.GetKey(KeyCode.LeftShift); }

        //float inrSprintSpeed;
        //inrSprintSpeed = sprintSpeed;
        //if(dynamicSpeedCurve.useDynamicSpeedCurve){
            
        //    if(isSprinting) { dynamicSpeedCurve.inrSpeedMultiplier = Mathf.Lerp(dynamicSpeedCurve.inrSpeedMultiplier, dynamicSpeedCurve.speedMultiplier, Time.deltaTime / dynamicSpeedCurve.curveTime);   }
        //    else { dynamicSpeedCurve.inrSpeedMultiplier = 1; }
        //    inrSprintSpeed = sprintSpeed * dynamicSpeedCurve.inrSpeedMultiplier;
        //}
        //speed = walkByDefault ? isCrouching ? walkSpeed : (isSprinting ? inrSprintSpeed : walkSpeed) : (isSprinting ? walkSpeed : inrSprintSpeed);
        //Ray ray = new Ray(transform.position, -transform.up);
        //if(IsGrounded || fps_Rigidbody.velocity.y < 0.1) {
        //    RaycastHit[] hits = Physics.RaycastAll(ray, capsule.height * jumpRayLength);
        //    float nearest = float.PositiveInfinity;
        //    IsGrounded = false;
        //    for(int i = 0; i < hits.Length; i++) {
        //        if(!hits[i].collider.isTrigger && hits[i].distance < nearest) {
        //            IsGrounded = true;
        //            nearest = hits[i].distance;
        //        }
        //    }
        //}

        //float horizontalInput = Input.GetAxis("Horizontal");
        //float verticalInput = Input.GetAxis("Vertical");

        //inputXY = new Vector2(horizontalInput, verticalInput);
        //if(inputXY.magnitude > 1) { inputXY.Normalize(); }
        //Vector3 dMove = transform.forward * inputXY.y * speed + transform.right * inputXY.x * strafeSpeed;
        //float yv = fps_Rigidbody.velocity.y;
        //bool didJump = Input.GetButton("Jump");
        //if(IsGrounded && didJump && jumpPower > 0)
        //{
        //    yv += jumpPower;
        //    IsGrounded = false;
        //}

        //if(playerCanMove)
        //{
        //    fps_Rigidbody.velocity = dMove + Vector3.up * yv;
        //} else{fps_Rigidbody.velocity = Vector3.zero;}

        //if(dMove.magnitude > 0 || !IsGrounded) {
        //    GetComponent<Collider>().material = advanced.zeroFrictionMaterial;
        //} else { GetComponent<Collider>().material = advanced.highFrictionMaterial; }

        //fps_Rigidbody.AddForce(Physics.gravity * (advanced.gravityMultiplier - 1));
        //if(fOVKick.useFOVKick && wasWalking == isSprinting && fps_Rigidbody.velocity.magnitude > 0.1f && !isCrouching){
        //    StopAllCoroutines();
        //    StartCoroutine(wasWalking ? FOVKickOut() : FOVKickIn());
        //}

        //if(useCrouch && _crouchModifiers.crouchInputAxis != string.Empty) {


        //    if(Input.GetButton(_crouchModifiers.crouchInputAxis)) { if(isCrouching == false) {
        //            capsule.height /= 2;
                  
        //                walkSpeed = _crouchModifiers.walkSpeedWhileCrouching;
        //                sprintSpeed = _crouchModifiers.sprintSpeedWhileCrouching;
        //                strafeSpeed = _crouchModifiers.strafeSpeedWhileCrouching;
        //                jumpPower = _crouchModifiers.jumpPowerWhileCrouching;
                    
                     
        //            isCrouching = true;

        //        } } else if(isCrouching == true) {
        //        capsule.height *= 2;
        //    if(!_crouchModifiers.useExternalModifier)
        //    {
        //        walkSpeed = _crouchModifiers.defaultWalkSpeed;
        //        sprintSpeed = _crouchModifiers.defaultSprintSpeed;
        //        strafeSpeed = _crouchModifiers.defaultStrafeSpeed;
        //        jumpPower = _crouchModifiers.defaultJumpPower;
        //    } else
        //    {
        //            walkSpeed = _crouchModifiers.walkSpeed_External;
        //            sprintSpeed = _crouchModifiers.sprintSpeed_External;
        //            strafeSpeed = _crouchModifiers.strafeSpeed_External;
        //            jumpPower = _crouchModifiers.jumpPower_External;
        //        }
        //        isCrouching = false;

        //    }

        //}

        #endregion

        #region BETA_SETTINGS - FixedUpdate

        #endregion

        #region Headbobbing Settings - FixedUpdate

        if(useHeadbob == true)
        {

            Vector3 vel = (fps_Rigidbody.position - previousPosition) / Time.deltaTime;
            Vector3 velChange = vel - previousVelocity;
            previousPosition = fps_Rigidbody.position;
            previousVelocity = vel;
            springVelocity -= velChange.y;
            springVelocity -= springPosition * springElastic;
            springVelocity *= springDampen;
            springPosition += springVelocity * Time.deltaTime;
            springPosition = Mathf.Clamp(springPosition, -0.3f, 0.3f);

            if(Mathf.Abs(springVelocity) < springVelocityThreshold && Mathf.Abs(springPosition) < springPositionThreshold) { springPosition = 0; springVelocity = 0; }
            float flatVel = new Vector3(vel.x, 0.0f, vel.z).magnitude;
            float strideLangthen = 1 + (flatVel * bobStrideSpeedLength);
            headbobCycle += (flatVel / strideLangthen) * (Time.deltaTime / headbobFrequency);
            float bobFactor = Mathf.Sin(headbobCycle * Mathf.PI * 2);
            float bobSwayFactor = Mathf.Sin(Mathf.PI * (2 * headbobCycle + 0.5f));
            bobFactor = 1 - (bobFactor * 0.5f + 1);
            bobFactor *= bobFactor;

            float yPos = 0;
            float xPos = 0;
            float zTilt = 0;
            float xTilt = -springPosition * jumpLandTilt;

            if(IsGrounded)
            {
                if(new Vector3(vel.x, 0.0f, vel.z).magnitude < 0.1f) { headbobFade = Mathf.Lerp(headbobFade, 0.0f, Time.deltaTime); } else { headbobFade = Mathf.Lerp(headbobFade, 1.0f, Time.deltaTime); }
                float speedHeightFactor = 1 + (flatVel * headbobSpeedMultiplier);
                xPos = -headbobSideMovement * bobSwayFactor;
                yPos = springPosition * jumpLandMove + bobFactor * headbobHeight * headbobFade * speedHeightFactor;
                zTilt = bobSwayFactor * headbobSwayAngle * headbobFade;
            }

            head.localPosition = originalLocalPosition + new Vector3(xPos, yPos, 0);
            head.localRotation = Quaternion.Euler(xTilt, 0, zTilt);
            gunBobPassThrough_POS = new Vector3(xPos, yPos, 0);
            gunBobPassThrough_QUA = Quaternion.Euler(xTilt, 0, zTilt);
            #region footStep
            //if(dynamicFootstep.useDynamicFootStepSettings)
            //{
            //    Vector3 dwn = Vector3.down;
            //    RaycastHit hit = new RaycastHit();
            //    if(Physics.Raycast(transform.position, dwn, out hit))
            //    {
            //        dynamicFootstep.qikAC = (hit.collider.sharedMaterial == dynamicFootstep._Wood) ? 
            //        dynamicFootstep._woodFootsteps : ((hit.collider.sharedMaterial == dynamicFootstep._Grass) ? 
            //        dynamicFootstep._GrassFootsteps : ((hit.collider.sharedMaterial == dynamicFootstep._metalAndGlass) ? 
            //        dynamicFootstep._metalAndGlassFootsteps : ((hit.collider.sharedMaterial == dynamicFootstep._rockAndConcrete) ? 
            //        dynamicFootstep._rockAndConcreteFootsteps : ((hit.collider.sharedMaterial == dynamicFootstep._dirtAndGravle) ? 
            //        dynamicFootstep._dirtAndGravelFootsteps : ((hit.collider.sharedMaterial == dynamicFootstep._Mud)? 
            //        dynamicFootstep._MudFootsteps : ((hit.collider.sharedMaterial == dynamicFootstep._CustomMaterial)? 
            //        dynamicFootstep._CustomMaterialFoorsteps : footStepSounds))))));

            //        if(IsGrounded)
            //        {
            //            if(!previousGrounded)
            //            {
            //                if(_useFootStepSounds) { audioSource.PlayOneShot(dynamicFootstep.qikAC[Random.Range(0, dynamicFootstep.qikAC.Length)],Volume/10); }
            //                nextStepTime = headbobCycle + 0.5f;
            //            } else
            //            {
            //                if(headbobCycle > nextStepTime)
            //                {
            //                    nextStepTime = headbobCycle + 0.5f;
            //                    if(_useFootStepSounds){ audioSource.PlayOneShot(dynamicFootstep.qikAC[Random.Range(0, dynamicFootstep.qikAC.Length)],Volume/10); }
            //                }
            //            }
            //            previousGrounded = true;
            //        } else
            //        {
            //            if(previousGrounded)
            //            {
            //                if(_useFootStepSounds){ audioSource.PlayOneShot(dynamicFootstep.qikAC[Random.Range(0, dynamicFootstep.qikAC.Length)],Volume/10); }
            //            }
            //            previousGrounded = false;
            //        }

            //    } else {
            //        dynamicFootstep.qikAC = footStepSounds;
            //        if(IsGrounded)
            //        {
            //            if(!previousGrounded)
            //            {
            //                if(_useFootStepSounds){ audioSource.PlayOneShot(landSound,Volume/10); }
            //                nextStepTime = headbobCycle + 0.5f;
            //            } else
            //            {
            //                if(headbobCycle > nextStepTime)
            //                {
            //                    nextStepTime = headbobCycle + 0.5f;
            //                    int n = Random.Range(0, footStepSounds.Length);
            //                    if(_useFootStepSounds){ audioSource.PlayOneShot(footStepSounds[n],Volume/10); }
            //                    footStepSounds[n] = footStepSounds[0];
            //                }
            //            }
            //            previousGrounded = true;
            //        } else
            //        {
            //            if(previousGrounded)
            //            {
            //                if(_useFootStepSounds){ audioSource.PlayOneShot(jumpSound,Volume/10); }
            //            }
            //            previousGrounded = false;
            //        }
            //    }

            //} else
            //{
            //    if(IsGrounded)
            //    {
            //        if(!previousGrounded)
            //        {
            //            if(_useFootStepSounds) { audioSource.PlayOneShot(landSound,Volume/10); }
            //            nextStepTime = headbobCycle + 0.5f;
            //        } else
            //        {
            //            if(headbobCycle > nextStepTime)
            //            {
            //                nextStepTime = headbobCycle + 0.5f;
            //                int n = Random.Range(0, footStepSounds.Length);
            //                if(_useFootStepSounds){ audioSource.PlayOneShot(footStepSounds[n],Volume/10); footStepSounds[n] = footStepSounds[0];}

            //            }
            //        }
            //        previousGrounded = true;
            //    } else
            //    {
            //        if(previousGrounded)
            //        {
            //            if(_useFootStepSounds) { audioSource.PlayOneShot(jumpSound,Volume/10); }
            //        }
            //        previousGrounded = false;
            //    }
            //}
            #endregion

        }
        #endregion

    }
    public void DropToBottom()
    {
        enableCameraMovement = false;
    }
    #region  other
    public void UpdateAndApplyExternalCrouchModifies(){
        walkSpeed = _crouchModifiers.walkSpeed_External;
        sprintSpeed = _crouchModifiers.sprintSpeed_External;
        strafeSpeed = _crouchModifiers.strafeSpeed_External;
        jumpPower = _crouchModifiers.jumpPower_External;
    }

    public IEnumerator FOVKickOut()
    {
        float t = Mathf.Abs((playerCamera.GetComponent<Camera>().fieldOfView - fOVKick.fovStart) / fOVKick.FOVKickAmount);
        while(t < fOVKick.changeTime)
        {
            playerCamera.GetComponent<Camera>().fieldOfView = fOVKick.fovStart + (fOVKick.KickCurve.Evaluate(t / fOVKick.changeTime) * fOVKick.FOVKickAmount);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator FOVKickIn()
    {
        float t = Mathf.Abs((playerCamera.GetComponent<Camera>().fieldOfView - fOVKick.fovStart) / fOVKick.FOVKickAmount);
        while(t > 0)
        {
            playerCamera.GetComponent<Camera>().fieldOfView = fOVKick.fovStart + (fOVKick.KickCurve.Evaluate(t / fOVKick.changeTime) * fOVKick.FOVKickAmount);
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        playerCamera.GetComponent<Camera>().fieldOfView = fOVKick.fovStart;
    }

    public void UpdateOriginalRotation(Vector3 rot) { originalRotation = rot;}
    #endregion
}


