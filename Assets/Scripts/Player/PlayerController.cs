using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Damageable
{
    private CharacterController _characterController;
    private PlayerMovement _playerMovement;
    private PlayerCombat _playerCombat;
    private PlayerInput _playerInput;
    private InputActionAsset _inputActions;
    private Animator _animator;
    private BloodSplatterScreen _bloodSplatterScreen;
    private CameraSwitcher _cameraSwitcher;
    private FirstPersonMovement _firstPersonMovement;

    [SerializeField] private ParticleSystem SwordEffect;

    [SerializeField] public GameObject HandSword;
    [SerializeField] public GameObject BackSword;

    [SerializeField] public GameObject RightArm;

    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float jumpButtonGracePeriod = 0.2f;
    [SerializeField] private float jumpHorizontalSpeed = 2f;

    [SerializeField] private UIManager _uiManager;

    public AudioClip PerfectParrySound;
    private AudioSource _audioSource;

    private float ySpeed;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private bool isJumping;
    private bool isGrounded;
    private bool isCrouching = false;

    private bool isEquip = false;

    private float originalHeight;
    private float jumpfallingHeight;
    private float crouchingHeight;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float xRotation;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private HotbarSlot[] hotbarSlots;
    [SerializeField] private FlashlightManager flashlight;
    protected override void Start()
    {
        base.Start();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCombat = GetComponent<PlayerCombat>();
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _bloodSplatterScreen = GetComponent<BloodSplatterScreen>();
        _audioSource = GetComponent<AudioSource>();
        _cameraSwitcher = GetComponent<CameraSwitcher>();
        _firstPersonMovement = GetComponent<FirstPersonMovement>();

        _inputActions = _playerInput.actions;

        originalHeight = _characterController.height;
        jumpfallingHeight = _characterController.height *= 0.8f;
        crouchingHeight = _characterController.height *= 0.8f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        HandSword.SetActive(false);
        BackSword.SetActive(true);

        SwordEffect.gameObject.SetActive(false);

        _uiManager.SetMaxHealth(health);
        _uiManager.UnEquipUI();

        flashlight = FindObjectOfType<FlashlightManager>();
    }

    void Update()
    {
        if (!IsAlive) return;


        if (_cameraSwitcher != null && _cameraSwitcher.IsFirstPerson)
        {
            Vector2 lookInput = _inputActions["Look"].ReadValue<Vector2>();

            float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.Rotate(0, mouseX, 0);

            if (cameraPivot != null)
            {
                cameraPivot.localRotation = Quaternion.Euler(xRotation, 0, 0);
            }
        }

        {
            //if (_playerCombat.CanTriggerFinalAttack)
            //{
            //    _uiManager.EnableFinisherUI();
            //}

            //if (_inputActions["Equip"].WasPressedThisFrame())
            //{
            //    isEquip = !isEquip;
            //    _animator.SetBool("IsEquip", isEquip);
            //    if (isEquip)
            //    {
            //        _animator.SetTrigger("Equip");
            //        _uiManager.EquipUI();
            //    }
            //    else
            //    {
            //        _animator.SetTrigger("UnEquip");
            //        _uiManager.UnEquipUI();
            //    }
            //}
        }


        Vector2 moveInput = _inputActions["Move"].ReadValue<Vector2>();
        bool IsRunning = _inputActions["Sprinting"].IsPressed();
        if (_inputActions["Crouching"].WasPressedThisFrame())
        {
            isCrouching = !isCrouching;
        }
        bool jumpPressed = _inputActions["Jump"].WasPressedThisFrame();

        if (_cameraSwitcher.IsFirstPerson)
        {
            //_animator.enabled = false;
            _playerMovement.Reset();
            _firstPersonMovement.Move(moveInput, IsRunning, isCrouching);
            _firstPersonMovement.Jump(jumpPressed);
            _playerMovement.Reset();

        }
        else
        {
           // _animator.enabled = enabled;
            _playerMovement.ProcessMovement(moveInput, IsRunning, isCrouching);
            ProcessJump(jumpPressed);

            isGrounded = _characterController.isGrounded;
            _animator.SetBool("IsGrounded", isGrounded);

            if (!isCrouching && isGrounded)
            {
                _characterController.center = new Vector3(0, 1.1f, 0);
                _characterController.height = originalHeight;

            }
            else if (isCrouching)
            {
                _characterController.center = new Vector3(0, 0.8f, 0);
                _characterController.height = crouchingHeight;
            }
        }

        if (moveInput != Vector2.zero && isGrounded)
        {
            AudioHandler.Instance.PlaySFXIfNotPlaying("Player", 0, this.transform);
        }

        {
            //if (_inputActions["Parry"].WasPressedThisFrame() && isEquip && !_animator.GetBool("IsAttack"))
            //{
            //    _playerCombat.StartParry();
            //}

            //if (_inputActions["LightAttack"].WasPressedThisFrame() && !isEquip)
            //{
            //    _playerCombat.QueueLightAttack();
            //}

            //if (_inputActions["LightAttack"].WasPressedThisFrame() && isEquip)
            //{
            //    _playerCombat.QueueSwordAttack();
            //}

            //if (_inputActions["HeavyAttack"].WasPressedThisFrame() && isEquip)
            //{
            //    _playerCombat.QueueFinalSwordAttack();
            //    _uiManager.DisableFinisherUI();
            //}

            //if (_inputActions["HeavyAttack"].WasPressedThisFrame() && !isEquip)
            //{
            //    _playerCombat.QueueHeavyAttack();
            //}

        }
        // Toggle Inventory UI
        if (_inputActions["ToggleInventory"].WasPressedThisFrame())
        {
            InventoryManager.instance.ToggleInventory();
        }
        //Toggle Flashlight
        if (_inputActions["ToggleFlashlight"].WasPressedThisFrame())
        {
            if (flashlight != null)
            {
                flashlight.ToggleFlashlight();
            }
        }
        //Interact With Items
        if (_inputActions["Interact"].WasPressedThisFrame())
        {
            TryPickUpItem();
        }
        if (_inputActions["UseItem1"].WasPressedThisFrame())
        {
            UseHotbarItem(0);
        }
        if (_inputActions["UseItem2"].WasPressedThisFrame())
        {
            UseHotbarItem(1);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            _cameraSwitcher.ToggleCamera();
        }
    }
    //Picking Up Item
    void TryPickUpItem()
    {
        float pickupRadius = 1f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRadius);
        foreach (Collider collider in colliders)
        {
            PickupItem pickupItem = collider.GetComponent<PickupItem>();
            if (pickupItem != null && pickupItem.isPlayerNearby)
            {
                pickupItem.PickUp();
                return;
            }
        }
    }

    private void UseHotbarItem(int slotIndex)
    {
        // Prevent OutOfBounds exception
        if (hotbarSlots == null || slotIndex >= hotbarSlots.Length)
        {
            Debug.LogError("Hotbar slot " + slotIndex + " is out of range! Make sure all hotbar slots are assigned.");
            return;
        }

        if (hotbarSlots[slotIndex].transform.childCount > 0) // Check if slot has an item
        {
            Transform itemInSlot = hotbarSlots[slotIndex].transform.GetChild(0);
            Item item = InventoryManager.instance.items.Find(i => i.itemName == itemInSlot.name);

            if (item != null)
            {
                if (item.itemType == ItemType.Consumable)
                {
                    // Use consumable item
                    Consumable consumable = itemInSlot.GetComponent<Consumable>();
                    if (consumable != null)
                    {
                        Debug.Log("Using consumable from Hotbar Slot " + (slotIndex + 1));
                        consumable.UseConsumable();
                        hotbarSlots[slotIndex].RemoveItem();
                    }
                }
                else if (item.itemType == ItemType.QuestItem)
                {
                    TryOpenDoorWithKey(itemInSlot, item, slotIndex);
                }
                else
                {
                    Debug.Log("Item in Hotbar Slot " + (slotIndex + 1) + " is not usable.");
                }
            }
        }
        else
        {
            Debug.Log("Hotbar Slot " + (slotIndex + 1) + " is empty.");
        }
    }
    private void TryOpenDoorWithKey(Transform itemInSlot, Item keyItem, int slotIndex)
    {
        // Find all doors in the scene
        Door[] doors = FindObjectsOfType<Door>();

        foreach (Door door in doors)
        {
            // Check if the player is near a door that requires a key
            if (door.openMethod == Door.DoorOpenMethod.Key && !door.isOpen && door.PlayerIsNearby())
            {
                // Check if the key matches the door's keyName
                if (keyItem.itemName == door.keyName)
                {
                    door.OpenDoor();
                    Destroy(itemInSlot.gameObject); // Remove the key from the hotbar
                    hotbarSlots[slotIndex].RemoveItem(); // Remove UI reference
                    Debug.Log($"Used {keyItem.itemName} to open {door.keyName}!");
                    return;
                }
                else
                {
                    Debug.Log($"{keyItem.itemName} does not match the required key ({door.keyName})!");
                }
            }
        }
    }

    private void ProcessJump(bool jumpPressed)
    {
        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            _animator.SetBool("IsJumping", false);
            _animator.SetBool("IsFalling", false);
        }

        if (jumpPressed)
        {
            jumpButtonPressedTime = Time.time;
        }

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            ySpeed = -0.5f;
            isJumping = false;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                isJumping = true;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
                _animator.SetBool("IsJumping", true);
                _characterController.height = jumpfallingHeight;
            }
        }
        else
        {
            float fallThreshold = -1.0f;

            if ((isJumping && ySpeed < fallThreshold))
            {
                _animator.SetBool("IsFalling", true);
                _characterController.height = jumpfallingHeight;
            }
            else
            {
                _animator.SetBool("IsFalling", false);
            }
        }

        Vector3 velocity = new Vector3(0, ySpeed, 0);

        if (!isGrounded && isJumping)
        {
            velocity += transform.forward * jumpHorizontalSpeed;
        }

        _characterController.Move(velocity * Time.deltaTime);
    }

    public override void TakeDamage(float damage)
    {
        if (!_playerCombat.IsParrying())
        {
            base.TakeDamage(damage);

            if (IsAlive)
            {

                _bloodSplatterScreen.ShowBloodSplatter();

                _animator.SetTrigger("Hurt");

                DisableSwordEffect();

                StartCoroutine(ResetComboAfterFrames(80));

                _uiManager.UpdateHealth(health);
            }

            //_playerCombat.ResetCombo();
        }
        else
        {
            Debug.Log("Parry");
            _audioSource.PlayOneShot(PerfectParrySound);
        }

    }
    private IEnumerator ResetComboAfterFrames(int frameCount)
    {
        for (int i = 0; i < frameCount; i++)
        {
            yield return null; 
        }

        _playerCombat.ResetCombo();
    }

    protected override void OnDestroyed()
    {
        _animator.SetBool("IsAlive", false);
    }

    public void EquipWeapon()
    {
        HandSword.SetActive(true);
        BackSword.SetActive(false);
    }

    public void UnEquipWeapon()
    {
        HandSword.SetActive(false);
        BackSword.SetActive(true);
    }

    public void EnableSwordEffect()
    {
        SwordEffect.gameObject.SetActive(true);
        SwordEffect.Play();
    }

    public void DisableSwordEffect()
    {
        SwordEffect.Stop();
        SwordEffect.gameObject.SetActive(false);
    }
}
