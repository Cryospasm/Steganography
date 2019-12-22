using SteganoWave;
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

namespace Steganography
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }


        private void Home_Load(object sender, EventArgs e)
        {
            txtDstFile.Visible = false;
            groupBox7.Visible = false;
        }

        // I M A G E //
        // I M A G E //
        // I M A G E //
        // I M A G E //
        // I M A G E //
        // I M A G E //

        string loadedTrueImagePath, loadedFilePath, saveToImage, DLoadImagePath, DSaveFilePath;
        int height, width;
        long fileSize, fileNameSize;
        Image loadedTrueImage, DecryptedImage, AfterEncryption;
        Bitmap loadedTrueBitmap, DecryptedBitmap;
        Rectangle previewImage = new Rectangle(20, 160, 490, 470);
        bool canPaint = false, EncriptionDone = false;
        byte[] fileContainer;

        private void browseImgBtn1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadedTrueImagePath = openFileDialog1.FileName;
                imagePathTxt1.Text = loadedTrueImagePath;
                loadedTrueImage = Image.FromFile(loadedTrueImagePath);
                height = loadedTrueImage.Height;
                width = loadedTrueImage.Width;
                loadedTrueBitmap = new Bitmap(loadedTrueImage);

                FileInfo imginf = new FileInfo(loadedTrueImagePath);
                float fs = (float)imginf.Length / 1024;
                ImageSize_lbl.Text = smalldecimal(fs.ToString(), 2) + " KB";
                ImageHeight_lbl.Text = loadedTrueImage.Height.ToString() + " Pixel";
                ImageWidth_lbl.Text = loadedTrueImage.Width.ToString() + " Pixel";
                double cansave = (8.0 * ((height * (width / 3) * 3) / 3 - 1)) / 1024;
                CanSave_lbl.Text = smalldecimal(cansave.ToString(), 2) + " KB";

                canPaint = true;
                this.Invalidate();
            }
        }

        private string smalldecimal(string inp, int dec)
        {
            int i;
            for (i = inp.Length - 1; i > 0; i--)
                if (inp[i] == '.')
                    break;
            try
            {
                return inp.Substring(0, i + dec + 1);
            }
            catch
            {
                return inp;
            }
        }

        private void selectFileBtn1_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                loadedFilePath = openFileDialog2.FileName;
                fileImgTxt1.Text = loadedFilePath;
                FileInfo finfo = new FileInfo(loadedFilePath);
                fileSize = finfo.Length;
                fileNameSize = justFName(loadedFilePath).Length;
            }
        }

        private void imgEncBtn_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveToImage = saveFileDialog1.FileName;
            }
            else
                return;
            if (imagePathTxt1.Text == String.Empty || fileImgTxt1.Text == String.Empty)
            {
                MessageBox.Show("Encrypton information is incomplete!\nPlease complete them frist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (8 * ((height * (width / 3) * 3) / 3 - 1) < fileSize + fileNameSize)
            {
                MessageBox.Show("File size is too large!\nPlease use a larger image to hide this file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            fileContainer = File.ReadAllBytes(loadedFilePath);
            EncryptLayer();
        }

        private void EncryptLayer()
        {
            toolStripStatusLabel1.Text = "Encrypting... Please wait";
            Application.DoEvents();
            long FSize = fileSize;
            Bitmap changedBitmap = EncryptLayer(8, loadedTrueBitmap, 0, (height * (width / 3) * 3) / 3 - fileNameSize - 1, true);
            FSize -= (height * (width / 3) * 3) / 3 - fileNameSize - 1;
            if (FSize > 0)
            {
                for (int i = 7; i >= 0 && FSize > 0; i--)
                {
                    changedBitmap = EncryptLayer(i, changedBitmap, (((8 - i) * height * (width / 3) * 3) / 3 - fileNameSize - (8 - i)), (((9 - i) * height * (width / 3) * 3) / 3 - fileNameSize - (9 - i)), false);
                    FSize -= (height * (width / 3) * 3) / 3 - 1;
                }
            }
            changedBitmap.Save(saveToImage);
            toolStripStatusLabel1.Text = "Encrypted image has been successfully saved.";
            EncriptionDone = true;
            AfterEncryption = Image.FromFile(saveToImage);
            this.Invalidate();
        }

        private Bitmap EncryptLayer(int layer, Bitmap inputBitmap, long startPosition, long endPosition, bool writeFileName)
        {
            Bitmap outputBitmap = inputBitmap;
            layer--;
            int i = 0, j = 0;
            long FNSize = 0;
            bool[] t = new bool[8];
            bool[] rb = new bool[8];
            bool[] gb = new bool[8];
            bool[] bb = new bool[8];
            Color pixel = new Color();
            byte r, g, b;

            if (writeFileName)
            {
                FNSize = fileNameSize;
                string fileName = justFName(loadedFilePath);

                //write fileName:
                for (i = 0; i < height && i * (height / 3) < fileNameSize; i++)
                    for (j = 0; j < (width / 3) * 3 && i * (height / 3) + (j / 3) < fileNameSize; j++)
                    {
                        byte2bool((byte)fileName[i * (height / 3) + j / 3], ref t);
                        pixel = inputBitmap.GetPixel(j, i);
                        r = pixel.R;
                        g = pixel.G;
                        b = pixel.B;
                        byte2bool(r, ref rb);
                        byte2bool(g, ref gb);
                        byte2bool(b, ref bb);
                        if (j % 3 == 0)
                        {
                            rb[7] = t[0];
                            gb[7] = t[1];
                            bb[7] = t[2];
                        }
                        else if (j % 3 == 1)
                        {
                            rb[7] = t[3];
                            gb[7] = t[4];
                            bb[7] = t[5];
                        }
                        else
                        {
                            rb[7] = t[6];
                            gb[7] = t[7];
                        }
                        Color result = Color.FromArgb((int)bool2byte(rb), (int)bool2byte(gb), (int)bool2byte(bb));
                        outputBitmap.SetPixel(j, i, result);
                    }
                i--;
            }
            //write file (after file name):
            int tempj = j;

            for (; i < height && i * (height / 3) < endPosition - startPosition + FNSize && startPosition + i * (height / 3) < fileSize + FNSize; i++)
                for (j = 0; j < (width / 3) * 3 && i * (height / 3) + (j / 3) < endPosition - startPosition + FNSize && startPosition + i * (height / 3) + (j / 3) < fileSize + FNSize; j++)
                {
                    if (tempj != 0)
                    {
                        j = tempj;
                        tempj = 0;
                    }
                    byte2bool((byte)fileContainer[startPosition + i * (height / 3) + j / 3 - FNSize], ref t);
                    pixel = inputBitmap.GetPixel(j, i);
                    r = pixel.R;
                    g = pixel.G;
                    b = pixel.B;
                    byte2bool(r, ref rb);
                    byte2bool(g, ref gb);
                    byte2bool(b, ref bb);
                    if (j % 3 == 0)
                    {
                        rb[layer] = t[0];
                        gb[layer] = t[1];
                        bb[layer] = t[2];
                    }
                    else if (j % 3 == 1)
                    {
                        rb[layer] = t[3];
                        gb[layer] = t[4];
                        bb[layer] = t[5];
                    }
                    else
                    {
                        rb[layer] = t[6];
                        gb[layer] = t[7];
                    }
                    Color result = Color.FromArgb((int)bool2byte(rb), (int)bool2byte(gb), (int)bool2byte(bb));
                    outputBitmap.SetPixel(j, i, result);

                }
            long tempFS = fileSize, tempFNS = fileNameSize;
            r = (byte)(tempFS % 100);
            tempFS /= 100;
            g = (byte)(tempFS % 100);
            tempFS /= 100;
            b = (byte)(tempFS % 100);
            Color flenColor = Color.FromArgb(r, g, b);
            outputBitmap.SetPixel(width - 1, height - 1, flenColor);

            r = (byte)(tempFNS % 100);
            tempFNS /= 100;
            g = (byte)(tempFNS % 100);
            tempFNS /= 100;
            b = (byte)(tempFNS % 100);
            Color fnlenColor = Color.FromArgb(r, g, b);
            outputBitmap.SetPixel(width - 2, height - 1, fnlenColor);

            return outputBitmap;
        }

        private void DecryptLayer()
        {
            toolStripStatusLabel1.Text = "Decrypting... Please wait";
            Application.DoEvents();
            int i, j = 0;
            bool[] t = new bool[8];
            bool[] rb = new bool[8];
            bool[] gb = new bool[8];
            bool[] bb = new bool[8];
            Color pixel = new Color();
            byte r, g, b;
            pixel = DecryptedBitmap.GetPixel(width - 1, height - 1);
            long fSize = pixel.R + pixel.G * 100 + pixel.B * 10000;
            pixel = DecryptedBitmap.GetPixel(width - 2, height - 1);
            long fNameSize = pixel.R + pixel.G * 100 + pixel.B * 10000;
            byte[] res = new byte[fSize];
            string resFName = "";
            byte temp;

            //Read file name:
            for (i = 0; i < height && i * (height / 3) < fNameSize; i++)
                for (j = 0; j < (width / 3) * 3 && i * (height / 3) + (j / 3) < fNameSize; j++)
                {
                    pixel = DecryptedBitmap.GetPixel(j, i);
                    r = pixel.R;
                    g = pixel.G;
                    b = pixel.B;
                    byte2bool(r, ref rb);
                    byte2bool(g, ref gb);
                    byte2bool(b, ref bb);
                    if (j % 3 == 0)
                    {
                        t[0] = rb[7];
                        t[1] = gb[7];
                        t[2] = bb[7];
                    }
                    else if (j % 3 == 1)
                    {
                        t[3] = rb[7];
                        t[4] = gb[7];
                        t[5] = bb[7];
                    }
                    else
                    {
                        t[6] = rb[7];
                        t[7] = gb[7];
                        temp = bool2byte(t);
                        resFName += (char)temp;
                    }
                }

            //Read file on layer 8 (after file name):
            int tempj = j;
            i--;

            for (; i < height && i * (height / 3) < fSize + fNameSize; i++)
                for (j = 0; j < (width / 3) * 3 && i * (height / 3) + (j / 3) < (height * (width / 3) * 3) / 3 - 1 && i * (height / 3) + (j / 3) < fSize + fNameSize; j++)
                {
                    if (tempj != 0)
                    {
                        j = tempj;
                        tempj = 0;
                    }
                    pixel = DecryptedBitmap.GetPixel(j, i);
                    r = pixel.R;
                    g = pixel.G;
                    b = pixel.B;
                    byte2bool(r, ref rb);
                    byte2bool(g, ref gb);
                    byte2bool(b, ref bb);
                    if (j % 3 == 0)
                    {
                        t[0] = rb[7];
                        t[1] = gb[7];
                        t[2] = bb[7];
                    }
                    else if (j % 3 == 1)
                    {
                        t[3] = rb[7];
                        t[4] = gb[7];
                        t[5] = bb[7];
                    }
                    else
                    {
                        t[6] = rb[7];
                        t[7] = gb[7];
                        temp = bool2byte(t);
                        res[i * (height / 3) + j / 3 - fNameSize] = temp;
                    }
                }

            //Read file on other layers:
            long readedOnL8 = (height * (width / 3) * 3) / 3 - fNameSize - 1;

            for (int layer = 6; layer >= 0 && readedOnL8 + (6 - layer) * ((height * (width / 3) * 3) / 3 - 1) < fSize; layer--)
                for (i = 0; i < height && i * (height / 3) + readedOnL8 + (6 - layer) * ((height * (width / 3) * 3) / 3 - 1) < fSize; i++)
                    for (j = 0; j < (width / 3) * 3 && i * (height / 3) + (j / 3) + readedOnL8 + (6 - layer) * ((height * (width / 3) * 3) / 3 - 1) < fSize; j++)
                    {
                        pixel = DecryptedBitmap.GetPixel(j, i);
                        r = pixel.R;
                        g = pixel.G;
                        b = pixel.B;
                        byte2bool(r, ref rb);
                        byte2bool(g, ref gb);
                        byte2bool(b, ref bb);
                        if (j % 3 == 0)
                        {
                            t[0] = rb[layer];
                            t[1] = gb[layer];
                            t[2] = bb[layer];
                        }
                        else if (j % 3 == 1)
                        {
                            t[3] = rb[layer];
                            t[4] = gb[layer];
                            t[5] = bb[layer];
                        }
                        else
                        {
                            t[6] = rb[layer];
                            t[7] = gb[layer];
                            temp = bool2byte(t);
                            res[i * (height / 3) + j / 3 + (6 - layer) * ((height * (width / 3) * 3) / 3 - 1) + readedOnL8] = temp;
                        }
                    }

            if (File.Exists(DSaveFilePath + "\\" + resFName))
            {
                MessageBox.Show("File \"" + resFName + "\" already exist please choose another path to save file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
                File.WriteAllBytes(DSaveFilePath + "\\" + resFName, res);
            toolStripStatusLabel1.Text = "Decrypted file has been successfully saved.";
            Application.DoEvents();
        }

        private void byte2bool(byte inp, ref bool[] outp)
        {
            if (inp >= 0 && inp <= 255)
                for (short i = 7; i >= 0; i--)
                {
                    if (inp % 2 == 1)
                        outp[i] = true;
                    else
                        outp[i] = false;
                    inp /= 2;
                }
            else
                throw new Exception(" number is illegal.");
        }

        private byte bool2byte(bool[] inp)
        {
            byte outp = 0;
            for (short i = 7; i >= 0; i--)
            {
                if (inp[i])
                    outp += (byte)Math.Pow(2.0, (double)(7 - i));
            }
            return outp;
        }

        private void imgDecBtn_Click(object sender, EventArgs e)
        {
            if (fileImgTxt2.Text == String.Empty || imagePathTxt2.Text == String.Empty)
            {
                MessageBox.Show("Text boxes must not be empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (System.IO.File.Exists(imagePathTxt2.Text) == false)
            {
                MessageBox.Show("Select image file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                imagePathTxt2.Focus();
                return;
            }



            DecryptLayer();
        }

        private void browseImgBtn2_Click(object sender, EventArgs e)
        {
            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                DLoadImagePath = openFileDialog3.FileName;
                imagePathTxt2.Text = DLoadImagePath;
                DecryptedImage = Image.FromFile(DLoadImagePath);
                height = DecryptedImage.Height;
                width = DecryptedImage.Width;
                DecryptedBitmap = new Bitmap(DecryptedImage);

                FileInfo imginf = new FileInfo(DLoadImagePath);
                float fs = (float)imginf.Length / 1024;
                ImageSize_lbl.Text = smalldecimal(fs.ToString(), 2) + " KB";
                ImageHeight_lbl.Text = DecryptedImage.Height.ToString() + " Pixel";
                ImageWidth_lbl.Text = DecryptedImage.Width.ToString() + " Pixel";
                double cansave = (8.0 * ((height * (width / 3) * 3) / 3 - 1)) / 1024;
                CanSave_lbl.Text = smalldecimal(cansave.ToString(), 2) + " KB";

                canPaint = true;
                this.Invalidate();
            }
        }

        private void selectFileBtn2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                DSaveFilePath = folderBrowserDialog1.SelectedPath;
                fileImgTxt2.Text = DSaveFilePath;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (canPaint)
                try
                {
                    if (!EncriptionDone)
                        e.Graphics.DrawImage(loadedTrueImage, previewImage);
                    else
                        e.Graphics.DrawImage(AfterEncryption, previewImage);
                }
                catch
                {
                    e.Graphics.DrawImage(DecryptedImage, previewImage);
                }
        }

        private string justFName(string path)
        {
            string output;
            int i;
            if (path.Length == 3)  
                return path.Substring(0, 1);
            for (i = path.Length - 1; i > 0; i--)
                if (path[i] == '\\')
                    break;
            output = path.Substring(i + 1);
            return output;
        }

        private string justEx(string fName)
        {
            string output;
            int i;
            for (i = fName.Length - 1; i > 0; i--)
                if (fName[i] == '.')
                    break;
            output = fName.Substring(i + 1);
            return output;
        }

        private void imgBtn_Click(object sender, EventArgs e)
        {
            audiopanel.Visible = false;
            videopanel.Visible = false;
            imagepanel.Visible = true;
            imagepanel.Location = new Point(230, 16);
            imagepanel.Height = 920;
            imagepanel.Width = 880;

        }

        private void audioBtn_Click(object sender, EventArgs e)
        {
            imagepanel.Visible = false;
            videopanel.Visible = false;
            audiopanel.Visible = true;
            audiopanel.Location = new Point(230, 16);
            audiopanel.Height = 920;
            audiopanel.Width = 880;

        }

        private void videoBtn_Click(object sender, EventArgs e)
        {
            imagepanel.Visible = false;
            audiopanel.Visible = false;
            videopanel.Visible = true;
            videopanel.Location = new Point(230, 16);
            videopanel.Height = 920;
            videopanel.Width = 880;
            
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // I M A G E //
        // I M A G E //
        // I M A G E //
        // I M A G E //
        // I M A G E //
        // I M A G E //

        // - - - - - //

        // A U D I O //
        // A U D I O //
        // A U D I O //
        // A U D I O //
        // A U D I O //
        // A U D I O //

        // Butonlara open dile dialog ayarlandı
        private void btnMsgFile1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            GetFileName(dlg, txtMsgFile, false);
        }

        private void btnSrcFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            GetFileName(dlg, txtSrcFile, true);
        }

        private void btnKeyFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            GetFileName(dlg, txtKeyFile, false);
        }


        // Wav filter ve dosya ismi alma işlemleri

        private void GetFileName(FileDialog dialog, TextBox control, bool useFilter)
        {
            if (useFilter) { dialog.Filter = "Wave Audio (*.wav)|*.wav"; }
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                control.Text = dialog.FileName;
            }
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            // Kayıt
            SaveFileDialog dlg = new SaveFileDialog();
            GetFileName(dlg, txtDstFile, true);

            // Tanımlamalar
            Stream sourceStream = null;
            FileStream destinationStream = null;
            WaveStream audioStream = null;

            //Mesaj tanımalama
            Stream messageStream = GetMessageStream();
            //Key atama
            Stream keyStream = new FileStream(txtKeyFile.Text, FileMode.Open);

            try
            {
                //Max valuedan fazla ise hata ver
                long countSamplesRequired = WaveUtility.CheckKeyForMessage(keyStream, messageStream.Length);

                if (countSamplesRequired > Int32.MaxValue)
                {
                    throw new Exception("Mesaj çok uzun ya da key yanlış.  " + countSamplesRequired + " " + Int32.MaxValue);
                }

                sourceStream = new FileStream(txtSrcFile.Text, FileMode.Open);
                
                this.Cursor = Cursors.WaitCursor;

                // Dosya oluştur
                destinationStream = new FileStream(txtDstFile.Text, FileMode.Create);

                // Header kopyala
                audioStream = new WaveStream(sourceStream, destinationStream);
                if (audioStream.Length <= 0)
                {
                    throw new Exception("Invalid WAV file");
                }

                // Taşıyıcı veride yeteri kadar yer var mı?
                if (countSamplesRequired > audioStream.CountSamples)
                {
                    String errorReport = "Taşıyıcı veri çok küçük!\r\n"
                        + "Müsait alan: " + audioStream.CountSamples + "\r\n"
                        + "Gerekli alan: " + countSamplesRequired;
                    throw new Exception(errorReport);
                }

                // Mesajı gizle
                WaveUtility utility = new WaveUtility(audioStream, destinationStream);
                utility.Hide(messageStream, keyStream);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (keyStream != null) { keyStream.Close(); }
                if (messageStream != null) { messageStream.Close(); }
                if (audioStream != null) { audioStream.Close(); }
                if (sourceStream != null) { sourceStream.Close(); }
                if (destinationStream != null) { destinationStream.Close(); }
                this.Cursor = Cursors.Default;
            }

        }
        
        private Stream GetMessageStream()
        {
            
            BinaryWriter messageWriter = new BinaryWriter(new MemoryStream());
            
            FileStream fs = new FileStream(txtMsgFile.Text, FileMode.Open);
            int fileLength = (int)fs.Length;
            messageWriter.Write(fileLength);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, fileLength);
            messageWriter.Write(buffer);
            fs.Close();
            
            
            messageWriter.Seek(0, SeekOrigin.Begin);
            return messageWriter.BaseStream;
        }


        // Extract İşlemleri

        private void btnSrcFile2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            GetFileName(dlg, txtSrcFile2, true);
        }

        private void btnKeyFile2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            GetFileName(dlg, txtKeyFile2, false);
        }

        private void btnMessageDstFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            GetFileName(dlg, txtMessageDstFile, false);
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            if (txtSrcFile2.Text.Length == 0)
            {
                errorProvider.SetError(txtSrcFile, "Taşıyıcı veri alanını boş bırakmayınız.");
            }
            else if (txtKeyFile2.Text.Length == 0)
            {
                errorProvider.SetError(txtKeyFile, "Bir şifre dosyası seçiniz.");
            }
            else
            {
                this.Cursor = Cursors.WaitCursor;

                FileStream sourceStream = null;
                WaveStream audioStream = null;
                //Extract etmek için boş bir steam yaratıldı
                MemoryStream messageStream = new MemoryStream();
                //Key file açıldı
                Stream keyStream = new FileStream(txtKeyFile2.Text, FileMode.Open);


                try
                {
                    // Taşıyıcı dosya açıldı
                    sourceStream = new FileStream(txtSrcFile2.Text, FileMode.Open);
                    audioStream = new WaveStream(sourceStream);
                    WaveUtility utility = new WaveUtility(audioStream);

                    // Mesaj açıldı
                    utility.Extract(messageStream, keyStream);

                    messageStream.Seek(0, SeekOrigin.Begin);
                    // Sonuç dosyaya kaydedildi
                    FileStream fs = new FileStream(txtMessageDstFile.Text, FileMode.Create);

                    byte[] buffer = new byte[messageStream.Length];
                    messageStream.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                    
                    
                }
                catch (Exception ex)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (keyStream != null) { keyStream.Close(); }
                    if (messageStream != null) { messageStream.Close(); }
                    if (audioStream != null) { audioStream.Close(); }
                    if (sourceStream != null) { sourceStream.Close(); }
                    this.Cursor = Cursors.Default;
                }
            }
        }





        // A U D I O //
        // A U D I O //
        // A U D I O //
        // A U D I O //
        // A U D I O //
        // A U D I O //





















    }
}
