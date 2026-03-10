using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreItemDestroyer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<ScorableItem>() != null)
        {
            Destroy(collision.collider.gameObject);
        }
    }
}
