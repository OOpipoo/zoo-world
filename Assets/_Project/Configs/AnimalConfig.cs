using UnityEngine;

namespace _Project.Configs
{
	public class AnimalConfig : ScriptableObject
	{
		[Header("Identity")]
		[SerializeField] private AnimalTypeSO _animalType;
		[SerializeField] private bool _isPrey;

		[Header("Movement")]
		[SerializeField] private float _moveSpeed = 3f;

		[Header("Bounds")]
		[SerializeField] private float _boundsRadius = 0.5f;

		[Header("Spawn")]
		[SerializeField] private float _spawnHeight = 0.5f;
		[SerializeField] private GameObject _prefab;
		[SerializeField] private int _poolSize = 5;

		public AnimalTypeSO AnimalType => _animalType;
		public bool IsPrey => _isPrey;
		public float MoveSpeed => _moveSpeed;
		public float BoundsRadius => _boundsRadius;
		public float SpawnHeight => _spawnHeight;
		public GameObject Prefab => _prefab;
		public int PoolSize => _poolSize;
	}
}
