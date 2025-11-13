using System.Collections.Generic;
using ScriptableObjects;
using ScriptableObjects.Level;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LevelSO))]
    public class LevelEditor : UnityEditor.Editor
    {
        private LevelSO _levelSO;
        private GemSO _selectedGem;
        private ObstacleSO _selectedObstacle;
        private bool _isPlacingGem = true;
        private Vector2 _scrollPosition;
        private const float CELL_SIZE = 50f;
        private const float CELL_SPACING = 5f;
        
        // Referencias a las gemas disponibles
        private List<GemSO> _availableGems = new List<GemSO>();
        private List<ObstacleSO> _availableObstacles = new List<ObstacleSO>();

        private void OnEnable()
        {
            _levelSO = (LevelSO)target;
            LoadAvailableGems();
            LoadAvailableObstacles();
            
            // Inicializar diccionarios si son null
            if (_levelSO.initialGems == null)
            {
                _levelSO.initialGems = new AYellowpaper.SerializedCollections.SerializedDictionary<Vector2Int, GemSO>();
            }
            if (_levelSO.initialObstacles == null)
            {
                _levelSO.initialObstacles = new AYellowpaper.SerializedCollections.SerializedDictionary<Vector2Int, ObstacleSO>();
            }
        }

        private void LoadAvailableGems()
        {
            _availableGems.Clear();
            string[] gemNames = { "Orange", "Pear", "Apple", "Banana", "Blueberry", "Strawberry", "Grape" };
            
            foreach (string gemName in gemNames)
            {
                string[] guids = AssetDatabase.FindAssets($"{gemName} t:GemSO");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    GemSO gem = AssetDatabase.LoadAssetAtPath<GemSO>(path);
                    if (gem != null)
                    {
                        _availableGems.Add(gem);
                    }
                }
            }
        }

        private void LoadAvailableObstacles()
        {
            _availableObstacles.Clear();
            string[] guids = AssetDatabase.FindAssets("t:ObstacleSO");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ObstacleSO obstacle = AssetDatabase.LoadAssetAtPath<ObstacleSO>(path);
                if (obstacle != null)
                {
                    _availableObstacles.Add(obstacle);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Level Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // Propiedades básicas del nivel
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("levelID"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("height"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveLimit"));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // Selector de modo (Gema u Obstáculo)
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Placement Mode", EditorStyles.boldLabel);
            _isPlacingGem = EditorGUILayout.Toggle("Place Gems", _isPlacingGem);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // Selector de gemas
            if (_isPlacingGem)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Select Gem", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal();
                foreach (var gem in _availableGems)
                {
                    if (gem == null) continue;
                    
                    bool isSelected = _selectedGem == gem;
                    Color originalColor = GUI.color;
                    
                    if (isSelected)
                    {
                        GUI.color = Color.green;
                    }
                    
                    if (gem.Icon != null)
                    {
                        if (GUILayout.Button(gem.Icon.texture, GUILayout.Width(50), GUILayout.Height(50)))
                        {
                            _selectedGem = gem;
                            _selectedObstacle = null;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(gem.name, GUILayout.Width(50), GUILayout.Height(50)))
                        {
                            _selectedGem = gem;
                            _selectedObstacle = null;
                        }
                    }
                    
                    GUI.color = originalColor;
                }
                EditorGUILayout.EndHorizontal();
                
                if (_selectedGem != null)
                {
                    EditorGUILayout.LabelField($"Selected: {_selectedGem.name}", EditorStyles.helpBox);
                }
                else
                {
                    EditorGUILayout.HelpBox("Select a gem to place", MessageType.Info);
                }
                
                EditorGUILayout.EndVertical();
            }
            else
            {
                // Selector de obstáculos
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Select Obstacle", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal();
                foreach (var obstacle in _availableObstacles)
                {
                    if (obstacle == null) continue;
                    
                    bool isSelected = _selectedObstacle == obstacle;
                    Color originalColor = GUI.color;
                    
                    if (isSelected)
                    {
                        GUI.color = Color.cyan;
                    }
                    
                    if (obstacle.Icon != null)
                    {
                        if (GUILayout.Button(obstacle.Icon.texture, GUILayout.Width(50), GUILayout.Height(50)))
                        {
                            _selectedObstacle = obstacle;
                            _selectedGem = null;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(obstacle.name, GUILayout.Width(50), GUILayout.Height(50)))
                        {
                            _selectedObstacle = obstacle;
                            _selectedGem = null;
                        }
                    }
                    
                    GUI.color = originalColor;
                }
                EditorGUILayout.EndHorizontal();
                
                if (_selectedObstacle != null)
                {
                    EditorGUILayout.LabelField($"Selected: {_selectedObstacle.name}", EditorStyles.helpBox);
                }
                else
                {
                    EditorGUILayout.HelpBox("Select an obstacle to place", MessageType.Info);
                }
                
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(10);

            // Grid del nivel
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Level Grid", EditorStyles.boldLabel);
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            DrawLevelGrid();
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // Botones de utilidad
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear All Gems"))
            {
                if (EditorUtility.DisplayDialog("Clear All Gems", "Are you sure you want to clear all gems?", "Yes", "No"))
                {
                    _levelSO.initialGems.Clear();
                    EditorUtility.SetDirty(_levelSO);
                }
            }
            
            if (GUILayout.Button("Clear All Obstacles"))
            {
                if (EditorUtility.DisplayDialog("Clear All Obstacles", "Are you sure you want to clear all obstacles?", "Yes", "No"))
                {
                    _levelSO.initialObstacles.Clear();
                    EditorUtility.SetDirty(_levelSO);
                }
            }
            
            if (GUILayout.Button("Clear All"))
            {
                if (EditorUtility.DisplayDialog("Clear All", "Are you sure you want to clear everything?", "Yes", "No"))
                {
                    _levelSO.initialGems.Clear();
                    _levelSO.initialObstacles.Clear();
                    EditorUtility.SetDirty(_levelSO);
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // Botón para rellenar zonas vacías
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Fill Empty Cells with Random Gems"))
            {
                FillEmptyCells();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Mostrar propiedades restantes
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scores"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("objetivesGems"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("objetives"));

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLevelGrid()
        {
            if (_levelSO.width <= 0 || _levelSO.height <= 0)
            {
                EditorGUILayout.HelpBox("Set width and height to display the grid", MessageType.Warning);
                return;
            }

            Event currentEvent = Event.current;
            
            // Obtener el rect del área del grid
            Rect gridAreaRect = GUILayoutUtility.GetRect(
                _levelSO.width * (CELL_SIZE + CELL_SPACING) + CELL_SPACING,
                _levelSO.height * (CELL_SIZE + CELL_SPACING) + CELL_SPACING,
                GUILayout.ExpandWidth(false),
                GUILayout.ExpandHeight(false)
            );

            // Dibujar el grid
            for (int y = 0; y < _levelSO.height; y++)
            {
                for (int x = 0; x < _levelSO.width; x++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    
                    // Calcular posición de la celda
                    float cellX = gridAreaRect.x + CELL_SPACING + x * (CELL_SIZE + CELL_SPACING);
                    float cellY = gridAreaRect.y + CELL_SPACING + y * (CELL_SIZE + CELL_SPACING);
                    Rect cellRect = new Rect(cellX, cellY, CELL_SIZE, CELL_SIZE);
                    
                    // Verificar si hay gema u obstáculo en esta posición
                    bool hasGem = _levelSO.initialGems.ContainsKey(gridPos);
                    bool hasObstacle = _levelSO.initialObstacles.ContainsKey(gridPos);
                    
                    GemSO gemAtPos = hasGem ? _levelSO.initialGems[gridPos] : null;
                    ObstacleSO obstacleAtPos = hasObstacle ? _levelSO.initialObstacles[gridPos] : null;

                    // Dibujar fondo de celda
                    Color originalColor = GUI.color;
                    Color cellColor = hasGem || hasObstacle ? Color.white : new Color(0.8f, 0.8f, 0.8f, 1f);
                    GUI.color = cellColor;
                    GUI.Box(cellRect, "");
                    GUI.color = originalColor;

                    // Dibujar borde de celda
                    EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, cellRect.width, 1), Color.black);
                    EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, 1, cellRect.height), Color.black);
                    EditorGUI.DrawRect(new Rect(cellRect.x + cellRect.width - 1, cellRect.y, 1, cellRect.height), Color.black);
                    EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y + cellRect.height - 1, cellRect.width, 1), Color.black);

                    // Dibujar gema si existe
                    if (gemAtPos != null && gemAtPos.Icon != null)
                    {
                        GUI.DrawTexture(cellRect, gemAtPos.Icon.texture, ScaleMode.ScaleToFit);
                    }
                    else if (gemAtPos != null)
                    {
                        GUI.Label(cellRect, gemAtPos.name, EditorStyles.centeredGreyMiniLabel);
                    }

                    // Dibujar obstáculo si existe (con overlay en la esquina)
                    if (obstacleAtPos != null && obstacleAtPos.Icon != null)
                    {
                        Rect obstacleRect = new Rect(cellRect.x + cellRect.width - 20, cellRect.y + 2, 18, 18);
                        GUI.DrawTexture(obstacleRect, obstacleAtPos.Icon.texture, ScaleMode.ScaleToFit);
                    }
                    else if (obstacleAtPos != null)
                    {
                        GUI.Label(cellRect, "O", EditorStyles.centeredGreyMiniLabel);
                    }

                    // Manejar clicks
                    if (currentEvent.type == EventType.MouseDown && cellRect.Contains(currentEvent.mousePosition))
                    {
                        if (currentEvent.button == 0) // Click izquierdo
                        {
                            HandleCellClick(gridPos);
                            currentEvent.Use();
                            GUI.changed = true;
                        }
                        else if (currentEvent.button == 1) // Click derecho - eliminar
                        {
                            HandleCellRightClick(gridPos);
                            currentEvent.Use();
                            GUI.changed = true;
                        }
                    }
                }
            }

            // Redibujar si hay eventos
            if (currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.MouseUp)
            {
                Repaint();
            }
        }

        private void HandleCellClick(Vector2Int gridPos)
        {
            if (_isPlacingGem)
            {
                if (_selectedGem != null)
                {
                    // Si ya hay una gema, reemplazarla
                    if (_levelSO.initialGems.ContainsKey(gridPos))
                    {
                        _levelSO.initialGems[gridPos] = _selectedGem;
                    }
                    else
                    {
                        _levelSO.initialGems.Add(gridPos, _selectedGem);
                    }
                    EditorUtility.SetDirty(_levelSO);
                }
            }
            else
            {
                if (_selectedObstacle != null)
                {
                    // Si ya hay un obstáculo, reemplazarlo
                    if (_levelSO.initialObstacles.ContainsKey(gridPos))
                    {
                        _levelSO.initialObstacles[gridPos] = _selectedObstacle;
                    }
                    else
                    {
                        _levelSO.initialObstacles.Add(gridPos, _selectedObstacle);
                    }
                    EditorUtility.SetDirty(_levelSO);
                }
            }
        }

        private void HandleCellRightClick(Vector2Int gridPos)
        {
            // Eliminar según el modo actual
            if (_isPlacingGem)
            {
                if (_levelSO.initialGems.ContainsKey(gridPos))
                {
                    _levelSO.initialGems.Remove(gridPos);
                    EditorUtility.SetDirty(_levelSO);
                }
            }
            else
            {
                if (_levelSO.initialObstacles.ContainsKey(gridPos))
                {
                    _levelSO.initialObstacles.Remove(gridPos);
                    EditorUtility.SetDirty(_levelSO);
                }
            }
        }

        private void FillEmptyCells()
        {
            if (_availableGems.Count == 0)
            {
                EditorUtility.DisplayDialog("No Gems Available", "No gems are available to fill empty cells. Please ensure gem assets are loaded.", "OK");
                return;
            }

            if (_levelSO.width <= 0 || _levelSO.height <= 0)
            {
                EditorUtility.DisplayDialog("Invalid Grid Size", "Please set a valid width and height for the level.", "OK");
                return;
            }

            int filledCount = 0;

            // Recorrer todas las celdas del grid
            for (int y = 0; y < _levelSO.height; y++)
            {
                for (int x = 0; x < _levelSO.width; x++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    
                    // Verificar si la celda está vacía (sin gema ni obstáculo)
                    bool hasGem = _levelSO.initialGems.ContainsKey(gridPos);
                    bool hasObstacle = _levelSO.initialObstacles.ContainsKey(gridPos);
                    
                    if (!hasGem && !hasObstacle)
                    {
                        // Seleccionar una gema aleatoria
                        int randomIndex = UnityEngine.Random.Range(0, _availableGems.Count);
                        GemSO randomGem = _availableGems[randomIndex];
                        
                        // Colocar la gema
                        _levelSO.initialGems.Add(gridPos, randomGem);
                        filledCount++;
                    }
                }
            }

            EditorUtility.SetDirty(_levelSO);
            
            if (filledCount > 0)
            {
                EditorUtility.DisplayDialog("Fill Complete", $"Filled {filledCount} empty cell(s) with random gems.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("No Empty Cells", "All cells already have gems or obstacles.", "OK");
            }
        }
    }
}

