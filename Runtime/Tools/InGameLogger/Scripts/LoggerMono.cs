using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tools.InGameLogger
{
    public class LoggerMono : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private GameObject container;
        [SerializeField] private TextMeshProUGUI currentTimeTextMeshPro;
        [SerializeField] private TMP_InputField loggerTextMeshPro;
        [SerializeField] private ScrollRect logScrollRect;
        [SerializeField] private Button autoScrollButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button timeButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private GameObject overloadIndicatorGameObject;
        [SerializeField] private Slider textSizeSlider;
#pragma warning restore 649

        public GameObject Container => container;
        public TextMeshProUGUI CurrentTimeTextMeshPro => currentTimeTextMeshPro;
        public TMP_InputField LoggerTextMeshPro => loggerTextMeshPro;
        public ScrollRect LogScrollRect => logScrollRect;
        public Button AutoScrollButton => autoScrollButton;
        public Button TimeButton => timeButton;
        public Button ClearButton => clearButton;
        public GameObject OverloadIndicatorGameObject => overloadIndicatorGameObject;
        public Slider TextSizeSlider => textSizeSlider;

        private Logger _logger;
        private bool _isActive = false;

        private static LoggerMono _instance;
        
        #region Constants
        private const string Password = "53864";
        #endregion

        private readonly List<KeyCode> _keyCodesList = new()
        {
            KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
            KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9
        };

        private Queue<int> _currentPassword = new Queue<int>();

        private void Awake()
        {
            SetActive(_isActive);
            if (!IsInstanceValid())
            {
                Destroy(gameObject);
                return;
            }

            Init();
        }

        private void Init()
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
            _logger = new Logger(this);
            closeButton.onClick.AddListener(() => SetActive(false));
        }
        

        private bool IsInstanceValid()
        {
            return _instance == null;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isActive = false;
                SetActive(_isActive);
            }
            
            for (var index = 0; index < _keyCodesList.Count; index++)
            {
                var keyCode = _keyCodesList[index];
                if (Input.GetKeyDown(keyCode))
                {
                    if (_currentPassword.Count == Password.Length)
                    {
                        _currentPassword.Dequeue();
                    }
                    _currentPassword.Enqueue(index);
                    string result = String.Empty;
                    foreach (var value in _currentPassword)
                    {
                        result += value;
                    }
                    Debug.Log(result);
                }
            }

            if (_currentPassword.Count == Password.Length)
            {
                string result = String.Empty;
                foreach (var value in _currentPassword)
                {
                    result += value;
                }
                
                if (result == Password)
                {
                    _isActive = true;
                    SetActive(_isActive);
                    _currentPassword.Clear();
                }
            }

            if (Input.touchCount >= 4)
            {
                SetActive(Input.touchCount == 5);
            }

            if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.C))
            {
                GUIUtility.systemCopyBuffer = GetString(GUIUtility.systemCopyBuffer);
                Debug.Log("Copied to Clipboard!");
            }
        }

        private void SetActive(bool value)
        {
            container.SetActive(value);
            _isActive = value;
        }

        private string GetString(string str)
        {
            Regex rich = new Regex(@"<[^>]*>");

            if (rich.IsMatch(str))
            {
                str = rich.Replace(str, string.Empty);
            }

            return str;
        }
    }
}