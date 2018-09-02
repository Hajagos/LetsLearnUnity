using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: refactor this and turretshooting, maybe playershooting to parent class (duplicated proreties and functions)
public class AreaDamaging : MonoBehaviour {

	public int damagePerSec = 20;
    public float damageFrequency = 0.15f;
    public float range = 0.001f;
	public Light areaEffectLight;	
	public ParticleSystem areaEffect;

	public BoxCollider effectingArea;

	public Animator animator;
	
	float timer;
    Ray shootRay = new Ray();
	LineRenderer gunLine;
    int shootableMask;

	float effectsDisplayTime = 0f;
	// Use this for initialization
	void Awake () {
		shootableMask = LayerMask.GetMask ("Shootable");
        gunLine = GetComponent <LineRenderer> ();
	}
	
	void Update() {
		timer += Time.deltaTime;
		turnToClosestEnemyInRange();
		
		// if (timer >= damageFrequency && Time.timeScale != 0) {
		// 	EnableEffects();
		// }

		
	}

	private void EnableEffects() {
		Debug.Log("ENABLE EFFECTST");
		areaEffectLight.enabled = true;
		if (!areaEffect.isPlaying) {
			areaEffect.gameObject.SetActive(true);
			areaEffect.Simulate(1);
			areaEffect.Play();
		}
		
	}

	public void DisableEffects() {
        areaEffect.Stop();
		areaEffectLight.enabled = false;
    }

	void turnToClosestEnemyInRange() {
		List<EnemyHealth> enemiesInRange = EnemyFinder.getClosestEnemies(this.transform.position, this.range);
		if (enemiesInRange.Count > 0) {
			EnemyHealth closestEnemy = enemiesInRange[0];
			if (closestEnemy != null && closestEnemy.currentHealth > 0) {
				Vector3 enemyPosition = closestEnemy.transform.position;
				//transform.parent.LookAt(enemyPosition);

				// Quaternion endRotation = Quaternion.LookRotation(transform.parent.forward, enemyPosition - transform.parent.position);
				// transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, endRotation, 0.2f);

				Quaternion targetRotation = Quaternion.LookRotation(enemyPosition - transform.parent.position);
     
				// Smoothly rotate towards the target point.
				transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, targetRotation, 1 * Time.deltaTime);
			}
		} else {
			DisableEffects();
		}
	}

	void OnTriggerEnter(Collider other) {
		//Debug.Log("Enter");
	}

	void OnTriggerStay(Collider other) {
		EnemyHealth collidingEnemy = other.GetComponentInParent<EnemyHealth>();
			//Debug.Log("enemy stay: " + other);
		if (collidingEnemy != null &&collidingEnemy.currentHealth > 0) {
			//Debug.Log("ENEMY TAKE DMG");
			EnableEffects();
			if (timer >= damageFrequency && Time.timeScale != 0) {
				doDamageOnEnemy(collidingEnemy);
			}
		}
	}

	private void doDamageOnEnemy(EnemyHealth enemy) {
		// timer = 0f;
		enemy.TakeDamage(damagePerSec, new Vector3(enemy.transform.position.x, 1f, enemy.transform.position.z));
	}
}
