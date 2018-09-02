using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20;
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;

    public float targetVerticalOffset = 0f;

    float timer;
    Ray shootRay = new Ray();

    int shootableMask;
    public ParticleSystem muzzleFlash;
    public ParticleSystem bulletImpactWood;
    public ParticleSystem bulletImpactConcrete;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;

    Animator animator;


    void Awake ()
    {
        shootableMask = LayerMask.GetMask ("Shootable");

        gunLine = GetComponent <LineRenderer> ();
        gunAudio = GetComponent<AudioSource> ();
        gunLight = GetComponent<Light> ();
        animator = GetComponentInParent<Animator>();
    }


    void Update ()
    {
        timer += Time.deltaTime;
        //transform.localPosition = new Vector3(0, 0, 0);

		if(Input.GetButton ("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            Shoot();
        }

        if(timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffects ();
        }

        //Debug.Log("Playershooting-Update, Barrelend pos: " + this.transform.position);
    }


    public void DisableEffects ()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
        animator.ResetTrigger("Shoot");
        muzzleFlash.Stop();
    }

    void Shoot ()
    {
        timer = 0f;

        gunAudio.Play();
        
        muzzleFlash.gameObject.SetActive(true);
        muzzleFlash.Simulate(1);
        muzzleFlash.Play();

        animator.SetTrigger("Shoot");

        gunLight.enabled = true;
        
        gunLine.enabled = true;
        //gunLine.SetPosition(0, transform.position);

        shootRay.origin = transform.position;

        //Source: https://answers.unity.com/questions/346804/is-there-a-way-to-get-mouse-position-in-3d-space-a.html?sort=votes
        Plane plane = new Plane(Vector3.up,0);
        float dist;

        Vector3 target = Input.mousePosition;
        target.y += targetVerticalOffset;

        Ray ray = Camera.main.ScreenPointToRay(target);
        if (plane.Raycast(ray, out dist)) {
            gunLine.SetPosition (1, ray.GetPoint(dist));
            
            Vector3 targetPosition = ray.GetPoint(dist);

            targetPosition.y += targetVerticalOffset;
 
            shootRay.direction = targetPosition;

            //If cube spawning, aiming is accurate ???

            //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //  cube.transform.position = targetPosition;
            // cube.transform.localScale = new Vector3 (1, 1, 1);
            // Instantiate (cube);
            
            RaycastHit hit;      
            gunLine.SetPosition(0, transform.position);
            // Debug.Log("t: " + transform);
            //  Debug.Log("BEFORE linecast tp: " + transform.position);
            //  Debug.Log("BEFORE target: " + targetPosition);

            // Debug.Log("c: " + cube);
            // Debug.Log("ct: " + cube.transform);
            // Debug.Log("ctp: " + cube.transform.position);
            
            if (Physics.Linecast(transform.position, targetPosition, out hit)) {
                EnemyHealth enemyHealth = hit.collider.GetComponent <EnemyHealth>();
            
                if (enemyHealth == null) {
                    //Enemy was hit by the shot
                    enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
                }
                if (enemyHealth != null) {
                    //Debug.Log("enemyHealth: " + enemyHealth.ToString());
                    enemyHealth.TakeDamage(damagePerShot, hit.point);
                } else {
                    //Other object was hit by the shot, most likely the floor or walls
                    ParticleSystem floorHitParticle = Instantiate(bulletImpactWood, targetPosition, Quaternion.identity);
                    floorHitParticle.gameObject.SetActive(true);
                    floorHitParticle.Stop();
                    floorHitParticle.Play();
                }
                //gunLine.SetPosition (1, hit.point);                
            } else {              
                //Debug.Log("no hit");
            }

            // Debug.Log("AFTER linecast tp: " + transform.position);
            // Debug.Log("AFTER target: " + targetPosition);
         }
    }
}
