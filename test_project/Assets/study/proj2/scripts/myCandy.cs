using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myCandy : MonoBehaviour {

    //클릭 시 캔디가 터지는 로직이 저장된 스크립트
    private candyCtrl candyControl;

    public enum moveDir { up, down, left, right };

    [HideInInspector] public bool isChecked = false, isMoving = false;          //isChecked는 터질 예정인 캔디, isMoving은 터져서 빈 상태(내려오는중)
    [HideInInspector] public int x, y;
    [HideInInspector] public int leftCheck, rightCheck, upCheck, downCheck;     //상하좌우에 연속된 색상의 캔디 개수
    [HideInInspector] public int bonusCnt;                      
    [HideInInspector] public candyCtrl.candyColors myColor;
    [HideInInspector] public candyCtrl.candyTypes myType;
    [HideInInspector] public myCandy upCandy, downCandy, leftCandy, rightCandy; //상하좌우에 활성화 되어있는 캔디의 myCandy 스크립트
    [HideInInspector] public List<myCandy> upCol = new List<myCandy>(), upRow = new List<myCandy>();
    [HideInInspector] public List<myCandy> downCol = new List<myCandy>(), downRow = new List<myCandy>();
    [HideInInspector] public List<myCandy> leftCol = new List<myCandy>(), leftRow = new List<myCandy>();
    [HideInInspector] public List<myCandy> rightCol = new List<myCandy>(), rightRow = new List<myCandy>();
    /*
    [HideInInspector] public List<myCandy> upList = new List<myCandy>(), downList = new List<myCandy>();
    [HideInInspector] public List<myCandy> leftList = new List<myCandy>(), rightList = new List<myCandy>();
    */
    [HideInInspector] public int upCnt, downCnt, leftCnt, rightCnt;
    [HideInInspector] public moveDir choiceDir;

    /// <summary>
    /// UP, DOWN, RIGHT, LEFT 위치의 가장 가까이 활성화되어있는 캔디를 저장
    /// </summary>
    public void SetDirect() {
        candyControl = GameObject.Find("candys").GetComponent<candyCtrl>();
        myCandy self = gameObject.GetComponent<myCandy>();

        //디폴트값은 자기자신
        upCandy = self; downCandy = self; leftCandy = self; rightCandy = self;

        //트라이 각각 따로 쓴 이유는 각 방향 중 하나라도 없으면 에러가 나기 때문
        //각 방향에 대해 1칸씩 이동하면서 활성화 되있는 캔디를 찾음
        //활성화되어있는 캔디가 없으면 자기 자신을 저장
        try
        {
            while (true)
            {
                Transform curUp = manager.candyContainer.Find("candy" + upCandy.x.ToString() + (upCandy.y + 1).ToString());
                if (curUp == null) break;
                upCandy = curUp.Find("candy").GetComponent<myCandy>();
                if (upCandy.gameObject.activeInHierarchy) break;
            }
        }
        catch (System.NullReferenceException) { }
        if (!upCandy.gameObject.activeInHierarchy) upCandy = self;

        try
        {
            while (true)
            {
                Transform curDown = manager.candyContainer.Find("candy" + downCandy.x.ToString() + (downCandy.y - 1).ToString());
                if (curDown == null) break;
                downCandy = curDown.Find("candy").GetComponent<myCandy>();
                if (downCandy.gameObject.activeInHierarchy) break;
            }
        }
        catch (System.NullReferenceException) { }
        if (!downCandy.gameObject.activeInHierarchy) downCandy = self;

        try
        {
            while (true)
            {
                Transform curRight = manager.candyContainer.Find("candy" + (rightCandy.x + 1).ToString() + rightCandy.y.ToString());
                if (curRight == null) break;
                rightCandy = curRight.Find("candy").GetComponent<myCandy>();
                if (rightCandy.gameObject.activeInHierarchy) break;
            }
        }
        catch (System.NullReferenceException) { }
        if (!rightCandy.gameObject.activeInHierarchy)rightCandy = self;

        try {
            while (true)
            {
                Transform curLeft = manager.candyContainer.Find("candy" + (leftCandy.x - 1).ToString() + leftCandy.y.ToString());
                if (curLeft == null) break;
                leftCandy = curLeft.Find("candy").GetComponent<myCandy>();
                if (leftCandy.gameObject.activeInHierarchy) break;
            }
        }
        catch (System.NullReferenceException) { }
        if (!leftCandy.gameObject.activeInHierarchy) leftCandy = self;
    }

    /// <summary>
    /// 투터치 방식으로 첫 번째 누른 캔디와 두 번째 누른 캔디 지정 및 격자 활성화 여부 파악
    /// </summary>
    /// <param name="isPress">If set to <c>true</c> is press.</param>
    void OnPress(bool isPress)
    {
        //if (isChecked || isBoom || isMoving) return;

        if (isPress == true &&                              //마우스 눌렀을때
            gameObject.activeInHierarchy == true &&         //게임오브젝트가 활성화되있고
            candyCtrl.curState == candyCtrl.gameState.wait) //현재 상태가 대기상태이면
        {
            //curCandy가 현재 누른 캔디
            manager.touchCnt += 1;
            manager.touchCnt %= 2;
            manager.curPos = gameObject.transform.position;
            manager.curCandy = gameObject.GetComponent<myCandy>();
            //첫 번째 누른 캔디에서 격자가 활성화된다
            if (manager.touchCnt == 1)
            {
                manager.grid.transform.position = manager.curPos;
                manager.grid.SetActive(true);
                manager.lastPos = manager.curPos;
                manager.lastCandy = manager.curCandy;
            }
            else
            {
                int curX = manager.curCandy.x, curY = manager.curCandy.y;
                int lastX = manager.lastCandy.x, lastY = manager.lastCandy.y;

                //인접 캔디를 클릭 시 터지는 여부를 확인
                if (Mathf.Abs(curX - lastX) + Mathf.Abs(curY - lastY) == 1)
                {
                    manager.grid.SetActive(false);
                    candyControl.CheckCandyWithClick(manager.curCandy, manager.lastCandy);
                }
                //먼 캔디 클릭하면 격자가 이동(다시 첫 번째로 누른 캔디)
                else
                {
                    manager.grid.transform.position = manager.curPos;
                    manager.touchCnt += 1;
                    manager.touchCnt %= 2;
                    manager.lastPos = manager.curPos;
                    manager.lastCandy = manager.curCandy;
                }
            }
        }
    }
}
