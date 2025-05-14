using UnityEngine; 
    public class Handler_View : MonoBehaviour
    {
        private Camera MainCamera;
        public Camera maincamera {get => MainCamera; set => MainCamera = value; }
        
        private Outline Outline {get; set;}
        public Outline outline {get => Outline; set => Outline = value; }


    }
