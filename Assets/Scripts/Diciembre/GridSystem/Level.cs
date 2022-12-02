using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Diciembre
{
    public enum TILE_TYPE {ROCK = -1, GRASS = 0 , WATER = 1, SAND = 2}
    [System.Serializable]
    public class Level : MonoBehaviour
    {
        #region PUBLICS_FIELDS

        [SerializeField,Range(0,50)] public int columns,rows;

        [NonSerialized] public TILE_TYPE[,] board;

        [NonSerialized] public Action OnMyValidate;

        #endregion

        #region PRIVATE_FIELDS

        private int prevColumns=0, prevRows=0;

        private Dictionary<Vector2Int, TILE_TYPE> data;

        #endregion

        #region UNITY_CALLS

        private void OnValidate()
        {
            if(columns<=0)
                columns = 1;
            if (rows<=0)
                rows = 1;
            if (columns + rows >= 50)
                Debug.LogWarning("la grilla es muy grande.");
            if (columns!=prevColumns||rows!=prevRows)
                SetGrid();
            if(data!=null)
                LoadData();
        }

        #endregion

        #region PRIVATE_METHODS
        private void SetGrid()
        {
            prevColumns = columns;
            prevRows = rows;
            OnMyValidate = SaveData;
            board = new TILE_TYPE[columns, rows];
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    board[i, j] = TILE_TYPE.GRASS;
        }
        private void SaveData()
        {
            data = new Dictionary<Vector2Int, TILE_TYPE>();
            data.Clear();
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    data.Add(new Vector2Int(i, j), board[i, j]);
        }
        private void LoadData()
        {
            if (data == null)
                return;
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    TILE_TYPE value = TILE_TYPE.GRASS;
                    data.TryGetValue(new Vector2Int(i, j), out value);
                    board[i, j] = value;
                }
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public static float GetSpeedInTerrain(Vector3 pos)
        {
            Vector2Int position = NodeUtils.GetVec3IntFromVector3(pos);
            float weight = NodeGenerator.map[NodeUtils.PositionToIndex(position)].weight;
            if (weight == 0)
                return 0;
            return 1 / weight;
        }
        #endregion
    }
    #region CUSTOM_EDITOR
    [CustomEditor(typeof(Level))]
    public class LevelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Level level = (Level)target;

            EditorGUILayout.Space();

            EditorGUI.indentLevel = 0;

            GUIStyle tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(2, 2, 2, 2);
            tableStyle.margin.left = 10;

            GUIStyle headerColumnStyle = new GUIStyle();
            headerColumnStyle.fixedWidth = 35;

            GUIStyle columnStyle = new GUIStyle();
            columnStyle.fixedWidth = 65;

            GUIStyle rowStyle = new GUIStyle();
            rowStyle.fixedHeight = 25;

            GUIStyle rowHeaderStyle = new GUIStyle();
            rowHeaderStyle.fixedWidth = columnStyle.fixedWidth - 1;

            GUIStyle columnHeaderStyle = new GUIStyle();
            columnHeaderStyle.fixedWidth = 30;
            columnHeaderStyle.fixedHeight = 25.5f;

            GUIStyle columnLabelStyle = new GUIStyle();
            columnLabelStyle.fixedWidth = rowHeaderStyle.fixedWidth - 6;
            columnLabelStyle.alignment = TextAnchor.MiddleCenter;
            columnLabelStyle.fontStyle = FontStyle.Bold;

            GUIStyle cornerLabelStyle = new GUIStyle();
            cornerLabelStyle.fixedWidth = 42;
            cornerLabelStyle.alignment = TextAnchor.MiddleRight;
            cornerLabelStyle.fontStyle = FontStyle.BoldAndItalic;
            cornerLabelStyle.fontSize = 14;
            cornerLabelStyle.padding.top = -5;

            GUIStyle rowLabelStyle = new GUIStyle();
            rowLabelStyle.fixedWidth = 25;
            rowLabelStyle.alignment = TextAnchor.MiddleRight;
            rowLabelStyle.fontStyle = FontStyle.Bold;

            GUIStyle enumStyle = new GUIStyle("popup");
            rowStyle.fixedWidth = 64;

            EditorGUILayout.BeginHorizontal(tableStyle);
            for (int x = -1; x < level.columns; x++)
            {
                EditorGUILayout.BeginVertical((x == -1) ? headerColumnStyle : columnStyle);
                for (int y = 0; y <= level.rows; y++)
                {
                    int trueY = level.rows - y - 1;
                    if (x == -1 && y == level.rows)
                    {
                        EditorGUILayout.BeginVertical(rowHeaderStyle);
                        EditorGUILayout.LabelField("[X,Y]", cornerLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (x == -1)
                    {
                        EditorGUILayout.BeginVertical(columnHeaderStyle);
                        EditorGUILayout.LabelField((trueY).ToString(), rowLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (y == level.rows)
                    {
                        EditorGUILayout.BeginVertical(rowHeaderStyle);
                        EditorGUILayout.LabelField(x.ToString(), columnLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }

                    if (x >= 0 && y < level.rows)
                    {
                        EditorGUILayout.BeginHorizontal(rowStyle);
                        if (level.board == null)
                            return;

                        if (x >= level.columns || y >= level.rows || x < 0 || y < 0 || level.columns == 0 || level.rows == 0)
                            continue;
                        TILE_TYPE newValue;
                        newValue = (TILE_TYPE)EditorGUILayout.EnumPopup(level.board[x, trueY], enumStyle);
                        if (level.board[x, trueY]!= newValue)
                        {
                            level.board[x, trueY] = newValue;
                            level.OnMyValidate?.Invoke();
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    #endregion
}
