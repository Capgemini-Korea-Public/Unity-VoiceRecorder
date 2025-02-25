using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyAudioPackage.Core; // �ھ� Ŭ���� ���ӽ����̽�

namespace MyAudioPackage.UI
{
    public class AudioRecorderUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public Button startButton;
        public Button stopButton;
        public Button playButton;
        public TMP_InputField durationInputField;
        public TextMeshProUGUI logText;
        public TextMeshProUGUI maxRecordingDurationText;

        public AudioSource analysisSource;
        public AudioSource playbackSource;

        // �ھ� ���� �ν��Ͻ�: MonoBehaviour�� �������� �ʴ� ���� C# Ŭ����
        private AudioRecorder recorderCore;
        private AudioFileManager fileManagerCore;

        private void Awake()
        {
            recorderCore = new AudioRecorder(analysisSource, playbackSource);
            fileManagerCore = new AudioFileManager();

            // UI ��ư �̺�Ʈ ����
            if (startButton != null)
                startButton.onClick.AddListener(OnStartButtonClicked);
            if (stopButton != null)
                stopButton.onClick.AddListener(OnStopButtonClicked);
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayButtonClicked);

            // �Է� �ʵ� ���� �� ���� �ð� ������Ʈ
            if (durationInputField != null)
                durationInputField.onValueChanged.AddListener(OnDurationInputChanged);

            // �ھ� �̺�Ʈ ����: ���� ����/���� �� �α� �޽��� ����
            recorderCore.OnRecordingStarted += HandleRecordingStarted;
            recorderCore.OnRecordingStopped += HandleRecordingStopped;
            recorderCore.OnLog += AppendLog;
        }

        private async void OnStartButtonClicked()
        {
            // �Էµ� ���� �ð��� ������ �ھ �ݿ�
            if (int.TryParse(durationInputField.text, out int duration))
            {
                recorderCore.SetRecordingDuration(duration);
            }
            // ���� ���� (�񵿱� ȣ��)
            await recorderCore.StartRecordingAsync();
        }

        private void OnStopButtonClicked()
        {
            recorderCore.StopRecording();
        }

        private void OnPlayButtonClicked()
        {
            recorderCore.PlayRecording();
        }

        private void OnDurationInputChanged(string value)
        {
            if (int.TryParse(value, out int duration))
            {
                recorderCore.SetRecordingDuration(duration);
                maxRecordingDurationText.text = $"Max Recording Duration: {duration} seconds";
            }
        }

        private void HandleRecordingStarted()
        {
            AppendLog("Recording started.");
            // UI ������Ʈ: ���� ���, ��ư ���� ���� �� �߰� ����
        }

        private void HandleRecordingStopped()
        {
            AppendLog("Recording stopped.");
            // UI ������Ʈ: ���� ���, ��ư ���� ���� �� �߰� ����
        }

        private void AppendLog(string message)
        {
            if (logText != null)
                logText.text += message + "\n";
            Debug.Log(message);
        }
    }
}
