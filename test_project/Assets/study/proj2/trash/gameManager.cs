using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour {

    /*
    public GameObject grid;
    [HideInInspector]
    public int touchCnt = 0;
    public Vector3 curPos, lastPos;
    public myCandy curCandy, lastCandy;


    private List<List<GameObject>> myCandys;
    private float time = 0;
    private List<int> movingCnt = new List<int>();

    public enum gameState { wait, bomb };
    public static gameManager Instance;

    public const string mainSceneName = "main";
    public const float timeHudle = 2.2f;

    /// <summary>
    /// singleton class manger.
    /// </summary>
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start ()
    {

        myCandys = receiveData.candys;
        //movingCnt = new int[myCandys.Count];
        grid.SetActive(false);
    }

    void bombCheck()
    {
        //cur,up,down,left,right 중에는 움직이는 애가 있으먼 암됨 -> 조건 추가해야
        myCandy up, down, left, right;
        //System.Array.Clear(movingCnt, 0, myCandys.Count);
        movingCnt.Clear();
        for (int i = 0; i < myCandys.Count; i++)
        {
            movingCnt.Add(0);
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                if (myCandys[i][j].GetComponent<myCandy>().isMoving) movingCnt[i] += 1;
            }
        }

        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                if (myCandys[i][j].activeInHierarchy && !myCandys[i][j].GetComponent<myCandy>().isChecked)
                {
                    myCandy cur = myCandys[i][j].GetComponent<myCandy>();
                    if (movingCnt[cur.x] == 0) //아마 isMoving보다 밑에 있는 놈은 터지지 않을 것이라 생각됨 //이 라인에 isMoving이 있으면 체크 못하는걸로
                    {
                        up = cur; down = cur; left = cur; right = cur;
                        for (int c = 0; c < 4; c++)
                        {
                            if (up.upCandy.y == up.y + 1 && up.upCandy.candyColor == up.candyColor && movingCnt[up.upCandy.x] == 0)
                            {
                                up = up.upCandy;
                            }
                            if (down.downCandy.y == down.y - 1 && down.downCandy.candyColor == down.candyColor && movingCnt[down.downCandy.x] == 0)
                            {
                                down = down.downCandy;
                            }
                            if (left.leftCandy.x == left.x - 1 && left.leftCandy.candyColor == left.candyColor && movingCnt[left.leftCandy.x] == 0)
                            {
                                left = left.leftCandy;
                            }
                            if (right.rightCandy.x == right.x + 1 && right.rightCandy.candyColor == right.candyColor && movingCnt[right.rightCandy.x] == 0)
                            {
                                right = right.rightCandy;
                            }
                        }
                        //체크 두개는 초기화해야되는지? -> 안해도될거같은데 해보고 검사
                        if (right.x - left.x >= 2)
                        {
                            cur.leftCheck = cur.x - left.x;
                            cur.rightCheck = right.x - cur.x;
                            for (int x = left.x; x <= right.x; x++)
                            {
                                myCandys[x][cur.y].GetComponent<myCandy>().isChecked = true;
                            }
                        }
                        if (up.y - down.y >= 2)
                        {
                            cur.upCheck = up.y - cur.y;
                            cur.upCheck = cur.y - down.y;
                            for (int y = down.y; y <= up.y; y++)
                            {
                                myCandys[cur.x][y].GetComponent<myCandy>().isChecked = true;
                            }
                        }
                    }
                }
            }
        }
        int checkNum = 0;
        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                if(myCandys[i][j].GetComponent<myCandy>().isChecked || myCandys[i][j].GetComponent<myCandy>().isMoving)
                {
                    checkNum += 1;
                }
            }
        }
        if (checkNum == 0)
        {
            for (int i = 0; i < myCandys.Count; i++)
            {
                for (int j = 0; j < myCandys[i].Count; j++)
                {
                    myCandy _myCandy = myCandys[i][j].GetComponent<myCandy>();
                    _myCandy.upCheck = 0;
                    _myCandy.downCheck = 0;
                    myCandys[i][j].GetComponent<myCandy>().leftCheck = 0;
                    myCandys[i][j].GetComponent<myCandy>().rightCheck = 0;
                }
            }
            candyCtrl.curState = gameState.wait;
        }
    }

    void resetLineCnt(myCandy cur)
    {

        for (int x = cur.x - cur.leftCheck; x <= cur.x + cur.rightCheck; x++)
        {
            myCandys[x][cur.y].GetComponent<myCandy>().upCheck = 0;
            myCandys[x][cur.y].GetComponent<myCandy>().downCheck = 0;
            myCandys[x][cur.y].GetComponent<myCandy>().leftCheck = 0;
            myCandys[x][cur.y].GetComponent<myCandy>().rightCheck = 0;
        }
        for (int y = cur.y - cur.downCheck; y <= cur.y + cur.upCheck; y++)
        {
            myCandys[cur.x][y].GetComponent<myCandy>().upCheck = 0;
            myCandys[cur.x][y].GetComponent<myCandy>().downCheck = 0;
            myCandys[cur.x][y].GetComponent<myCandy>().leftCheck = 0;
            myCandys[cur.x][y].GetComponent<myCandy>().rightCheck = 0;
        }
    }

    void candyBomb()
    {
        //if 둘중하나라도 4이상이면 초코생성
        //else if 둘다 2이상이면 폭탄생성
        //else if 들증 하나 3이상이면 즐생성

        //생성 조건도 하나하나 다 따져줘야된다......
        //생성은 터질때 화면 비우고 0.01f뒤 별도 함수 Invoke하던가

        //지금 문제상황
        //캔디 6개이상은 연속으로 못나오게
        //캔디 못움직이는 경우 체크
        //아이템 생성 위치와 중복 처리

        List<List<bool>> getChoco = new List<List<bool>>(), getBoom = new List<List<bool>>(); 
        List<List<bool>> getCol = new List<List<bool>>(), getRow = new List<List<bool>>();
        
        for (int i = 0; i < myCandys.Count; i++)
        {
            getChoco.Add(new List<bool>());
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                getChoco[i].Add(false);
                myCandy cur = myCandys[i][j].GetComponent<myCandy>();
                if ((cur.leftCheck == 2 && cur.rightCheck >= 2) || (cur.upCheck == 2 && cur.downCheck >= 2))
                {
                    getChoco[i][j] = true;
                    resetLineCnt(cur);
                }
            }
        }

        for (int i = 0; i < myCandys.Count; i++)
        {
            getBoom.Add(new List<bool>());
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                getBoom[i].Add(false);
                myCandy cur = myCandys[i][j].GetComponent<myCandy>();
                if(cur.leftCheck + cur.rightCheck >=2 && cur.upCheck + cur.downCheck >= 2)
                {
                    getBoom[i][j] = true;
                    resetLineCnt(cur);
                }
            }
        }

        for (int i = 0; i < myCandys.Count; i++)
        {
            getCol.Add(new List<bool>());
            getRow.Add(new List<bool>());
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                getCol[i].Add(false);
                getRow[i].Add(false);
                myCandy cur = myCandys[i][j].GetComponent<myCandy>();
                myCandy clickedCur = curCandy.GetComponent<myCandy>();
                myCandy clickedLast = lastCandy.GetComponent<myCandy>();
                if (cur.leftCheck + cur.rightCheck >= 3)// && cur.upCheck + cur.downCheck >= 2)
                {
                    if (cur.isClicked)
                    {
                        if (clickedCur.x == cur.x || clickedCur.y == cur.y)
                        {
                            Debug.Log("curcol");
                            getCol[clickedCur.x][clickedCur.y] = true;
                        }
                        else
                        {
                            Debug.Log("lastcol");
                            getCol[clickedLast.x][clickedLast.y] = true;
                        }
                    }
                    else
                    {
                        getCol[i][j] = true;
                    }
                    resetLineCnt(cur);
                }
                if (cur.upCheck + cur.downCheck >= 3)
                {
                    if (cur.isClicked)
                    {
                        if (clickedCur.x == cur.x || clickedCur.y == cur.y)
                        {
                            Debug.Log("currow");
                            getRow[clickedCur.x][clickedCur.y] = true;
                        }
                        else
                        {
                            Debug.Log("lastrow");
                            getRow[clickedLast.x][clickedLast.y] = true;
                        }
                    }
                    else
                    {
                        getRow[i][j] = true;
                    }
                }
            }
        }

        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                myCandy candyData = myCandys[i][j].GetComponent<myCandy>();
                if (candyData.isChecked)
                {
                    if(candyData.candyType == candyCtrl.types[1])
                    {
                        for(int x = 0; x < myCandys.Count; x++)
                        {
                            if(myCandys[x][candyData.y].activeInHierarchy && 
                                !myCandys[x][candyData.y].GetComponent<myCandy>().isChecked && !myCandys[x][candyData.y].GetComponent<myCandy>().isMoving)
                            {
                                myCandys[x][candyData.y].GetComponent<myCandy>().isChecked = true;
                            }
                        }
                    }
                    else if(candyData.candyType == candyCtrl.types[2])
                    {
                        for(int y = 0; y < myCandys[i].Count; y++)
                        {
                            if (myCandys[candyData.x][y].activeInHierarchy &&
                                !myCandys[candyData.x][y].GetComponent<myCandy>().isChecked && !myCandys[candyData.x][y].GetComponent<myCandy>().isMoving)
                            {
                                myCandys[candyData.x][y].GetComponent<myCandy>().isChecked = true;
                            }
                        }
                    }
                    else if (candyData.candyType == candyCtrl.types[3] || candyData.candyType == candyCtrl.types[4])
                    {
                        for(int x = candyData.x - 1; x <= candyData.x + 1; x++)
                        {
                            for(int y = candyData.y - 1; y <= candyData.y + 1; y++)
                            {
                                if (myCandys[x][y].activeInHierarchy &&
                                    !myCandys[x][y].GetComponent<myCandy>().isChecked && !myCandys[x][y].GetComponent<myCandy>().isMoving)
                                {
                                    myCandys[x][y].GetComponent<myCandy>().isChecked = true;
                                }
                            }
                        }
                    }
                    if (candyData.candyType == candyCtrl.types[3])
                    {
                        //make bonus!
                        candyData.candyType = candyCtrl.types[4];
                        StartCoroutine(bonusCandy(candyData));
                    }
                    //make item!
                    else if (getChoco[i][j])
                    {
                        candyData.candyType = candyCtrl.types[5];
                        candyData.candyColor = null;
                    }
                    else if (getBoom[i][j])
                    {
                        candyData.candyType = candyCtrl.types[3];
                    }
                    else if (getCol[i][j])
                    {
                        candyData.candyType = candyCtrl.types[2];
                    }
                    else if (getRow[i][j])
                    {
                        candyData.candyType = candyCtrl.types[1];
                    }
                    else
                    {
                        candyData.candyType = null;
                        candyData.isMoving = true;
                        movingCnt[candyData.x] += 1;
                    }
                    myCandys[i][j].GetComponent<UISprite>().spriteName = candyData.candyColor + "_" + candyData.candyType;
                    candyData.isChecked = false;
                    candyData.isClicked = false;
                }         
            }
        }
        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                myCandys[i][j].GetComponent<myCandy>().upCheck = 0;
                myCandys[i][j].GetComponent<myCandy>().downCheck = 0;
                myCandys[i][j].GetComponent<myCandy>().leftCheck = 0;
                myCandys[i][j].GetComponent<myCandy>().rightCheck = 0;
            }
        }
    }

    IEnumerator bonusCandy(myCandy bonus)
    {
        yield return new WaitForSeconds(1f);
        bonus.isChecked = true;
    }

    void slideMove()
    {
        for (int i = 0; i < myCandys.Count; i++)
        {
            if (movingCnt[i] != 0) {
                myCandy top = myCandys[i][0].GetComponent<myCandy>(), bottom = myCandys[i][myCandys[i].Count - 1].GetComponent<myCandy>();
                for (int j = 0; j < myCandys[i].Count; j++)
                {
                    if (myCandys[i][j].activeInHierarchy && myCandys[i][j].GetComponent<myCandy>().isMoving)
                    {
                        if (myCandys[i][j].GetComponent<myCandy>().y > top.y)
                        {
                            top = myCandys[i][j].GetComponent<myCandy>();
                        }
                        if (myCandys[i][j].GetComponent<myCandy>().y < bottom.y)
                        {
                            bottom = myCandys[i][j].GetComponent<myCandy>();
                        }
                    }
                }
                try
                {
                    while (true)
                    {
                        if (bottom.upCandy.y == bottom.y)
                        {
                            bottom.candyColor = candyCtrl.colors[Random.Range(0, candyCtrl.colors.Length)];
                            bottom.candyType = candyCtrl.types[0];
                            bottom.gameObject.GetComponent<UISprite>().spriteName = bottom.candyColor + "_" + bottom.candyType;
                            break;
                        }
                        bottom.candyColor = bottom.upCandy.candyColor;
                        bottom.candyType = bottom.upCandy.candyType;
                        bottom.gameObject.GetComponent<UISprite>().spriteName = bottom.candyColor + "_" + bottom.candyType;
                        bottom = bottom.upCandy;
                    }
                    top.isMoving = false;
                    top = top.downCandy;
                }
                catch (System.NullReferenceException) { }
            }
        }
    }

    void Update()
    {
        time += Time.deltaTime; 
        if (candyCtrl.curState == gameState.bomb)
        {
            if (time > timeHudle)
            {
                bombCheck();
                candyBomb();
                slideMove();
                time = 2f;
            }
        }
    }

    //버튼 클릭시 메인신으로 이동
    //mapSaver는 누적되므로 삭제한다
    public void backToMain()
    {
        Destroy(GameObject.Find("mapSaver"));
        //Application.LoadLevel("main");
        SceneManager.LoadScene(mainSceneName);
    }
    */
}
