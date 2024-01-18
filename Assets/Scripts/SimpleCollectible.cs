using UnityEngine;

namespace DefaultNamespace
{
    public class SimpleCollectible : MonoBehaviour, ICollectbile
    {
        [SerializeField] private float duration;
        [SerializeField] private AnimationCurve ease;

        private Vector3 _startingScale;
        private bool _isActive; // i'm just too lazy right now to import dotween to this project sry.
        private float _startTime;
        
        public void OnCollect()
        {
            _startTime = Time.time;
            _startingScale = transform.localScale;
            _isActive = true;
        }

        private void Update()
        {
            if (_isActive)
            {
                transform.localScale = Vector3.Lerp(_startingScale, Vector3.zero, ease.Evaluate((Time.time - _startTime) / duration));
                
                if (Time.time - _startTime >= duration)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}