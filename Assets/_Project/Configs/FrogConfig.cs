using UnityEngine;

namespace _Project.Configs
{
	[CreateAssetMenu(fileName = "FrogConfig", menuName = "SO/ZooWorld/Configs/Frog Config")]
	public class FrogConfig : AnimalConfig
	{
		[Header("Jump Settings")]
		[SerializeField] private float _jumpDistance = 2f;
		[SerializeField] private float _jumpInterval = 1.5f;
		[SerializeField] private float _jumpForce = 5f;
 
		public float JumpDistance => _jumpDistance;
		public float JumpInterval => _jumpInterval;
		public float JumpForce => _jumpForce;
	}
}