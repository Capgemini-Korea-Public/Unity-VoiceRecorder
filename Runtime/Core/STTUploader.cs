using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MyAudioPackage.Core
{
    /// <summary>
    /// STTUploaderCore�� WAV ������ ����Ʈ �����͸� STT ������ ���ε��ϴ� ���� C# ��ƿ��Ƽ Ŭ�����Դϴ�.
    /// MonoBehaviour�� �������� �ʰ�, async/await ������� �񵿱� ���ε带 �����մϴ�.
    /// </summary>
    public static class STTUploader
    {
        /// <summary>
        /// WAV ������ ����Ʈ �����͸� �־��� URL�� POST ��� ���ε��մϴ�.
        /// ���� �� ���� �ؽ�Ʈ�� ��ȯ�ϸ�, ���� �� ���ܸ� �߻���ŵ�ϴ�.
        /// </summary>
        /// <param name="wavData">���ε��� WAV ������ ����Ʈ �迭</param>
        /// <param name="url">���ε��� ������ URL</param>
        /// <returns>������ ���� �ؽ�Ʈ</returns>
        public static async Task<string> UploadWavBytesAsync(byte[] wavData, string url)
        {
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(wavData);
                request.uploadHandler.contentType = "audio/wav";
                request.downloadHandler = new DownloadHandlerBuffer();

                // UnityWebRequestAsyncOperation�� ��ٸ��� ���� Task.Yield()�� ����մϴ�.
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.text;
                }
                else
                {
                    throw new Exception("STT Upload Failed: " + request.error);
                }
            }
        }
    }
}
