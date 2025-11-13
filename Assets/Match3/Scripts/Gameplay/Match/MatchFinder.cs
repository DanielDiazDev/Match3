using Core;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Sound;
using UnityEngine;


namespace Systems
{
    public struct MatchData
    {
        public List<Vector2Int> Positions;
        public MatchPattern Pattern;
        public Vector2Int Origin;

        public MatchData(List<Vector2Int> positions, MatchPattern pattern, Vector2Int origin)
        {
            Positions = positions;
            Pattern = pattern;
            Origin = origin;
        }
        
    }

    public enum MatchPattern
    {
        LineHorizontal,
        LineVertical,
        TShape,
        LShape,
        Square,
        FiveInLine
    }
    public class MatchFinder
    {
        private int _height;
        private int _width;
        private GridSystem<GridObject<IGem>> _gridSystem;
        private readonly List<IMatchDetector> _detectors;
        public MatchFinder(int height, int width, GridSystem<GridObject<IGem>> gridSystem)
        {
            _height = height;
            _width = width;
            _gridSystem = gridSystem;
            _detectors = new List<IMatchDetector>()
            {
                new LineHorizontalMatchDetector(),
                new LineVerticalMatchDetector(),
                new TShapeMatchDetector(),
               new LShapeMatchDetector(),
               new SquareMatchDetector(),
               new FiveInLineMatchDetector()
            };
        }

        public List<MatchData> FindMatches()
        {
            ServiceLocator.Instance.Get<SoundManager>().PlaySFX(SoundId.Match);
            var allMatches = new List<MatchData>();
            foreach(var detector in  _detectors)
            {
                var matches = detector.FindMatches(_gridSystem, _width, _height);
                allMatches.AddRange(matches);
            }
            allMatches = MergeOverlappingMatches(allMatches).OrderBy(m => m.Positions.Count).ToList();
            return allMatches;

            //if (matches.Count == 0)
            //{
            //    audioManager.PlayNoMatch();
            //}
            //else
            //{
            //    audioManager.PlayMatch();
            //}

        }

        private List<MatchData> MergeOverlappingMatches(List<MatchData> allMatches)
        {
            var merged = new List<MatchData>();

            foreach (var match in allMatches)
            {
                var mergedIntoExisting = false;

                for (int i = 0; i < merged.Count; i++)
                {
                    if (merged[i].Positions.Intersect(match.Positions).Any())
                    {
                        var union = merged[i].Positions.Union(match.Positions).ToList();

                        var origin = merged[i].Origin;
                        var pattern = merged[i].Pattern;
                        if (match.Positions.Count > merged[i].Positions.Count)
                        {
                            origin = match.Origin;
                            pattern = match.Pattern;
                        }

                        merged[i] = new MatchData(union, pattern, origin);
                        mergedIntoExisting = true;
                        break;
                    }
                }

                if (!mergedIntoExisting)
                    merged.Add(match);
            }

            return merged;
        }

    }
    public interface IMatchDetector
    {
        List<MatchData> FindMatches(GridSystem<GridObject<IGem>> gridSystem, int width, int height);
    }
    public class LineHorizontalMatchDetector : IMatchDetector
    {
        public List<MatchData> FindMatches(GridSystem<GridObject<IGem>> gridSystem, int width, int height)
        {
            var matches = new List<MatchData>();
            for (int y = 0; y < height; y++)
            {
                int count = 1;
                for (int x = 1; x < width; x++)
                {
                    var currentObj = gridSystem.GetValue(x, y);
                    var previousObj = gridSystem.GetValue(x - 1, y);

                    var currentGem = currentObj?.GetValue()?.GetGem();
                    var previousGem = previousObj?.GetValue()?.GetGem();

                    if (currentGem == null || previousGem == null)
                        count = 1;
                    else if (!currentGem.IsPowerUp && !previousGem.IsPowerUp && currentGem == previousGem)
                        count++;
                    else
                        count = 1;

                    if (count >= 3)
                    {
                        var positions = Enumerable.Range(x - count + 1, count)
                            .Select(p => new Vector2Int(p, y))
                            .ToList();

                        matches.Add(new MatchData(positions,
                            MatchPattern.LineHorizontal,
                            positions[positions.Count / 2]));
                    }
                }
            }
            return matches;
        }
    }

