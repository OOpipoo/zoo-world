using UnityEngine;

namespace _Project.Configs
{
	[CreateAssetMenu(fileName = "NewAnimalType", menuName = "SO/ZooWorld/Animal Type")]
	public class AnimalTypeSO : ScriptableObject
	{
		[SerializeField] private string _displayName;

		public string DisplayName => _displayName;
	}
}
