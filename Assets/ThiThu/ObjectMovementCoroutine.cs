using System.Collections;
using UnityEngine;

public class ObjectMovementCoroutine : MonoBehaviour
{
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float moveDuration = 3f;
    [SerializeField] private float waitDuration = 1f;
    [SerializeField] private float rotationDuration = 2f;
    [SerializeField] private float rotationAngle = 180f;

    private void Start()
    {
        StartCoroutine(MoveAndRotateSequence());
    }

    private IEnumerator MoveAndRotateSequence()
    {
        // 1. Move from current position (Point A) to target position (Point B)
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        // 2. Wait at target position for waitDuration (1 second)
        yield return new WaitForSeconds(waitDuration);

        // 3. Rotate continuously by rotationAngle (180 degrees) over rotationDuration (2 seconds)
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, rotationAngle, 0f);
        elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;

        // 4. Log "Completed" when finished
        Debug.Log("Completed");
    }
}
