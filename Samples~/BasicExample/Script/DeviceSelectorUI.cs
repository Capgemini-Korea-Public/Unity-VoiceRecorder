using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyAudioPackage.Core; // �ھ� ���ӽ����̽�

namespace MyAudioPackage.UI
{
    public class DeviceSelectorUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public TMP_Dropdown deviceDropdown;
        public TextMeshProUGUI deviceError;

        // �ھ� ���� �ν��Ͻ�
        public DeviceManager deviceManagerCore = new DeviceManager();

        private WaitForSeconds deviceCheckDelay = new WaitForSeconds(1f);

        private void Start()
        {
            // �ھ� �̺�Ʈ ����
            deviceManagerCore.OnDeviceSelected += HandleDeviceSelected;
            deviceManagerCore.OnDeviceListChanged += HandleDeviceListChanged;

            PopulateDropdown();
            StartCoroutine(MonitorDeviceChanges());
        }

        /// <summary>
        /// ��Ӵٿ ���� ����ũ ����̽� ����� ä��ϴ�.
        /// </summary>
        private void PopulateDropdown()
        {
            string[] devices = deviceManagerCore.GetAvailableDevices();
            deviceDropdown.ClearOptions();

            if (devices.Length == 0)
            {
                deviceError.text = "No microphone device found.";
                deviceDropdown.interactable = false;
                return;
            }

            List<string> options = new List<string>(devices);
            deviceDropdown.AddOptions(options);
            deviceDropdown.interactable = true;
            deviceError.text = "";

            // ������ ������ ��ġ�� �ִٸ� ����, ������ �⺻������ ù ��° ����
            int index = Array.IndexOf(devices, deviceManagerCore.SelectedDevice);
            if (index < 0)
            {
                deviceManagerCore.SetSelectedDevice(options[0]);
                index = 0;
            }
            deviceDropdown.value = index;

            deviceDropdown.onValueChanged.RemoveAllListeners();
            deviceDropdown.onValueChanged.AddListener(OnDropdownChanged);
        }

        private void OnDropdownChanged(int index)
        {
            string selectedDevice = deviceDropdown.options[index].text;
            deviceManagerCore.SetSelectedDevice(selectedDevice);
        }

        private void HandleDeviceSelected(string device)
        {
            Debug.Log("Selected device: " + device);
            // UI �߰� ������Ʈ�� �ʿ��ϸ� �̰����� ó�� (��: ���� �޽��� �ʱ�ȭ ��)
        }

        private void HandleDeviceListChanged()
        {
            PopulateDropdown();
        }

        private IEnumerator MonitorDeviceChanges()
        {
            while (true)
            {
                yield return deviceCheckDelay;
                deviceManagerCore.RefreshDeviceList();
            }
        }
    }
}
