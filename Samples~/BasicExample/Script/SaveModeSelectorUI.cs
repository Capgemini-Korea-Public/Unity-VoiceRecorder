using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyAudioPackage.Core;

namespace MyAudioPackage.UI
{
    public class SaveModeSelectorUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public TMP_Dropdown saveModeDropdown;

        // �ھ� ���� ��� ���� �ν��Ͻ� (�ܺο��� ����ϰų� AudioManager ��� ���� ����)
        public SaveModeSelector saveModeSelectorCore = new SaveModeSelector();

        private void Start()
        {
            // ��Ӵٿ� �ɼ� ����
            List<string> options = new List<string> { "Save To Wav", "Save To Ogg", "None" };
            saveModeDropdown.ClearOptions();
            saveModeDropdown.AddOptions(options);

            // �⺻�� ����: ���� ��� "None"�� �⺻������ ��� (�ε��� 2)
            saveModeDropdown.value = 0;
            saveModeSelectorCore.SetSaveModeFromIndex(saveModeDropdown.value);

            // ��Ӵٿ� �� ���� �̺�Ʈ�� ������ �߰�
            saveModeDropdown.onValueChanged.AddListener(OnDropdownChanged);
        }

        private void OnDropdownChanged(int index)
        {
            saveModeSelectorCore.SetSaveModeFromIndex(index);
            Debug.Log("Selected Save Mode: " + saveModeSelectorCore.SelectedSaveMode);
        }
    }
}
