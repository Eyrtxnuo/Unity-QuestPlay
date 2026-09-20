using PingPong;
using UnityEngine;

public class AssaultRifleGunControl : GunControl
{
    
    public float fireDelay = 0.15f;
    public bool automaticFire = false;
    public AudioClip[] shotSounds;
    [Range(0f, 1f)] public float volume = 0.8f;

    private AudioSource _audioSource;
    private float _lastFire = 0f;
    private bool _hasReleased = false;
    
    public ParticleSystem muzzleFlash;
    
    protected override void Start()
    {
        base.Start();
        _audioSource = GetComponent<AudioSource>();
    }
    
    protected override void Update()
    {
        if(PauseManager.is_paused) return;// if paused inputs are handled by the OVR interaction manager
        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.RTouch))
        {
            if (Time.time >= _lastFire + fireDelay && (_hasReleased || automaticFire))
            {
                Debug.Log("Trigger premuto.");
                ExecuteShot();
                PlayShotSFX();
                PlayVisualEffects();
                _lastFire = Time.time;
                _hasReleased = false;
            }
        }
        else
        {
            _hasReleased = true;
        }
    }
    
    private void PlayShotSFX()
    {
        if (_audioSource && shotSounds != null)
        {
            AudioClip shotSound = shotSounds[Random.Range(0, shotSounds.Length)];
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.PlayOneShot(shotSound, volume);
        }
    }
    
    private void PlayVisualEffects()
    {
        if (muzzleFlash)
        {
            // Play() avvia il Burst che abbiamo configurato
            muzzleFlash.Play(); 
        }
    }
}