    public class LineVerticalMatchDetector : IMatchDetector
    {
        public List<MatchData> FindMatches(GridSystem<GridObject<IGem>> gridSystem, int width, int height)
        {
            var matches = new List<MatchData>();
            for (int x = 0; x < width; x++)
            {
                int count = 1;
                for (int y = 1; y < height; y++)
                {
                    var currentObj = gridSystem.GetValue(x, y);
                    var previousObj = gridSystem.GetValue(x, y - 1);

                    // Protección contra null en el GridObject o en su valor
                    var currentGem = currentObj?.GetValue()?.GetGem();
                    var previousGem = previousObj?.GetValue()?.GetGem();

                    // Si falta cualquiera, reiniciamos el conteo
                    if (currentGem == null || previousGem == null)
                    {
                        count = 1;
                    }
                    else if (!currentGem.IsPowerUp && !previousGem.IsPowerUp && currentGem == previousGem)
                    {
                        count++;
                    }
                    else
                    {
                        count = 1;
                    }

                    if (count >= 3)
                    {
                        var positions = Enumerable.Range(y - count + 1, count)
                            .Select(py => new Vector2Int(x, py))
                            .ToList();

                        matches.Add(new MatchData(positions,
                            MatchPattern.LineVertical,
                            positions[positions.Count / 2]));
                    }
                }
            }

            return matches;
        }
    }

    public class TShapeMatchDetector : IMatchDetector
    {
        public List<MatchData> FindMatches(GridSystem<GridObject<IGem>> gridSystem, int width, int height)
        {
            var matches = new List<MatchData>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var centerObj = gridSystem.GetValue(x, y);
                    if (centerObj == null || centerObj.GetValue() == null) continue;

                    var centerGem = centerObj.GetValue().GetGem();
                    if (centerGem == null || centerGem.IsPowerUp) continue; 

                    var up = GetMatchingPositionsInDirection(gridSystem, x, y, 0, 1, centerGem, width, height);
                    var down = GetMatchingPositionsInDirection(gridSystem, x, y, 0, -1, centerGem, width, height);
                    var left = GetMatchingPositionsInDirection(gridSystem, x, y, -1, 0, centerGem, width, height);
                    var right = GetMatchingPositionsInDirection(gridSystem, x, y, 1, 0, centerGem, width, height);

                    if ((up.Count + down.Count + 1) >= 3 && (left.Count + right.Count + 1) >= 3)
                    {
                        var positions = new HashSet<Vector2Int> { new Vector2Int(x, y) };
                        foreach (var p in up) positions.Add(p);
                        foreach (var p in down) positions.Add(p);
                        foreach (var p in left) positions.Add(p);
                        foreach (var p in right) positions.Add(p);

                        matches.Add(new MatchData(positions.ToList(), MatchPattern.TShape, new Vector2Int(x, y)));
                    }
                }
            }

