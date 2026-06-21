using UnityEngine;

public class AssaultRifleGunControl : GunControl
{
    
    public float fireDelay = 0.15f;
    private float lastFire = 0f;

    private AudioSource audioSource;
    public AudioClip[] shotSounds;
    [Range(0f, 1f)] public float volume = 0.8f;
    
    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
    }
    
    protected override void Update()
    {
        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.RTouch))
        {
            if (Time.time >= lastFire + fireDelay)
            {
                Debug.Log("Trigger premuto.");
                ExecuteShot();
                PlayShotSFX();
                lastFire = Time.time;
            }
        }
    }
    
    private void PlayShotSFX()
    {
        if (audioSource != null && shotSounds != null)
        {
            AudioClip shotSound = shotSounds[Random.Range(0, shotSounds.Length)];
            audioSource.PlayOneShot(shotSound, volume);
        }
    }
}
