using System;
using UniRx;
using UnityEngine;

namespace _Project.Ui
{
	public class TastyLabel : MonoBehaviour
	{
		[SerializeField] private float _displayDuration = 2f;
		[SerializeField] private float _offsetY = -1f;
		
		private CompositeDisposable _disposable = new CompositeDisposable();
		
		public void Show(Transform predatorTransform)
		{
			_disposable.Clear();
 
			var position = predatorTransform.position + Vector3.up * _offsetY;
			transform.position = position;
			gameObject.SetActive(true);
 
			Observable
				.Timer(TimeSpan.FromSeconds(_displayDuration))
				.Subscribe(_ => gameObject.SetActive(false))
				.AddTo(_disposable);
		}
 
		private void OnDestroy()
		{
			_disposable.Dispose();
		}
	}
}