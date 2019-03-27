namespace HRD
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    public class Ini
    {
        private readonly string filename;

        public Ini(string iniFilename)
        {
            if (iniFilename.IndexOf(@":\\") == -1)
            {
                this.filename = Application.StartupPath + @"\" + iniFilename;
            }
            else
            {
                this.filename = iniFilename;
            }
        }

        private static void WriteFile(string fileName, string text)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                try
                {
                    fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                    sw = new StreamWriter(fs);
                    sw.Write(text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("文件操作异常：" + ex.Message);
                }
            }
            finally
            {
                if (fs != null)
                {
                    sw.Close();
                    fs.Close();
                }
            }
        }

        public static void DoStepFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件(*.txt)|*.txt";
            openFileDialog.Title = "导入数据......";
            openFileDialog.InitialDirectory = @"c:\";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ShowHelp = true;
            string fileName = null;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
            }
            else
            {
                return;
            }
            if (fileName.Length < 1)
            {
                MessageBox.Show("文件路径不能为空");
            }
            else
            {
                ListView listView1 = frmMain._frmMain.listView1;
                listView1.Items.Clear();
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                for (string content = sr.ReadLine(); content != null; content = sr.ReadLine())
                {
                    if (content.Length > 2)
                    {
                        string[] a = content.Split(new char[] { '\t' });
                        ListViewItem item = new ListViewItem("步" + a[0]);
                        item.SubItems.Add("" + a[1]);
                        item.SubItems.Add("" + a[2]);
                        item.SubItems.Add("" + a[3]);
                        listView1.Items.Add(item);
                    }
                }
            }
        }

        public static string GetMap(string resourceName)
        {
            using (var pStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var streamReader = new StreamReader(pStream, Encoding.UTF8))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public string ReadValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(0xff);
            int i = GetPrivateProfileString(section, key, "", temp, 0xff, this.filename);
            return temp.ToString();
        }

        public static string ReadValue(string section, string key, string filename)
        {
            StringBuilder temp = new StringBuilder(0xff);
            int i = GetPrivateProfileString(section, key, "", temp, 0xff, filename);
            return temp.ToString();
        }

        public static void SaveFile(ListView lBox)
        {
            if (lBox.Items.Count < 1)
            {
                MessageBox.Show("没有可保存的步骤。");
            }
            else
            {
                string text = null;
                for (int i = 0; i < lBox.Items.Count; i++)
                {
                    text = text + lBox.Items[i].SubItems[0].Text + "\t" + lBox.Items[i].SubItems[1].Text + "\t" + lBox.Items[i].SubItems[2].Text + "\t" + lBox.Items[i].SubItems[3].Text + "\r\n";
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                saveFileDialog.Title = "保存导出数据......";
                saveFileDialog.InitialDirectory = @"c:\";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.ShowHelp = true;
                string fileName = null;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
                if (fileName.Length < 1)
                {
                    MessageBox.Show("文件路径不能为空");
                }
                else
                {
                    WriteFile(fileName, text);
                    MessageBox.Show("写入成功");
                }
            }
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        public void WriteValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this.filename);
        }

        public static void WriteValue(string section, string key, string value, string filename)
        {
            WritePrivateProfileString(section, key, value, filename);
        }
    }
}

