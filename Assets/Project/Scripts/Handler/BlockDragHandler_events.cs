
using DG.Tweening;
using UnityEngine;


public partial class BlockDragHandler
{
    private Vector3 GetMouseWorldPosition() //마우스 월드 좌표 반환
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = model.zDistanceToCamera;
        return view.maincamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    public void ReleaseInput() // 콜라이더, 리지드 초기화
    {
        if (model.col != null) model.col.enabled = false;
        model.isDragging = false;
        view.outline.enabled = false;
        model.rb.linearVelocity = Vector3.zero;
        model.rb.isKinematic = true;
    }

    private void OnDisable() //애니메이션 제거
    {
        transform.DOKill(true);
    }

    private void OnDestroy() //애니메이션 제거
    {
        transform.DOKill(true);
    }

        private void ResetCollisionState()  // 충돌 상태 초기화
    {
        model.isColliding = false;
        model.lastCollisionNormal = Vector3.zero;
    }
    
    // 충돌 감지
    void OnCollisionEnter(Collision collision) //충돌 처리
    {
        HandleCollision(collision);
    }
    
    void OnCollisionStay(Collision collision)
    {
        HandleCollision(collision);
    }
    
    private void HandleCollision(Collision collision)
    {
        if (!model.isDragging) return;
        
        if (collision.contactCount > 0 && collision.gameObject.layer != LayerMask.NameToLayer("Board"))
        {
            Vector3 normal = collision.contacts[0].normal;
            
            // 수직 충돌(바닥과의 충돌)은 무시
            if (Vector3.Dot(normal, Vector3.up) < 0.8f)
            {
                model.isColliding = true;
                model.lastCollisionNormal = normal;
                model.lastCollisionTime = Time.time; // 충돌 시간 갱신
            }
        }
    }
    
    void OnCollisionExit(Collision collision) //충돌 상태 초기화
    {
        // 현재 충돌 중인 오브젝트가 떨어질 때만 충돌 상태 해제
        if (collision.contactCount > 0)
        {
            Vector3 normal = collision.contacts[0].normal;
            
            // 현재 저장된 충돌 normal과 유사한 경우에만 해제
            if (Vector3.Dot(normal, model.lastCollisionNormal) > 0.8f)
            {
                ResetCollisionState();
            }
        }
    }

    void OnMouseDown() // 드래그 시작
    {
        if (!model.Enabled) return;
        
        model.isDragging = true;
        model.rb.isKinematic = false;
        view.outline.enabled = true;
        
        // 카메라와의 z축 거리 계산
        model.zDistanceToCamera = Vector3.Distance(transform.position, view.maincamera.transform.position);
        
        // 마우스와 오브젝트 간의 오프셋 저장
        model.offset = transform.position - GetMouseWorldPosition();
        
        // 충돌 상태 초기화
        ResetCollisionState();
    }

    void OnMouseUp()// 드래그 종료
    {
        model.isDragging = false;
        view.outline.enabled = false;
        if (!model.rb.isKinematic)
        {
            model.rb.linearVelocity = Vector3.zero;
            model.rb.isKinematic = true;
        }
        SetBlockPosition();
        ResetCollisionState();
    }

    public Vector3 GetCenterX() //중심좌표 계산
    {
        if (model.blocks == null || model.blocks.Count == 0)
        {
            return Vector3.zero; // Return default value if list is empty
        }

        float minX = float.MaxValue;
        float maxX = float.MinValue;

        foreach (var block in model.blocks)
        {
            float blockX = block.transform.position.x;
        
            if (blockX < minX)
            {
                minX = blockX;
            }
        
            if (blockX > maxX)
            {
                maxX = blockX;
            }
        }
    
        // Calculate the middle value between min and max
        return new Vector3((minX + maxX) / 2f, transform.position.y, 0);
    }

    public Vector3 GetCenterZ() //중심좌표 계산
    {
        if (model.blocks == null || model.blocks.Count == 0)
        {
            return Vector3.zero; // Return default value if list is empty
        }

        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        foreach (var block in model.blocks)
        {
            float blockZ = block.transform.position.z;
        
            if (blockZ < minZ)
            {
                minZ = blockZ;
            }
        
            if (blockZ > maxZ)
            {
                maxZ = blockZ;
            }
        }
    
        return new Vector3(transform.position.x, transform.position.y, (minZ + maxZ) / 2f);
    }

    private void ClearPreboardBlockObjects()
    {
        foreach (var b in model.blocks)
        {
            if (b.preBoardBlockObject != null)
            {
                b.preBoardBlockObject.playingBlock = null;
            }
        }
    }
    public void DestroyMove(Vector3 pos, ParticleSystem particle)
    {
        ClearPreboardBlockObjects();
        
        transform.DOMove(pos, 1f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Destroy(particle.gameObject);
                Destroy(gameObject);
                //block.GetComponent<BlockShatter>().Shatter();
            });
    }
}
