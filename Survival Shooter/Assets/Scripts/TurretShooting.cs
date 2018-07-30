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

	void Awake () {
		shootableMask = LayerMask.GetMask ("Shootable");
        gunLine = GetComponent <LineRenderer> ();
        // gunAudio = GetComponent<AudioSource> ();
	}


	void Update () {
		timer += Time.deltaTime;
		if(timer >= timeBetweenShots && Time.timeScale != 0) {
			attackNearbyEnemies();
		}

		if(timer >= timeBetweenShots * effectsDisplayTime)
        {
            DisableEffects();
        }
	}
	public void DisableEffects () {
        muzzleFlash.Stop();
		gunLight.enabled = false;
    }

	Transform[] getTransformsOfEnemies(List<EnemyHealth> enemies) {
		Transform[] enemyTransforms = new Transform[enemies.Count];
		for (int i = 0; i < enemies.Count; i++) {
			enemyTransforms[i] = enemies[i].transform;
		}

		return enemyTransforms;
	}

	void attackNearbyEnemies() {	
		timer = 0f;
		List<EnemyHealth> enemiesInRange = this.enemiesInRange();
		EnemyHealth closestEnemy = getClosestEnemy(enemiesInRange);
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

	EnemyHealth getClosestEnemy(List<EnemyHealth> enemies) {
		float minDist = Mathf.Infinity;
		Vector3 currentPos = transform.position;
		int indexOfNearestEnemy = 0;
		int currentEnemyIndex = 0;

		Transform[] enemyTransforms = getTransformsOfEnemies(enemies);
		foreach (Transform t in enemyTransforms) {
			float dist = Vector3.Distance(t.position, currentPos);
			if (dist < minDist) {
				minDist = dist;
				indexOfNearestEnemy = currentEnemyIndex;
			}
			currentEnemyIndex++;
		}

		if (enemies.Count - 1 >= indexOfNearestEnemy) {
			return enemies[indexOfNearestEnemy];
		} else {
			return null;
		}
	}

	List<EnemyHealth> enemiesInRange() {
		List<EnemyHealth> enemies = new List<EnemyHealth>();

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
		for (int i = 0; i < hitColliders.Length; i++) {
			Collider c = hitColliders[i];
			EnemyHealth enemy = c.gameObject.transform.GetComponentInParent<EnemyHealth>();	
			if (enemy != null) {
				enemies.Add(enemy);
			}
		}
		return enemies;
	}

}
