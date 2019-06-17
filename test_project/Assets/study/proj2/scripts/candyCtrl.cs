using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class candyCtrl : MonoBehaviour {

    private float time = 0, waitSwapTime = .2f, waitBombTime = .5f, waitSlideTime = .7f, waitResetTime = .7f;//, initWaitTime = 0.1f;
    private List<int> movingCnt = new List<int>();
    private List<myCandy> canMoveCandy = new List<myCandy>();
    private List<candyColors> exceptNoneColors = new List<candyColors>();

    private static int boomCnt;
    private static List<List<int>> getChoco = new List<List<int>>(), getBoom = new List<List<int>>();
    private static List<List<int>> getCol = new List<List<int>>(), getRow = new List<List<int>>();

    public enum gameState { wait, bomb };
    public enum candyColors { none, red, orange, yellow, green, blue, purple };
    public enum candyTypes { none, normal, row, col, boom, bonus, choco };

    public GameObject hintEffect;

    public static List<List<myCandy>> myCandys;
    public static gameState curState = gameState.wait;

    void Start()
    {
        myCandys = manager.candys;

        for(int i = 0; i < myCandys.Count; i++)
        {
            for(int j = 0; j < myCandys[i].Count; j++)
            {
                myCandys[i][j].SetDirect();
            }
        }

        foreach (candyColors curColor in Enum.GetValues(typeof(candyColors)))
        {
            if (curColor != candyColors.none)
            {
                exceptNoneColors.Add(curColor);
            }
        }

        InitCandy();
    }

    /// <summary>
    /// 게임 시작 시 캔디 초기화
    /// </summary>
    void InitCandy()
    {
        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                //활성화 되있는경우만 표시
                if (myCandys[i][j].gameObject.activeInHierarchy)
                {
                    //초기값은 항상 노멀캔디에 색깔은 랜덤
                    myCandys[i][j].myType = candyTypes.normal;
                    myCandys[i][j].myColor = exceptNoneColors[UnityEngine.Random.Range(0, exceptNoneColors.Count)];

                    //3연속 색깔 중복 시 예외처리를 위한 colorCopy
                    List<candyColors> colorCopy = new List<candyColors>();
                    foreach (candyColors curColor in exceptNoneColors)
                    {
                        colorCopy.Add(curColor);
                    }

                    //아래, 왼쪽 2칸 전 캔디부터 캔디 색이 중복이면
                    //colorCopy에서 색을 제외시켜 다른 색을 출력하도록 한다 
                    if (myCandys[i][j].leftCandy.leftCandy.x == myCandys[i][j].x - 2 && myCandys[i][j].leftCandy.myColor == myCandys[i][j].leftCandy.leftCandy.myColor)
                    {
                        colorCopy.Remove(myCandys[i][j].leftCandy.myColor);
                        myCandys[i][j].myColor = colorCopy[UnityEngine.Random.Range(0, colorCopy.Count)];
                    }
                    if (myCandys[i][j].downCandy.downCandy.y == myCandys[i][j].y - 2 && myCandys[i][j].downCandy.myColor == myCandys[i][j].downCandy.downCandy.myColor)
                    {
                        colorCopy.Remove(myCandys[i][j].downCandy.myColor);
                        myCandys[i][j].myColor = colorCopy[UnityEngine.Random.Range(0, colorCopy.Count)];
                    }
                    /*
                    if (i == 4 && j == 5)
                    {
                        myCandys[i][j].myColor = candyColors.green;
                        myCandys[i][j].myType = candyTypes.col;
                    }
                    if (i == 5 && j == 5)
                    {
                        myCandys[i][j].myColor = candyColors.blue;
                        myCandys[i][j].myType = candyTypes.boom;
                    }
                    if (i == 6 && j == 5)
                    {
                        myCandys[i][j].myColor = candyColors.red;
                        myCandys[i][j].myType = candyTypes.col;
                    }
                    */
                    if (i == 4 && j == 0)
                    {
                        myCandys[i][j].myColor = candyColors.green;
                        myCandys[i][j].myType = candyTypes.boom;
                    }
                    if (i == 7 && j == 3)
                    {
                        myCandys[i][j].myColor = candyColors.yellow;
                        myCandys[i][j].myType = candyTypes.col;
                    }
                    if (i == 0 && j == 6)
                    {
                        myCandys[i][j].myColor = candyColors.none;
                        myCandys[i][j].myType = candyTypes.choco;
                    }
                    if (i == 8 && j == 5)
                    {
                        myCandys[i][j].myColor = candyColors.red;
                        myCandys[i][j].myType = candyTypes.boom;
                    }
                    if (i == 2 && j == 5)
                    {
                        myCandys[i][j].myColor = candyColors.purple;
                        myCandys[i][j].myType = candyTypes.row;
                    }

                    //스프라이트 콤포넌트를 새로 더해주는 이유는 스프라이트를 프리팹이 미리 가지고있으면 값이 바뀌지 않는듯해서 추가
                    myCandys[i][j].gameObject.AddComponent<UISprite>();
                    UISprite candySprite = myCandys[i][j].gameObject.GetComponent<UISprite>();

                    candySprite.SetDimensions(40, 40);
                    candySprite.depth = 5;
                    //아틀라스는 비활성화 되어있는 grid에서 가져오고 type, color를 통해 스프라이트 결정
                    candySprite.atlas = manager.grid.GetComponent<UISprite>().atlas;
                    candySprite.spriteName = myCandys[i][j].myColor.ToString() + "_" + myCandys[i][j].myType.ToString();
                }
            }
        }
        getBombReady();
    }

    /// <summary>
    /// 2개 기준으로 하나 옮길 시를 파악해서 움직인 가능한 캔디를 찾는 로직
    /// </summary>
    void getBombReady()
    {
        int tempUp, tempDown, tempLeft, tempRight;
        canMoveCandy.Clear();
        //각 포지션 기준 값 초기화
        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                if (myCandys[i][j].gameObject.activeInHierarchy)
                {
                    myCandys[i][j].upCol.Clear(); myCandys[i][j].upRow.Clear(); myCandys[i][j].upCnt = 0;
                    myCandys[i][j].downCol.Clear(); myCandys[i][j].downRow.Clear(); myCandys[i][j].downCnt = 0;
                    myCandys[i][j].leftCol.Clear(); myCandys[i][j].leftRow.Clear(); myCandys[i][j].leftCnt = 0;
                    myCandys[i][j].rightCol.Clear(); myCandys[i][j].rightRow.Clear(); myCandys[i][j].rightCnt = 0;

                    myCandys[i][j].upCol.Add(myCandys[i][j]); myCandys[i][j].upRow.Add(myCandys[i][j]);
                    myCandys[i][j].downCol.Add(myCandys[i][j]); myCandys[i][j].downRow.Add(myCandys[i][j]);
                    myCandys[i][j].leftCol.Add(myCandys[i][j]); myCandys[i][j].leftRow.Add(myCandys[i][j]);
                    myCandys[i][j].rightCol.Add(myCandys[i][j]); myCandys[i][j].rightRow.Add(myCandys[i][j]);
                }
            }
        }

        //반복문을 돌면서 2연속이거나 한칸 띄고 연속인 캔디를 찾는다
        //초코나 아이템 캔디는 두칸 인접하고 있으면 가능
        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                if (i > 0 && myCandys[i][j].gameObject.activeInHierarchy && myCandys[i - 1][j].gameObject.activeInHierarchy)
                {
                    if (myCandys[i - 1][j].myType == candyTypes.choco || myCandys[i][j].myType == candyTypes.choco)
                    {
                        myCandys[i - 1][j].rightCnt = (int)myCandys[i][j].myType;
                        myCandys[i][j].leftCnt = (int)myCandys[i - 1][j].myType;
                    }
                    else if (myCandys[i - 1][j].myType != candyTypes.normal && myCandys[i][j].myType != candyTypes.normal)
                    {
                        myCandys[i - 1][j].rightCnt = (int)myCandys[i][j].myType;
                        myCandys[i][j].leftCnt = (int)myCandys[i - 1][j].myType;
                    }
                    else if (myCandys[i][j].myColor == myCandys[i - 1][j].myColor)
                    {
                        if (i > 1 && myCandys[i - 2][j].gameObject.activeInHierarchy)
                        {
                            if (i > 2 && myCandys[i - 3][j].myColor == myCandys[i][j].myColor)
                            {
                                myCandys[i - 3][j].rightRow.Add(myCandys[i - 1][j]);
                                myCandys[i - 3][j].rightRow.Add(myCandys[i][j]);
                            }
                            if (i > 1)
                            {
                                if (j > 0 && myCandys[i - 2][j - 1].myColor == myCandys[i][j].myColor)
                                {
                                    myCandys[i - 2][j - 1].upRow.Add(myCandys[i - 1][j]);
                                    myCandys[i - 2][j - 1].upRow.Add(myCandys[i][j]);
                                }
                                if (j < myCandys[i].Count - 1 && myCandys[i - 2][j + 1].myColor == myCandys[i][j].myColor)
                                {
                                    myCandys[i - 2][j + 1].downRow.Add(myCandys[i - 1][j]);
                                    myCandys[i - 2][j + 1].downRow.Add(myCandys[i][j]);
                                }
                            }
                        }
                        if (i < myCandys.Count - 1 && myCandys[i + 1][j].gameObject.activeInHierarchy)
                        {
                            if (i < myCandys.Count - 2 && myCandys[i + 2][j].myColor == myCandys[i][j].myColor)
                            {
                                myCandys[i + 2][j].leftRow.Add(myCandys[i - 1][j]);
                                myCandys[i + 2][j].leftRow.Add(myCandys[i][j]);
                            }
                            if (i < myCandys.Count - 1)
                            {
                                if (j > 0 && myCandys[i + 1][j - 1].myColor == myCandys[i][j].myColor)
                                {
                                    myCandys[i + 1][j - 1].upRow.Add(myCandys[i - 1][j]);
                                    myCandys[i + 1][j - 1].upRow.Add(myCandys[i][j]);
                                }
                                if (j < myCandys[i].Count - 1 && myCandys[i + 1][j + 1].myColor == myCandys[i][j].myColor)
                                {
                                    myCandys[i + 1][j + 1].downRow.Add(myCandys[i - 1][j]);
                                    myCandys[i + 1][j + 1].downRow.Add(myCandys[i][j]);
                                }
                            }
                        }
                    }
                }
                if (j > 0 && myCandys[i][j].gameObject.activeInHierarchy && myCandys[i][j - 1].gameObject.activeInHierarchy)
                {
                    if (myCandys[i][j - 1].myType == candyTypes.choco || myCandys[i][j].myType == candyTypes.choco)
                    {
                        myCandys[i][j - 1].upCnt = (int)myCandys[i][j].myType;
                        myCandys[i][j].downCnt = (int)myCandys[i][j - 1].myType;
                    }
                    else if (myCandys[i][j - 1].myType != candyTypes.normal && myCandys[i][j].myType != candyTypes.normal)
                    {
                        myCandys[i][j - 1].upCnt = (int)myCandys[i][j].myType;
                        myCandys[i][j].downCnt = (int)myCandys[i][j - 1].myType;
                    }
                    else if (myCandys[i][j].myColor == myCandys[i][j - 1].myColor)
                    {
                        if (j > 1 && myCandys[i][j - 2].gameObject.activeInHierarchy)
                        {
                            if (j > 2 && myCandys[i][j - 3].myColor == myCandys[i][j].myColor)
                            {
                                myCandys[i][j - 3].upCol.Add(myCandys[i][j - 1]);
                                myCandys[i][j - 3].upCol.Add(myCandys[i][j]);
                            }
                            if (j > 1)
                            {
                                if (i > 0 && myCandys[i - 1][j - 2].myColor == myCandys[i][j].myColor)
                                {
                                    myCandys[i - 1][j - 2].rightCol.Add(myCandys[i][j - 1]);
                                    myCandys[i - 1][j - 2].rightCol.Add(myCandys[i][j]);
                                }
                                if (i < myCandys.Count - 1 && myCandys[i + 1][j - 2].myColor == myCandys[i][j].myColor)
                                {
                                    myCandys[i + 1][j - 2].leftCol.Add(myCandys[i][j - 1]);
                                    myCandys[i + 1][j - 2].leftCol.Add(myCandys[i][j]);
                                }
                            }
                        }
                        if (j < myCandys[i].Count - 1 && myCandys[i][j + 1].gameObject.activeInHierarchy)
                        {
                            if (j < myCandys[i].Count - 2 && myCandys[i][j + 2].myColor == myCandys[i][j].myColor)
                            {
                                myCandys[i][j + 2].downCol.Add(myCandys[i][j - 1]);
                                myCandys[i][j + 2].downCol.Add(myCandys[i][j]);
                            }
                            if (j < myCandys[i].Count - 1)
                            {
                                if (i > 0 && myCandys[i - 1][j + 1].myColor == myCandys[i][j].myColor)
                                {
                                    myCandys[i - 1][j + 1].rightCol.Add(myCandys[i][j - 1]);
                                    myCandys[i - 1][j + 1].rightCol.Add(myCandys[i][j]);
                                }
                                if (i < myCandys.Count - 1 && myCandys[i + 1][j + 1].myColor == myCandys[i][j].myColor)
                                {
                                    myCandys[i + 1][j + 1].leftCol.Add(myCandys[i][j - 1]);
                                    myCandys[i + 1][j + 1].leftCol.Add(myCandys[i][j]);
                                }
                            }
                        }
                    }
                }
                if (i > 1 && myCandys[i][j].gameObject.activeInHierarchy && myCandys[i][j].myColor != candyColors.none && myCandys[i - 2][j].myColor == myCandys[i][j].myColor)
                {
                    if (myCandys[i - 1][j].gameObject.activeInHierarchy)
                    {
                        if (j > 0 && myCandys[i - 1][j - 1].myColor == myCandys[i][j].myColor)
                        {
                            myCandys[i - 1][j - 1].upRow.Add(myCandys[i - 2][j]);
                            myCandys[i - 1][j - 1].upRow.Add(myCandys[i][j]);
                        }
                        if (j < myCandys[i].Count - 1 && myCandys[i - 1][j + 1].myColor == myCandys[i][j].myColor)
                        {
                            myCandys[i - 1][j + 1].downRow.Add(myCandys[i - 2][j]);
                            myCandys[i - 1][j + 1].downRow.Add(myCandys[i][j]);
                        }
                    }
                }
                if (j > 1 && myCandys[i][j].gameObject.activeInHierarchy && myCandys[i][j].myColor != candyColors.none && myCandys[i][j - 2].myColor == myCandys[i][j].myColor)
                {
                    if (myCandys[i][j - 1].gameObject.activeInHierarchy)
                    {
                        if (i > 0 && myCandys[i - 1][j - 1].myColor == myCandys[i][j].myColor)
                        {
                            myCandys[i - 1][j - 1].rightCol.Add(myCandys[i][j - 2]);
                            myCandys[i - 1][j - 1].rightCol.Add(myCandys[i][j]);
                        }
                        if (i < myCandys.Count - 1 && myCandys[i + 1][j - 1].myColor == myCandys[i][j].myColor)
                        {
                            myCandys[i + 1][j - 1].leftCol.Add(myCandys[i][j - 2]);
                            myCandys[i + 1][j - 1].leftCol.Add(myCandys[i][j]);
                        }
                    }
                }
            }
        }

        //중복을 제거하고 3매치 안되는건 따로 처리하기 위해 초기, 이후 움직인 가능한 캔디 리스트에 추가
        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                if (myCandys[i][j].gameObject.activeInHierarchy)
                {
                    myCandys[i][j].upCol = new List<myCandy>(new HashSet<myCandy>(myCandys[i][j].upCol));
                    myCandys[i][j].upRow = new List<myCandy>(new HashSet<myCandy>(myCandys[i][j].upRow));
                    myCandys[i][j].downCol = new List<myCandy>(new HashSet<myCandy>(myCandys[i][j].downCol));
                    myCandys[i][j].downRow = new List<myCandy>(new HashSet<myCandy>(myCandys[i][j].downRow));
                    myCandys[i][j].leftCol = new List<myCandy>(new HashSet<myCandy>(myCandys[i][j].leftCol));
                    myCandys[i][j].leftRow = new List<myCandy>(new HashSet<myCandy>(myCandys[i][j].leftRow));
                    myCandys[i][j].rightCol = new List<myCandy>(new HashSet<myCandy>(myCandys[i][j].rightCol));
                    myCandys[i][j].rightRow = new List<myCandy>(new HashSet<myCandy>(myCandys[i][j].rightRow));

                    if (myCandys[i][j].upCol.Count < 3) myCandys[i][j].upCol.Clear();
                    if (myCandys[i][j].upRow.Count < 3) myCandys[i][j].upRow.Clear();
                    if (myCandys[i][j].downCol.Count < 3) myCandys[i][j].downCol.Clear();
                    if (myCandys[i][j].downRow.Count < 3) myCandys[i][j].downRow.Clear();
                    if (myCandys[i][j].leftCol.Count < 3) myCandys[i][j].leftCol.Clear();
                    if (myCandys[i][j].leftRow.Count < 3) myCandys[i][j].leftRow.Clear();
                    if (myCandys[i][j].rightCol.Count < 3) myCandys[i][j].rightCol.Clear();
                    if (myCandys[i][j].rightRow.Count < 3) myCandys[i][j].rightRow.Clear();

                    tempUp = myCandys[i][j].upCnt; tempDown = myCandys[i][j].downCnt; tempLeft = myCandys[i][j].leftCnt; tempRight = myCandys[i][j].rightCnt;
                    //3매치인 경우 두 캔디의 매치 카운트를 모두 더해준다
                    if (myCandys[i][j].upCnt == 0)
                    {
                        myCandys[i][j].upCnt = myCandys[i][j].upCol.Count + myCandys[i][j].upRow.Count;
                        if (j < myCandys[i].Count - 1) tempUp = myCandys[i][j].upCnt + myCandys[i][j + 1].downCol.Count + myCandys[i][j + 1].downRow.Count;
                    }
                    if (myCandys[i][j].downCnt == 0)
                    {
                        myCandys[i][j].downCnt = myCandys[i][j].downCol.Count + myCandys[i][j].downRow.Count;
                        if (j > 0) tempDown = myCandys[i][j].downCnt + myCandys[i][j - 1].upCol.Count + myCandys[i][j - 1].upRow.Count;
                    }
                    if (myCandys[i][j].leftCnt == 0)
                    {
                        myCandys[i][j].leftCnt = myCandys[i][j].leftCol.Count + myCandys[i][j].leftRow.Count;
                        if (i > 0) tempLeft = myCandys[i][j].leftCnt + myCandys[i - 1][j].rightCol.Count + myCandys[i - 1][j].rightRow.Count;
                    }
                    if (myCandys[i][j].rightCnt == 0)
                    {
                        myCandys[i][j].rightCnt = myCandys[i][j].rightCol.Count + myCandys[i][j].rightRow.Count;
                        if (i < myCandys.Count - 1) tempRight = myCandys[i][j].rightCnt + myCandys[i + 1][j].leftCol.Count + myCandys[i + 1][j].leftRow.Count;
                    }

                    if (Mathf.Max(tempUp, tempDown, tempLeft, tempRight) == tempUp)
                    {
                        myCandys[i][j].choiceDir = myCandy.moveDir.up;
                    }
                    else if (Mathf.Max(tempUp, tempDown, tempLeft, tempRight) == tempRight)
                    {
                        myCandys[i][j].choiceDir = myCandy.moveDir.right;
                    }
                    else if (Mathf.Max(tempUp, tempDown, tempLeft, tempRight) == tempDown)
                    {
                        myCandys[i][j].choiceDir = myCandy.moveDir.down;
                    }
                    else if (Mathf.Max(tempUp, tempDown, tempLeft, tempRight) == tempLeft)
                    {
                        myCandys[i][j].choiceDir = myCandy.moveDir.left;
                    }

                    if (Mathf.Max(myCandys[i][j].upCnt, myCandys[i][j].downCnt, myCandys[i][j].leftCnt, myCandys[i][j].rightCnt) != 0)
                    {
                        canMoveCandy.Add(myCandys[i][j]);
                    }
                }
            }
        }
        StartCoroutine(FindCantMove());
    }

    /// <summary>
    /// 한 블록 기준 자신의 움직임에 따라 매치가 되는 캔디를 찾는 로직
    /// </summary>
    void GetBombReady()
    {
        int tempUp, tempDown, tempLeft, tempRight;
        canMoveCandy.Clear();
        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                if (myCandys[i][j].gameObject.activeInHierarchy)
                {
                    myCandys[i][j].upCol.Clear(); myCandys[i][j].upRow.Clear(); myCandys[i][j].upCnt = 0;
                    myCandys[i][j].downCol.Clear(); myCandys[i][j].downRow.Clear(); myCandys[i][j].downCnt = 0;
                    myCandys[i][j].leftCol.Clear(); myCandys[i][j].leftRow.Clear(); myCandys[i][j].leftCnt = 0;
                    myCandys[i][j].rightCol.Clear(); myCandys[i][j].rightRow.Clear(); myCandys[i][j].rightCnt = 0;

                    if (myCandys[i][j].myType == candyTypes.choco) //초코볼 캔디의 경우 무조건 이동 가능
                    {
                        if (i > 0 && myCandys[i - 1][j].gameObject.activeInHierarchy) myCandys[i][j].leftCnt = (int)myCandys[i - 1][j].myType;
                        if (i < myCandys.Count - 1 && myCandys[i + 1][j].gameObject.activeInHierarchy) myCandys[i][j].rightCnt = (int)myCandys[i + 1][j].myType;
                        if (j > 0 && myCandys[i][j - 1].gameObject.activeInHierarchy) myCandys[i][j].downCnt = (int)myCandys[i][j - 1].myType;
                        if (j < myCandys[i].Count - 1 && myCandys[i][j + 1].gameObject.activeInHierarchy) myCandys[i][j].upCnt = (int)myCandys[i][j + 1].myType;
                    }
                    else if (myCandys[i][j].myType != candyTypes.normal) //초코 외 아이템 캔디의 경우 인접한 아이템 캔디가 있으면 이동 가능이고 그 외는 3매치 체크
                    {
                        if (i > 0 && myCandys[i - 1][j].gameObject.activeInHierarchy && myCandys[i - 1][j].myType != candyTypes.normal)
                        {
                            myCandys[i][j].leftCnt = (int)myCandys[i - 1][j].myType;
                        }
                        if (i < myCandys.Count - 1 && myCandys[i + 1][j].gameObject.activeInHierarchy && myCandys[i + 1][j].myType != candyTypes.normal)
                        {
                            myCandys[i][j].rightCnt = (int)myCandys[i + 1][j].myType;
                        }
                        if (j > 0 && myCandys[i][j - 1].gameObject.activeInHierarchy && myCandys[i][j - 1].myType != candyTypes.normal)
                        {
                            myCandys[i][j].downCnt = (int)myCandys[i][j - 1].myType;
                        }
                        if (j < myCandys[i].Count - 1 && myCandys[i][j + 1].gameObject.activeInHierarchy && myCandys[i][j + 1].myType != candyTypes.normal)
                        {
                            myCandys[i][j].upCnt = (int)myCandys[i][j + 1].myType;
                        }

                        //인접한 아이템 캔디가 없는 경우
                        if (myCandys[i][j].upCnt + myCandys[i][j].downCnt + myCandys[i][j].leftCnt + myCandys[i][j].rightCnt == 0) ThreeMatchMove(myCandys[i][j]);
                    }
                    else if (myCandys[i][j].myColor != candyColors.none) //노멀 캔디의 경우 초코볼이 인접하면 이동 가능이고 그 외는 3매치 체크
                    {
                        if (i > 0 && myCandys[i - 1][j].gameObject.activeInHierarchy && myCandys[i - 1][j].myType == candyTypes.choco)
                        {
                            myCandys[i][j].leftCnt = (int)myCandys[i - 1][j].myType;
                        }
                        if (i < myCandys.Count - 1 && myCandys[i + 1][j].gameObject.activeInHierarchy && myCandys[i + 1][j].myType == candyTypes.choco)
                        {
                            myCandys[i][j].rightCnt = (int)myCandys[i + 1][j].myType;
                        }
                        if (j > 0 && myCandys[i][j - 1].gameObject.activeInHierarchy && myCandys[i][j - 1].myType == candyTypes.choco)
                        {
                            myCandys[i][j].downCnt = (int)myCandys[i][j - 1].myType;
                        }
                        if (j < myCandys[i].Count - 1 && myCandys[i][j + 1].gameObject.activeInHierarchy && myCandys[i][j + 1].myType == candyTypes.choco)
                        {
                            myCandys[i][j].upCnt = (int)myCandys[i][j + 1].myType;
                        }

                        //인접한 초코캔디가 없는 경우
                        if (myCandys[i][j].upCnt + myCandys[i][j].downCnt + myCandys[i][j].leftCnt + myCandys[i][j].rightCnt == 0) ThreeMatchMove(myCandys[i][j]);
                    }
                }
            }
        }

        //3매치인 경우 움직이는 두 캔디의 3매치를 다 고려해서 점수를 매겨야 하므로 더해준다
        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                tempUp = myCandys[i][j].upCnt; tempDown = myCandys[i][j].downCnt;
                tempLeft = myCandys[i][j].leftCnt; tempRight = myCandys[i][j].rightCnt;

                if (j < myCandys[i].Count - 1 && 
                    (myCandys[i][j].myType != candyTypes.choco && myCandys[i][j + 1].myType != candyTypes.choco) &&
                    (myCandys[i][j].myType == candyTypes.normal || myCandys[i][j + 1].myType == candyTypes.normal))
                {
                    tempUp = myCandys[i][j].upCnt + myCandys[i][j + 1].downCnt;
                }
                if (j > 0 &&
                    (myCandys[i][j].myType != candyTypes.choco && myCandys[i][j - 1].myType != candyTypes.choco) &&
                    (myCandys[i][j].myType == candyTypes.normal || myCandys[i][j - 1].myType == candyTypes.normal))
                {
                    tempDown = myCandys[i][j].downCnt + myCandys[i][j - 1].upCnt;
                }
                if (i > 0 &&
                    (myCandys[i][j].myType != candyTypes.choco && myCandys[i - 1][j].myType != candyTypes.choco) &&
                    (myCandys[i][j].myType == candyTypes.normal || myCandys[i - 1][j].myType == candyTypes.normal))
                {
                    tempLeft = myCandys[i][j].leftCnt + myCandys[i - 1][j].rightCnt;
                }
                if (i < myCandys.Count - 1 &&
                    (myCandys[i][j].myType != candyTypes.choco && myCandys[i + 1][j].myType != candyTypes.choco) &&
                    (myCandys[i][j].myType == candyTypes.normal || myCandys[i + 1][j].myType == candyTypes.normal))
                {
                    tempRight = myCandys[i][j].rightCnt + myCandys[i + 1][j].leftCnt;
                }

                if (Mathf.Max(tempUp, tempDown, tempLeft, tempRight) == tempUp)
                {
                    myCandys[i][j].choiceDir = myCandy.moveDir.up;
                }
                else if (Mathf.Max(tempUp, tempDown, tempLeft, tempRight) == tempRight)
                {
                    myCandys[i][j].choiceDir = myCandy.moveDir.right;
                }
                else if (Mathf.Max(tempUp, tempDown, tempLeft, tempRight) == tempDown)
                {
                    myCandys[i][j].choiceDir = myCandy.moveDir.down;
                }
                else if (Mathf.Max(tempUp, tempDown, tempLeft, tempRight) == tempLeft)
                {
                    myCandys[i][j].choiceDir = myCandy.moveDir.left;
                }

                if (Mathf.Max(myCandys[i][j].upCnt, myCandys[i][j].downCnt, myCandys[i][j].leftCnt, myCandys[i][j].rightCnt) != 0)
                {
                    canMoveCandy.Add(myCandys[i][j]);
                }
            }
        }
        StartCoroutine(FindCantMove());
    }

    void ThreeMatchMove(myCandy curCandy)
    {
        int x = curCandy.x, y = curCandy.y;

        curCandy.upCol.Add(curCandy); curCandy.upRow.Add(curCandy);
        curCandy.downCol.Add(curCandy); curCandy.downRow.Add(curCandy);
        curCandy.leftCol.Add(curCandy); curCandy.leftRow.Add(curCandy);
        curCandy.rightCol.Add(curCandy); curCandy.rightRow.Add(curCandy);

        if (x < myCandys.Count - 1 && myCandys[x + 1][y].gameObject.activeInHierarchy)
        {
            if (y < myCandys[x].Count - 1 && myCandys[x + 1][y + 1].myColor == curCandy.myColor)
            {
                curCandy.rightCol.Add(myCandys[x + 1][y + 1]);
                if (y < myCandys[x].Count - 2 && myCandys[x + 1][y + 2].myColor == curCandy.myColor)
                {
                    curCandy.rightCol.Add(myCandys[x + 1][y + 2]);
                }
            }
            if (y > 0 && myCandys[x + 1][y - 1].myColor == curCandy.myColor)
            {
                curCandy.rightCol.Add(myCandys[x + 1][y - 1]);
                if (y > 1 && myCandys[x + 1][y - 2].myColor == curCandy.myColor)
                {
                    curCandy.rightCol.Add(myCandys[x + 1][y - 2]);
                }
            }
            if (x < myCandys.Count - 2 && myCandys[x + 2][y].myColor == curCandy.myColor)
            {
                curCandy.rightRow.Add(myCandys[x + 2][y]);
                if (x < myCandys.Count - 3 && myCandys[x + 3][y].myColor == curCandy.myColor)
                {
                    curCandy.rightRow.Add(myCandys[x + 3][y]);
                }
            }
        }

        if (x > 0 && myCandys[x - 1][y].gameObject.activeInHierarchy)
        {
            if (y < myCandys[x].Count - 1 && myCandys[x - 1][y + 1].myColor == curCandy.myColor)
            {
                curCandy.leftCol.Add(myCandys[x - 1][y + 1]);
                if (y < myCandys[x].Count - 2 && myCandys[x - 1][y + 2].myColor == curCandy.myColor)
                {
                    curCandy.leftCol.Add(myCandys[x - 1][y + 2]);
                }
            }
            if (y > 0 && myCandys[x - 1][y - 1].myColor == curCandy.myColor)
            {
                curCandy.leftCol.Add(myCandys[x - 1][y - 1]);
                if (y > 1 && myCandys[x - 1][y - 2].myColor == curCandy.myColor)
                {
                    curCandy.leftCol.Add(myCandys[x - 1][y - 2]);
                }
            }
            if (x > 1 && myCandys[x - 2][y].myColor == curCandy.myColor)
            {
                curCandy.leftRow.Add(myCandys[x - 2][y]);
                if (x > 2 && myCandys[x - 3][y].myColor == curCandy.myColor)
                {
                    curCandy.leftRow.Add(myCandys[x - 3][y]);
                }
            }
        }

        if (y < myCandys[x].Count - 1 && myCandys[x][y + 1].gameObject.activeInHierarchy)
        {
            if (x < myCandys.Count - 1 && myCandys[x + 1][y + 1].myColor == curCandy.myColor)
            {
                curCandy.upRow.Add(myCandys[x + 1][y + 1]);
                if (x < myCandys.Count - 2 && myCandys[x + 2][y + 1].myColor == curCandy.myColor)
                {
                    curCandy.upRow.Add(myCandys[x + 2][y + 1]);
                }
            }
            if (x > 0 && myCandys[x - 1][y + 1].myColor == curCandy.myColor)
            {
                curCandy.upRow.Add(myCandys[x - 1][y + 1]);
                if (x > 1 && myCandys[x - 2][y + 1].myColor == curCandy.myColor)
                {
                    curCandy.upRow.Add(myCandys[x - 2][y + 1]);
                }
            }
            if (y < myCandys[x].Count - 2 && myCandys[x][y + 2].myColor == curCandy.myColor)
            {
                curCandy.upCol.Add(myCandys[x][y + 2]);
                if (y < myCandys[x].Count - 3 && myCandys[x][y + 3].myColor == curCandy.myColor)
                {
                    curCandy.upCol.Add(myCandys[x][y + 3]);
                }
            }
        }

        if (y > 0 && myCandys[x][y - 1].gameObject.activeInHierarchy)
        {
            if (x < myCandys.Count - 1 && myCandys[x + 1][y - 1].myColor == curCandy.myColor)
            {
                curCandy.downRow.Add(myCandys[x + 1][y - 1]);
                if (x < myCandys.Count - 2 && myCandys[x + 2][y - 1].myColor == curCandy.myColor)
                {
                    curCandy.downRow.Add(myCandys[x + 2][y - 1]);
                }
            }
            if (x > 0 && myCandys[x - 1][y - 1].myColor == curCandy.myColor)
            {
                curCandy.downRow.Add(myCandys[x - 1][y - 1]);
                if (x > 1 && myCandys[x - 2][y - 1].myColor == curCandy.myColor)
                {
                    curCandy.downRow.Add(myCandys[x - 2][y - 1]);
                }
            }
            if (y > 1 && myCandys[x][y - 2].myColor == curCandy.myColor)
            {
                curCandy.downCol.Add(myCandys[x][y - 2]);
                if (y > 2 && myCandys[x][y - 3].myColor == curCandy.myColor)
                {
                    curCandy.downCol.Add(myCandys[x][y - 3]);
                }
            }
        }

        //3매치 아닌 것들은 초기화 -> 힌트 찾을때 따로 분류를 위해 리스트를 아얘 비워준다
        if (curCandy.upCol.Count < 3) curCandy.upCol.Clear();
        if (curCandy.upRow.Count < 3) curCandy.upRow.Clear();
        if (curCandy.downCol.Count < 3) curCandy.downCol.Clear();
        if (curCandy.downRow.Count < 3) curCandy.downRow.Clear();
        if (curCandy.leftCol.Count < 3) curCandy.leftCol.Clear();
        if (curCandy.leftRow.Count < 3) curCandy.leftRow.Clear();
        if (curCandy.rightCol.Count < 3) curCandy.rightCol.Clear();
        if (curCandy.rightRow.Count < 3) curCandy.rightRow.Clear();

        curCandy.upCnt = curCandy.upCol.Count + curCandy.upRow.Count;
        curCandy.downCnt = curCandy.downCol.Count + curCandy.downRow.Count;
        curCandy.leftCnt = curCandy.leftCol.Count + curCandy.leftRow.Count;
        curCandy.rightCnt = curCandy.rightCol.Count + curCandy.rightRow.Count;
    }

    IEnumerator FindCantMove()
    {
        yield return new WaitForSeconds(waitResetTime);

        if (canMoveCandy.Count == 0) InitCandy();
    }

    /// <summary>
    /// 버튼 클릭 시 최고의 한 수 힌트를 보여준다
    /// </summary>
    public void ViewHint()
    {
        if (curState == gameState.wait && canMoveCandy.Count > 0) {
            int x, y, matchCnt;
            int maxLinkCnt = 0, maxLinkIdx = 0, maxLinkOther = 0;
            List<List<myCandy>> linkedCandy = new List<List<myCandy>>();
            List<List<myCandy>> linkedOtherCandy = new List<List<myCandy>>();
            List<int> boomScore = new List<int>();
            List<int> boomOtherScore = new List<int>();
            for (int i = 0; i < canMoveCandy.Count; i++)
            {
                matchCnt = 0;
                matchCnt += canMoveCandy[i].upCol.Count + canMoveCandy[i].upRow.Count;
                matchCnt += canMoveCandy[i].downCol.Count + canMoveCandy[i].downRow.Count;
                matchCnt += canMoveCandy[i].leftCol.Count + canMoveCandy[i].leftRow.Count;
                matchCnt += canMoveCandy[i].rightCol.Count + canMoveCandy[i].rightRow.Count;

                linkedCandy.Add(new List<myCandy>());
                linkedOtherCandy.Add(new List<myCandy>());
                linkedCandy[i].Add(canMoveCandy[i]);
                boomScore.Add(0);
                boomOtherScore.Add(0);
                x = canMoveCandy[i].x; y = canMoveCandy[i].y;
                //3 매치가 아닌 경우(초코볼이거나 아이템끼리)
                if (matchCnt == 0)
                {
                    //가장 높은 값을 가지는 캔디와 같이 힌트 리스트에 추가(폭탄 > 줄무늬 > 일반)
                    switch (canMoveCandy[i].choiceDir)
                    {
                        case myCandy.moveDir.up:
                            linkedCandy[i].Add(myCandys[x][y + 1]); break;
                        case myCandy.moveDir.down:
                            linkedCandy[i].Add(myCandys[x][y - 1]); break;
                        case myCandy.moveDir.left:
                            linkedCandy[i].Add(myCandys[x - 1][y]); break;
                        case myCandy.moveDir.right:
                            linkedCandy[i].Add(myCandys[x + 1][y]); break;
                        default: break;
                    }
                }
                //3매치 캔디의 경우
                else
                {
                    //가장 높은 값을 가지는 방향으로
                    //이 부분은 두 캔디 움직임을 모두 반영하도록 수정 필요
                    switch (canMoveCandy[i].choiceDir)
                    {
                        case myCandy.moveDir.up:
                            linkedCandy[i].AddRange(canMoveCandy[i].upCol);
                            linkedCandy[i].AddRange(canMoveCandy[i].upRow);
                            linkedCandy[i] = new List<myCandy>(new HashSet<myCandy>(linkedCandy[i]));
                            if (canMoveCandy[i].y < myCandys[canMoveCandy[i].x].Count - 1)
                            {
                                linkedOtherCandy[i].AddRange(myCandys[canMoveCandy[i].x][canMoveCandy[i].y + 1].downCol);
                                linkedOtherCandy[i].AddRange(myCandys[canMoveCandy[i].x][canMoveCandy[i].y + 1].downRow);
                                linkedOtherCandy[i] = new List<myCandy>(new HashSet<myCandy>(linkedOtherCandy[i]));
                            }
                            break;
                        case myCandy.moveDir.down:
                            linkedCandy[i].AddRange(canMoveCandy[i].downCol);
                            linkedCandy[i].AddRange(canMoveCandy[i].downRow);
                            linkedCandy[i] = new List<myCandy>(new HashSet<myCandy>(linkedCandy[i]));
                            if (canMoveCandy[i].y > 0)
                            {
                                linkedOtherCandy[i].AddRange(myCandys[canMoveCandy[i].x][canMoveCandy[i].y - 1].upCol);
                                linkedOtherCandy[i].AddRange(myCandys[canMoveCandy[i].x][canMoveCandy[i].y - 1].upRow);
                                linkedOtherCandy[i] = new List<myCandy>(new HashSet<myCandy>(linkedOtherCandy[i]));
                            }
                            break;
                        case myCandy.moveDir.left:
                            linkedCandy[i].AddRange(canMoveCandy[i].leftCol);
                            linkedCandy[i].AddRange(canMoveCandy[i].leftRow);
                            linkedCandy[i] = new List<myCandy>(new HashSet<myCandy>(linkedCandy[i]));
                            if (canMoveCandy[i].x > 0)
                            {
                                linkedOtherCandy[i].AddRange(myCandys[canMoveCandy[i].x - 1][canMoveCandy[i].y].rightCol);
                                linkedOtherCandy[i].AddRange(myCandys[canMoveCandy[i].x - 1][canMoveCandy[i].y].rightRow);
                                linkedOtherCandy[i] = new List<myCandy>(new HashSet<myCandy>(linkedOtherCandy[i]));
                            }
                            break;
                        case myCandy.moveDir.right:
                            linkedCandy[i].AddRange(canMoveCandy[i].rightCol);
                            linkedCandy[i].AddRange(canMoveCandy[i].rightRow);
                            linkedCandy[i] = new List<myCandy>(new HashSet<myCandy>(linkedCandy[i]));
                            if (canMoveCandy[i].x < myCandys.Count - 1)
                            {
                                linkedOtherCandy[i].AddRange(myCandys[canMoveCandy[i].x + 1][canMoveCandy[i].y].leftCol);
                                linkedOtherCandy[i].AddRange(myCandys[canMoveCandy[i].x + 1][canMoveCandy[i].y].leftRow);
                                linkedOtherCandy[i] = new List<myCandy>(new HashSet<myCandy>(linkedOtherCandy[i]));
                            }
                            break;
                        default: break;
                    }
                }

                //각 터질 수 있는 캔디의 점수(임의 설정)
                for (int j = 0; j < linkedCandy[i].Count; j++)
                {
                    switch (linkedCandy[i][j].myType)
                    {
                        case candyTypes.normal:
                            boomScore[i] += 1; break;
                        case candyTypes.row:
                        case candyTypes.col:
                            boomScore[i] += 10; break;
                        case candyTypes.boom:
                            boomScore[i] += 15; break;
                        case candyTypes.choco:
                            boomScore[i] += 25; break;
                        default: break;
                    }
                }
                //3매치인 경우 같이 움직여지는 캔디의 점수도 포함
                for (int j = 0; j < linkedOtherCandy[i].Count; j++)
                {
                    switch (linkedOtherCandy[i][j].myType)
                    {
                        case candyTypes.normal:
                            boomOtherScore[i] += 1; break;
                        case candyTypes.row:
                        case candyTypes.col:
                            boomOtherScore[i] += 10; break;
                        case candyTypes.boom:
                            boomOtherScore[i] += 15; break;
                        case candyTypes.choco:
                            boomOtherScore[i] += 25; break;
                        default: break;
                    }
                }
                if (boomScore[i] > maxLinkCnt)
                {
                    maxLinkCnt = boomScore[i];
                    maxLinkOther = boomOtherScore[i];
                    maxLinkIdx = i;
                }
                else if (boomScore[i] == maxLinkCnt && boomOtherScore[i] > maxLinkOther)
                {
                    maxLinkOther = boomOtherScore[i];
                    maxLinkIdx = i;
                }
            }
            for (int i = 0; i < linkedCandy[maxLinkIdx].Count; i++)
            {
                GameObject particle = Instantiate(hintEffect, linkedCandy[maxLinkIdx][i].transform.position, Quaternion.identity);
                Destroy(particle, 2f);
            }
        }
    }

    //처음 두 캔디 선택시는 바로 바뀌어야하기때문에 void메소드를 사용하고
    //캔디 두개가 조건에 맞지 않아 원 상태로 돌아가기 위해 대기 시간이 필요한 경우는 코루틴으로 작성
    //기능은 color와 type을 바꿔 이를 통해 스프라이트 이름을 바꿔주는 것으로 동일
    void SwapCandyData(myCandy candy1, myCandy candy2)
    {
        candyColors tempColor;

        tempColor = candy1.myColor;
        candy1.myColor = candy2.myColor;
        candy2.myColor = tempColor;

        candyTypes tempType;

        tempType = candy1.myType;
        candy1.myType = candy2.myType;
        candy2.myType = tempType;

        candy1.gameObject.GetComponent<UISprite>().spriteName = candy1.myColor.ToString() + "_" + candy1.myType.ToString();
        candy2.gameObject.GetComponent<UISprite>().spriteName = candy2.myColor.ToString() + "_" + candy2.myType.ToString();
    }
    IEnumerator SwapCandyData(myCandy candy1, myCandy candy2, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        candyColors tempColor;

        tempColor = candy1.myColor;
        candy1.myColor = candy2.myColor;
        candy2.myColor = tempColor;

        candyTypes tempType;

        tempType = candy1.myType;
        candy1.myType = candy2.myType;
        candy2.myType = tempType;

        candy1.gameObject.GetComponent<UISprite>().spriteName = candy1.myColor.ToString() + "_" + candy1.myType.ToString();
        candy2.gameObject.GetComponent<UISprite>().spriteName = candy2.myColor.ToString() + "_" + candy2.myType.ToString();
    }

    //캔디 두개가 클릭되었을때 터지는 여부 파악
    //초코가 하나라도 있거나 둘다 아이템 캔디이면 3연속 캔디가 아니어도 되기때문에 별도 함수로 처리
    //캔디 위치 바꿨을때 3연속 터지는게 없으면 원상태로 복구
    public void CheckCandyWithClick(myCandy curCandy, myCandy lastCandy)
    {
        SwapCandyData(curCandy, lastCandy);

        //초코 캔디가 껴있는 경우
        //초코캔디는 상대를 작동시키면 그냥 터지기 때문에 자신도 터지는 것을 체크
        //초코 캔디가 아닌 캔디(초코캔디일수도 있다)를 인자로 삼는 별도 함수 실행 후 대기상태에서 상태값을 변경 후 return
        if (curCandy.myType == candyTypes.choco || lastCandy.myType == candyTypes.choco)
        {
            if (curCandy.myType == candyTypes.choco)
            {
                curCandy.isChecked = true;
                ChocoCheck(lastCandy);
            }
            else
            {
                lastCandy.isChecked = true;
                ChocoCheck(curCandy);
            }
            curState = gameState.bomb;
            boomCnt = 0;
            return;
        }

        //아이템 캔디끼리 터지는 경우(초코제외)
        //두 캔디를 인자로 받는 별도 함수 실행 후 초코캔디와 동일하게 return
        else if (curCandy.myType != candyTypes.normal && lastCandy.myType != candyTypes.normal)
        {
            ItemCheck(curCandy, lastCandy);
            curState = gameState.bomb;
            boomCnt = 0;
            return;
        }

        //cur가 나중에 클릭된 캔디, last가 처음 클릭된 캔디
        //상하좌우 연속된 캔디가 있는지를 확인
        myCandy cur = curCandy;
        myCandy curUp = cur, curDown = cur, curLeft = cur, curRight = cur;
        myCandy last = lastCandy;
        myCandy lastUp = last, lastDown = last, lastLeft = last, lastRight = last;
        for (int i = 0; i < 2; i++)
        {
            if (curUp.upCandy.y == curUp.y + 1 && curUp.upCandy.myColor == curUp.myColor) curUp = curUp.upCandy;
            if (curDown.downCandy.y == curDown.y - 1 && curDown.downCandy.myColor == curDown.myColor) curDown = curDown.downCandy;
            if (curLeft.leftCandy.x == curLeft.x - 1 && curLeft.leftCandy.myColor == curLeft.myColor) curLeft = curLeft.leftCandy;
            if (curRight.rightCandy.x == curRight.x + 1 && curRight.rightCandy.myColor == curRight.myColor) curRight = curRight.rightCandy;

            if (lastUp.upCandy.y == lastUp.y + 1 && lastUp.upCandy.myColor == lastUp.myColor) lastUp = lastUp.upCandy;
            if (lastDown.downCandy.y == lastDown.y - 1 && lastDown.downCandy.myColor == lastDown.myColor) lastDown = lastDown.downCandy;
            if (lastLeft.leftCandy.x == lastLeft.x - 1 && lastLeft.leftCandy.myColor == lastLeft.myColor) lastLeft = lastLeft.leftCandy;
            if (lastRight.rightCandy.x == lastRight.x + 1 && lastRight.rightCandy.myColor == lastRight.myColor) lastRight = lastRight.rightCandy;
        }
        int curRowLeng = curRight.x - curLeft.x, curColLeng = curUp.y - curDown.y;
        int lastRowLeng = lastRight.x - lastLeft.x, lastColLeng = lastUp.y - lastDown.y;

        //4가지 경우 중 하나라도 터지는 캔디가 있으면
        if ((curRowLeng >= 2 || curColLeng >= 2) || (lastRowLeng >= 2 || lastColLeng >= 2))
        {
            if(curRowLeng >= 2)
            {
                cur.leftCheck = cur.x - curLeft.x;
                cur.rightCheck = curRight.x - cur.x;
                for (int x = curLeft.x; x <= curRight.x; x++)
                {
                    myCandys[x][cur.y].isChecked = true;
                }
            }
            if (curColLeng >= 2)
            {
                cur.upCheck = curUp.y - cur.y;
                cur.downCheck = cur.y - curDown.y;
                for (int y = curDown.y;  y <= curUp.y; y++)
                {
                    myCandys[cur.x][y].isChecked = true;
                }
            }
            if (lastRowLeng >= 2)
            {
                last.leftCheck = last.x - lastLeft.x;
                last.rightCheck = lastRight.x - last.x;
                for (int x = lastLeft.x; x <= lastRight.x; x++)
                {
                    myCandys[x][last.y].isChecked = true;
                }
            }
            if (lastColLeng >= 2)
            {
                last.upCheck = lastUp.y - last.y;
                last.downCheck = last.y - lastDown.y;
                for (int y = lastDown.y; y <= lastUp.y; y++)
                {
                    myCandys[last.x][y].isChecked = true;
                }
            }
            curState = gameState.bomb;
            boomCnt = 0;
        }
        //터지는 캔디 없으면 원상복구
        else
        {
            StartCoroutine(SwapCandyData(curCandy, lastCandy, waitSwapTime));
        }
    }

    /// <summary>
    /// 초코 캔디를 선택한 경우는 상대 캔디에 따라 로직 결정
    /// </summary>
    /// <param name="otherCandy">Other candy.</param>
    void ChocoCheck(myCandy otherCandy)
    {
        //상대가 노말이면 같은 색 모두 체크
        if (otherCandy.myType == candyTypes.normal)
        {
            for (int i = 0; i < myCandys.Count; i++)
            {
                for (int j = 0; j < myCandys[i].Count; j++)
                {
                    if (myCandys[i][j].myColor == otherCandy.myColor)
                    {
                        myCandys[i][j].isChecked = true;
                    }
                }
            }
        }
        //상대 캔디가 가로, 세로 줄무늬 캔디
        //색이 같은 캔디를 모두 랜덤한 가로, 세로 줄무늬로 만들고 체크
        else if (otherCandy.myType == candyTypes.row || otherCandy.myType == candyTypes.col)
        {
            candyTypes[] line = { candyTypes.row, candyTypes.col };
            for (int i = 0; i < myCandys.Count; i++)
            {
                for (int j = 0; j < myCandys[i].Count; j++)
                {
                    if (myCandys[i][j].myColor == otherCandy.myColor)
                    {
                        candyTypes randLine = line[UnityEngine.Random.Range(0, line.Length)];
                        myCandys[i][j].myType = randLine;
                        myCandys[i][j].gameObject.GetComponent<UISprite>().spriteName = myCandys[i][j].myColor.ToString() + "_" + randLine.ToString();
                        myCandys[i][j].isChecked = true;
                    }
                }
            }
        }
        //상대 캔디가 폭탄
        //상대 캔디와 색이 같은 캔디를 모두 폭탄캔디로 만들고 체크
        else if (otherCandy.myType == candyTypes.boom)
        {
            for (int i = 0; i < myCandys.Count; i++)
            {
                for (int j = 0; j < myCandys[i].Count; j++)
                {
                    if (myCandys[i][j].myColor == otherCandy.myColor)
                    {
                        myCandys[i][j].myType = candyTypes.boom;
                        myCandys[i][j].gameObject.GetComponent<UISprite>().spriteName = myCandys[i][j].myColor.ToString() + "_" + candyTypes.boom.ToString();
                        myCandys[i][j].isChecked = true;
                    }
                }
            }
        }
        //상대 캔디가 초코
        //모든 캔디를 체크해서 터지는 상태로 만든다
        else
        {
            for (int i = 0; i < myCandys.Count; i++)
            {
                for (int j = 0; j < myCandys[i].Count; j++)
                {
                    if (myCandys[i][j].gameObject.activeInHierarchy)
                    {
                        myCandys[i][j].isChecked = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 아이탬 캔디끼리 터지는 경우
    /// </summary>
    /// <param name="curCandy">Current candy.</param>
    /// <param name="lastCandy">Last candy.</param>
    void ItemCheck(myCandy curCandy, myCandy lastCandy)
    {
        //줄무늬끼리
        //첫 클릭 위치(last)를 cur위치로 옮기는 것은 cur위치에서 십자로 터지기 때문(x, y값만 바꿔서 스프라이트는 이상 없음)
        //만약 둘다 가로거나 세로면 하나를 바꿔준다
        if((curCandy.myType == candyTypes.row || curCandy.myType == candyTypes.col) && (lastCandy.myType == candyTypes.row || lastCandy.myType == candyTypes.col))
        {
            lastCandy.x = curCandy.x;
            lastCandy.y = curCandy.y;
            lastCandy.isChecked = true;
            curCandy.isChecked = true;
            if (curCandy.myType == candyTypes.row && lastCandy.myType == candyTypes.row)
            {
                lastCandy.myType = candyTypes.col;
            }
            if (curCandy.myType == candyTypes.col && lastCandy.myType == candyTypes.col)
            {
                lastCandy.myType = candyTypes.row;
            }
        }
        //가로+폭탄 : cur 기준으로 가로 세줄을 모두 체크(맵 범위 이내)
        else if((curCandy.myType == candyTypes.row && lastCandy.myType == candyTypes.boom) || (curCandy.myType == candyTypes.boom && lastCandy.myType == candyTypes.row))
        {
            for (int i = 0; i < myCandys.Count; i++)
            {
                for (int j = Mathf.Max(curCandy.y - 1, 0); j <= Mathf.Min(curCandy.y + 1, myCandys[i].Count - 1); j++)
                {
                    if (myCandys[i][j].gameObject.activeInHierarchy)
                    {
                        myCandys[i][j].isChecked = true;
                    }
                }
            }
        }
        //세로+폭탄 : 가로폭탄과 같은 로직
        else if ((curCandy.myType == candyTypes.col && lastCandy.myType == candyTypes.boom) || (curCandy.myType == candyTypes.boom && lastCandy.myType == candyTypes.col))
        {
            for (int i = Mathf.Max(curCandy.x - 1, 0); i <= Mathf.Min(curCandy.x + 1, myCandys.Count - 1); i++)
            {
                for (int j = 0; j < myCandys[i].Count; j++)
                {
                    if (myCandys[i][j].gameObject.activeInHierarchy)
                    {
                        myCandys[i][j].isChecked = true;
                    }
                }
            }
        }
        //폭탄+폭탄 : cur 기준 5x5 모두 체크
        else
        {
            for (int i = Mathf.Max(curCandy.x - 2, 0); i <= Mathf.Min(curCandy.x + 2, myCandys.Count - 1); i++)
            {
                for (int j = Mathf.Max(curCandy.y - 2, 0); j <= Mathf.Min(curCandy.y + 2, myCandys[i].Count - 1); j++)
                {
                    if (myCandys[i][j].gameObject.activeInHierarchy)
                    {
                        myCandys[i][j].isChecked = true;
                    }
                }
            }
        }
    }


    void BombCheck()
    {
        myCandy up, down, left, right;
        movingCnt.Clear();
        getChoco.Clear(); getBoom.Clear(); getCol.Clear(); getRow.Clear();
        for (int i = 0; i < myCandys.Count; i++)
        {
            movingCnt.Add(0);
            getChoco.Add(new List<int>()); getBoom.Add(new List<int>()); getCol.Add(new List<int>()); getRow.Add(new List<int>());
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                if (myCandys[i][j].isMoving) movingCnt[i] += 1;
                getChoco[i].Add(0); getBoom[i].Add(0); getCol[i].Add(0); getRow[i].Add(0);
            }
        }

        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                if (myCandys[i][j].gameObject.activeInHierarchy)
                {
                    myCandy cur = myCandys[i][j];
                    if (movingCnt[cur.x] == 0) //아마 isMoving보다 밑에 있는 놈은 터지지 않을 것이라 생각됨 //이 라인에 isMoving이 있으면 체크 못하는걸로
                    {
                        up = cur; down = cur; left = cur; right = cur;
                        for (int c = 0; c < 4; c++)
                        {
                            if (up.upCandy.y == up.y + 1 && up.upCandy.myColor == up.myColor && movingCnt[up.upCandy.x] == 0)
                            {
                                up = up.upCandy;
                            }
                            if (down.downCandy.y == down.y - 1 && down.downCandy.myColor == down.myColor && movingCnt[down.downCandy.x] == 0)
                            {
                                down = down.downCandy;
                            }
                            if (left.leftCandy.x == left.x - 1 && left.leftCandy.myColor == left.myColor && movingCnt[left.leftCandy.x] == 0)
                            {
                                left = left.leftCandy;
                            }
                            if (right.rightCandy.x == right.x + 1 && right.rightCandy.myColor == right.myColor && movingCnt[right.rightCandy.x] == 0)
                            {
                                right = right.rightCandy;
                            }
                        }
                        if (right.x - left.x >= 2)
                        {
                            cur.leftCheck = cur.x - left.x;
                            cur.rightCheck = right.x - cur.x;
                            for (int x = left.x; x <= right.x; x++)
                            {
                                myCandys[x][cur.y].isChecked = true;
                            }
                        }
                        if (up.y - down.y >= 2)
                        {
                            cur.upCheck = up.y - cur.y;
                            cur.downCheck = cur.y - down.y;
                            for (int y = down.y; y <= up.y; y++)
                            {
                                myCandys[cur.x][y].isChecked = true;
                            }
                        }

                        if ((cur.upCheck == 2 && cur.downCheck >= 2) || (cur.leftCheck == 2 && cur.rightCheck >= 2))
                        {
                            getChoco[cur.x][cur.y] = 1;
                            for (int x = cur.x - cur.leftCheck; x <= cur.x + cur.rightCheck; x++)
                            {
                                getBoom[x][cur.y] = -1; getCol[x][cur.y] = -1; getRow[x][cur.y] = -1;
                            }
                            for (int y = cur.y - cur.downCheck; y <= cur.y + cur.upCheck; y++)
                            {
                                getBoom[cur.x][y] = -1; getCol[cur.x][y] = -1; getRow[cur.x][y] = -1;
                            }
                        }
                        else if (cur.upCheck + cur.downCheck >= 2 && cur.leftCheck + cur.rightCheck >= 2)
                        {
                            if (getBoom[cur.x][cur.y] != -1)
                            {
                                getBoom[cur.x][cur.y] = 1;
                                for (int x = cur.x - cur.leftCheck; x <= cur.x + cur.rightCheck; x++)
                                {
                                    getCol[x][cur.y] = -1; getRow[x][cur.y] = -1;
                                }
                                for (int y = cur.y - cur.downCheck; y <= cur.y + cur.upCheck; y++)
                                {
                                    getCol[cur.x][y] = -1; getRow[cur.x][y] = -1;
                                }
                            }
                        }
                        else
                        {
                            myCandy clickedCur = manager.curCandy.GetComponent<myCandy>();
                            myCandy clickedLast = manager.lastCandy.GetComponent<myCandy>();
                            if (cur.upCheck + cur.downCheck >= 3)
                            {
                                if (boomCnt == 0)
                                {
                                    if (clickedCur.x == cur.x && (clickedCur.y <= cur.y + cur.upCheck && clickedCur.y >= cur.y - cur.downCheck) &&
                                        getRow[clickedCur.x][clickedCur.y] != -1)
                                    {
                                        getRow[clickedCur.x][clickedCur.y] = 1;
                                    }
                                    else if (clickedLast.x == cur.x && (clickedLast.y <= cur.y + cur.upCheck && clickedLast.y >= cur.y - cur.downCheck) &&
                                        getRow[clickedLast.x][clickedLast.y] != -1)
                                    {
                                        getRow[clickedLast.x][clickedLast.y] = 1;
                                    }
                                }
                                else if (getRow[cur.x][cur.y + cur.upCheck] != -1)
                                {
                                    getRow[cur.x][cur.y + cur.upCheck] = 1;
                                }
                            }
                            else if (cur.leftCheck + cur.rightCheck >= 3)
                            {
                                if (boomCnt == 0)
                                {
                                    if (clickedCur.y == cur.y && (clickedCur.x <= cur.x + cur.rightCheck && clickedCur.x >= cur.x - cur.leftCheck) &&
                                        getCol[clickedCur.x][clickedCur.y] != -1)
                                    {
                                        getCol[clickedCur.x][clickedCur.y] = 1;
                                    }
                                    else if (clickedLast.y == cur.y && (clickedLast.x <= cur.x + cur.rightCheck && clickedLast.x >= cur.x - cur.leftCheck) &&
                                        getCol[clickedLast.x][clickedLast.y] != -1)
                                    {
                                        getCol[clickedLast.x][clickedLast.y] = 1;
                                    }
                                }
                                else if (getCol[cur.x - cur.leftCheck][cur.y] != -1)
                                {
                                    getCol[cur.x - cur.leftCheck][cur.y] = 1;
                                }

                            }
                        }
                    }
                }
            }
        }
        //체크되있거나 움직이는 중인 캔디 개수 확인
        int checkNum = 0;
        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                if (myCandys[i][j].isChecked || myCandys[i][j].isMoving)
                {
                    checkNum += 1;
                }
            }
        }
        //터지는 로직 진행중인 캔디가 없으면 모든 캔디 상하좌우 연속 캔디 개수 초기화 후 대기상태로 돌아간다
        if (checkNum == 0)
        {
            for (int i = 0; i < myCandys.Count; i++)
            {
                for (int j = 0; j < myCandys[i].Count; j++)
                {
                    myCandys[i][j].upCheck = 0;
                    myCandys[i][j].downCheck = 0;
                    myCandys[i][j].leftCheck = 0;
                    myCandys[i][j].rightCheck = 0;
                }
            }
            curState = gameState.wait;
            GetBombReady();
        }
    }

    /// <summary>
    /// 체크되어있는 캔디를 터뜨리고 아이탬을 생성
    /// </summary>
    void CandyBomb()
    {

        boomCnt += 1;

        List<myCandy> curCheckedCandy = new List<myCandy>();
        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                myCandy candyData = myCandys[i][j];
                if (candyData.isChecked)
                {
                    string candyName = candyData.transform.parent.name;
                    if (candyData.myType == candyTypes.row)
                    {
                        for (int x = 0; x < myCandys.Count; x++)
                        {
                            if (myCandys[x][candyData.y].gameObject.activeInHierarchy &&
                                !myCandys[x][candyData.y].isChecked && !myCandys[x][candyData.y].isMoving)
                            {
                                myCandys[x][candyData.y].isChecked = true;
                            }
                        }
                        candyData.x = int.Parse(candyName[candyName.Length - 2].ToString());
                        candyData.y = int.Parse(candyName[candyName.Length - 1].ToString());
                    }
                    else if (candyData.myType == candyTypes.col)
                    {
                        for (int y = 0; y < myCandys[i].Count; y++)
                        {
                            if (myCandys[candyData.x][y].gameObject.activeInHierarchy &&
                                !myCandys[candyData.x][y].isChecked && !myCandys[candyData.x][y].isMoving)
                            {
                                myCandys[candyData.x][y].isChecked = true;
                            }
                        }
                        candyData.x = int.Parse(candyName[candyName.Length - 2].ToString());
                        candyData.y = int.Parse(candyName[candyName.Length - 1].ToString());
                    }
                    else if (candyData.myType == candyTypes.boom || candyData.myType == candyTypes.bonus)
                    {
                        for (int x = Mathf.Max(candyData.x - 1, 0); x <= Mathf.Min(candyData.x + 1, myCandys.Count - 1); x++)
                        {
                            for (int y = Mathf.Max(candyData.y - 1, 0); y <= Mathf.Min(candyData.y + 1, myCandys[i].Count - 1); y++)
                            {
                                if (myCandys[x][y].gameObject.activeInHierarchy &&
                                    !myCandys[x][y].isChecked && !myCandys[x][y].isMoving)
                                {
                                    myCandys[x][y].isChecked = true;
                                }
                            }
                        }
                    }
                    else if (candyData.myType == candyTypes.choco &&
                        (candyData.x != manager.curCandy.x || candyData.y != manager.curCandy.y) &&
                        (candyData.y != manager.lastCandy.x || candyData.y != manager.lastCandy.y))
                    {
                        candyColors tempColor = exceptNoneColors[UnityEngine.Random.Range(0, exceptNoneColors.Count)];
                        for (int x = 0; x < myCandys.Count; x++)
                        {
                            for (int y = 0; y < myCandys[x].Count; y++)
                            {
                                if (myCandys[x][y].gameObject.activeInHierarchy &&
                                    !myCandys[x][y].isChecked && !myCandys[x][y].isMoving &&
                                     myCandys[x][y].myColor == tempColor)
                                {
                                    myCandys[x][y].isChecked = true;
                                }
                            }
                        }
                    }

                    //폭탄 캔디 위치에 보너스 캔디 생성
                    if (candyData.myType == candyTypes.boom)
                    {
                        candyData.myType = candyTypes.bonus;
                        candyData.bonusCnt = 0;
                    }
                    //초코, 폭탄, 줄무늬 아이탬 캔디 생성
                    else if (getChoco[i][j] == 1)
                    {
                        candyData.myType = candyTypes.choco;
                        candyData.myColor = candyColors.none;
                    }
                    else if (getBoom[i][j] == 1)
                    {
                        candyData.myType = candyTypes.boom;
                    }
                    else if (getCol[i][j] == 1)
                    {
                        candyData.myType = candyTypes.col;
                    }
                    else if (getRow[i][j] == 1)
                    {
                        candyData.myType = candyTypes.row;
                    }
                    //아이탬 생성 없음
                    else
                    {
                        //아이탬이 없는 경우 자리가 비기 때문에 isMoving을 true로 바꿈
                        candyData.myType = candyTypes.none;
                        candyData.isMoving = true;
                        movingCnt[candyData.x] += 1;
                    }
                    myCandys[i][j].gameObject.GetComponent<UISprite>().spriteName = candyData.myColor.ToString() + "_" + candyData.myType.ToString();
                    curCheckedCandy.Add(candyData);
                }
            }
        }
        //체크되었었던 캔디는 모든 폭발 검사가 끝난 후 한번에 풀어줘야
        //체크를 바로바로 풀면 다른 캔디 영향으로 바로 다시 터지는 경우 발생
        for(int i = 0; i < curCheckedCandy.Count; i++)
        {
            curCheckedCandy[i].isChecked = false;
        }
        //모든 맵의 상하좌우 연속 캔디 개수 초기화
        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                myCandys[i][j].upCheck = 0;
                myCandys[i][j].downCheck = 0;
                myCandys[i][j].leftCheck = 0;
                myCandys[i][j].rightCheck = 0;
            }
        }
    }

    /// <summary>
    /// 각 라인에서 터져서 빈 캔디를 한 칸씩 내려서 채워준다
    /// </summary>
    void SlideMove()
    {
        for (int i = 0; i < myCandys.Count; i++)
        {
            if (movingCnt[i] != 0) //각 라인에 내려야 할 캔디가 있는지
            {
                myCandy top = myCandys[i][0], bottom = myCandys[i][myCandys[i].Count - 1];
                for (int j = 0; j < myCandys[i].Count; j++)
                {
                    //빈 캔디 위체에서 가장 높은게 top, 가장 낮은게 bottom
                    if (myCandys[i][j].gameObject.activeInHierarchy && myCandys[i][j].isMoving)
                    {
                        if (myCandys[i][j].y > top.y)
                        {
                            top = myCandys[i][j];
                        }
                        if (myCandys[i][j].y < bottom.y)
                        {
                            bottom = myCandys[i][j];
                        }
                    }
                }
                try
                {
                    //bottom부터 위로 가면서 위 캔디 값을 가져옴
                    //맨 위 캔디는 랜덤색상 노말캔디로
                    while (true)
                    {
                        if (bottom.upCandy.y == bottom.y)
                        {
                            bottom.myColor = exceptNoneColors[UnityEngine.Random.Range(0, exceptNoneColors.Count)];
                            bottom.myType = candyTypes.normal;
                            bottom.gameObject.GetComponent<UISprite>().spriteName = bottom.myColor.ToString() + "_" + bottom.myType.ToString();
                            bottom.isMoving = false;
                            bottom.isChecked = false;
                            bottom.bonusCnt = 0;
                            break;
                        }
                        bottom.myColor = bottom.upCandy.myColor;
                        bottom.myType = bottom.upCandy.myType;
                        bottom.gameObject.GetComponent<UISprite>().spriteName = bottom.myColor.ToString() + "_" + bottom.myType.ToString();
                        bottom.isMoving = bottom.upCandy.isMoving;
                        bottom.isChecked = bottom.upCandy.isChecked;
                        bottom.bonusCnt = bottom.upCandy.bonusCnt;
                        bottom = bottom.upCandy;
                    }
                }
                catch (System.NullReferenceException) { }
            }
        }
    }

    /// <summary>
    /// 보너스 캔디의 터지는 카운트와 bomb 상태일때 캔디가 터지는 로직을 항상 체크한다
    /// </summary>
    void Update()
    {
        //보너스 캔디의 카운트 증가 후 횟수 채워지면 체크되도록 한다
        for (int i = 0; i < myCandys.Count; i++)
        {
            for (int j = 0; j < myCandys[i].Count; j++)
            {
                if(myCandys[i][j].myType == candyTypes.bonus)
                {
                    myCandys[i][j].bonusCnt += 1;
                    if(myCandys[i][j].bonusCnt == 80)
                    {
                        myCandys[i][j].isChecked = true;
                        curState = gameState.bomb; //체크 시 터짐 상태로 바꿔야 연쇄 폭밣 가능
                    }
                }
            }
        }

        //터짐 상태일때 체크된 것을 터뜨리고 빈 부분을 내려서 채운다
        if (curState == gameState.bomb)
        {
            time += Time.deltaTime;
            if (time > waitBombTime && waitSlideTime > time)
            {
                BombCheck();
                CandyBomb();
            }
            //터진 후 슬라이트를 따로 처리하는 이유는 아이탬 효과에 따른 연속 폭발을 자연스럽게 처리하기 위해서
            else if (time > waitSlideTime) 
            {
                SlideMove();
                time = 0;
            }
        }
    }
}
