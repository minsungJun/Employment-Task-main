
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public partial class BlockDragHandler : MonoBehaviour
{
    [SerializeField] private Handler_Model _model;
    [SerializeField] private Handler_View _view;

    public Handler_Model model => _model;
    public Handler_View view => _view;
    
    

    void Start()
    {
        view.maincamera = Camera.main;
        model.rb = GetComponent<Rigidbody>();
        
        model.rb.useGravity = false;
        model.rb.isKinematic = true;
        model.rb.interpolation = RigidbodyInterpolation.Interpolate;
        model.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 충돌 감지 모드 향상
        
        view.outline = gameObject.AddComponent<Outline>();
        view.outline.OutlineMode = Outline.Mode.OutlineAll;
        view.outline.OutlineColor = Color.yellow;
        view.outline.OutlineWidth = 2f;
        view.outline.enabled = false;

        //model.moveSpeed = 30f;
    }


    
    void Update()
    {
        // 충돌 상태 자동 해제 검사
        if (model.isColliding && Time.time - model.lastCollisionTime > model.collisionResetTime)
        {
            // 일정 시간 동안 충돌 갱신이 없으면 충돌 상태 해제
            ResetCollisionState();
        }
    }

    void FixedUpdate()
    {
        if (!model.Enabled || !model.isDragging) return;
        
        SetBlockPosition(false);
        
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 targetPosition = mouseWorldPos + model.offset;
        Vector3 moveVector = targetPosition - transform.position;

        //Debug.Log(mouseWorldPos + " " + targetPosition + " " + moveVector);
    
        // 충돌 상태에서 마우스가 충분히 멀어지면 충돌 상태 해제
        float distanceToMouse = Vector3.Distance(transform.position, targetPosition);
        if (model.isColliding && distanceToMouse > 0.5f)
        {
            if (Vector3.Dot(moveVector.normalized, model.lastCollisionNormal) > 0.1f)
            {
                ResetCollisionState();
            }
        }

        // 속도 계산 개선
        Vector3 velocity;
        if (model.isColliding)
        {
            // 충돌면에 대해 속도 투영 (실제 이동)
            Vector3 projectedMove = Vector3.ProjectOnPlane(moveVector, model.lastCollisionNormal);
            
            velocity = projectedMove * model.moveSpeed;
        }
        else
        {
            velocity = moveVector * model.followSpeed;
        }
    
        // 속도 제한
        if (velocity.magnitude > model.maxSpeed)
        {
            velocity = velocity.normalized * model.maxSpeed;
        }
        
        if(!model.rb.isKinematic)
        {
            model.rb.linearVelocity = Vector3.Lerp(model.rb.linearVelocity, velocity, Time.fixedDeltaTime * 10f);
            
        } 
    }
    
    
    
    private void SetBlockPosition(bool mouseUp = true) // 블록의 위치를 조정
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 coordinate = hit.transform.position;
            
            Vector3 targetPos = new Vector3(coordinate.x, transform.position.y, coordinate.z);
            if(mouseUp) transform.position = targetPos;
            
            Vector2 pos = model.centerPos;

            pos.x = Mathf.Round(transform.position.x / 0.79f);
            pos.y = Mathf.Round(transform.position.z / 0.79f);

            model.centerPos = pos;
            
            if (hit.collider.TryGetComponent(out BoardBlockObject boardBlockObject))
            {
                foreach (var blockObject in model.blocks)
                {
                    blockObject.SetCoordinate(model.centerPos);
                }
                foreach (var blockObject in model.blocks)
                {
                    boardBlockObject.CheckAdjacentBlock(blockObject, targetPos);
                    blockObject.CheckBelowBoardBlock(targetPos);
                }
            }
        }
        else
        {
            Debug.LogWarning("Nothing Detected");
        }
    }
    
    
    
   
    

    
    


    
}