using System.IO;
using UnityEngine;

namespace MyAudioPackage.Core
{
    /// <summary>
    /// AudioFileManagerCore�� WAV ���� ���� �� OGG ��ȯ ����� ���� C#���� ������ Ŭ�����Դϴ�.
    /// MonoBehaviour�� �������� �ʰ�, �����ڿ��� ���� ��� ���� �����մϴ�.
    /// </summary>
    public class AudioFileManager
    {
        /// <summary>
        /// ������ WAV ������ ��ġ�� ���� ����Դϴ�.
        /// �⺻������ Application.persistentDataPath�� ����մϴ�.
        /// </summary>
        public string WavFolderPath { get; private set; }

        /// <summary>
        /// FFmpeg ���� ������ ��ü ����Դϴ�.
        /// </summary>
        public string FfmpegPath { get; private set; }

        /// <summary>
        /// FFmpeg ���� ������ ��� ���. �ʿ信 ���� ������ �� �ֽ��ϴ�.
        /// </summary>
        public string FfmpegRelativePath { get; set; } = "Plugin/FFmpeg/bin/ffmpeg.exe";

        /// <summary>
        /// �����ڿ��� ���� ��ο� FFmpeg ��θ� �����մϴ�.
        /// </summary>
        public AudioFileManager()
        {
            // Application.persistentDataPath�� �� �÷����� �´� ���� ���� ����Դϴ�.
            WavFolderPath = Application.persistentDataPath;
            // ���� ������ ������ �����մϴ�.
            if (!Directory.Exists(WavFolderPath))
                Directory.CreateDirectory(WavFolderPath);

            // Application.dataPath�� Assets ������ ��θ� ��Ÿ����, FFmpegRelativePath�� �����Ͽ� FFmpeg ���� ������ ��θ� ����ϴ�.
            FfmpegPath = Path.Combine(Application.dataPath, FfmpegRelativePath);
        }

        /// <summary>
        /// AudioClip�� �Ϻ�(������ ���ñ���)�� �߶� WAV ���Ϸ� �����մϴ�.
        /// </summary>
        /// <param name="clip">������ AudioClip</param>
        /// <param name="lastSample">���� ������ ������ ���� ��ġ</param>
        public void SaveAsWav(AudioClip clip, int lastSample)
        {
            int channels = clip.channels;
            int frequency = clip.frequency;

            // ���� ������ �����͸�ŭ�� ���� �迭�� �����մϴ�.
            float[] samples = new float[lastSample * channels];
            clip.GetData(samples, 0);

            // ������ �����͸� �����ϴ� �� AudioClip�� �����մϴ�.
            AudioClip trimmedClip = AudioClip.Create(clip.name + "_trimmed", lastSample, channels, frequency, false);
            trimmedClip.SetData(samples, 0);

            // ������ WAV ���� ��θ� �����մϴ�.
            string wavFilePath = Path.Combine(WavFolderPath, clip.name + ".wav");
            // WavUtility�� ������ �ۼ��� WAV ��ȯ ����� Ŭ�����Դϴ�.
            WavUtility.SaveWavFile(trimmedClip, wavFilePath);
            Debug.Log("WAV file saved: " + wavFilePath);
        }

        /// <summary>
        /// AudioClip�� WAV ���Ϸ� ������ ��, FFmpeg�� ����Ͽ� OGG ���Ϸ� ��ȯ�մϴ�.
        /// ��ȯ ��, ���� WAV ������ �����մϴ�.
        /// </summary>
        /// <param name="clip">������ AudioClip</param>
        /// <param name="lastSample">���� ������ ������ ���� ��ġ</param>
        public void SaveAsOgg(AudioClip clip, int lastSample)
        {
            // ���� WAV ���Ϸ� �����մϴ�.
            SaveAsWav(clip, lastSample);

            // ������ WAV ���� ��θ� �����ɴϴ�.
            string wavFilePath = Path.Combine(WavFolderPath, clip.name + ".wav");
            Debug.Log("WAV file saved: " + wavFilePath);

            // ��ȯ�� OGG ���� ��θ� �����մϴ�.
            string oggFilePath = Path.Combine(WavFolderPath, clip.name + ".ogg");
            // FFmpegConverter�� FFmpeg�� ����Ͽ� ���� ��ȯ�� �����ϴ� ����� Ŭ�����Դϴ�.
            FFmpegConverter.ConvertWavToOgg(wavFilePath, oggFilePath, FfmpegPath);

            // ��ȯ �� ���� WAV ���� ����
            if (File.Exists(wavFilePath))
            {
                File.Delete(wavFilePath);
                Debug.Log("Deleted WAV file: " + wavFilePath);
            }
        }
    }
}
