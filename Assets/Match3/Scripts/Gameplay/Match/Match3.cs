using Cysharp.Threading.Tasks;
using Input;
using ScriptableObjects;
using ScriptableObjects.Level;
using System.Collections.Generic;
using System.Linq;
using Systems;
using Systems.Score;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public class Match3 : MonoBehaviour
    {
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private Vector3 _originPosition = Vector3.zero;
        [SerializeField] private bool _debug = true;
        [SerializeField] GemSO[] _gems;
        [Header("Systems")]
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private PowerUpSystem _powerUpSystem;
        [SerializeField] private ExplodeSystem _explodeSystem;
        private GridSystem<GridObject<IGem>> _gridSystem;
        private GemFactory _gemFactory;
        private GemSpawner _gemSpawner;
        private SwapHandler _swapHandler;
        private MatchFinder _matchFinder;
        private GravityManager _gravityManager;
        private GemFiller _gemFiller;
        private LevelSO _levelSO;
        [Header("Test")]
        [SerializeField] private LevelSO _levelTest;
        [SerializeField] private bool _isTest;
        private void OnEnable()
        {
            _inputReader.OnSwipe += OnSwipe;
        }

        private void OnDestroy()
        {
            _inputReader.OnSwipe -= OnSwipe;
        }
        private void Start()
        {
            if(_isTest)
            {
                Init(_levelTest);
            }
        }
        public void Init(LevelSO levelSO)
        {
            _levelSO = levelSO;
            _gemFactory = new(transform);

            InitializeGrid();
        }

        private void OnSwipe(Vector2 screenStart, Vector2Int direction)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenStart);
            worldPos.z = 0;
            Vector2Int gridPosA = _gridSystem.GetXY(worldPos);
            if (!IsValidPosition(gridPosA) || IsEmptyPosition(gridPosA))
                return;
            Vector2Int gridPosB = gridPosA + direction;
            if (!IsValidPosition(gridPosB) || IsEmptyPosition(gridPosB))
                return;
            RunGameLoop(gridPosA, gridPosB).Forget();
        }

        void InitializeGrid()
        {
            _gridSystem = GridSystem<GridObject<IGem>>.VerticalGrid(_levelSO.width, _levelSO.height, _cellSize, _originPosition, _debug);
            _gemSpawner = new(_gemFactory, _gridSystem);
            _swapHandler = new(_gridSystem);
            _matchFinder = new(_levelSO.height, _levelSO.width, _gridSystem);
            _explodeSystem.Init(_gridSystem);
            _powerUpSystem.Init(_gridSystem, _explodeSystem);
            _gravityManager = new(_gridSystem, _levelSO.width, _levelSO.height);
            _gemFiller = new(_gridSystem, _levelSO.width, _levelSO.height, _gemSpawner);
            for (var x = 0; x < _levelSO.width; x++)
            {
                for (var y = 0; y < _levelSO.height; y++)
                {
                    var position = new Vector2Int(x, y);

                    if (_levelSO.initialGems.TryGetValue(position, out var gemSO))
                    {
                        var worldPos = _gridSystem.GetWorldPositionCenter(x, y);
                        _gemSpawner.CreateGem(gemSO, x, y, worldPos);
                    }
                    else
                    {
                        Debug.LogWarning($"No gem type defined for position {position}");
                    }
                }
            }
        }
        private async UniTaskVoid RunGameLoop(Vector2Int gridPosA, Vector2Int gridPosB)
        {
            _inputReader.InputEnabled = false;
            await _swapHandler.SwapGems(gridPosA, gridPosB);
            var gemA = _gridSystem.GetValue(gridPosA.x, gridPosA.y).GetValue();
            var gemB = _gridSystem.GetValue(gridPosB.x, gridPosB.y).GetValue();
            bool powerUpActivated = false;
            if (gemA.GetGem().IsPowerUp || gemB.GetGem().IsPowerUp)
            {
                var powerGem = gemA.GetGem().IsPowerUp ? gemA : gemB;
                var gemsDestroyed = await _powerUpSystem.ActivatePowerUp(powerGem, gridPosA, gridPosB, _levelSO.width, _levelSO.height);
                powerUpActivated = true;
                ScoreManager.Instance.AddScore(ScoreType.PowerUp, gemsDestroyed);
            }
            if (powerUpActivated)
            {
                await _gravityManager.MakeGemsFall();
                await _gemFiller.FillEmptySpots(_gems);
            }
            var matches = _matchFinder.FindMatches();
            // TODO: Calculate score
            if (matches.Count == 0)
            {
                await _swapHandler.SwapGems(gridPosA, gridPosB);
                
                _inputReader.InputEnabled = true;
                return;
            }
            var comboLevel = 0;
            while (matches.Count > 0)
            {
                comboLevel++;
                foreach (var matchGroup in matches)
                {
                    int matchSize = matchGroup.Positions.Count;
                    ScoreManager.Instance.AddScore(ScoreType.Match, matchSize);
                }
                if(comboLevel > 1)
                {
                    ScoreManager.Instance.AddScore(ScoreType.Combo, comboLevel);
                }
                await _explodeSystem.ExplodeGems(matches);
                await _gravityManager.MakeGemsFall();
                await _gemFiller.FillEmptySpots(_gems);

                matches = _matchFinder.FindMatches();
            }
            _inputReader.InputEnabled = true;
            // TODO: Check if game is over
        }
        private bool IsEmptyPosition(Vector2Int gridPosition) => _gridSystem.GetValue(gridPosition.x, gridPosition.y) == null;
        private bool IsValidPosition(Vector2Int gridPosition) => gridPosition.x >= 0 && gridPosition.x < _levelSO.width && gridPosition.y >= 0 && gridPosition.y < _levelSO.height;
    }

}