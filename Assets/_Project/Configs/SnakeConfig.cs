using System;
using UnityEngine;

namespace _Project.Configs
{
	[Serializable]
	[CreateAssetMenu(fileName = "SnakeConfig", menuName = "SO/ZooWorld/Configs/Snake Config")]
	public class SnakeConfig : AnimalConfig
	{
		[Header("Linear Movement Settings")]
		[Tooltip("How often does the snake change direction in seconds")]
		[SerializeField] private float _directionChangeInterval = 2f;
 
		[Tooltip("The rate of turn to a new direction. More = sharper")]
		[SerializeField] private float _rotationSpeed = 5f;
 
		[Tooltip("Maximum rotation angle per direction change in degrees 90 or 180")]
		[SerializeField] [Range(10f, 120f)] private float _maxTurnAngle = 60f;
		
		[Tooltip("External pulse decay rate")]
		[SerializeField] private float _velocityDamping = 15f;
		
 
		public float DirectionChangeInterval => _directionChangeInterval;
		public float RotationSpeed => _rotationSpeed;
		public float MaxTurnAngle => _maxTurnAngle;
		public float VelocityDamping => _velocityDamping;
	}
}