using DG.Tweening;
using UnityEngine;

public class TapaOlla : MonoBehaviour
{
    private Olla olla;
    public Transform dropPosition;
    private Vector3 startPosition;
    private bool doOnce1;
    private bool doOnce2;
    private void Awake()
    {
        olla = GetComponentInParent<Olla>();
        
        doOnce1 = true;
        doOnce2 = true;
    }

    void Update()
    {
        if(olla.firstFood && doOnce1)
        {
            TakeOutLid();
            doOnce1 = false;
        }
        if(olla.lastFood && doOnce2)
        {
            TakeInLid();
            doOnce2 = false;
        }

    }

    public void TakeOutLid()
    {
        startPosition = transform.position;
        Sequence TakeOut = DOTween.Sequence();
        TakeOut.Append(transform.DOMoveY(transform.position.y + 0.15f, 0.5f));
        TakeOut.Append(transform.DOMoveX(dropPosition.position.x, 0.5f));
        TakeOut.Append(transform.DOMove(dropPosition.position, 0.5f));
        
    }

    public void TakeInLid()
    {
        Sequence TakeIn = DOTween.Sequence();
        TakeIn.Append(transform.DOMoveY(startPosition.y + 0.15f, 0.5f));
        TakeIn.Append(transform.DOMoveX(startPosition.x, 0.5f));
        TakeIn.Append(transform.DOMove(startPosition, 0.5f));
    }

    
}
