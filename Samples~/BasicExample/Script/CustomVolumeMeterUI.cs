using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyAudioPackage.Core; // �ھ� ���ӽ����̽�

namespace MyAudioPackage.UI
{
    /// <summary>
    /// CustomVolumeMeterUI�� ������ ���۵Ǿ� AudioSource�� ���޵Ǹ�
    /// �ش� AudioSource�� AudioClip�� �м��� ����(dB)�� UI(Slider, Text)�� ������Ʈ�մϴ�.
    /// Inspector������ AudioSource�� ���� �Ҵ����� �ʽ��ϴ�.
    /// </summary>
    public class CustomVolumeMeterUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public Slider volumeSlider;
        public TextMeshProUGUI volumeText;

        [Header("Settings")]
        public int sampleWindow = 1024;

        // �������� �Ҵ�� ������ AudioSource (Inspector�� �������� ����)
        private AudioSource recordingSource;

        // �ھ� ���� ���� �ν��Ͻ�
        private VolumeMeter volumeMeterCore;

        private WaitForSeconds checkDelay = new WaitForSeconds(0.05f);

        private void Start()
        {
            // ���� ������Ʈ �ڷ�ƾ ����
            StartCoroutine(UpdateVolumeRoutine());
        }

        /// <summary>
        /// AudioRecorderUI���� ���� ���� �� ȣ��Ǿ� AudioSource�� ���޹޽��ϴ�.
        /// </summary>
        /// <param name="source">������ ���Ǵ� AudioSource</param>

        private IEnumerator UpdateVolumeRoutine()
        {
            while (true)
            {
                // ���� ���� ���� recordingSource�� null�̸�, AudioRecorderCore�� ���� �ʵ带 Ȯ��
                if (recordingSource == null)
                {
                    recordingSource = AudioRecorder.CurrentRecordingSource;
                    // ������ ���۵Ǿ� clip�� �Ҵ�Ǿ����� �ھ� �ν��Ͻ� �ʱ�ȭ
                    if (recordingSource != null && recordingSource.clip != null && volumeMeterCore == null)
                    {
                        volumeMeterCore = new VolumeMeter(recordingSource.clip, sampleWindow);
                    }
                }

                if (recordingSource != null && recordingSource.clip != null && volumeMeterCore != null)
                {
                    float dB = volumeMeterCore.UpdateVolume();
                    float normalizedVolume = Mathf.InverseLerp(-80f, 0f, dB);
                    if (volumeSlider != null)
                        volumeSlider.value = normalizedVolume;
                    if (volumeText != null)
                        volumeText.text = $"Volume: {dB:F2} dB";
                }
                yield return checkDelay;
            }
        }
    }
}

