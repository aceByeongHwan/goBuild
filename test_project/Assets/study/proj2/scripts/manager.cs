using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class manager : MonoBehaviour
{
    private float myCol, myRow;
    private string mainSceneName = "main", gameSceneName = "game";
    private string[] choiceMap = { "map1", "map2", "map3", "map4", "map5", "make map" };
    private Vector2 strPos;                                                                 //맵이 그려지는 시작 위치(왼쪽 아래)
    private GameObject selectRow, selectCol, showMap;                                       //맵을 직접 만들 경우 나오는 UI 오브젝트
    private UILabel row, col, myMap;
    private List<int[,]> mapInput = new List<int[,]>();                                     //디폴트 맵 데이터
    private List<List<GameObject>> panels = new List<List<GameObject>>();                   //선택할 맵을 화면에 보여주는 패널

    public GameObject block;        //패널의 프리팹 오브젝트
    public GameObject candyBlock;   //게임 씬의 캔디의 프리팹 오브젝트

    public GameObject selectPopup;

    public static int touchCnt;                                                         //게임 씬에서의 캔디 터치 횟수
    public static Vector3 curPos, lastPos;                                              //게인 씬에서 터치 된 두 캔디
    public static manager Instance;
    public static Transform candyContainer;                                             //게임 씬에서 캔디가 생성되는 위치
    public static myCandy curCandy, lastCandy;
    public static GameObject grid;
    public static List<List<Vector2>> mapPos = new List<List<Vector2>>();
    public static List<List<bool>> mapUse = new List<List<bool>>();
    public static List<List<GameObject>> candyBlocks = new List<List<GameObject>>();    //게임 씬에서 생성되는 캔디와 window를 가지는 빈 오브젝트
    public static List<List<myCandy>> candys = new List<List<myCandy>>();               //게임에서 실제 캔디의 로직을 가지는 myCandy 리스트

    /// <summary>
    /// 오브젝트를 싱글톤으로 만든다
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// 메인 씬에서 오브젝트를 가져온다(싱클톤 화면 전환 시 연결이 끊기기 때문)
    /// </summary>
    void LoadMainSceneData()
    {
        GameObject mainUI = GameObject.Find("mainUI");
        selectRow = mainUI.GetComponent<mainUIset>().selectRow;
        selectCol = mainUI.GetComponent<mainUIset>().selectCol;
        showMap = mainUI.GetComponent<mainUIset>().showMap;
        row = mainUI.GetComponent<mainUIset>().row;
        col = mainUI.GetComponent<mainUIset>().col;
        myMap = mainUI.GetComponent<mainUIset>().myMap;

        selectRow.SetActive(false);
        selectCol.SetActive(false);
        showMap.SetActive(false);
    }
    /// <summary>
    /// 씬 전환 시 작동되는 로직 저장
    /// </summary>
    /// <param name="sceneNum">Scene number.</param>
    private void OnLevelWasLoaded(int sceneNum)
    {
        if(sceneNum == SceneManager.GetSceneByName(mainSceneName).buildIndex)
        {
            LoadMainSceneData();
        }
        //게임 신이면 맵을 로드하고 터치 시 나오는 격자를 찾아 비활성화
        else if (sceneNum == SceneManager.GetSceneByName(gameSceneName).buildIndex)
        {
            LoadMap();
            grid = GameObject.Find("UI Root").transform.Find("grid").gameObject;
            grid.SetActive(false);
            touchCnt = 0;
        }
    }

    private void Start()
    {
#if QA_SERVER || IOS_SIMULATOR_QA
        selectPopup.SetActive(false);
#endif
        LoadMainSceneData();
        //디폴트값으로 가지고있는 맵의 정보를 저장
        mapInput.Add(new int[,] { { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 } });
        mapInput.Add(new int[,] { { 0, 0, 1, 1, 1, 1, 1, 0, 0 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 0, 0, 1, 1, 1, 1, 1, 0, 0 } });
        mapInput.Add(new int[,] { { 0, 1, 1, 1, 1, 1, 1, 1, 0 }, { 0, 0, 1, 1, 1, 1, 1, 0, 0 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 0, 0, 1, 1, 1, 1, 1, 0, 0 }, { 0, 1, 1, 1, 1, 1, 1, 1, 0 } });
        mapInput.Add(new int[,] { { 1, 0, 1, 0, 1, 0, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 0, 1 }, { 1, 1, 1, 1, 1, 1, 1, 0, 1 }, { 1, 1, 1, 1, 1, 1, 1, 0, 1 }, { 1, 1, 1, 1, 1, 1, 1, 0, 1 }, { 1, 0, 1, 0, 1, 0, 1, 1, 1 } });
        mapInput.Add(new int[,] { { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 } });
    }

    /// <summary>
    /// 맵 선택할 때마다 초기화를 위해 오브젝트와 리스트를 비움
    /// </summary>
    public void ResetBlock()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            for (int j = 0; j < panels[i].Count; j++)
            {
                if (panels[i][j] != null)
                {
                    Destroy(panels[i][j].gameObject);
                }
            }
        }
        panels.Clear();
    }

    public void BackToMain()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    /// <summary>
    /// popup list에서 맵을 고르면 실행
    /// </summary>
    public void SelectMap()
    {
        //직접 만드는 경우 맵 생성 UI 활성화
        if (myMap.text == choiceMap[5])
        {
            ResetBlock();

            selectRow.SetActive(true);
            selectCol.SetActive(true);
            showMap.SetActive(true);
        }
        //디폴트값이면 자신의 맵 번호를 찾아 맵을 그린다
        else
        {
            ResetBlock();

            selectRow.SetActive(false);
            selectCol.SetActive(false);
            showMap.SetActive(false);

            for (int i = 0; i < choiceMap.Length - 1; i++)
            {
                if (myMap.text == choiceMap[i])
                {
                    strPos = new Vector2(-(mapInput[i].GetLength(0) - 1) / 2, -(mapInput[i].GetLength(1) - 1) / 2);
                    for (int x = 0; x < mapInput[i].GetLength(0); x++)
                    {
                        panels.Add(new List<GameObject>());
                        for (int y = 0; y < mapInput[i].GetLength(1); y++)
                        {
                            panels[x].Add(Instantiate(block, new Vector3(strPos.x + x * 1.1f, strPos.y + y * 1.1f, 0), transform.rotation));
                            if (mapInput[i][x, y] == 1)
                            {
                                panels[x][y].SetActive(true);
                            }
                            else
                            {
                                panels[x][y].SetActive(false);
                            }
                        }
                    }
                }
            }

        }
    }  

    /// <summary>
    /// 직접 맵을 만드는 경우 column, row값을 받아 크기만큼 화면에 띄움
    /// </summary>
    public void TakeMapSize()
    {
        ResetBlock();
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

    void MakeMap(int i)
    {
        strPos = new Vector2(-(mapInput[i].GetLength(0) - 1) / 2, -(mapInput[i].GetLength(1) - 1) / 2);
        for (int x = 0; x < mapInput[i].GetLength(0); x++)
        {
            mapPos.Add(new List<Vector2>());
            mapUse.Add(new List<bool>());
            for (int y = 0; y < mapInput[i].GetLength(1); y++)
            {
                mapPos[x].Add(new Vector2((strPos.x + x) * 50, (strPos.y + y) * 50));
                if (mapInput[i][x, y] == 1)
                {
                    mapUse[x].Add(true);
                }
                else
                {
                    mapUse[x].Add(false);
                }
            }
        }
    }

    /// <summary>
    /// 각 패널의 위치와 선택 여부를 저장하고 게임 씬으로 화면 전환
    /// </summary>
    public void MapSetting()
    {
        mapPos.Clear();
        mapUse.Clear();

#if IOS_SIMULATOR_DEV
        MakeMap(0);
#else
        //맵이 디폴트값인 경우
        for (int i = 0; i < choiceMap.Length - 1; i++)
        {
            if (myMap.text == choiceMap[i])
            {
                MakeMap(i);
            }
        }
        //맵을 직접 그린 경우
        if (myMap.text == choiceMap[5])
        {
            for (int i = 0; i < panels.Count; i++)
            {
                mapPos.Add(new List<Vector2>());
                mapUse.Add(new List<bool>());
                for (int j = 0; j < panels[i].Count; j++)
                {
                    mapPos[i].Add(new Vector2((strPos.x + i) * 50, (strPos.y + j) * 50));
                    mapUse[i].Add(panels[i][j].GetComponent<mapBlock>().selected);
                }
            }
        }
#endif
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// 게임 씬에서 캔디를 생성하고 리스트에 저장 및 위치 값 설정
    /// </summary>
    void LoadMap()
    {
        candyContainer = GameObject.Find("candys").transform;
        candyBlocks.Clear();
        candys.Clear();
        try
        {
            for (int i = 0; i < mapPos.Count; i++)
            {
                candyBlocks.Add(new List<GameObject>());
                candys.Add(new List<myCandy>());
                for (int j = 0; j < mapPos[i].Count; j++)
                {
                    candyBlocks[i].Add(Instantiate(candyBlock, new Vector3(mapPos[i][j].x, mapPos[i][j].y, 0), transform.rotation, candyContainer));
                    candyBlocks[i][j].SetActive(mapUse[i][j]);
                    candyBlocks[i][j].name = "candy" + i.ToString() + j.ToString();

                    //캔디블록은 window도 포함되어있는 빈 오브젝트이고 실제 캔디를 작동시키는 로직이 들어있는 myCandy스크립트의 리스트를 별도 저장
                    candys[i].Add(candyBlocks[i][j].transform.Find("candy").gameObject.GetComponent<myCandy>());
                    candys[i][j].x = i;
                    candys[i][j].y = j;
                }
            }
        }
        catch (System.NullReferenceException) { }
    }
}