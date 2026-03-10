using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MouseShoot : MonoBehaviour
{
    public float range = 20;
    public float damage = 10;
    public AudioClip fireSoundEffects;

    private Camera fpsCam;
    private AudioSource source;

    void Start()
    {
        fpsCam = Camera.main;
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            source.PlayOneShot(fireSoundEffects);
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit,range))
            {
                if (hit.collider.GetComponent<IDamagable>() != null)
                {
                    hit.collider.GetComponent<IDamagable>().TakeDamage(damage);
                }
            }
        }
    }
}
