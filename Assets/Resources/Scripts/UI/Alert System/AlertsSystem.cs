using UnityEngine;
using UnityEngine.Pool;


namespace UI.AlertSystem
{
    public class AlertsSystem : MonoBehaviour
    {
        [SerializeField] private AlertWindow _prefab;
        [SerializeField] private RectTransform _showPosition;
        [SerializeField] private RectTransform _hidePosition;
        [SerializeField] private float _defaultShowDuration = 2f;
        [SerializeField] private float _autoHideDelay = 5f;

        private const int POOL_SIZE = 5;
        private ObjectPool<AlertWindow> _alertPool;

        public static AlertsSystem Instance { get; private set; }


        private void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = this;

            CreatePool();
        }


        private void OnEnable()
        {
            AlertWindow.OnHide += HideAlert;
        }


        private void OnDisable()
        {
            AlertWindow.OnHide -= HideAlert;
        }


        private void OnDestroy()
        {
            _alertPool.Clear();
        }


        private AlertWindow CreateAlertWindow()
        {
            AlertWindow alertWindow = Instantiate(_prefab, transform);
            alertWindow.Initialize(_defaultShowDuration,_autoHideDelay, _showPosition, _hidePosition);
            return alertWindow;
        }


        private void CreatePool()
        {
            _alertPool = new ObjectPool<AlertWindow>(
                createFunc: CreateAlertWindow,
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: POOL_SIZE
            );
        }


        private void HideAlert(AlertWindow alert)
        {
            _alertPool.Release(alert);
        }


        public void InvokeAlert(string message, bool autoHide = true, AlertStatus status = AlertStatus.Success)
        {
            AlertWindow alert = _alertPool.Get();
            alert.Show(message, autoHide, status);
        }


        public void Test(int variant)
        {
            if(variant == 0)
                InvokeAlert("Succes", false, AlertStatus.Success);
            if (variant == 1)
                InvokeAlert("Warning", status: AlertStatus.Warning);
            if (variant == 2)
                InvokeAlert("Error", false, AlertStatus.Error);
        }
    }
}