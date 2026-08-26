using UnityEngine;

public class FallTargetManager : MonoBehaviour
{
    public GameObject tableTop;
    public Ball3D ball;
    private Collider _tableTopCollider;
    private float _fallTargetOffsetY = 0.001f;
    private Renderer _thisRenderer = null;
    
    void Start()
    {
        _tableTopCollider = tableTop.gameObject.GetComponent<Collider>();
        _thisRenderer = this.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var fallPosition = ball.PredictFallPosition();
        if (fallPosition.x > _tableTopCollider.bounds.max.x || fallPosition.x < -_tableTopCollider.bounds.max.x ||
            fallPosition.z > _tableTopCollider.bounds.max.z || fallPosition.z < -_tableTopCollider.bounds.max.z ||
            ball.ServeBind)
        {
            _thisRenderer.enabled = false;
            return;
        }
        fallPosition.y = _tableTopCollider.bounds.max.y + _fallTargetOffsetY;
        this.transform.position = fallPosition;
        _thisRenderer.enabled = true;
    }
}
