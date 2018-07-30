using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class EnemyFinder : MonoBehaviour {

	public static List<EnemyHealth> getEnemiesInRange(Vector3 origin, float range) {
		List<EnemyHealth> enemies = new List<EnemyHealth>();

		Collider[] hitColliders = Physics.OverlapSphere(origin, range);
		for (int i = 0; i < hitColliders.Length; i++) {
			Collider c = hitColliders[i];
			EnemyHealth enemy = c.gameObject.transform.GetComponentInParent<EnemyHealth>();	
			if (enemy != null) {
				enemies.Add(enemy);
			}
		}
		return enemies;
	}

	public static List<EnemyHealth> getClosestEnemies(Vector3 origin, float range, int enemyCount = 1) {
		List<EnemyHealth> enemies = getEnemiesInRange(origin, range);
		enemies.Sort(new DistanceComparer(origin));
		if (enemies.Count >= enemyCount) {
			return enemies.GetRange(0, enemyCount);
		} else {
			return enemies;
		}
	}

	public class DistanceComparer : IComparer<EnemyHealth> {
		private Vector3 origin;

		public DistanceComparer(Vector3 origin) {
			this.origin = origin;
		}
        public int Compare(EnemyHealth e1, EnemyHealth e2)
        {
			return (int) (Vector3.Distance(e1.transform.position, this.origin) - Vector3.Distance(e2.transform.position, this.origin));
        }
    }

}
