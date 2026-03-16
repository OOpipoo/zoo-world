using UnityEngine;

namespace _Project.Configs
{
	[CreateAssetMenu(fileName = "SnakeConfig", menuName = "SO/ZooWorld/Configs/Snake Config")]
	public class SnakeConfig : AnimalConfig
	{
		[Header("Linear Movement Settings")]
		[SerializeField] private float _directionChangeInterval = 2f;
 
		public float DirectionChangeInterval => _directionChangeInterval;
	}
}