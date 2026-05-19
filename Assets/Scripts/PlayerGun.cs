using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGun : MonoBehaviour
{
    [Header("Gun Stats")]
    public float damage = 25f;
    public float range = 100f;
    public Camera fpsCamera; 

    [Header("Ammo Settings")]
    public int maxAmmo = 10;
    public int currentReserveAmmo = 30; // Total spare ammo
    public float reloadTime = 2f;
    public Text ammoText; 
    
    [Header("Visual & Recoil")]
    public ParticleSystem muzzleFlash;
    [Tooltip("How much the camera kicks up when shooting")]
    public float recoilAmount = 2f;
    private Look lookScript;
    public Move playerMovement; // Assign the player's Move script here
    
    [Header("Procedural Animation & Animator")]
    public Animator gunAnimator; // Assign this if you have an animator!
    public float runTiltAngle = 35f;
    public float tiltSpeed = 10f;
    private Quaternion originalRotation;

    [Header("Accuracy & Spread")]
    [Tooltip("Base bullet spread even when fully recovered")]
    public float baseSpread = 0.01f;
    [Tooltip("Maximum bullet spread after continuous firing")]
    public float maxSpread = 0.08f;
    [Tooltip("How much spread increases per shot")]
    public float spreadIncreasePerShot = 0.03f;
    [Tooltip("How quickly spread recovers per second")]
    public float spreadRecoveryRate = 0.05f;
    [Tooltip("Additional spread applied while moving")]
    public float movementSpreadPenalty = 0.05f;
    [Tooltip("Additional spread applied while sprinting")]
    public float sprintSpreadPenalty = 0.1f;
    private float currentSpread = 0f;
    
    [Header("UI & Effects")]
    public GameObject hitMarkerUI;
    [Tooltip("The UI RectTransform for the crosshair to scale with spread")]
    public RectTransform crosshairUI;
    public float baseCrosshairSize = 50f;
    public float maxCrosshairSize = 150f;
    public AudioClip hitSound;
    
    // AUDIO VARIABLES
    public AudioSource gunAudioSource;
    public AudioClip shootSound;

    private int currentAmmo;
    private bool isReloading = false;

    void Start()
    {
        originalRotation = transform.localRotation;
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        if (fpsCamera != null)
        {
            lookScript = fpsCamera.GetComponentInParent<Look>();
            if (lookScript == null) lookScript = fpsCamera.GetComponent<Look>();
        }
    }

    void Update()
    {
        bool isRunning = playerMovement != null && playerMovement.IsRunning();

        if (gunAnimator != null)
        {
            // Use your own animation if Animator is assigned
            gunAnimator.SetBool("isRunning", isRunning);
        }
        else
        {
            // Procedural run animation (tilt gun down)
            Quaternion targetRotation = isRunning ? originalRotation * Quaternion.Euler(runTiltAngle, 0, 0) : originalRotation;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
        }

        if (currentSpread > 0)
        {
            currentSpread -= spreadRecoveryRate * Time.deltaTime;
            currentSpread = Mathf.Max(0, currentSpread);
        }

        if (crosshairUI != null)
        {
            float targetBaseSpread = baseSpread;
            bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
            if (isRunning) targetBaseSpread += sprintSpreadPenalty;
            else if (isMoving) targetBaseSpread += movementSpreadPenalty;

            float totalSpread = targetBaseSpread + currentSpread;
            float maxPossibleSpread = maxSpread + sprintSpreadPenalty;

            float size = Mathf.Lerp(baseCrosshairSize, maxCrosshairSize, totalSpread / maxPossibleSpread);
            crosshairUI.sizeDelta = new Vector2(size, size);
        }

        if (isReloading) return;
        
        // Prevent shooting or reloading while running
        if (isRunning) return;

        if ((Input.GetKeyDown(KeyCode.R) || currentAmmo <= 0) && currentAmmo < maxAmmo && currentReserveAmmo > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButtonDown("Fire1") && currentAmmo > 0) 
        {
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        if (ammoText != null) ammoText.text = "RELOADING...";
        yield return new WaitForSeconds(reloadTime);
        
        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, currentReserveAmmo);
        currentAmmo += ammoToLoad;
        currentReserveAmmo -= ammoToLoad;
        
        isReloading = false;
        UpdateAmmoUI();
    }

    void Shoot()
    {
        currentAmmo--; 
        UpdateAmmoUI();

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (lookScript != null)
        {
            lookScript.ApplyRecoil(recoilAmount);
        }

        // PLAY THE GUNSHOT SOUND
        if (gunAudioSource != null && shootSound != null)
        {
            gunAudioSource.PlayOneShot(shootSound);
        }

        float targetBaseSpread = baseSpread;
        bool isRunning = playerMovement != null && playerMovement.IsRunning();
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        if (isRunning) targetBaseSpread += sprintSpreadPenalty;
        else if (isMoving) targetBaseSpread += movementSpreadPenalty;

        float spread = targetBaseSpread + currentSpread;
        Vector3 shootDirection = fpsCamera.transform.forward + (Random.insideUnitSphere * spread);
        shootDirection.Normalize();

        currentSpread = Mathf.Min(maxSpread, currentSpread + spreadIncreasePerShot);

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, shootDirection, out hit, range))
        {
            ZombieHealth target = hit.transform.GetComponent<ZombieHealth>();
            if (target != null)
            {
                Vector3 pushDirection = (target.transform.position - transform.position).normalized;
                pushDirection.y = 0; 

                target.TakeDamage(damage, pushDirection);
                
                if (gunAudioSource != null && hitSound != null)
                {
                    gunAudioSource.PlayOneShot(hitSound);
                }
                
                StartCoroutine(ShowHitMarker());
            }
        }
    }

    IEnumerator ShowHitMarker()
    {
        if (hitMarkerUI != null)
        {
            hitMarkerUI.SetActive(true);
            yield return new WaitForSeconds(0.1f); 
            hitMarkerUI.SetActive(false);
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + " / " + currentReserveAmmo;
        }
    }
}