using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ScorableItem : MonoBehaviour
{
    public int scoreAmount = 1;
    public AudioClip creationSound;
    public AudioClip scoringSound;

    private AudioSource source;

    private void Start()
    {
        SimpleScoreTrack.currentActive++;
        source = GetComponent<AudioSource>();
        source.PlayOneShot(creationSound);
    }

    private void OnDestroy()
    {
        source.PlayOneShot(scoringSound);
        SimpleScoreTrack.currentActive--;
        SimpleScoreTrack.score += scoreAmount;
    }
}
