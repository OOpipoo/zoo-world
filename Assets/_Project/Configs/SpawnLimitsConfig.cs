using UnityEngine;

namespace _Project.Configs
{
	[CreateAssetMenu(fileName = "SpawnLimitsConfig", menuName = "SO/ZooWorld/Configs/Spawn Limits Config")]
	public class SpawnLimitsConfig : ScriptableObject
	{
		[SerializeField] private int _maxPrey = 10;
		[SerializeField] private int _maxPredators = 5;
		[Space]
		[SerializeField] private float _minSpawnDistance = 2f;

		public int MaxPrey => _maxPrey;
		public int MaxPredators => _maxPredators;
		public float MinSpawnDistance => _minSpawnDistance;
	}
}