            return matches;
        }

        private List<Vector2Int> GetMatchingPositionsInDirection(GridSystem<GridObject<IGem>> gridSystem, int startX, int startY, int dx, int dy, GemSO centerGem, int width, int height)
        {
            var list = new List<Vector2Int>();
            int x = startX + dx;
            int y = startY + dy;

            while (x >= 0 && x < width && y >= 0 && y < height)
            {
                var obj = gridSystem.GetValue(x, y);
                if (obj == null || obj.GetValue() == null) break;

                var gem = obj.GetValue().GetGem();
                if (gem == null || gem.IsPowerUp || gem != centerGem) break; 

                list.Add(new Vector2Int(x, y));
                x += dx;
                y += dy;
            }

            return list;
        }
    }

    public class LShapeMatchDetector : IMatchDetector
    {
        public List<MatchData> FindMatches(GridSystem<GridObject<IGem>> gridSystem, int width, int height)
        {
            var matches = new List<MatchData>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var centerObj = gridSystem.GetValue(x, y);
                    if (centerObj == null || centerObj.GetValue() == null) continue;

                    var centerGem = centerObj.GetValue().GetGem();
                    if (centerGem == null || centerGem.IsPowerUp) continue; 

                    var up = GetMatches(gridSystem, x, y, 0, 1, centerGem, width, height);
                    var down = GetMatches(gridSystem, x, y, 0, -1, centerGem, width, height);
                    var left = GetMatches(gridSystem, x, y, -1, 0, centerGem, width, height);
                    var right = GetMatches(gridSystem, x, y, 1, 0, centerGem, width, height);

                    var possibleShapes = new[]
                    {
                    up.Concat(right).ToList(),
                    up.Concat(left).ToList(),
                    down.Concat(right).ToList(),
                    down.Concat(left).ToList()
                };

                    foreach (var shape in possibleShapes)
                    {
                        if (shape.Count + 1 >= 5)
                        {
                            var positions = new HashSet<Vector2Int>(shape) { new Vector2Int(x, y) };
                            matches.Add(new MatchData(positions.ToList(), MatchPattern.LShape, new Vector2Int(x, y)));
                            break;
                        }
                    }
                }
            }

            return matches;
        }

        private List<Vector2Int> GetMatches(GridSystem<GridObject<IGem>> gridSystem, int x, int y, int dx, int dy, GemSO gem, int width, int height)
        {
            var list = new List<Vector2Int>();
            x += dx;
            y += dy;
            while (x >= 0 && x < width && y >= 0 && y < height)
            {
                var obj = gridSystem.GetValue(x, y);
                if (obj == null || obj.GetValue() == null) break;

                var otherGem = obj.GetValue().GetGem();
                if (otherGem == null || otherGem.IsPowerUp || otherGem != gem) break; 

                list.Add(new Vector2Int(x, y));
                x += dx;
                y += dy;
            }
            return list;
        }
    }

    public class SquareMatchDetector : IMatchDetector
    {
        public List<MatchData> FindMatches(GridSystem<GridObject<IGem>> gridSystem, int width, int height)
        {
            var matches = new List<MatchData>();

            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    var g1 = gridSystem.GetValue(x, y)?.GetValue()?.GetGem();
                    var g2 = gridSystem.GetValue(x + 1, y)?.GetValue()?.GetGem();
                    var g3 = gridSystem.GetValue(x, y + 1)?.GetValue()?.GetGem();
                    var g4 = gridSystem.GetValue(x + 1, y + 1)?.GetValue()?.GetGem();

                    if (g1 == null || g2 == null || g3 == null || g4 == null) continue;

                    if (g1.IsPowerUp || g2.IsPowerUp || g3.IsPowerUp || g4.IsPowerUp) continue;

                    if (g1 == g2 && g1 == g3 && g1 == g4)
                    {
                        var positions = new List<Vector2Int>
                    {
                        new(x, y),
                        new(x + 1, y),
                        new(x, y + 1),
                        new(x + 1, y + 1)
                    };
                        matches.Add(new MatchData(positions, MatchPattern.Square, new Vector2Int(x, y)));
                    }
                }
            }

            return matches;
        }
    }

    public class FiveInLineMatchDetector : IMatchDetector
    {
        public List<MatchData> FindMatches(GridSystem<GridObject<IGem>> gridSystem, int width, int height)
        {
            var matches = new List<MatchData>();

            // Horizontal
            for (int y = 0; y < height; y++)
            {
                int count = 1;
                for (int x = 1; x < width; x++)
                {
                    var currentGem = gridSystem.GetValue(x, y)?.GetValue()?.GetGem();
                    var previousGem = gridSystem.GetValue(x - 1, y)?.GetValue()?.GetGem();

                    if (currentGem == null || previousGem == null)
                        count = 1;
                    else if (!currentGem.IsPowerUp && !previousGem.IsPowerUp && currentGem == previousGem)
                        count++;
                    else
                        count = 1;

                    if (count >= 5)
                    {
                        var positions = Enumerable.Range(x - count + 1, count)
                            .Select(px => new Vector2Int(px, y))
                            .ToList();

                        matches.Add(new MatchData(positions, MatchPattern.FiveInLine, positions[count / 2]));
                    }
                }
            }

            // Vertical
            for (int x = 0; x < width; x++)
            {
                int count = 1;
                for (int y = 1; y < height; y++)
                {
                    var currentGem = gridSystem.GetValue(x, y)?.GetValue()?.GetGem();
                    var previousGem = gridSystem.GetValue(x, y - 1)?.GetValue()?.GetGem();

                    if (currentGem == null || previousGem == null)
                        count = 1;
                    else if (!currentGem.IsPowerUp && !previousGem.IsPowerUp && currentGem == previousGem)
                        count++;
                    else
                        count = 1;

                    if (count >= 5)
                    {
                        var positions = Enumerable.Range(y - count + 1, count)
                            .Select(py => new Vector2Int(x, py))
                            .ToList();

                        matches.Add(new MatchData(positions, MatchPattern.FiveInLine, positions[count / 2]));
                    }
                }
            }

            return matches;
        }
    }

}
