using System.Collections;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public SpawnGameObject bulletSpawner; //gọi component script SpawnGameObject => kéo gameobject Spawner chứa script SpawnGameObject vào
                                         //câu lệnh bật nó lên nằm ở đoạn code dưới cùng 
                                         //kéo Spawner vào thành con của nòng súng của Tank
                                         //reset transform của Spawner
    Vector3 target = new Vector3(0, 0, 10f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // StartCoroutine(MoveTank(transform, target, 5f));
        // StartCoroutine(DiChuyenRoiBanDan());
        StartCoroutine(TankAction());
    }

    // Update is called once per frame
    void Update()
    {
        // Cách viết để di chuyển càng sát đích càng chậm dần
        // transform.position = Vector3.Lerp(transform.position,target,5f*Time.deltaTime);
    }

    IEnumerator DiChuyenRoiBanDan()
    {
        yield return StartCoroutine(MoveTank(transform, target, 5f));

        // transform.Rotate(0f,-90f,0f); // Cách ngắn - ko cần dùng Quaternion
        transform.rotation = Quaternion.Euler(0f, -90f, 0f);

        yield return new WaitForSeconds(1f);

        Debug.Log("Đã bắn đạn");
    }

    // Cách viết để di chuyển đến đích với tốc độ đều (tốc độ không đổi):
    IEnumerator MoveTank(Transform tank, Vector3 target, float duration)
    {
        Vector3 startPos = tank.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            tank.position = Vector3.Lerp(startPos, target, elapsedTime / duration); // hàm nội suy tuyến tính
            elapsedTime += Time.deltaTime;
            // Debug.Log(tank.position.z); // in vị trí của object ra console
            yield return null; // Qua 1 frame thì chạy tiếp
        }

        tank.position = target; // để vị trí cuối cùng của tank = 10 (loại bỏ sai số nhỏ mà Lerp tạo ra)
    }

    IEnumerator RotateTank()
    {
        Quaternion start = transform.rotation;
        Quaternion end = start * Quaternion.Euler(0, 90f, 0);

        float elapsedTime = 0;
        float duration = 2f;

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime; // cộng dần thời gian lên theo frame
            yield return null;
        }

        transform.rotation = end;
    }

    IEnumerator TankAction()
    {
        Debug.Log("Start sau 1s");
        yield return new WaitForSeconds(1f);
        Debug.Log("Bắt đầu di chuyển");
        yield return StartCoroutine(MoveTank(transform, target, 5f)); //Đợi object đến đích rồi mới chạy code tiếp theo
        Debug.Log("Đã đến nơi, chờ 1s");
        yield return new WaitForSeconds(1f);
        Debug.Log("Bắt đầu xoay");
        yield return StartCoroutine(RotateTank());
        Debug.Log("Đã xoay xong - chờ 1s");
        yield return new WaitForSeconds(1f);
        Debug.Log("Attack");

        yield return bulletSpawner.enabled = true; //thêm dòng này để bật script spam viên đạn

    }
}
