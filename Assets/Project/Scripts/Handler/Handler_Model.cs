
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

    public class Handler_Model : MonoBehaviour 
    {
        public int horizon = 1;
        public int vertical = 1;
        public int uniqueIndex;
        public List<ObjectPropertiesEnum.BlockGimmickType> gimmickType;
        public List<BlockObject> blocks = new List<BlockObject>();
        public List<Vector2> blockOffsets = new List<Vector2>();
        public bool Enabled = true;
        
        private Vector2 CenterPos;
        public Vector2 centerPos {get => CenterPos; set => CenterPos = value; }
        private bool isdragging = false;
        public bool isDragging {get => isdragging; set => isdragging = value; }
        private Vector3 Offset {get; set;}
        public Vector3 offset {get => Offset; set => Offset = value; }
        private float zdistanceToCamera;
        public float zDistanceToCamera {get => zdistanceToCamera; set => zdistanceToCamera = value; }
        private float MaxSpeed = 20f;
        public float maxSpeed {get => MaxSpeed;  set => MaxSpeed = value;}
        private Vector2 previousXY {get; set;}
        private Rigidbody Rigid;
        public Rigidbody rb {get => Rigid; set => Rigid = value; }

        // 충돌 감지 변수
        public Collider col;
        private bool iscolliding = false;
        public bool isColliding {get => iscolliding; set => iscolliding = value; }
        private Vector3 LastCollisionNormal;
        public Vector3 lastCollisionNormal {get => LastCollisionNormal; set => LastCollisionNormal = value; }
        private float CollisionResetTime = 0.1f; // 충돌 상태 자동 해제 시간
        public float collisionResetTime {get => CollisionResetTime; set => CollisionResetTime = value; }
        private float LastCollisionTime;  
        public float lastCollisionTime {get => LastCollisionTime; set => LastCollisionTime = value; }
        private float MoveSpeed = 25f;           
        public float moveSpeed {get => MoveSpeed;  set => MoveSpeed = value;}
        private float FollowSpeed = 30f;   
        public float followSpeed {get => FollowSpeed;  set => FollowSpeed = value;}
           
    }
