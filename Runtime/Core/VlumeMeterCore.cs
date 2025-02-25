using System;
using UnityEngine;

namespace MyAudioPackage.Core
{
    /// <summary>
    /// VolumeMeterCore�� AudioClip�� �ֱ� ���õ��� �м��Ͽ� ���� ����(dB)�� ����ϴ� ���� C# Ŭ�����Դϴ�.
    /// �� Ŭ������ UI�� ���ӵ��� ������, ���� ���� ����� ��ȯ�մϴ�.
    /// </summary>
    public class VolumeMeter
    {
        /// <summary>
        /// �� ���� ó���� �ִ� ���� �� (�⺻��: 1024)
        /// </summary>
        public int SampleWindow { get; set; } = 1024;

        // ���������� ó���� ���� ��ġ
        private int lastSamplePos = 0;

        // �м��� AudioClip (������ Ŭ��)
        public AudioClip Clip { get; set; }

        /// <summary>
        /// ������: AudioClip�� (����������) SampleWindow�� �����մϴ�.
        /// </summary>
        /// <param name="clip">�м��� AudioClip</param>
        /// <param name="sampleWindow">�� ���� ó���� ���� ��</param>
        public VolumeMeter(AudioClip clip, int sampleWindow = 1024)
        {
            Clip = clip;
            SampleWindow = sampleWindow;
            lastSamplePos = 0;
        }

        /// <summary>
        /// AudioClip�� �ֱ� ���õ��� �м��Ͽ� dB ���� ����մϴ�.
        /// �� �޼���� Microphone.GetPosition�� ȣ���Ͽ� ���� ���� ��ġ�� �������� �� ������ ó���մϴ�.
        /// </summary>
        /// <returns>���� ���� (dB ����)</returns>
        public float UpdateVolume()
        {
            if (Clip == null)
            {
                Debug.LogWarning("VolumeMeterCore: AudioClip is null.");
                return -80f;
            }

            int currentPos = Microphone.GetPosition(null);
            if (currentPos < 0)
            {
                return -80f;
            }

            int newSamples = 0;
            if (currentPos >= lastSamplePos)
                newSamples = currentPos - lastSamplePos;
            else
                newSamples = (Clip.samples - lastSamplePos) + currentPos;

            newSamples = Mathf.Max(newSamples, 0);
            newSamples = Mathf.Min(newSamples, SampleWindow);

            float dB = -80f;
            if (newSamples > 0)
            {
                float[] samples = new float[newSamples * Clip.channels];

                if (lastSamplePos + newSamples <= Clip.samples)
                {
                    Clip.GetData(samples, lastSamplePos);
                }
                else
                {
                    int firstPart = Clip.samples - lastSamplePos;
                    float[] samplesPart1 = new float[firstPart * Clip.channels];
                    float[] samplesPart2 = new float[(newSamples - firstPart) * Clip.channels];
                    Clip.GetData(samplesPart1, lastSamplePos);
                    Clip.GetData(samplesPart2, 0);
                    samplesPart1.CopyTo(samples, 0);
                    samplesPart2.CopyTo(samples, firstPart * Clip.channels);
                }

                // RMS ���
                float sum = 0f;
                foreach (float sample in samples)
                {
                    sum += sample * sample;
                }
                float rms = Mathf.Sqrt(sum / samples.Length);
                dB = 20f * Mathf.Log10(rms + 1e-6f);
            }

            lastSamplePos = currentPos;
            return dB;
        }
    }
}
