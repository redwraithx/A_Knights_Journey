
using System;
using UnityEngine;

	public class BossDoor : MonoBehaviour
	{
		[SerializeField] private GameObject bossDoorContainer = null;
		[SerializeField] private EnemyAttributes banditBoss_Attributes = null;
		
		
		
		void Start()
		{
			if (!bossDoorContainer)
				throw new Exception("Error! the bosses door container is missing its reference, fix this and try again.");

			if (!banditBoss_Attributes)
				throw new Exception("Error! reference to bandit boss enemy attributes script is missing.");
			
			if(bossDoorContainer.activeInHierarchy)
				bossDoorContainer.SetActive(false);
			
		}

		private void OnTriggerEnter(Collider other)
		{
			if (bossDoorContainer.activeInHierarchy)
				return;
			
			bossDoorContainer.SetActive(true);
			
			// activate the boss so he does not attack before he should
			banditBoss_Attributes.EnableMovement(true);
		}
	}