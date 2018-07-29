using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShooting : MonoBehaviour {

	public int damagePerShot = 20;
    public float timeBetweenShots = 0.15f;
    public float range = 0.001f;

	float timer;
    Ray shootRay = new Ray();
	LineRenderer gunLine;
    int shootableMask;

	void Awake () {
		shootableMask = LayerMask.GetMask ("Shootable");
        gunLine = GetComponent <LineRenderer> ();

        // gunAudio = GetComponent<AudioSource> ();
        // gunLight = GetComponent<Light> ();

	}

	void Update () {
		//Debug.Log("Turret - isEnemyNearby: " + isEnemyNearby());
	}

	void FixedUpdate() {

		timer += Time.deltaTime;
		if(timer >= timeBetweenShots && Time.timeScale != 0) {
			attackNearbyEnemies();
		}
	}

	Transform[] getTransformsOfEnemies(List<EnemyHealth> enemies) {
		Transform[] enemyTransforms = new Transform[enemies.Count];

		for (int i = 0; i < enemies.Count; i++) {
			enemyTransforms[i] = enemies[i].transform;
		}

		return enemyTransforms;
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

	void attackNearbyEnemies() {
		
		// bool enemyInRange = false;
		// int enemyCount = 0;
		//Debug.Log("hitcolliders count : " + hitColliders.Length);
		
		List<EnemyHealth> enemiesInRange = this.enemiesInRange();
		EnemyHealth closestEnemy = getClosestEnemy(enemiesInRange);
		if (closestEnemy != null) {
			Vector3 enemyPosition = closestEnemy.transform.position;
			closestEnemy.TakeDamage(damagePerShot, new Vector3(enemyPosition.x, 0.5f, enemyPosition.z));
		}
		// for (int i = 0; i < hitColliders.Length; i++) {
		// 	Collider c = hitColliders[i];
		// 	EnemyHealth enemy = c.gameObject.transform.GetComponentInParent<EnemyHealth>();
			
		// 	if (enemy != null) {
		// 		Debug.Log("nearby enemy: " + enemy);
		// 		enemyInRange = true;
		// 		enemyCount++;

		// 		enemy.TakeDamage(damagePerShot, enemy.transform.position);
		// 	}
		// }

		// if (enemyCount > 0) {
		// 	Debug.Log("Enemy count: " + enemyCount);
		// }
	}
}
