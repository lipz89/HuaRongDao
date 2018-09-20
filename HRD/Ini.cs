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
        public static string _jsIP = "";
        public static short _picTM = 200;
        public static string _sIP = "";
        public static short _TMD = 10;
        public static bool _ZJ = false;
        public static bool autoHide = false;
        public static bool autoStart = false;
        private string filename;
        public static bool Innet = true;

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

        private static void _writeFile(string fileName, string text)
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
            OpenFileDialog OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.Filter = "文本文件(*.txt)|*.txt";
            OpenFileDialog.Title = "导入数据......";
            OpenFileDialog.InitialDirectory = @"c:\";
            OpenFileDialog.RestoreDirectory = true;
            OpenFileDialog.ShowHelp = true;
            string fileName = null;
            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = OpenFileDialog.FileName;
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

        public static void GetHrDFile(string ResourceName)
        {
            string text = getStrByResFileName(ResourceName);
            _writeFile(@"C:\Hrd_temp.xml", text);
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        private static string getStrByResFileName(string ResourceName)
        {
            Stream pStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);
            string str = "";
            StreamReader m_streamReader = new StreamReader(pStream, Encoding.UTF8);
            m_streamReader.BaseStream.Seek(0L, SeekOrigin.Begin);
            for (string strLine = m_streamReader.ReadLine(); strLine != null; strLine = m_streamReader.ReadLine())
            {
                str = str + strLine + "\n";
            }
            m_streamReader.Close();
            return str;
        }

        public string ReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(0xff);
            int i = GetPrivateProfileString(Section, Key, "", temp, 0xff, this.filename);
            return temp.ToString();
        }

        public static string ReadValue(string Section, string Key, string filename)
        {
            StringBuilder temp = new StringBuilder(0xff);
            int i = GetPrivateProfileString(Section, Key, "", temp, 0xff, filename);
            return temp.ToString();
        }

        public static void saveFile(ListView lBox)
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
                    _writeFile(fileName, text);
                    MessageBox.Show("写入成功");
                }
            }
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        public void WriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.filename);
        }

        public static void WriteValue(string Section, string Key, string Value, string filename)
        {
            WritePrivateProfileString(Section, Key, Value, filename);
        }
    }
}

