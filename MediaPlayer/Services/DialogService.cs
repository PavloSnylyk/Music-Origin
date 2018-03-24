using MediaPlayer.Interfaces;
using System.Windows.Forms;


namespace MediaPlayer.Services
{
    public class DialogService : IDialogService
    {
        public string FilePath { get; set; }

        // Write path if file exist
        public bool OpenFileDialog()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                FilePath = folderBrowserDialog.SelectedPath;
                return true;
            }
            return false;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

    }
}
