using Input;
using PrimeTween;
using ScriptableObjects;
using System;
using System.Collections;
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
        [SerializeField] Gem gemPrefab;
        [SerializeField] GemSO[] gemTypes;
    //  [SerializeField] GameObject explosion;
        [Header("Systems")]
        private GridSystem<GridObject<IGem>> _gridSystem;
        [SerializeField] private InputReader _inputReader;
        private GemFactory _gemFactory;

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
            StartCoroutine(RunGameLoop(gridPosA, gridPosB));
        }
       
        void InitializeGrid()
        {
            _gridSystem = GridSystem<GridObject<IGem>>.VerticalGrid(_width, _height, _cellSize, _originPosition, _debug);
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    CreateGem(x, y, _gridSystem.GetWorldPositionCenter(x, y));
                }
            }
        }

        private IGem CreateGem(int x, int y, Vector3 spawnPosition, bool animate = false) //Ver si no devuelve nada luego
        {
            var gemSO = gemTypes[Random.Range(0, gemTypes.Length)];
            var gem = _gemFactory.Create(gemSO, spawnPosition, Quaternion.identity);
            gem.Init(gemSO);

            var gridObject = new GridObject<IGem>(_gridSystem, x, y);
            gridObject.SetValue(gem);
            _gridSystem.SetValue(x, y, gridObject);

            if (animate)
            {
                var target = _gridSystem.GetWorldPositionCenter(x, y);
                Tween.LocalPosition(gem.Transform, target, 0.3f, Ease.OutBounce);
            }

            return gem;
        }


        private IEnumerator RunGameLoop(Vector2Int gridPosA, Vector2Int gridPosB)
        {
            _inputReader.InputEnabled = false;
            yield return StartCoroutine(SwapGems(gridPosA, gridPosB));
            List<Vector2Int> matches = FindMatches();
            // TODO: Calculate score
            if (matches.Count == 0)
            {
                yield return StartCoroutine(SwapGems(gridPosA, gridPosB));
                _inputReader.InputEnabled = true;
                yield break;
            }

            // Si hubo match, seguimos con el ciclo
            while (matches.Count > 0)
            {
                yield return StartCoroutine(ExplodeGems(matches));
                yield return StartCoroutine(MakeGemsFall());
                yield return StartCoroutine(FillEmptySpots());

                matches = FindMatches();
            }
            _inputReader.InputEnabled = true;
            // TODO: Check if game is over
        }
        private IEnumerator SwapGems(Vector2Int gridPosA, Vector2Int gridPosB)
        {
            var gridObjectA = _gridSystem.GetValue(gridPosA.x, gridPosA.y); // Ver si hacer lo de velocidad aumaneta si no hay match y volvemos a su pocion original
            var gridObjectB = _gridSystem.GetValue(gridPosB.x, gridPosB.y);
            Tween.LocalPosition(gridObjectA.GetValue().Transform, _gridSystem.GetWorldPositionCenter(gridPosB.x, gridPosB.y), 0.5f, Ease.InQuad); //Moverlo a clase gem
            Tween.LocalPosition(gridObjectB.GetValue().Transform, _gridSystem.GetWorldPositionCenter(gridPosA.x, gridPosA.y), 0.5f, Ease.InQuad);

            _gridSystem.SetValue(gridPosA.x, gridPosA.y, gridObjectB);
            _gridSystem.SetValue(gridPosB.x, gridPosB.y, gridObjectA);
            yield return new WaitForSeconds(0.5f);
        }
        private List<Vector2Int> FindMatches()
        {
            HashSet<Vector2Int> matches = new();

            // Horizontal
            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width - 2; x++)
                {
                    var gemA = _gridSystem.GetValue(x, y);
                    var gemB = _gridSystem.GetValue(x + 1, y);
                    var gemC = _gridSystem.GetValue(x + 2, y);

                    if (gemA == null || gemB == null || gemC == null) continue;

                    if (gemA.GetValue().GetGem() == gemB.GetValue().GetGem()
                        && gemB.GetValue().GetGem() == gemC.GetValue().GetGem())
                    {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x + 1, y));
                        matches.Add(new Vector2Int(x + 2, y));
                    }
                }
            }

            // Vertical
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height - 2; y++)
                {
                    var gemA = _gridSystem.GetValue(x, y);
                    var gemB = _gridSystem.GetValue(x, y + 1);
                    var gemC = _gridSystem.GetValue(x, y + 2);

                    if (gemA == null || gemB == null || gemC == null) continue;

                    if (gemA.GetValue().GetGem() == gemB.GetValue().GetGem()
                        && gemB.GetValue().GetGem() == gemC.GetValue().GetGem())
                    {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x, y + 1));
                        matches.Add(new Vector2Int(x, y + 2));
                    }
                }
            }

            //if (matches.Count == 0)
            //{
            //    audioManager.PlayNoMatch();
            //}
            //else
            //{
            //    audioManager.PlayMatch();
            //}

            return new List<Vector2Int>(matches);
        }
        private IEnumerator ExplodeGems(List<Vector2Int> matches)
        {
            if (matches == null || matches.Count == 0)
                yield break;

            float maxDuration = 0.25f; // Duración máxima del tween

            foreach (var match in matches)
            {
                var gridObj = _gridSystem.GetValue(match.x, match.y);
                if (gridObj == null) continue;

                var gem = gridObj.GetValue();
                _gridSystem.SetValue(match.x, match.y, null);
                // ExplodeVFX()
                Tween.PunchScale(gem.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                Tween.Scale(gem.Transform, Vector3.zero, 0.2f, Ease.InBack);

                Destroy((gem as Gem).gameObject, maxDuration); //Ponerlo en la clase gem
            }

            // Esperar la duración más larga de las animaciones
            yield return Tween.Delay(maxDuration).ToYieldInstruction();

        }
        //void ExplodeVFX(Vector2Int match)
        //{
        //    // TODO: Pool
        //    var fx = Instantiate(explosion, transform);
        //    fx.transform.position = _gridSystem.GetWorldPositionCenter(match.x, match.y);
        //    Destroy(fx, 5f);
        //}
        private IEnumerator MakeGemsFall()
        {
            float maxDuration = 0.25f; 
            bool moved = false;

            for (var x = 0; x < _width; x++)
            {
                int emptyY = -1;
                for (var y = 0; y < _height; y++)
                {
                    if (_gridSystem.GetValue(x, y) == null)
                    {
                        if (emptyY == -1) emptyY = y;
                    }
                    else if (emptyY != -1)
                    {
                        moved = true;
                        var gridObject = _gridSystem.GetValue(x, y);
                        var gem = gridObject.GetValue();
                        _gridSystem.SetValue(x, emptyY, gridObject);
                        _gridSystem.SetValue(x, y, null);

                        var targetPos = _gridSystem.GetWorldPositionCenter(x, emptyY);
                        Tween.LocalPosition(gem.Transform, targetPos, maxDuration, Ease.InQuad);

                        emptyY++;
                    }
                }
            }

            if (moved)
                yield return Tween.Delay(maxDuration).ToYieldInstruction();
        }
        private IEnumerator FillEmptySpots()
        {
            float maxDuration = 0.3f;

            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    if (_gridSystem.GetValue(x, y) == null)
                    {
                        CreateGem(x, y, _gridSystem.GetWorldPositionCenter(x, _height + 1), true);
                        //audioManager.PlayPop();
                    }
                }
            }

            yield return Tween.Delay(maxDuration).ToYieldInstruction();
        }
        private bool IsEmptyPosition(Vector2Int gridPosition) => _gridSystem.GetValue(gridPosition.x, gridPosition.y) == null;
        private bool IsValidPosition(Vector2 gridPosition) => gridPosition.x >= 0 && gridPosition.x < _width && gridPosition.y >= 0 && gridPosition.y < _height;
    }

}