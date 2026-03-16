using System;
using UnityEngine;

namespace _Project.Configs
{
	[Serializable]
	[CreateAssetMenu(fileName = "FrogConfig", menuName = "SO/ZooWorld/Configs/Frog Config")]
	public class FrogConfig : AnimalConfig
	{
		[Header("Jump Settings")]
		[Tooltip("Pause between jumps in seconds")]
		[SerializeField] private float _jumpInterval = 1.5f;
 
		[Tooltip("Duration of one jump in seconds")]
		[SerializeField] private float _jumpDuration = 0.5f;
 
		[Tooltip("Distance of one jump on XZ")]
		[SerializeField] private float _jumpDistance = 2f;
 
		[Tooltip("Jump arc height in Y")]
		[SerializeField] private float _jumpHeight = 0.5f;
 
		public float JumpInterval => _jumpInterval;
		public float JumpDuration => _jumpDuration;
		public float JumpDistance => _jumpDistance;
		public float JumpHeight => _jumpHeight;
	}
}
