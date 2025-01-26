using UnityEngine;

public class audio : MonoBehaviour
{
    public AudioClip passSound; // The audio clip to play
    private AudioSource audioSource;

    void Start()
    {
        // Add an AudioSource component to the car
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = passSound;
        audioSource.loop = true;       // Enable looping if you want the sound to repeat
        audioSource.playOnAwake = true; // Play as soon as the car spawns
        audioSource.volume = 3f;
        audioSource.spatialBlend = 1f; // Make the sound 3D (optional, for spatial audio)
        audioSource.minDistance = 1f;  // Set the distance where the sound is at full volume
        audioSource.maxDistance = 50f; // Set the distance where the sound fades out
        audioSource.Play();            // Start playing the audio


    }
}
