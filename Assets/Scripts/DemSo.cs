using System.Collections;
using UnityEngine;

public class DemSo : MonoBehaviour
{
    public int count;
    Coroutine cr;
    bool isMoving; // mặc định false
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // StartCoroutine(DoiDemXong());
        // StartCoroutine(Test());
        StartCoroutine(TestWaitWhile());
    }

    private void Update()
    {
        // if (count > 5)
        // {
        //     StopCoroutine(cr);
        // }

        if (count < 10)
        {
            count++;
            Debug.Log(count);
        }
    }

    IEnumerator TestWaitWhile() // dùng WaitWhile để dừng thực thi khi 1 điều kiện vốn true trở thành false
    {
        yield return new WaitWhile(() => count < 10); // Chờ đến khi điều kiện này false (ngược với WaitUntil)
        Debug.Log("Hoàn thành!");
    }

    IEnumerator Test() // Các ví dụ về các hàm Wait
    {
        Debug.Log("Start");

        // 2 cách viết dưới đây cho kết quả như nhau khi chạy (cả 2 đều chạy và chờ coroutine MoveObject hoàn thành):

        // Cách 1:
        // yield return StartCoroutine(MoveObject());

        // Cách 2:
        StartCoroutine(MoveObject());
        yield return new WaitUntil(() => !isMoving);
        Debug.Log("xong cách 2");

        // yield return new WaitForEndOfFrame();
        // yield return new WaitForFixedUpdate();

        // Time.timeScale = 0f;
        // yield return new WaitForSeconds(1f);
        // yield return new WaitForSecondsRealtime(1f);
        // Tip: dùng thời gian để cân bằng game (thời gian lên level của nhân vật,v.v...)


        // Dùng Wait với hàm thông thường
        yield return new WaitUntil(testCount);
        Debug.Log("xong");
    }

    IEnumerator MoveObject()
    {
        isMoving = true;
        Debug.Log("Bắt đầu di chuyển");
        // Giả lập việc di chuyển
        yield return new WaitForSeconds(2f);
        isMoving = false; // false nghĩa là di chuyển đến nơi thì dừng lại (về logic)

    }

    bool testCount()
    {
        if (count < 100) // dùng if thì chỉ chạy 1 lần
        {
            Debug.Log("Đếm: " + count);
            count++;
            // return false; // có dòng này thì đến đây thì ngắt
        }
        return true;
    }
    // IEnumerator Dem()
    // {
    //     while (count < 10)
    //     {
    //         count++;
    //         Debug.Log(count);
    //         yield return new WaitForSeconds(1f);
    //         // Nếu là yield return null; --> Code chạy đến đây thì chờ hết frame rồi mới tiếp tục
    //     }
    // }

    // IEnumerator DoiDemXong()
    // {        
    //     Debug.Log("Bắt đầu đếm:");
    //     cr = StartCoroutine(Dem());
    //     yield return cr;
    //     Debug.Log("Đã đếm xong");
    // }
}
