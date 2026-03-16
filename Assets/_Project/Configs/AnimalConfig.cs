using System;
using UnityEngine;

namespace _Project.Configs
{
	[Serializable]
	public class AnimalConfig : ScriptableObject
	{
		[Header("Identity")]
		[SerializeField] private AnimalTypeSO _animalType;
		[SerializeField] private bool _isPrey;
 
		[Header("Movement")]
		[SerializeField] private float _moveSpeed = 3f;
 
		[Header("Spawn")]
		[SerializeField] private GameObject _prefab;
 
		public AnimalTypeSO AnimalType => _animalType;
		public bool IsPrey => _isPrey;
		public float MoveSpeed => _moveSpeed;
		public GameObject Prefab => _prefab;
	}
}