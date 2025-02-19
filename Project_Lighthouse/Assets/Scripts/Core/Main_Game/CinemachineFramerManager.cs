using UnityEngine;

public class CinemachineFramerManager : MonoBehaviour
{
    //FRAMERS
    public GameObject activeFramer;
    public Transform FramerBounds1;
    public Transform FramerBounds2;
    public Vector3 FramerBoundsStartPosition1;
    public Vector3 FramerBoundsStartPosition2;

    //CURTAIN
    /*[SerializeField] private GameObject Curtain;
    [SerializeField] private GameObject CurtainCloser;
    private Vector3 originalCurtainSize;
    private Vector3 originalCurtainCloserSize;*/

    void Start()
    {
        FramerBoundsStartPosition1 = FramerBounds1.localPosition;
        FramerBoundsStartPosition2 = FramerBounds2.localPosition;
        /*originalCurtainCloserSize = CurtainCloser.transform.localScale / Camera.main.orthographicSize;
        originalCurtainSize = Curtain.transform.localScale / Camera.main.orthographicSize;*/

    }

    void Update()
    {
        if (activeFramer != null)
        {
            FramerBounds1.position = activeFramer.transform.GetChild(0).position;
            FramerBounds2.position = activeFramer.transform.GetChild(1).position;
        }
        //AdaptCurtainToCamera();
    }

    public void ToggleFramer(GameObject framer)
    {
        activeFramer = framer;
    }

    public void UntoggleFramer()
    {
        activeFramer = null;
        FramerBounds1.localPosition = FramerBoundsStartPosition1;
        FramerBounds2.localPosition = FramerBoundsStartPosition2;
    }

    /*void AdaptCurtainToCamera()
    {
        Curtain.transform.localScale = originalCurtainSize * Camera.main.orthographicSize;
        if (!player.CurtainCloserUsedByTween)
        {
            CurtainCloser.transform.localScale = originalCurtainCloserSize * Camera.main.orthographicSize;
        }
    }*/

}
