using Cysharp.Threading.Tasks;
using Input;
using ScriptableObjects;
using System.Collections.Generic;
using Systems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public class Match3 : MonoBehaviour
    {
        [SerializeField] private int _width = 8;
        [SerializeField] private int _height = 8;
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private Vector3 _originPosition = Vector3.zero;
        [SerializeField] private bool _debug = true;
        [SerializeField] GemSO[] gemTypes;
        [SerializeField] private ExplodeSystem _explodeSystem;
    //  [SerializeField] GameObject explosion;
        [Header("Systems")]
        private GridSystem<GridObject<IGem>> _gridSystem;
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private PowerUpSystem _powerUpSystem;
        private GemFactory _gemFactory;
        private GemSpawner _gemSpawner;
        private SwapHandler _swapHandler;
        private MatchFinder _matchFinder;
        private GravityManager _gravityManager;
        private GemFiller _gemFiller;
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
            _gridSystem = GridSystem<GridObject<IGem>>.VerticalGrid(_width, _height, _cellSize, _originPosition, _debug);
            _gemSpawner = new(_gemFactory, _gridSystem);
            _swapHandler = new(_gridSystem);
            _matchFinder = new(_height, _width, _gridSystem);
            _explodeSystem.Init(_gridSystem);
            _powerUpSystem.Init(_gridSystem, _explodeSystem);
            _gravityManager = new(_gridSystem, _width, _height);
            _gemFiller = new(_gridSystem, _width, _height, _gemSpawner);
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    _gemSpawner.CreateGem(gemTypes[Random.Range(0, gemTypes.Length)],x, y, _gridSystem.GetWorldPositionCenter(x, y));
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
                await _powerUpSystem.ActivatePowerUp(powerGem, gridPosA, gridPosB, _width, _height);
                powerUpActivated = true;
            }
            if (powerUpActivated)
            {
                await _gravityManager.MakeGemsFall();
                await _gemFiller.FillEmptySpots(gemTypes);
            }
            var matches = _matchFinder.FindMatches();
            // TODO: Calculate score
            if (matches.Count == 0)
            {
                await _swapHandler.SwapGems(gridPosA, gridPosB);
                
                _inputReader.InputEnabled = true;
                return;
            }
            while (matches.Count > 0)
            {
                await _explodeSystem.ExplodeGems(matches);
                await _gravityManager.MakeGemsFall();
                await _gemFiller.FillEmptySpots(gemTypes);

                matches = _matchFinder.FindMatches();
            }
            _inputReader.InputEnabled = true;
            // TODO: Check if game is over
        }
        private bool IsEmptyPosition(Vector2Int gridPosition) => _gridSystem.GetValue(gridPosition.x, gridPosition.y) == null;
        private bool IsValidPosition(Vector2Int gridPosition) => gridPosition.x >= 0 && gridPosition.x < _width && gridPosition.y >= 0 && gridPosition.y < _height;
    }

}