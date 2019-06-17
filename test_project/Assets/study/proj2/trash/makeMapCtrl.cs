using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeMapCtrl : MonoBehaviour {

    public GameObject selectRow, selectCol, showMap;
    public UILabel row, col, myMap;
    public List<List<GameObject>> panels = new List<List<GameObject>>();
    public Vector2 strPos;
    float myCol, myRow;
    public GameObject block;
    public GameObject mapSaver;

    string[] choiceMap = { "map1", "map2", "map3", "map4", "map5", "make map" };

    public void makeMapSelf()
    {
        //맵을 직접 만드는 것을 선택하면
        //화면에 행, 열, 화면에 표시하는 UI를 띄운다
        if(myMap.text == choiceMap[5])
        {
            selectRow.SetActive(true);
            selectCol.SetActive(true);
            showMap.SetActive(true);
        }
        else
        {
            selectRow.SetActive(false);
            selectCol.SetActive(false);
            showMap.SetActive(false);
        }
    }

    public void takeMapSize()
    {
        //이미 다른 크기의 판넬이 있는 경우
        //판넬 오브젝트는 모두 지우고 리스트도 비워준다
        for (int i = 0; i < panels.Count; i++)
        {
            for (int j = 0; j < panels[i].Count; j++)
            {
                Destroy(panels[i][j]);
            }
        }
        panels.Clear();

        //popup list가 선택이 안되어있을 수 있기 때문에 예외처리하고
        //행과 열값을 가져온다
        try
        {
            myCol = float.Parse(col.text);
        }
        catch (System.FormatException) { }
        try
        {
            myRow = float.Parse(row.text);
        }
        catch (System.FormatException) { }

        //왼쪽 아래부터 오른쪽 위로 열 단위로 블록을 더해준다
        strPos = new Vector2(-(myCol - 1) / 2, -(myRow - 1) / 2);
        for (int i = 0; i < myCol; i++)
        {
            panels.Add(new List<GameObject>());
            for (int j = 0; j < myRow; j++)
            {
                panels[i].Add(Instantiate(block, new Vector3(strPos.x + i * 1.1f, strPos.y + j * 1.1f, 0), transform.rotation));
            }
        }
    }

    public void mapSetting()
    {
        mapInfo myMapData = mapSaver.GetComponent<mapInfo>();
        List<int[,]> mapInput = new List<int[,]>();

        //선택가능한 디폴트맵 1~5의 정보
        mapInput.Add(new int[,] {{1,1,1,1,1},{1,1,1,1,1},{1,1,1,1,1},{1,1,1,1,1},{1,1,1,1,1},{1,1,1,1,1},{1,1,1,1,1},{1,1,1,1,1}});
        mapInput.Add(new int[,] {{0,0,1,1,1,1,1,0,0},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{0,0,1,1,1,1,1,0,0}});
        mapInput.Add(new int[,] {{0,1,1,1,1,1,1,1,0},{0,0,1,1,1,1,1,0,0},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{0,0,1,1,1,1,1,0,0},{0,1,1,1,1,1,1,1,0}}); 
        mapInput.Add(new int[,] {{1,0,1,0,1,0,1,1,1},{1,1,1,1,1,1,1,0,1},{1,1,1,1,1,1,1,0,1},{1,1,1,1,1,1,1,0,1},{1,1,1,1,1,1,1,0,1},{1,0,1,0,1,0,1,1,1}});
        mapInput.Add(new int[,] {{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1},{1,1,1,1,1,1,1,1,1}});

        //디폴트맵을 선택한 경우
        //선택하면 화면에 판넬을 통해 맵 생긴거를 표시하는 것 추가할 예정
        for (int i = 0; i < choiceMap.Length - 1; i++) {
            if (myMap.text == choiceMap[i])
            {
                strPos = new Vector2(-(mapInput[i].GetLength(0) - 1) / 2, -(mapInput[i].GetLength(1) - 1) / 2);
                for (int x = 0; x < mapInput[i].GetLength(0); x++)
                {
                    myMapData.mapPos.Add(new List<Vector2>());
                    myMapData.mapUse.Add(new List<bool>());
                    for(int y = 0; y < mapInput[i].GetLength(1); y++)
                    {
                        myMapData.mapPos[x].Add(new Vector2((strPos.x + x) * 50, (strPos.y + y) * 50));
                        if (mapInput[i][x, y] == 1)
                        {
                            myMapData.mapUse[x].Add(true);
                        }
                        else
                        {
                            myMapData.mapUse[x].Add(false);
                        }
                    }
                }
            }
        }

        //직접 만드는 것을 선택
        if(myMap.text == choiceMap[5])
        {
            for (int i = 0; i < panels.Count; i++)
            {
                myMapData.mapPos.Add(new List<Vector2>());
                myMapData.mapUse.Add(new List<bool>());
                for (int j = 0; j < panels[i].Count; j++)
                {
                    myMapData.mapPos[i].Add(new Vector2((strPos.x + i) * 50, (strPos.y + j) * 50));
                    myMapData.mapUse[i].Add(panels[i][j].GetComponent<mapBlock>().selected);
                }
            }
        }
        DontDestroyOnLoad(mapSaver);
        Application.LoadLevel("game");
    }

    void Update () {
        //직접 만드는 것 선택 시 바로 추가 버튼 표시
        makeMapSelf();
	}
}