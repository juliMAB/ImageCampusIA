using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Diciembre
{
    [System.Serializable]
    public class TileSave
    {
        public int x = 1, y = 1;
        public TILE_TYPE type;

        public TileSave(int x,int y, TILE_TYPE data)
        {
            this.x = x;
            this.y = y;
            this.type = data;
        }
    }
    public enum TILE_TYPE {ROCK = -1, GRASS = 0 , WATER = 1, SAND = 2}
    [System.Serializable]
    public class Level : MonoBehaviour
    {
        #region PUBLICS_FIELDS

        [SerializeField,Range(0,50)] public int columns=1,rows=1;

        [SerializeField] public TILE_TYPE[,] board;

        [SerializeField] public Action OnMyValidate;

        [SerializeField] public int realColums = 0, realRows = 0;
        #endregion

        #region PRIVATE_FIELDS

        private List<TileSave> data;

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
        }

        #endregion

        #region PRIVATE_METHODS
        private void SaveData()
        {
            Debug.Log("SaveData");
            data = new List<TileSave>();
            data.Clear();
            for (int i = 0; i < realColums; i++)
                for (int j = 0; j < realRows; j++)
                    data.Add(new TileSave(i,j,board[i, j]));
        }
        private void LoadData()
        {
            
            if (data == null)
                return;

            Debug.Log("LoadData");
            for (int i = 0; i < realColums; i++)
            {
                for (int j = 0; j < realRows; j++)
                {
                    TileSave t = TryGetTileOnList(i,j);
                    if (t == null)
                        board[i, j] = 0;
                    else
                        board[i, j] = t.type;
                }
            }
        }
        private TileSave TryGetTileOnList(int x,int y)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].x==x && data[i].y==y)
                    return data[i];
            }
            return null;
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetGrid()
        {
            Debug.Log("setGrid");
            OnMyValidate = SaveData;
            board = new TILE_TYPE[columns, rows];
            realColums= columns;
            realRows= rows;
            if (data != null)
                LoadData();
        }

        public void MyStart()
        {
            SetGrid();
        }
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

            if (GUILayout.Button("setGrid"))
                level.SetGrid();

            if (!level)
                return;
            if (level.columns<1||level.rows<1)
                return;
            if (level.board == null)
                return;
            if ((level.columns != level.realColums)|| (level.rows != level.realRows))
                return;

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
