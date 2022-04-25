using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetaImageEditor
{
    public partial class Form1 : Form
    {
        CreateFilename filename = new CreateFilename();
        List<MetaText> metas = new List<MetaText>(); //Content, Path; 
        List<MetaText> edited = new List<MetaText>(); //Original version of a meta file;

        int formatSize;
        int compressionQuality;

        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "meta files|*.meta";
            openFileDialog1.Multiselect = true;
            openFileDialog1.InitialDirectory = folderBrowserDialog1.SelectedPath;
            comboBox1.SelectedIndex = 0;
            trackBar1.Maximum = 100;
            label1.Text = $"Compressor quality: {compressionQuality}";
            label4.Text = "Waiting image";
        }

        //Set a picture in the picture box;
        void SetPicture(string path)
        {
            var pictureLink = path.Replace(".meta", "");
            pictureBox1.Image = Image.FromFile(pictureLink);
            label4.Text = $"Size: {pictureBox1.Image.Width}x{pictureBox1.Image.Height}, Max: {GetMax()}";
        }

        //Search a suitable size;
        int GetMax()
        {
            int maxValue = Math.Max(pictureBox1.Image.Width, pictureBox1.Image.Height);
            int returnValue = 0;
            if(maxValue <= 32)
            {
                returnValue = 32;
            }
            else if(maxValue > 32 && maxValue <= 64)
            {
                returnValue = 64;
            }
            else if (maxValue > 64 && maxValue <= 128)
            {
                returnValue = 128;
            }
            else if (maxValue > 128 && maxValue <= 256)
            {
                returnValue = 256;
            }
            else if (maxValue > 256 && maxValue <= 512)
            {
                returnValue = 512;
            }
            else if (maxValue > 512 && maxValue <= 1024)
            {
                returnValue = 1024;
            }
            else if (maxValue > 1024 && maxValue <= 2048)
            {
                returnValue = 2048;
            }
            else if (maxValue > 2048 && maxValue <= 4096)
            {
                returnValue = 4096;
            }
            else if (maxValue > 4096 && maxValue <= 8192)
            {
                returnValue = 8192;
            }
            return returnValue;
        }

        //Add a Nintendo information;
        string BuildMeta()
        {
            return $"  - serializedVersion: 3\n    buildTarget: Nintendo Switch\n    maxTextureSize: {GetMax()}\n    resizeAlgorithm: 0\n    textureFormat: {formatSize}\n    textureCompression: 3\n    compressionQuality: {compressionQuality}\n    crunchedCompression: 0\n    allowsAlphaSplitting: 0\n    overridden: 1\n    androidETC2FallbackOverride: 0\n    forceMaximumCompressionQuality_BC6H_BC7: 0";
        }

        //Remove Nintendo lines if a program meet them;
        string RemoveKebab(string[] text)
        {
            string returningText = string.Empty;
            int point = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].Contains("buildTarget: Nintendo Switch"))
                {
                    point = i;
                    break;
                }
            }
            var list = text.ToList<string>();
            list.RemoveRange(point-1, 12);

            foreach (var element in list)
            {
                returningText += $"{element}\n";
            }
            return returningText;
        }

        //Set a file in list;
        void SetFile(string text, string path)
        {
            var name = filename.ReturnFilename(path);
            listBox1.Items.Add(name);
            metas.Add(new MetaText(text, path));
            edited.Add(new MetaText(text, path));
            SetPicture(path);
            listBox1.SelectedIndex = 0;
        }

        void MetaEdit()
        {
            richTextBox1.Text = metas[listBox1.SelectedIndex].Meta;
            int point = 0;
            for (int i = 0; i < richTextBox1.Lines.Length; i++)
            {
                if (richTextBox1.Lines[i].Contains("  spriteSheet:"))
                {
                    point = i;
                    break;
                }
            }
            var list = richTextBox1.Lines.ToList<string>();
            list.Insert(point, BuildMeta());
            metas[listBox1.SelectedIndex].Meta = string.Empty;
            foreach (var element in list)
            {
                metas[listBox1.SelectedIndex].Meta += $"{element}\n";
            }
            richTextBox1.Text = metas[listBox1.SelectedIndex].Meta;
        }

        //Open meta files;
        private void OpenButton_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Clear();
                metas.Clear();
                edited.Clear();
                var directory = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.meta", SearchOption.AllDirectories);
                foreach(var fileName in directory)
                {
                    richTextBox1.LoadFile(fileName, RichTextBoxStreamType.PlainText);
                    if (richTextBox1.Find("TextureImporter") > 0)
                    {
                        if (richTextBox1.Find("Nintendo Switch") > 0)
                        {
                            richTextBox1.Text = RemoveKebab(richTextBox1.Lines);
                            SetFile(richTextBox1.Text, fileName);
                        }
                        else
                        {
                            SetFile(richTextBox1.Text, fileName);
                        }

                    }
                    else
                    {
                        if (listBox1.Items.Count == 0)
                        {
                            richTextBox1.Clear();
                        }
                        else
                        {
                            richTextBox1.Text = metas[0].Meta;
                            listBox1.SelectedIndex = 0;
                        }
                        //MessageBox.Show("It is not image.");
                    }
                    label3.Text = $"List: {listBox1.Items.Count}";
                }
                //using (Stream stream = openFileDialog1.OpenFile())
                //{
                    
                //}
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Text = metas[listBox1.SelectedIndex].Meta;
                SetPicture(metas[listBox1.SelectedIndex].Path);
            }
            catch
            {

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboBox1.SelectedIndex)
            {
                case 0: //DXT5;
                    formatSize = 12;
                    break;
                case 1: //BC7;
                    formatSize = 25;
                    break;
                case 2: //4;
                    formatSize = 48;
                    break;
                case 3: //5;
                    formatSize = 49;
                    break;
                case 4: //6;
                    formatSize = 50;
                    break;
                case 5: //8;
                    formatSize = 51;
                    break;
                case 6: //10;
                    formatSize = 52;
                    break;
                case 7: //12;
                    formatSize = 53;
                    break;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            compressionQuality = trackBar1.Value;
            label1.Text = $"Compressor quality: {compressionQuality}";
        }

        private void InsertButton_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show($"Meta fields is empty. Please open files.");
                return;
            }
            richTextBox1.Clear();
            metas[listBox1.SelectedIndex].Meta = edited[listBox1.SelectedIndex].Meta;
            MetaEdit();
            MessageBox.Show("Done.");
        }

        private void SaveSingleButton_Click(object sender, EventArgs e)
        {
            if (metas.Count != 0)
            {
                try
                {
                    richTextBox1.SaveFile(metas[listBox1.SelectedIndex].Path, RichTextBoxStreamType.PlainText);

                    MessageBox.Show($"Done.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }

            }
            else
            {
                MessageBox.Show($"Meta fields is empty. Please open files.");
            }
        }

        private void InsertAllButton_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show($"Meta fields is empty. Please open files.");
                return;
            }
            metas = new List<MetaText>(edited);
            for(int i = 0; i < metas.Count; i++)
            {
                listBox1.SelectedIndex = i;
                MetaEdit();
            }
            MessageBox.Show("All done.");
            listBox1.SelectedIndex = 0;
        }

        private void SaveAllButton_Click(object sender, EventArgs e)
        {
            if (metas.Count != 0)
            {
                try
                {
                    for (int i = 0; i < metas.Count; i++)
                    {
                        listBox1.SelectedIndex = i;
                        richTextBox1.SaveFile(metas[listBox1.SelectedIndex].Path, RichTextBoxStreamType.PlainText);
                    }
                    MessageBox.Show($"All done.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }

            }
            else
            {
                MessageBox.Show($"Meta fields is empty. Please open files.");
            }
        }
    }
}
