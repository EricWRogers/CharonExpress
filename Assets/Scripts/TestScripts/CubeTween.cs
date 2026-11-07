using DG.Tweening;
using UnityEngine;

public class CubeTween : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //transform.DOMove(new Vector3(0f, 1f, 0f), 1.0f);
        //transform.DOLocalMoveY(1f, 1f).OnComplete(DestroySelf);

        TweenHelper.PunchUp(transform);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
