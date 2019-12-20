using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerController : MonoBehaviour
{
    public static List<PlayerController> AllPlayers;
    public int playerID;

    [Header("Stats")]
    public int health;
    public int maxHealth = 100;

    [Header("Input")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpStrength = 5f;
    public float gravity = 20f;
    public float mouseSensitivity = 1f;
    private PlayerControls controls;
    private Vector2 movementInput;
    public Vector2 lookInput;
    private bool shootInput;
    public Vector3 Velocity { get; private set; }
    public Vector3 LookDirection { get { return cam.transform.position + cam.transform.forward; } }

    [Header("Weapon")]
    public Weapon starterWeapon;
    private WeaponHandler activeWeapon, secondWeapon;
    public float aimFOV;
    private float defaultFOV;
    public float aimZoomSpeed;
    public bool IsAiming { get; private set; }
    public float AimValue { get; private set; }
    public Dictionary<Weapon.AmmoType, int> ammoStash;

    [Header("Aim Assistance")]
    public bool aimAssist = true;
    public float aimAssistRadius;
    public float aimAssistRange;

    [Header("References")]
    public Transform handSocket_Arms;
    public Transform handSocket_Body;
    public Animator armsAnim;
    private Transform arms { get { return armsAnim.transform; } }
    private Quaternion lastArmsRotation = Quaternion.identity;
    public Animator bodyAnim;
    //private Vector3 defaultArmAimPos, defaultArmAimRot;
    private float defaultArmAimHeight;
    public Camera cam;
    private float camRotY;
    public int BodyLayerID { get { return 8 + playerID; } }
    public int ArmsLayerID { get { return 12 + playerID; } }

    [Header("DEBUG")]
    public float aggroRange = 100f;
    public bool hasHealthKit = false;
    private CharacterController cc;
    private bool isPaused = false;
    private PlayerInput playerInput;
    public PlayerUI PlayerUI { get; private set; }
    public int ShootingLayerMask { get; private set; }
    private Interactable selectedInteractable;


    void Awake()
    {
        if (AllPlayers == null)
            AllPlayers = new List<PlayerController>();
        playerID = GetComponent<PlayerInput>().playerIndex;
        AllPlayers.Insert(playerID, this);
        AIDirector.SetInCombat(this, false);

        cc = GetComponent<CharacterController>();
        ammoStash = new Dictionary<Weapon.AmmoType, int>();

        bool uh = false; //Needed a bool for some reason
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindAction("Shoot", uh).performed += ctx => shootInput = true;
        playerInput.actions.FindAction("Shoot", uh).canceled += ctx => shootInput = false;


        SetCullingMasks();
        SetUpUI();
        OnPause();//KÄND BUG - Måste börja med UI-actionmappen
    }

    private void OnEnable()
    {
        UIManager.SetMultiCameraLayout();
        print("Player " + playerID + " joined the game!");
    }

    private void OnDisable()
    {
        UIManager.SetMultiCameraLayout();
        print("Player " + playerID + " left the game!");
    }

    private void Start()
    {
        //defaultArmAimPos = armsAnim.transform.localPosition;
        //defaultArmAimRot = armsAnim.transform.localEulerAngles;
        defaultArmAimHeight = armsAnim.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        defaultFOV = cam.fieldOfView;

        SetPosition(GameManager.Instance.playerSpawn.position);

        AddWeapon(starterWeapon);
        PickUpAmmo(starterWeapon.ammoType, 50); //Test start ammo
        PlayerUI.SetAmmoText(activeWeapon);
    }

    void Update()
    {
        AimUpdate();
        InteractionUpdate();

        if (shootInput)
            activeWeapon.TryShoot();

        if (Input.GetKeyDown(KeyCode.I)) //DEBUG AGGRO ALL IN RANGE - VERY INEFFICIENT!
        {
            Debug.Log("AGGRO!");
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, aggroRange / 2, transform.forward, 16);
            foreach(RaycastHit hit in hits)
            {
                Debug.Log("Try aggro");
                Hitbox h = hit.transform.GetComponent<Hitbox>();
                if (h != null)
                {
                    h.ZombieParent.Aggro(this);
                }
            }
        }

    }

    private void FixedUpdate()
    {
        MovementUpdate();
        RotationUpdate();
    }

    //------------------------------------------Input-----------------------------------------------


    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnAimDown()
    {
        AimToggle();
    }

    public void OnJump()
    {
        if (cc.isGrounded)
        {
            Vector3 vel = Velocity;
            vel.y = jumpStrength;
            Velocity = vel;
            bodyAnim.SetTrigger("Jump");
        }
    }

    public void OnPause()
    {
        playerInput.SwitchCurrentActionMap(isPaused ? "Gameplay" : "UI");
        movementInput = Vector2.zero;
        lookInput = Vector2.zero;
        PlayerUI.ToggleMenu();
        isPaused = !isPaused;
    }

    public void OnReload()
    {
        if (activeWeapon != null)
            activeWeapon.Reload();
    }

    public void OnInteract()
    {
        if (selectedInteractable != null)
            selectedInteractable.Interaction(this);
    }

    public void OnSwitchWeapon()
    {
        if (secondWeapon == null) return;

        activeWeapon.Show(false);

        WeaponHandler temp = activeWeapon;
        activeWeapon = secondWeapon;
        secondWeapon = temp;

        activeWeapon.Show(true);
        SetAnimatorController(activeWeapon.weapon.animatorController);
        armsAnim.SetTrigger("Switch");
        activeWeapon.Unholster();
        PlayerUI.SetAmmoText(activeWeapon);
    }

    //------------------------------------------Functions-----------------------------------------------

    private WeaponHandler SpawnWeapon(Weapon weapon)
    {
        WeaponHandler w = weapon.SpawnWeapon(this, handSocket_Arms, handSocket_Body);
        w.SetOwner(this);
        return w;
    }

    public void AddWeapon(Weapon weapon)
    {
        if (activeWeapon == null)
        {
            activeWeapon = SpawnWeapon(weapon);
            SetAnimatorController(activeWeapon.weapon.animatorController);
            armsAnim.SetTrigger("Switch");
            activeWeapon.Unholster();
            PlayerUI.SetAmmoText(activeWeapon);
            //PlayerUI.SetFirstWeaponIcon(weapon.icon);
        }
        else if (secondWeapon == null)
        {
            secondWeapon = SpawnWeapon(weapon);
            OnSwitchWeapon();
            //PlayerUI.SetSecondWeaponIcon(weapon.icon);
        }
        else
        {
            activeWeapon.Drop();
            activeWeapon = SpawnWeapon(weapon);
            SetAnimatorController(activeWeapon.weapon.animatorController);
            activeWeapon.Unholster();
            armsAnim.SetTrigger("Switch");
            PlayerUI.SetAmmoText(activeWeapon);
            //PlayerUI.SetFirstWeaponIcon(weapon.icon);
        }
    }

    private void SetAnimatorController (AnimatorOverrideController controller)
    {
        armsAnim.runtimeAnimatorController = controller;
    }

    private void SetCullingMasks()
    {
        GeneralHelper.SetLayerRecursively(bodyAnim.gameObject, BodyLayerID); //Set body layer
        cam.cullingMask = cam.cullingMask & ~(1 << BodyLayerID); //Hide players body from self
        GeneralHelper.SetLayerRecursively(armsAnim.gameObject, ArmsLayerID); //Set arms layer
        cam.cullingMask = cam.cullingMask | (1 << ArmsLayerID); //Show players arms to self

        gameObject.layer = BodyLayerID;

        ShootingLayerMask = (1 << ArmsLayerID) | (1 << BodyLayerID);
        ShootingLayerMask = ~ShootingLayerMask;
    }

    private void SetUpUI()
    {
        PlayerUI = Instantiate(UIManager.PlayerUIPrefab, UIManager.Instance.transform).GetComponent<PlayerUI>();
        PlayerUI.SetUp(this, GetComponent<MultiplayerEventSystem>());
    }

    public void SetCameraRect(float x, float y, float width, float height)
    {
        cam.rect = new Rect(x, y, width, width);
    }

    public void SetPosition(Vector3 position)
    {
        cc.enabled = false;
        cc.transform.position = position;
        Velocity = Vector3.zero;
        cc.enabled = true;
    }

    public void ChangeHealth(int healthDiff)
    {
        health += healthDiff;
        //Set health in UI?
        //Stun?
    }

    private void RotationUpdate()
    {
        Vector2 input = lookInput * mouseSensitivity;
        transform.Rotate(transform.up * input.x);

        camRotY += input.y;
        ClampRotation();

        ////Arms
        //armsAnim.transform.parent.rotation = Quaternion.Lerp(lastArmsRotation, cam.transform.rotation, Time.fixedDeltaTime * (IsAiming ? 50 : 20));
        //lastArmsRotation = armsAnim.transform.parent.rotation;
    }

    private void ClampRotation()
    {
        camRotY = Mathf.Clamp(camRotY, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(-camRotY, 0f, 0f);
    }

    private void MovementUpdate()
    {
        if (cc.isGrounded)
            Velocity = Velocity.y * transform.up + (movementInput.y * transform.forward + movementInput.x * transform.right) * (IsAiming ? walkSpeed : runSpeed);
        else
            Velocity += Vector3.down * gravity * Time.deltaTime; //Gravity

        cc.Move(Velocity * Time.deltaTime); //Movement

        bodyAnim.SetFloat("Horizontal", movementInput.x);
        bodyAnim.SetFloat("Vertical", movementInput.y);
        bodyAnim.SetBool("IsAirborne", !cc.isGrounded);
    }

    private void InteractionUpdate()
    {
        if (Physics.Raycast(cam.gameObject.transform.position, cam.gameObject.transform.forward, out RaycastHit hit, 3f, ShootingLayerMask))
        {
            Interactable i = hit.transform.GetComponent<Interactable>();

            if (i == null)
            {
                if (selectedInteractable != null)
                {
                    selectedInteractable.Highlight(false);
                    selectedInteractable = null;
                }
            }
            else
            {
                if (selectedInteractable != null && selectedInteractable != i)
                    selectedInteractable.Highlight(false);
                selectedInteractable = i;
                selectedInteractable.Highlight(true);
            }
        } else if (selectedInteractable != null)
        {
            selectedInteractable.Highlight(false);
            selectedInteractable = null;
        }
    }

    public void ShootCallback(float recoilAmmount)
    {
        PlayerUI.SetAmmoText(activeWeapon);
        armsAnim.SetTrigger("Shoot");
        bodyAnim.SetTrigger("Shoot");

        //Recoil
        //camRotY += recoilAmmount;
        //ClampRotation();
    }

    private void AimToggle()
    {
        IsAiming = !IsAiming;
        armsAnim.SetBool("Aiming", IsAiming);
        bodyAnim.SetBool("Aiming", IsAiming);
    }

    private void AimUpdate()
    {
        if (IsAiming)
        {
            if (AimValue < 1)
                AimValue += aimZoomSpeed * Time.deltaTime;
            else AimValue = 1;
        }
        else
        {
            if (AimValue > 0)
                AimValue -= aimZoomSpeed * Time.deltaTime;
            else AimValue = 0;
        }

        cam.fieldOfView = Mathf.Lerp(defaultFOV, aimFOV, AimValue);
        //armsAnim.transform.localEulerAngles = Vector3.Lerp(defaultArmAimRot, activeWeapon.armAimRot, AimValue);
        //armsAnim.transform.localPosition = Vector3.Lerp(defaultArmAimPos, activeWeapon.armAimPos, AimValue);
        armsAnim.transform.localPosition = new Vector3(armsAnim.transform.localPosition.x, Mathf.Lerp(defaultArmAimHeight, activeWeapon.armAimHeight, AimValue), armsAnim.transform.localPosition.z);
        //armsAnim.transform.localPosition = Vector3.Lerp( new Vector3 (armsAnim.transform.localPosition.x, defaultArmAimHeight, armsAnim.transform.localPosition.z), new Vector3(armsAnim.transform.localPosition.x, activeWeapon.armAimHeight, armsAnim.transform.localPosition.z), AimValue);
        PlayerUI.SetCrosshairOpacity(Mathf.Abs(AimValue-1));
        armsAnim.SetFloat("AimValue", AimValue);
        bodyAnim.SetFloat("AimValue", AimValue);
    }

    public static PlayerController GetClosestPlayer(Vector3 position)
    {
        if (AllPlayers == null || AllPlayers.Count < 1) return null;

        PlayerController closestPlayer = AllPlayers[0];
        float closestDist = Vector3.Distance(closestPlayer.transform.position, position);
        foreach (PlayerController player in AllPlayers)
        {
            float newDist = Vector3.Distance(player.transform.position, position);
            if (newDist < closestDist)
            {
                closestPlayer = player;
                closestDist = newDist;
            }
        }
        return closestPlayer;
    }

    public void Disconnect()
    {
        AllPlayers.Remove(this);
        Destroy(gameObject);
    }

    public void PickUpAmmo(Weapon.AmmoType ammoType, int amount)
    {
        if (!ammoStash.ContainsKey(ammoType))
            ammoStash.Add(ammoType, amount);
        else
            ammoStash[ammoType] += amount;
    }

    //private IEnumerator Recoil(float recoilAmmount)
    //{
    //    yield return null;
    //}
}
