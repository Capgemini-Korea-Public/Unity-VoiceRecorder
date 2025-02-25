namespace MyAudioPackage.Core
{
    public enum SaveMode
    {
        SaveWav,
        SaveOgg,
        None  // �������� ����
    }

    public class SaveModeSelector
    {
        // ���� ���õ� ���� ��� (�⺻��: None)
        public SaveMode SelectedSaveMode { get; private set; } = SaveMode.None;

        // ���� ��� ���� �� �߻��ϴ� �̺�Ʈ
        public event System.Action<SaveMode> OnSaveModeChanged;

        /// <summary>
        /// SaveMode�� ���� �����մϴ�.
        /// </summary>
        public void SetSaveMode(SaveMode mode)
        {
            SelectedSaveMode = mode;
            OnSaveModeChanged?.Invoke(mode);
        }

        /// <summary>
        /// ��Ӵٿ� �ε����� ������� ���� ��带 �����մϴ�.
        /// 0: SaveWav, 1: SaveOgg, 2: None
        /// </summary>
        public void SetSaveModeFromIndex(int index)
        {
            switch (index)
            {
                case 0:
                    SetSaveMode(SaveMode.SaveWav);
                    break;
                case 1:
                    SetSaveMode(SaveMode.SaveOgg);
                    break;
                case 2:
                    SetSaveMode(SaveMode.None);
                    break;
                default:
                    SetSaveMode(SaveMode.SaveWav);
                    break;
            }
        }
    }
}
