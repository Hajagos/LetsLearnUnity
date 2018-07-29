using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public float sinkSpeed = 2.5f;
    public int scoreValue = 10;

    public AudioSource deathClip;

    public Image healthBar;
    public ParticleSystem bloodSplush;

    Animator anim;
    public AudioSource takeDamageSound1;

    public AudioSource takeDamageSound2;

    CapsuleCollider capsuleCollider;
    bool isDead;
    bool isSinking;

    void Awake ()
    {
        anim = GetComponent <Animator> ();
        bloodSplush = GetComponentInChildren <ParticleSystem> ();
        capsuleCollider = GetComponent <CapsuleCollider> ();

        currentHealth = startingHealth;
    }


    void Update ()
    {
        if(isSinking)
        {
            transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
        }
    }


    public void TakeDamage (int amount, Vector3 hitPoint)
    {
        if (isDead) {
            return;
        }

        //TODO: refactor to dynamic method and clean this shit up
        if (Random.Range(1,3) % 2 == 0) {
            if (takeDamageSound1) {
                if (!takeDamageSound1.isPlaying) {
                    takeDamageSound1.Play();
                }
            }
        } else {
            if (takeDamageSound2) {
                if (!takeDamageSound2.isPlaying) {
                    takeDamageSound2.Play();
                }
            }
        }

        
        
        currentHealth -= amount;

        healthBar.fillAmount = (float) currentHealth / startingHealth;

        if (bloodSplush != null)  {
            bloodSplush.transform.position = hitPoint;
            bloodSplush.Play();
        }   

        if(currentHealth <= 0)
        {
            Death ();
        }
    }

    void Death ()
    {
        isDead = true;

        capsuleCollider.isTrigger = true;

        anim.SetTrigger ("Dead");

        deathClip.Play();
        ScoreManager.score += scoreValue;
    }

    public void StartSinking ()
    {
        GetComponent <UnityEngine.AI.NavMeshAgent> ().enabled = false;
        GetComponent <Rigidbody> ().isKinematic = true;
        isSinking = true;
        
        Destroy (gameObject, 2f);
    }
}
