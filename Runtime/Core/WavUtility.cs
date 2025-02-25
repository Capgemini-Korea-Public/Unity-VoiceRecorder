using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    public static void SaveWavFile(AudioClip clip, string filePath)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null");
            return;
        }

        // AudioClip���� PCM ������ ����
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // WAV ���� ������ ����Ʈ �迭�� ��ȯ
        byte[] wavData = ConvertToWav(samples, clip.channels, clip.frequency);

        // ���Ϸ� ����
        File.WriteAllBytes(filePath, wavData);
        Debug.Log("WAV file is Saved : " + filePath);
    }

    public static byte[] ConvertToWav(float[] samples, int channels, int frequency)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            int sampleCount = samples.Length;
            int byteRate = frequency * channels * 2; // 16��Ʈ PCM: 2����Ʈ

            // RIFF ���
            stream.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, 4);
            stream.Write(BitConverter.GetBytes(36 + sampleCount * 2), 0, 4);
            stream.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, 4);

            // fmt ����ûũ
            stream.Write(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, 4);
            stream.Write(BitConverter.GetBytes(16), 0, 4);             // ����ûũ ũ��
            stream.Write(BitConverter.GetBytes((short)1), 0, 2);         // ����� ���� (1 = PCM)
            stream.Write(BitConverter.GetBytes((short)channels), 0, 2);    // ä�� ��
            stream.Write(BitConverter.GetBytes(frequency), 0, 4);          // ���ø� ����Ʈ
            stream.Write(BitConverter.GetBytes(byteRate), 0, 4);           // ����Ʈ����Ʈ
            stream.Write(BitConverter.GetBytes((short)(channels * 2)), 0, 2); // ��� ���� (BlockAlign)
            stream.Write(BitConverter.GetBytes((short)16), 0, 2);          // ���ô� ��Ʈ ��

            // data ����ûũ
            stream.Write(System.Text.Encoding.UTF8.GetBytes("data"), 0, 4);
            stream.Write(BitConverter.GetBytes(sampleCount * 2), 0, 4);

            // PCM ������: float�� 16��Ʈ PCM���� ��ȯ�Ͽ� ���
            for (int i = 0; i < sampleCount; i++)
            {
                // -1 ~ 1 ������ float ���� short ������ ��ȯ
                short intData = (short)(Mathf.Clamp(samples[i], -1f, 1f) * short.MaxValue);
                stream.Write(BitConverter.GetBytes(intData), 0, 2);
            }

            return stream.ToArray();
        }
    }
}
