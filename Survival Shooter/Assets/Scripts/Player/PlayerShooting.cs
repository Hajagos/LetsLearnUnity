using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20;
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;


    float timer;
    Ray shootRay = new Ray();
    RaycastHit shootHit;
    int shootableMask;
    //ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;

    Animator animator;

    void Awake ()
    {
        shootableMask = LayerMask.GetMask ("Shootable");
        //gunParticles = GetComponent<ParticleSystem> ();
        gunLine = GetComponent <LineRenderer> ();
        gunAudio = GetComponent<AudioSource> ();
        gunLight = GetComponent<Light> ();
        animator = GetComponentInParent<Animator>();
    }


    void Update ()
    {
        timer += Time.deltaTime;

		if(Input.GetButton ("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            Shoot();
        }

        if(timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffects ();
        }
    }


    public void DisableEffects ()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
        animator.ResetTrigger("Shoot");
    }


    void Shoot ()
    {
        timer = 0f;

        gunAudio.Play ();


        animator.SetTrigger("Shoot");

        gunLight.enabled = true;

        // gunParticles.Stop ();
        // gunParticles.Play ();

        gunLine.enabled = true;
        gunLine.SetPosition (0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
        {
            Debug.Log("hit");
            EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
            if (enemyHealth == null) {
                enemyHealth = shootHit.collider.GetComponentInParent<EnemyHealth>();
            }

            if (enemyHealth != null)
            {
                Debug.Log("enemyHealth: " + enemyHealth.ToString());
                enemyHealth.TakeDamage(damagePerShot, shootHit.point);
            }
            else {
                Debug.Log("target object is NULL");
            }
            gunLine.SetPosition (1, shootHit.point);
        }
        else
        {
            Debug.Log("no hit");
            gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
        }
    }
}
