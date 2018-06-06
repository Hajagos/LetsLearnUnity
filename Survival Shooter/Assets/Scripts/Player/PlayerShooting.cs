using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20;
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;

    float timer;
    Ray shootRay = new Ray();

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
        //gunLine.SetPosition(0, transform.position);

        shootRay.origin = transform.position;

        //Source: https://answers.unity.com/questions/346804/is-there-a-way-to-get-mouse-position-in-3d-space-a.html?sort=votes
        Plane plane = new Plane(Vector3.up,0);
        float dist;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out dist)) {
            //gunLine.SetPosition (1, ray.GetPoint(dist));
            
            Vector3 targetPosition = ray.GetPoint(dist);
 
            shootRay.direction = targetPosition;

            //If cube spawning, aiming is accurate ???

            //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //  cube.transform.position = targetPosition;
            // cube.transform.localScale = new Vector3 (1, 1, 1);
            // Instantiate (cube);
            
            RaycastHit hit;      
            gunLine.SetPosition(0, transform.position);
            
            if (Physics.Linecast(transform.position, targetPosition, out hit)) {
                EnemyHealth enemyHealth = hit.collider.GetComponent <EnemyHealth> ();
                if (enemyHealth == null) {
                    enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
                }
                if (enemyHealth != null) {
                    Debug.Log("enemyHealth: " + enemyHealth.ToString());
                    enemyHealth.TakeDamage(damagePerShot, hit.point);
                }
                else {
                    Debug.Log("target object is NULL");
                }
                gunLine.SetPosition (1, hit.point);                
            } else {
                Debug.Log("no hit");
            }
         }
    }
}
