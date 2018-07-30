using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShooting : MonoBehaviour {

	public int damagePerShot = 20;
    public float timeBetweenShots = 0.15f;
    public float range = 0.001f;
	public Light gunLight;	
	public ParticleSystem muzzleFlash;

	float timer;
    Ray shootRay = new Ray();
	LineRenderer gunLine;
    int shootableMask;

	float effectsDisplayTime = 0.2f;

	void Awake() {
		shootableMask = LayerMask.GetMask ("Shootable");
        gunLine = GetComponent <LineRenderer> ();
        // gunAudio = GetComponent<AudioSource> ();
	}


	void Update() {
		timer += Time.deltaTime;
		if (timer >= timeBetweenShots && Time.timeScale != 0) {
			attackClosestEnemy();
		}

		if (timer >= timeBetweenShots * effectsDisplayTime) {
            DisableEffects();
        }
	}
	public void DisableEffects() {
        muzzleFlash.Stop();
		gunLight.enabled = false;
    }

	void attackClosestEnemy() {	
		timer = 0f;
		List<EnemyHealth> enemiesInRange = EnemyFinder.getClosestEnemies(this.transform.position, this.range, 5);
		if (enemiesInRange.Count > 0) {
			EnemyHealth closestEnemy = enemiesInRange[0];
			if (closestEnemy != null && closestEnemy.currentHealth > 0) {
				Vector3 enemyPosition = closestEnemy.transform.position;
				transform.parent.LookAt(enemyPosition);

				gunLight.enabled = true;
				muzzleFlash.gameObject.SetActive(true);
				muzzleFlash.Simulate(1);
				muzzleFlash.Play();
				
				closestEnemy.TakeDamage(damagePerShot, new Vector3(enemyPosition.x, 1f, enemyPosition.z));
			}	
		}	
	}
}
