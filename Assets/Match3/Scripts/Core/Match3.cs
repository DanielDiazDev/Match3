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
    //  [SerializeField] GameObject explosion;
        [Header("Systems")]
        private GridSystem<GridObject<IGem>> _gridSystem;
        [SerializeField] private InputReader _inputReader;
        private GemFactory _gemFactory;
        private GemSpawner _gemSpawner;
        private SwapHandler _swapHandler;
        private MatchFinder _matchFinder;
        private ExplodeSystem _explodeSystem;
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
            _explodeSystem = new(_gridSystem);
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
            List<Vector2Int> matches = _matchFinder.FindMatches();
            // TODO: Calculate score
            if (matches.Count == 0)
            {
                await _swapHandler.SwapGems(gridPosA, gridPosB);
                _inputReader.InputEnabled = true;
                return;
            }

            // Si hubo match, seguimos con el ciclo
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
        private bool IsValidPosition(Vector2 gridPosition) => gridPosition.x >= 0 && gridPosition.x < _width && gridPosition.y >= 0 && gridPosition.y < _height;
    }

}