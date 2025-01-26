using UnityEngine;

public class CarAudioByPoint : MonoBehaviour
{
    public Transform soundTriggerPoint;
    public float triggerDistance = 5f;
    public AudioClip passSound;
    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = passSound;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (soundTriggerPoint != null)
        {
            float distance = Vector3.Distance(transform.position, soundTriggerPoint.position);

            if (!hasPlayed && distance <= triggerDistance)
            {
                audioSource.Play();
                hasPlayed = true;
            }
        }
    }
}
