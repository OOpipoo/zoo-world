using UnityEngine;
using ZooWorld.UI;

namespace _Project.Infrastructures.ObjectPools
{
    public class TastyLabelPool : BasePool
    {
        [SerializeField] private TastyLabel _prefab;
        [SerializeField] private int _initialSize = 5;
        [SerializeField] private float _displayDuration = 1.5f;

        public float DisplayDuration => _displayDuration;

        protected override void Awake()
        {
            Fill(_prefab, _initialSize);
        }

        public TastyLabel Get() => Get<TastyLabel>();

        public void Return(TastyLabel label)
        {
            if (label == null) return;
            label.Hide();
            Return<TastyLabel>(label);
        }
    }
}
