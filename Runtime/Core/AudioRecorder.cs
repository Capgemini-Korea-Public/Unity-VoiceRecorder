using System;
using System.Threading.Tasks;
using UnityEngine;

namespace MyAudioPackage.Core
{
    public class AudioRecorder
    {
        // �̺�Ʈ: �ܺ�(UI ����)���� �����Ͽ� ���� ��ȭ�� ���� UI ������Ʈ ����
        public event Action OnRecordingStarted;
        public event Action OnRecordingStopped;
        public event Action<string> OnLog;

        public static AudioSource CurrentRecordingSource { get; private set; }

        // ���� ����
        public int RecordingDuration { get; private set; } = 10; // �⺻ 10��, 1~30�� ����
        public int Frequency { get; private set; } = 44100;
        public bool IsRecording { get; private set; } = false;
        public AudioClip RecordedClip { get; private set; } = null;
        public int LastSample { get; private set; } = 0;

        // �ܺο��� ���Թ��� AudioSource (�м���, �����)
        public AudioSource AnalysisSource { get; private set; }
        public AudioSource PlaybackSource { get; private set; }
        public SaveMode RecordingSaveMode { get; set; } = SaveMode.SaveWav;

        // ���������� ����� ����ũ ��ġ
        private string selectedDevice = null;
        private string microphoneDevice = null;

        public AudioRecorder(AudioSource analysisSource, AudioSource playbackSource)
        {
            AnalysisSource = analysisSource;
            PlaybackSource = playbackSource;
        }
        /// <summary>
        /// ����ũ ��ġ�� �����մϴ�.
        /// </summary>
        public void SetDevice(string deviceName)
        {
            selectedDevice = deviceName;
            Log($"Device set to: {selectedDevice}");
        }


        /// <summary>
        /// ���� �ð��� �����մϴ�. (1 ~ 30��)
        /// </summary>
        public void SetRecordingDuration(int duration)
        {
            RecordingDuration = Mathf.Clamp(duration, 1, 30);
            Log($"Recording duration set to {RecordingDuration} seconds.");
        }

        /// <summary>
        /// ������ �����մϴ�. (�񵿱� ���)
        /// </summary>
        public async Task StartRecordingAsync()
        {
            if (IsRecording)
            {
                Log("Recording is already in progress.");
                return;
            }

            // ����ũ ��ġ�� ���õ��� ���� ��� �⺻ ��ġ�� ���
            if (string.IsNullOrEmpty(selectedDevice))
            {
                string[] devices = Microphone.devices;
                if (devices.Length > 0)
                {
                    selectedDevice = devices[0];
                    Log($"Automatically selected default microphone device: {selectedDevice}");
                }
                else
                {
                    Log("No microphone devices found.");
                    return;
                }
            }

            microphoneDevice = selectedDevice;

            // Microphone.Start: (��ġ, loop, �ִ� ���� �ð�, ���÷���Ʈ)
            RecordedClip = Microphone.Start(microphoneDevice, true, RecordingDuration, Frequency);
            Log("Recording started.");

            // �м��� AudioSource ����: (�ִٸ�)
            if (AnalysisSource != null)
            {
                AnalysisSource.clip = RecordedClip;
                AnalysisSource.loop = true;
                AnalysisSource.mute = true;
                AnalysisSource.Play();
            }

            CurrentRecordingSource = AnalysisSource;

            // ���� ���� ���۱��� ��� (Microphone.GetPosition > 0)
            while (Microphone.GetPosition(microphoneDevice) <= 0)
            {
                await Task.Yield();
            }

            IsRecording = true;
            OnRecordingStarted?.Invoke();

            // ���� ���� �ð���ŭ ��� �� �ڵ� ����
            await Task.Delay(RecordingDuration * 1000);
            StopRecording();
        }

        /// <summary>
        /// ������ �����մϴ�.
        /// </summary>
        public void StopRecording()
        {
            if (!IsRecording)
            {
                Log("Not currently recording.");
                return;
            }

            LastSample = Microphone.GetPosition(microphoneDevice);
            Microphone.End(microphoneDevice);
            IsRecording = false;
            Log("Recording stopped.");

            // �м��� AudioSource ����
            if (AnalysisSource != null && AnalysisSource.isPlaying)
                AnalysisSource.Stop();

            // ����� AudioSource�� ���� Ŭ�� �Ҵ� �� ��� (�ִٸ�)
            if (PlaybackSource != null && RecordedClip != null)
            {
                PlaybackSource.clip = RecordedClip;
                PlaybackSource.loop = false;
                PlaybackSource.Play();
            }

            OnRecordingStopped?.Invoke();

            if (RecordingSaveMode != SaveMode.None && RecordedClip != null)
            {
                SaveRecording(RecordingSaveMode);
            }
        }

        /// <summary>
        /// ������ Ŭ���� ����մϴ�.
        /// </summary>
        public void PlayRecording()
        {
            if (RecordedClip == null)
            {
                Log("No recorded clip available for playback.");
                return;
            }

            if (PlaybackSource != null)
            {
                PlaybackSource.clip = RecordedClip;
                PlaybackSource.loop = false;
                PlaybackSource.Play();
                Log("Playback started.");
            }
            else
            {
                Log("Playback source is not set.");
            }
        }

        /// <summary>
        /// SaveMode�� ���� ������ AudioClip�� ���Ϸ� �����մϴ�.
        /// </summary>
        public void SaveRecording(SaveMode mode)
        {
            // AudioFileManagerCore�� ���� ���� �� ��ȯ ����� ����ϴ� ���� C# Ŭ�����Դϴ�.
            AudioFileManager fileManager = new AudioFileManager();

            switch (mode)
            {
                case SaveMode.SaveWav:
                    fileManager.SaveAsWav(RecordedClip, LastSample);
                    break;
                case SaveMode.SaveOgg:
                    fileManager.SaveAsOgg(RecordedClip, LastSample);
                    break;
                case SaveMode.None:
                default:
                    Log("No save operation performed (SaveMode.None).");
                    break;
            }
        }

        /// <summary>
        /// �α� �޽����� �̺�Ʈ�� �����ϰ� �ֿܼ��� ����մϴ�.
        /// </summary>
        private void Log(string message)
        {
            OnLog?.Invoke(message);
            Debug.Log(message);
        }
    }
}
