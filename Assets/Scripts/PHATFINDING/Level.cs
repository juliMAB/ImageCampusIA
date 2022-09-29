using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.TerrainTools;

public enum TerrainType { undefined, normal, mud, water, sand, rock,centro};



[System.Serializable]
public class Level : MonoBehaviour
{
    public bool showBoard;

    public int columns;
    public int rows;

    public int[,] board; // unity no guarda arrays bidimensionales :(

    public List<int> data;

    [HideInInspector] public bool onPlay = false;
    List<GameObject> tiles = null;

    public GameObject tilePrefab;

    private void Start()
    {
        onPlay = true;
    }
    public void SetGrid()
    {
        board = new int[columns, rows];
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                board[i, j] = 1;
            }
        }
        Debug.Log("Guardar la data despues de configurar.");
    }

    public void SaveData()
    {
        data.Clear();
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                data.Add(board[i, j]);
            }
        }
    }
    public void LoadData()
    {
        int v=0;
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                board[i, j] = data[v];
                v++;
            }
        }
    }

    public void InstanciateGrid()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            Destroy(tiles[i]);
        }
        tiles.Clear();
        tiles = new List<GameObject>();
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject tile = Instantiate(tilePrefab,new Vector3(x,y,5),Quaternion.identity);
                MeshRenderer tileMaterial = tile.GetComponent<MeshRenderer>();
                switch (board[x,y])
                {
                    case 1:
                        tileMaterial.material.color = Color.green;
                        break;
                    case 2:
                        tileMaterial.material.color = Color.gray;
                        break;
                    case 3:
                        tileMaterial.material.color = Color.blue;
                        break;
                    case 4:
                        tileMaterial.material.color = Color.yellow;
                        break;
                    default:
                        tileMaterial.material.color = Color.white;
                        break;
                }
                
                tiles.Add(tile);
            }
        }
    }
    public void RestoreData()
    {
        SetGrid();
        int c = 0;
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                board[i, j] = data[c];
                c++;
            }
        }
    }

    public static float GetSpeedInTerrain(Vector3 pos)
    {
        Vector2Int position = NodeUtils.GetVec3IntFromVector3(pos);
        float weight = NodeGenerator.map[NodeUtils.PositionToIndex(position)].weight;
        if (weight == 0)
            return 0;
        return 1 / weight;
    }
}

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
 

    public bool showLevels = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Level level = (Level)target;

        if (GUILayout.Button("setGrid"))
            level.SetGrid();
        if (GUILayout.Button("SaveData"))
            level.SaveData();
        if (GUILayout.Button("LoadData"))
            level.LoadData();

        if (GUILayout.Button("InstanciateGrid"))
            level.InstanciateGrid();

        EditorGUILayout.Space();
        if(level.onPlay)
            return;

        if (level.showBoard)
        {

            EditorGUI.indentLevel = 0;

            GUIStyle tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(10, 10, 10, 10);
            tableStyle.margin.left = 32;

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
            rowStyle.fixedWidth = 65;

            EditorGUILayout.BeginHorizontal(tableStyle);
            for (int x = -1; x < level.columns; x++)
            {
                EditorGUILayout.BeginVertical((x == -1) ? headerColumnStyle : columnStyle);
                for (int y = -1; y < level.rows; y++)
                {
                    if (x == -1 && y == -1)
                    {
                        EditorGUILayout.BeginVertical(rowHeaderStyle);
                        EditorGUILayout.LabelField("[X,Y]", cornerLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (x == -1)
                    {
                        EditorGUILayout.BeginVertical(columnHeaderStyle);
                        EditorGUILayout.LabelField(y.ToString(), rowLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (y == -1)
                    {
                        EditorGUILayout.BeginVertical(rowHeaderStyle);
                        EditorGUILayout.LabelField(x.ToString(), columnLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }

                    if (x >= 0 && y >= 0)
                    {
                        EditorGUILayout.BeginHorizontal(rowStyle);
                        if (level.board == null)
                        {
                            Debug.LogWarning("no se seteo el mapa, se setea automatico.");
                            level.SetGrid();

                        }
                        else
                        {
                            if (x >= level.columns || y >= level.rows || x < 0 || y < 0 || level.columns==0|| level.rows == 0)
                                continue;
                            level.board[x, y] = EditorGUILayout.IntField(level.board[x, y], enumStyle);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

        }
    }
}