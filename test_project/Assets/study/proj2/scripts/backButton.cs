using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backButton : MonoBehaviour {

    /// <summary>
    /// 버튼을 누르면 메인화면으로 돌아간다
    /// </summary>
    public void BackToMain()
    {
        //매니저는 메인 화면에서 오늘 싱글톤이므로 NULL인지 체크
        //매니저의 BackToMain 메소드를 가져와 실행하는 이유는 다른 신에서도 메인으로 가는 경우가 있을 수 있기 때문(이 게임은 x)
        try
        {
            GameObject.Find("manager").GetComponent<manager>().BackToMain();
        }
        catch(System.NullReferenceException) { }
    }
}
