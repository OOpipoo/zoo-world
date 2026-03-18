using _Project.Infrastructure.Services;
using _Project.Infrastructures.Services;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Ui
{
	public class HudView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _preyDeathCountText;
		[SerializeField] private TextMeshProUGUI _predatorDeathCountText;
		[SerializeField] private Button _restartButton;

		private ScoreService _scoreService;
		private GameService _gameService;
		private readonly CompositeDisposable _disposable = new CompositeDisposable();

		[Inject]
		public void Construct(ScoreService scoreService, GameService gameService)
		{
			_scoreService = scoreService;
			_gameService = gameService;
		}

		private void Start()
		{
			_scoreService.PreyDeathCount
				.Subscribe(count => _preyDeathCountText.text = $"Prey deaths: {count}")
				.AddTo(_disposable);

			_scoreService.PredatorDeathCount
				.Subscribe(count => _predatorDeathCountText.text = $"Predator deaths: {count}")
				.AddTo(_disposable);

			_restartButton.onClick.AddListener(_gameService.Restart);
		}

		private void OnDestroy()
		{
			_restartButton.onClick.RemoveAllListeners();
			_disposable.Dispose();
		}
	}
}
