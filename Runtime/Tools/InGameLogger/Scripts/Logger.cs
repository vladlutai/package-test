using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Tools.InGameLogger
{
    public class Logger
    {
        private readonly LoggerMono _loggerMono;

        #region Constants
        private const string FullLogFormat = "<b><u>{0:HH:mm:ss tt} [{1}]:</b></u> <size=85%>{2} {3}</size>\r";
        private const string ShortLogFormat = "<b><u>{0:HH:mm:ss tt} [{1}]:</b></u> <size=85%>{2}</size>\r";

        private readonly Dictionary<LogType, string> _colorByLogTypeDictionary = new()
        {
            [LogType.Log] = "green",
            [LogType.Warning] = "yellow",
            [LogType.Error] = "red",
            [LogType.Exception] = "red",
            [LogType.Assert] = "red",
        };
        #endregion
        private bool _isOverload = false;
        private bool _isActive;
        private bool _isTimePaused = false;
        private bool _isAutoScrollActive = true;
        private Coroutine _processCoroutine;

        public Logger(LoggerMono loggerMono)
        {
            _loggerMono = loggerMono;
            Init();
        }

        private void Init()
        {
            Application.logMessageReceived += OnLogMessageReceived;
            _loggerMono.AutoScrollButton.onClick.AddListener(() =>
            {
                _isAutoScrollActive = !_isAutoScrollActive;
                Color color = _loggerMono.AutoScrollButton.image.color;
                color.a = _isAutoScrollActive ? 1 : 0.5f;
                _loggerMono.AutoScrollButton.image.color = color;
            });
            _loggerMono.TimeButton.onClick.AddListener(() =>
            {
                _isTimePaused = !_isTimePaused;
                Time.timeScale = _isTimePaused ? 0 : 1;
            });
            _loggerMono.ClearButton.onClick.AddListener(() =>
            {
                _loggerMono.LoggerTextMeshPro.text = "";
                _isOverload = false;
                _loggerMono.OverloadIndicatorGameObject.SetActive(false);
            });
            _isActive = true;
            _processCoroutine = _loggerMono.StartCoroutine(Process());
            _loggerMono.TextSizeSlider.onValueChanged.AddListener((value) => _loggerMono.LoggerTextMeshPro.textComponent.fontSize = value);
        }

        private void OnLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if(_isOverload)
                return;
            string messageToWrite =
                string.Format(type is LogType.Exception or LogType.Error or LogType.Assert ? FullLogFormat : ShortLogFormat, System.DateTime.Now,
                    System.Enum.GetName(typeof(LogType), type),
                    condition, stacktrace);
            string colorfulMessage = $"<color={_colorByLogTypeDictionary[type]}>{messageToWrite}</color>";
            _loggerMono.LoggerTextMeshPro.text += colorfulMessage + '\n';
            if (_loggerMono.LoggerTextMeshPro.text.Length > 19000)
            {
                _isOverload = true;
                _loggerMono.OverloadIndicatorGameObject.SetActive(true);
            }
        }

        private IEnumerator Process()
        {
            while (_isActive)
            {
                yield return new WaitForEndOfFrame();
                _loggerMono.CurrentTimeTextMeshPro.text = System.DateTime.Now.ToString("HH:mm:ss tt", CultureInfo.InvariantCulture);
                AutoScroll();
            }
        }

        private void AutoScroll()
        {
            if (!_isAutoScrollActive)
                return;
            _loggerMono.LogScrollRect.verticalNormalizedPosition = 0;
        }
    }
}