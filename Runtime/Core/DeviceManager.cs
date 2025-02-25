using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAudioPackage.Core
{
    public class DeviceManager
    {
        // ���õ� ��ġ ����� �ܺο� �˸��� �̺�Ʈ
        public event Action<string> OnDeviceSelected;
        // ����̽� ����� ����Ǿ��� �� �˸��� �̺�Ʈ
        public event Action OnDeviceListChanged;

        // ĳ�õ� ����̽� ���
        private List<string> cachedDevices = new List<string>();
        // ���� ���õ� ��ġ
        public string SelectedDevice { get; private set; } = null;

        /// <summary>
        /// ���� ��� ������ ����ũ ����̽� ����� ��ȯ�մϴ�.
        /// </summary>
        public string[] GetAvailableDevices()
        {
            return Microphone.devices;
        }

        /// <summary>
        /// ĳ�õ� ����̽� ��ϰ� ���Ӱ� ���� ����� ���Ͽ� ����Ǿ����� �Ǵ��մϴ�.
        /// </summary>
        private bool HasDeviceListChanged(string[] newDevices)
        {
            if (newDevices.Length != cachedDevices.Count)
                return true;

            for (int i = 0; i < newDevices.Length; i++)
            {
                if (newDevices[i] != cachedDevices[i])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// ����̽� ����� ���� ��ġ��, ����Ǿ����� �̺�Ʈ�� �߻��մϴ�.
        /// </summary>
        public void RefreshDeviceList()
        {
            string[] currentDevices = GetAvailableDevices();

            // ��ġ�� ������ ĳ�� �ʱ�ȭ �� �̺�Ʈ �߻�
            if (currentDevices.Length == 0)
            {
                cachedDevices.Clear();
                SelectedDevice = null;
                OnDeviceListChanged?.Invoke();
                return;
            }

            // ��Ͽ� ��ȭ�� ������ ������Ʈ
            if (HasDeviceListChanged(currentDevices))
            {
                cachedDevices = new List<string>(currentDevices);
                OnDeviceListChanged?.Invoke();

                // ������ ���õ� ��ġ�� ���ų� �� �̻� �������� ������ ù ��° ��ġ�� ����
                if (string.IsNullOrEmpty(SelectedDevice) || Array.IndexOf(currentDevices, SelectedDevice) < 0)
                {
                    SelectedDevice = currentDevices[0];
                    OnDeviceSelected?.Invoke(SelectedDevice);
                }
            }
        }

        /// <summary>
        /// �ܺ�(UI ����)���� ȣ���Ͽ� ������ ��ġ�� �����ϰ�, �̺�Ʈ�� �߻���ŵ�ϴ�.
        /// </summary>
        public void SetSelectedDevice(string device)
        {
            SelectedDevice = device;
            OnDeviceSelected?.Invoke(device);
        }

        /// <summary>
        /// ���� ĳ�õ� ��ġ ����� ��ȯ�մϴ�.
        /// </summary>
        public List<string> GetCachedDevices()
        {
            return new List<string>(cachedDevices);
        }
    }
}
