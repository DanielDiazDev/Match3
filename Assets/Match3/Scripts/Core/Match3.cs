using Input;
using PrimeTween;
using ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
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
    //    [SerializeField] GameObject explosion;

        [SerializeField] private InputReader _inputReader;

        GridSystem<GridObject<Gem>> _gridSystem;

        Vector2Int selectedGem = Vector2Int.one * -1;
        private void Start()
        {
            InitializeGrid();
            _inputReader.Fire += OnSelectGem;
        }

        private void OnDestroy()
        {
            _inputReader.Fire -= OnSelectGem;
        }
        void InitializeGrid()
        {
            _gridSystem = GridSystem<GridObject<Gem>>.VerticalGrid(_width, _height, _cellSize, _originPosition, _debug);

            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    CreateGem(x, y);
                }
            }
        }

        void CreateGem(int x, int y)
        {
            var gem = Instantiate(gemPrefab, _gridSystem.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
            gem.SetGem(gemTypes[Random.Range(0, gemTypes.Length)]);
            var gridObject = new GridObject<Gem>(_gridSystem, x, y);
            gridObject.SetValue(gem);
            _gridSystem.SetValue(x, y, gridObject);
        }
        private void OnSelectGem()
        {
            var gridPos = _gridSystem.GetXY(Camera.main.ScreenToWorldPoint(_inputReader.Selected));

            if (!IsValidPosition(gridPos) || IsEmptyPosition(gridPos)) return;

            if (selectedGem == gridPos)
            {
                DeselectGem();
                //audioManager.PlayDeselect();
            }
            else if (selectedGem == Vector2Int.one * -1)
            {
                SelectGem(gridPos);
               // audioManager.PlayClick();
            }
            else
            {
                StartCoroutine(RunGameLoop(selectedGem, gridPos));
            }
        }

        private IEnumerator RunGameLoop(Vector2Int gridPosA, Vector2Int gridPosB)
        {
            yield return StartCoroutine(SwapGems(gridPosA, gridPosB));

            // TODO: Calculate score
            while (true)
            {
                List<Vector2Int> matches = FindMatches();

                if (matches.Count == 0)
                {
                    //if (isPlayerSwap)
                    //    yield return StartCoroutine(Swap(gridPosB, gridPosA));
                    break;
                }

               // combo++; 
                yield return StartCoroutine(ExplodeGems(matches));
                yield return StartCoroutine(MakeGemsFall());
                yield return StartCoroutine(FillEmptySpots());
               // isPlayerSwap = false;
            }
           // _canSwipe = true;
            // TODO: Check if game is over

            DeselectGem();
        }
        private IEnumerator SwapGems(Vector2Int gridPosA, Vector2Int gridPosB)
        {
            var gridObjectA = _gridSystem.GetValue(gridPosA.x, gridPosA.y);
            var gridObjectB = _gridSystem.GetValue(gridPosB.x, gridPosB.y);
            Tween.LocalPosition(gridObjectA.GetValue().transform, _gridSystem.GetWorldPositionCenter(gridPosB.x, gridPosB.y), 0.5f, Ease.InQuad);
            Tween.LocalPosition(gridObjectB.GetValue().transform, _gridSystem.GetWorldPositionCenter(gridPosA.x, gridPosA.y), 0.5f, Ease.InQuad);

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
        private IEnumerator FillEmptySpots()
        {
            float maxDuration = 0.3f;

            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    if (_gridSystem.GetValue(x, y) == null)
                    {
                        //CreateGem(x, y);
                        var gem = Instantiate(
                            gemPrefab,
                            _gridSystem.GetWorldPositionCenter(x, _height + 1),
                            Quaternion.identity,
                            transform);

                        gem.SetGem(gemTypes[Random.Range(0, gemTypes.Length)]);

                        var gridObject = new GridObject<Gem>(_gridSystem, x, y);
                        gridObject.SetValue(gem);
                        _gridSystem.SetValue(x, y, gridObject);
                        //audioManager.PlayPop();
                        var target = _gridSystem.GetWorldPositionCenter(x, y);
                        Tween.LocalPosition(gem.transform, target, maxDuration, Ease.OutBounce);
                    }
                }
            }

            yield return Tween.Delay(maxDuration).ToYieldInstruction();
        }

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
                        Tween.LocalPosition(gem.transform, targetPos, maxDuration, Ease.InQuad);

                        emptyY++;
                    }
                }
            }

            if (moved)
                yield return Tween.Delay(maxDuration).ToYieldInstruction();
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
                Tween.PunchScale(gem.transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                Tween.Scale(gem.transform, Vector3.zero, 0.2f, Ease.InBack);

                Destroy(gem.gameObject, maxDuration);
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





        private void DeselectGem() => selectedGem = new Vector2Int(-1, -1);
        private void SelectGem(Vector2Int gridPos) => selectedGem = gridPos;

        private bool IsEmptyPosition(Vector2Int gridPosition) => _gridSystem.GetValue(gridPosition.x, gridPosition.y) == null;

        private bool IsValidPosition(Vector2 gridPosition) => gridPosition.x >= 0 && gridPosition.x < _width && gridPosition.y >= 0 && gridPosition.y < _height;
    }

}