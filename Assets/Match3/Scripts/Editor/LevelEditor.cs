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

        private List<GemSO> _availableGems = new List<GemSO>();
        private List<ObstacleSO> _availableObstacles = new List<ObstacleSO>();

        // ==========================
        // Objective Creator Fields
        // ==========================
        private string[] _objectiveTypeNames;
        private System.Type[] _objectiveTypes;
        private int _selectedObjectiveTypeIndex = 0;
        private int _newObjectiveTargetValue = 1;
        private string _newObjectiveName = "New Objective";
        private string _newObjectiveDescription = "Description...";
        private Sprite _newObjectiveIcon = null;
        // ==========================

        private void OnEnable()
        {
            _levelSO = (LevelSO)target;
            LoadAvailableGems();
            LoadAvailableObstacles();
            LoadObjectiveTypes();

            if (_levelSO.initialGems == null)
                _levelSO.initialGems = new AYellowpaper.SerializedCollections.SerializedDictionary<Vector2Int, GemSO>();

            if (_levelSO.initialObstacles == null)
                _levelSO.initialObstacles = new AYellowpaper.SerializedCollections.SerializedDictionary<Vector2Int, ObstacleSO>();
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

        // ==========================================
        // Load Objective Types Automatically
        // ==========================================
        private void LoadObjectiveTypes()
        {
            var baseType = typeof(ScriptableObjects.Level.Objetives.ObjectiveBaseSO);
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

            List<System.Type> foundTypes = new List<System.Type>();

            foreach (var asm in assemblies)
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                    {
                        foundTypes.Add(type);
                    }
                }
            }

            _objectiveTypes = foundTypes.ToArray();
            _objectiveTypeNames = foundTypes.ConvertAll(t => t.Name).ToArray();
        }

        // ==========================================
        // Crea un objetivo ScriptableObject
        // ==========================================
        private void CreateObjectiveInstance()
        {
            if (_objectiveTypes.Length == 0) return;

            System.Type selectedType = _objectiveTypes[_selectedObjectiveTypeIndex];

            string targetFolder = "Assets/Match3/ScriptableObjects/Level/Objectives";

            if (!AssetDatabase.IsValidFolder(targetFolder))
            {
                string parent = "Assets/Match3/ScriptableObjects/Level";
                AssetDatabase.CreateFolder(parent, "Objectives");
            }

            string newPath = AssetDatabase.GenerateUniqueAssetPath($"{targetFolder}/{_newObjectiveName}.asset");

            // Crear instancia
            ScriptableObject newObj = ScriptableObject.CreateInstance(selectedType);
            SerializedObject so = new SerializedObject(newObj);

            // Asignar campos base del objetivo (asegúrate de que existen en tu ObjectiveBaseSO)
            so.FindProperty("_name").stringValue = _newObjectiveName;
            so.FindProperty("_description").stringValue = _newObjectiveDescription;
            so.FindProperty("_icon").objectReferenceValue = _newObjectiveIcon;

            // Target value
            var targetValueProp = so.FindProperty("_targetValue");
            if (targetValueProp != null)
                targetValueProp.intValue = _newObjectiveTargetValue;

            so.ApplyModifiedProperties();

            // Crear archivo
            AssetDatabase.CreateAsset(newObj, newPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Agregar al LevelSO
            if (_levelSO.objetives == null)
                _levelSO.objetives = new List<ScriptableObjects.Level.Objetives.ObjectiveBaseSO>();

            _levelSO.objetives.Add((ScriptableObjects.Level.Objetives.ObjectiveBaseSO)newObj);
            EditorUtility.SetDirty(_levelSO);

            Debug.Log($"Objective created: {_newObjectiveName}");
        }


        // ==========================================
        // UI para crear objetivos
        // ==========================================
        private void DrawCreateObjectiveSection()
        {
            if (_objectiveTypes == null)
                LoadObjectiveTypes();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Create New Objective", EditorStyles.boldLabel);

            // Tipo de objetivo
            _selectedObjectiveTypeIndex = EditorGUILayout.Popup("Objective Type", _selectedObjectiveTypeIndex, _objectiveTypeNames);

            // Nombre
            _newObjectiveName = EditorGUILayout.TextField("Name", _newObjectiveName);

            // Descripción
            _newObjectiveDescription = EditorGUILayout.TextField("Description", _newObjectiveDescription);

            // Icono
            _newObjectiveIcon = (Sprite)EditorGUILayout.ObjectField("Icon", _newObjectiveIcon, typeof(Sprite), false);

            // Target value
            _newObjectiveTargetValue = EditorGUILayout.IntField("Target Value", _newObjectiveTargetValue);

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Create Objective"))
            {
                CreateObjectiveInstance();
            }
        }


        // ==========================================
        // INSPECTOR GUI PRINCIPAL
        // ==========================================
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Level Editor", EditorStyles.boldLabel);

            EditorGUILayout.Space(5);

            // --- Level Basic Info ---
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("levelID"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("height"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveLimit"));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // --- Placement mode ---
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Placement Mode", EditorStyles.boldLabel);
            _isPlacingGem = EditorGUILayout.Toggle("Place Gems", _isPlacingGem);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // --- GEM / OBSTACLE SELECTOR ---
            if (_isPlacingGem)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Select Gem", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                foreach (var gem in _availableGems)
                {
                    if (gem == null) continue;

                    bool selected = _selectedGem == gem;

                    Color original = GUI.color;
                    if (selected) GUI.color = Color.green;

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

                    GUI.color = original;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Select Obstacle", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                foreach (var obstacle in _availableObstacles)
                {
                    if (obstacle == null) continue;

                    bool selected = _selectedObstacle == obstacle;

                    Color original = GUI.color;
                    if (selected) GUI.color = Color.cyan;

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

                    GUI.color = original;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(15);

            // --- GRID ---
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Level Grid", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawLevelGrid();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(15);

            // --- Utility Buttons ---
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear All Gems"))
            {
                _levelSO.initialGems.Clear();
            }
            if (GUILayout.Button("Clear All Obstacles"))
            {
                _levelSO.initialObstacles.Clear();
            }
            if (GUILayout.Button("Clear All"))
            {
                _levelSO.initialGems.Clear();
                _levelSO.initialObstacles.Clear();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Fill Empty Cells with Random Gems"))
            {
                FillEmptyCells();
            }

            EditorGUILayout.Space(20);

            // ==================================================
            // OBJECTIVE CREATOR
            // ==================================================
            EditorGUILayout.BeginVertical("box");
            DrawCreateObjectiveSection();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // ==================================================
            // EXISTING OBJECTIVES LIST
            // ==================================================
            SerializedProperty objectivesProperty = serializedObject.FindProperty("objetives");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(objectivesProperty, new GUIContent("Objectives List"), true);

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        // ==========================================
        // GRID DRAWING
        // ==========================================
        private void DrawLevelGrid()
        {
            if (_levelSO.width <= 0 || _levelSO.height <= 0)
            {
                EditorGUILayout.HelpBox("Set width and height to display the grid.", MessageType.Warning);
                return;
            }

            Event evt = Event.current;

            Rect gridRect = GUILayoutUtility.GetRect(
                _levelSO.width * (CELL_SIZE + CELL_SPACING) + CELL_SPACING,
                _levelSO.height * (CELL_SIZE + CELL_SPACING) + CELL_SPACING,
                GUILayout.ExpandWidth(false),
                GUILayout.ExpandHeight(false)
            );

            for (int y = 0; y < _levelSO.height; y++)
            {
                for (int x = 0; x < _levelSO.width; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    float px = gridRect.x + CELL_SPACING + x * (CELL_SIZE + CELL_SPACING);
                    float py = gridRect.y + CELL_SPACING + y * (CELL_SIZE + CELL_SPACING);
                    Rect cell = new Rect(px, py, CELL_SIZE, CELL_SIZE);

                    bool hasGem = _levelSO.initialGems.ContainsKey(pos);
                    bool hasObstacle = _levelSO.initialObstacles.ContainsKey(pos);

                    GUI.Box(cell, "");

                    if (hasGem && _levelSO.initialGems[pos]?.Icon != null)
                    {
                        GUI.DrawTexture(cell, _levelSO.initialGems[pos].Icon.texture, ScaleMode.ScaleToFit);
                    }

                    if (hasObstacle && _levelSO.initialObstacles[pos]?.Icon != null)
                    {
                        Rect r = new Rect(cell.x + 28, cell.y + 2, 20, 20);
                        GUI.DrawTexture(r, _levelSO.initialObstacles[pos].Icon.texture, ScaleMode.ScaleToFit);
                    }

                    if (evt.type == EventType.MouseDown && cell.Contains(evt.mousePosition))
                    {
                        if (evt.button == 0) HandleCellClick(pos);
                        if (evt.button == 1) HandleCellRightClick(pos);
                        evt.Use();
                    }
                }
            }
        }

        private void HandleCellClick(Vector2Int pos)
        {
            if (_isPlacingGem && _selectedGem != null)
            {
                _levelSO.initialGems[pos] = _selectedGem;
            }
            else if (!_isPlacingGem && _selectedObstacle != null)
            {
                _levelSO.initialObstacles[pos] = _selectedObstacle;
            }

            EditorUtility.SetDirty(_levelSO);
        }

        private void HandleCellRightClick(Vector2Int pos)
        {
            if (_isPlacingGem)
            {
                _levelSO.initialGems.Remove(pos);
            }
            else
            {
                _levelSO.initialObstacles.Remove(pos);
            }

            EditorUtility.SetDirty(_levelSO);
        }

        private void FillEmptyCells()
        {
            if (_availableGems.Count == 0)
            {
                EditorUtility.DisplayDialog("No gems available", "Please load gem assets", "OK");
                return;
            }

            for (int y = 0; y < _levelSO.height; y++)
            {
                for (int x = 0; x < _levelSO.width; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (!_levelSO.initialGems.ContainsKey(pos) &&
                        !_levelSO.initialObstacles.ContainsKey(pos))
                    {
                        _levelSO.initialGems[pos] = _availableGems[Random.Range(0, _availableGems.Count)];
                    }
                }
            }

            EditorUtility.SetDirty(_levelSO);
        }
    }
}
