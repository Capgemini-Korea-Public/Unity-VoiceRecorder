using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class FFmpegConverter
{
    /// <summary>
    /// FFmpeg�� ����Ͽ� WAV ������ OGG ���Ϸ� ��ȯ�մϴ�.
    /// FFmpeg�� �ý��� PATH�� �ְų�, ffmpegPath�� FFmpeg ���� ������ ��ü ��θ� �����ؾ� �մϴ�.
    /// </summary>
    /// <param name="wavFilePath">��ȯ�� ���� WAV ������ ��ü ���</param>
    /// <param name="oggFilePath">������ OGG ������ ��ü ���</param>
    /// <param name="ffmpegPath">FFmpeg ���� ���� ��� (PATH�� �ִٸ� "ffmpeg"�� �Է�)</param>
    /// 


    public static void ConvertWavToOgg(string wavFilePath, string oggFilePath, string ffmpegPath)
    {
        if (!File.Exists(wavFilePath))
        {
            Debug.LogError("WAV file does not exist: " + wavFilePath);
            return;
        }
        if (!File.Exists(ffmpegPath))
        {
            Debug.LogError("FFmpeg executable does not exist: " + ffmpegPath);
        }

        // FFmpeg ��ɾ�: -y (�ڵ� �����), -i "input.wav" "output.ogg"
        string arguments = $"-y -i \"{wavFilePath}\" \"{oggFilePath}\"";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath, // ��: "ffmpeg" �Ǵ� "C:\\Path\\To\\ffmpeg.exe"
            Arguments = arguments,
            CreateNoWindow = true,    // �ܼ� â�� �������� ����
            UseShellExecute = false,  // Shell ��� �� ��
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        try
        {
            using (Process process = Process.Start(startInfo))
            {
                // FFmpeg�� ó���ϴ� ���� ��� �� ���� �޽����� �н��ϴ�.
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                Debug.Log("FFmpeg output: " + output);
                if (!string.IsNullOrEmpty(error))
                    Debug.LogWarning("FFmpeg error: " + error);

                if (process.ExitCode == 0)
                {
                    Debug.Log("Conversion successful: " + oggFilePath);
                }
                else
                {
                    Debug.LogError("FFmpeg conversion failed. Exit code: " + process.ExitCode);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception during FFmpeg execution: " + ex.Message);
        }
    }
}
