using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebClient;
using System.IO;
using System.Diagnostics;
using static System.Diagnostics.DiagnosticSource;
using System.Threading;
using System.Xml.Schema;
using System.Reflection;
using System.Security.Principal;
using EpilepsyEmergencyContainer.Hotkeys;
using System.Windows.Input;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Globalization;
using System.Windows.Controls;

namespace EpilepsyEmergencyContainer
{
    public partial class Form1 : Form
    {
        string startup_path = Application.StartupPath + "\\";
        string ny4 = "rlk0";
        bool soundPlaying = false, soundPlayChk=false;
        string ply_snd,mut_snd, shut_pc;//EN/TR Multilang
        //Get Default sound device
        private static readonly CoreAudioDevice defaultDevice = new CoreAudioController().DefaultPlaybackDevice;
        /// Fix to drag window / form without titlebar///////////////////////////
        /// Intentionally disabled / nerfed
        /// </summary>
        //private const int WM_NCHITTEST = 0x84;
        //private const int HTCLIENT = 0x1;
        //private const int HTCAPTION = 0x2;
        //protected override void WndProc(ref Message message)
        //{
        //    base.WndProc(ref message);
        //    if (message.Msg == WM_NCHITTEST && (int)message.Result == HTCLIENT)
        //        message.Result = (IntPtr)HTCAPTION;
        //}
        ////////////////////////////////////////////////////////////////
        public Form1()
        {
            InitializeComponent();
            HotkeysManager.AddHotkey(new GlobalHotkey(Key.F1, () => { Emergency(); }));
            HotkeysManager.SetupSystemHook();//Catch a key to trigger System Wide Black Screen
            FormClosing += Form1_FormClosing;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            HotkeysManager.ShutdownSystemHook();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkAdminAccess();
            ///Hide Title BAR///////////////////////////
            this.Text = string.Empty;
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            ////////////////////////////////////////////
            if (ny4 != "rlk0") { }
            else { }
            try
            {
                string langue = CultureInfo.CurrentCulture.Name;//tr-TR türkçe kalan her şey ingilizce
                if (langue=="tr-TR")
                {
                    ply_snd = "Ses Oynatma Açık";
                    shut_pc = "Bilgisayarı Kapat";
                    mut_snd= "Ses Oynatma Kapalı";
                }
                //Default to English
                else {
                    ply_snd = "Play Sound On";
                    shut_pc = "Shutdown Computer";
                    mut_snd = "Play Sound Off";
                }
            }
            catch (Exception){
                ply_snd = "Play Sound Açık / Ses Oynatma Açık";
                shut_pc = "Shutdown Computer / Bilgisayarı Kapat";
                mut_snd = "Play Sound Off / Ses Oynatma Kapalı";
            }
            label4.Text = mut_snd;
            label3.Text = shut_pc;
            this.WindowState = FormWindowState.Maximized;
        }
        private void checkAdminAccess()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);
            bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);

            if (!runAsAdmin)
            {
                // It is not possible to launch a ClickOnce app as administrator directly,
                // so instead we launch the app as administrator in a new process.
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);
                // The following properties run the new process as administrator
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";
                // Start the new process
                try { Process.Start(processInfo); }
                // The user did not allow the application to run as administrator
                catch (Exception) { MessageBox.Show("Since i can install direct X i need to run as Administrator.\nNow exiting."); }
                // Shut down the current process
                Application.Exit();
            }
        }

        //Exit button
        private void label1_Click(object sender, EventArgs e)
        { Application.Exit(); }
        //Minimize button
        private void label2_Click(object sender, EventArgs e)
        { this.WindowState = FormWindowState.Minimized; }
        private void label3_Click(object sender, EventArgs e)
        { this.WindowState = FormWindowState.Maximized; }
        public void Emergency()
        {
            //Perform emergency
            //Play a loud sound if they preferred i guess
            //Sound is this --> C:\Windows\Media\ringout.wav Windows 11
            if (soundPlayChk)
            {
                this.Invoke((MethodInvoker)delegate { playSound(); });
                this.Invoke((MethodInvoker)delegate { maxSound(); });
            }
            //Display a black window
            this.WindowState = FormWindowState.Maximized;
            this.Show();
            this.Activate();
        }
        public void playSound()
        {

            if (!soundPlaying)
            {
                try
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Windows\Media\Ring02.wav");//Windows 11
                    player.PlayLooping();
                }
                catch (Exception)
                {
                    try
                    {
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(startup_path + "\\sound\\Ring02.wav");
                        player.PlayLooping();
                    }
                    catch (Exception) //(Exception e)
                    { //*MessageBox.Show(e.ToString(), "");*//
                    }
                }
            }
            soundPlaying = true;
        }

        private void maxSound()
        {
            try{ defaultDevice.Volume = 100f; 
                defaultDevice.Mute(false);
            }//0 min sound, 100 max sound //does not do shit, if you are on headphones
            catch (Exception){ }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (soundPlayChk)
            {
                soundPlayChk = false;
                label4.Text=mut_snd;
            }
            else if (!soundPlayChk)
            {
                soundPlayChk = true;
                label4.Text = ply_snd;
            }
        }

        private void label3_Click_1(object sender, EventArgs e)
        {this.Invoke((MethodInvoker)delegate { shutdownComputer(); });}
        private void shutdownComputer(){
            try{
                string sysFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string cmd = sysFolder + "\\";
                Process.Start(cmd+"CMD.exe"," /C shutdown -s -t 0");
            }
            catch (Exception)
            {}
        }

    }
}
