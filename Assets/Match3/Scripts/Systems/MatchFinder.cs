using Core;
using System.Collections.Generic;
using UnityEngine;


namespace Systems
{
    public struct MatchData
    {
        public List<Vector2Int> Positions;
        public MatchPattern Pattern;
        public Vector2Int Origin;
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

        public MatchFinder(int height, int width, GridSystem<GridObject<IGem>> gridSystem)
        {
            _height = height;
            _width = width;
            _gridSystem = gridSystem;
        }

        public List<Vector2Int> FindMatches()
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
    }
}
