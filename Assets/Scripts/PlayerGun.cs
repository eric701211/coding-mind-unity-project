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
    public float reloadTime = 2f;
    public Text ammoText; 
    
    [Header("UI & Effects")]
    public GameObject hitMarkerUI;
    
    // NEW AUDIO VARIABLES
    public AudioSource gunAudioSource;
    public AudioClip shootSound;

    private int currentAmmo;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetKeyDown(KeyCode.R) || currentAmmo <= 0)
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
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    void Shoot()
    {
        currentAmmo--; 
        UpdateAmmoUI();

        // PLAY THE GUNSHOT SOUND
        if (gunAudioSource != null && shootSound != null)
        {
            gunAudioSource.PlayOneShot(shootSound);
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            ZombieHealth target = hit.transform.GetComponent<ZombieHealth>();
            if (target != null)
            {
                Vector3 pushDirection = (target.transform.position - transform.position).normalized;
                pushDirection.y = 0; 

                target.TakeDamage(damage, pushDirection);
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
            ammoText.text = currentAmmo + " / " + maxAmmo;
        }
    }
}