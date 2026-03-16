using _Project.Infrastructures.Services;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Project.Ui
{
	public class HudView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _preyDeathCountText;
		[SerializeField] private TextMeshProUGUI _predatorDeathCountText;
 
		private ScoreService _scoreService;
		private readonly CompositeDisposable _disposable = new CompositeDisposable();
 
		[Inject]
		public void Construct(ScoreService scoreService)
		{
			_scoreService = scoreService;
		}
 
		private void Start()
		{
			_scoreService.PreyDeathCount
				.Subscribe(count => _preyDeathCountText.text = $"Prey deaths: {count}")
				.AddTo(_disposable);
 
			_scoreService.PredatorDeathCount
				.Subscribe(count => _predatorDeathCountText.text = $"Predator deaths: {count}")
				.AddTo(_disposable);
		}
 
		private void OnDestroy()
		{
			_disposable.Dispose();
		}
	}
}
