using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Required to talk to the UI

public class PlayerGun : MonoBehaviour
{
    [Header("Gun Stats")]
    public float damage = 25f;
    public float range = 100f;
    public Camera fpsCamera; 

    [Header("Ammo Settings")]
    public int maxAmmo = 10;
    public float reloadTime = 2f;
    public Text ammoText; // Drag your AmmoText UI here
    
    private int currentAmmo;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        // If we are currently reloading, ignore all other input
        if (isReloading) return;

        // Start reload if we press 'R' or run out of bullets
        if (Input.GetKeyDown(KeyCode.R) || currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        // Only shoot if we have bullets left
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0) 
        {
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        
        if (ammoText != null) ammoText.text = "RELOADING...";
        
        // Pause this specific function for the reload time
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    void Shoot()
    {
        currentAmmo--; // Subtract a bullet
        UpdateAmmoUI();

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            ZombieHealth target = hit.transform.GetComponent<ZombieHealth>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
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