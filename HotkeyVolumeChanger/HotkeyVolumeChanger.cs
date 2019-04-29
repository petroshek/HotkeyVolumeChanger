using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace HotkeyVolumeChanger
{
	public class HotkeyVolumeChanger : Form
	{
        public static List<BoundKeys> ListBoundKeys;
        public static List<Keys> key;
        public static List<float> vol;
        public static List<int> pidkey;

        public static List<Keys> AllKeys;
        public static List<Keys> AvailableKeys;

        public static string pubgPID;

        private KeyboardHook hook = new KeyboardHook();
		private TypeConverter converter = TypeDescriptor.GetConverter(typeof(Keys));

        private ListBox listBox_processes;
		private DataTable dt;        
        private IContainer components;
		private Button buttonBindKeys;
		private Label madeByLabel;
		private TextBox textBoxVol1;
		private TextBox textBoxVol2;
        private TextBox textBoxVol3;
        private TextBox textBoxVol4;
        private Label label2;
		private Label label3;
		private Label label4;
		private Label label5;
		private Button buttonRefreshProcesses;
		private NotifyIcon notifyIcon1;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem menuToolStripMenuItem;
		private ToolStripMenuItem infoToolStripMenuItem;
        private ToolStripMenuItem processInfoToolStripMenuItem;
        private Label label1;
        private Label label6;
        private Label label7;
        private Label label8;
        private ToolStripMenuItem exitToolStripMenuItem;        
        private Button buttonBindKey1And2;
        private Label label9;
        private ListBox listBox_boundHotkeys;
        private Button button_unbindHotkey;
        private ListBox listBox_availableHotkeys;
        private Label label11;
        private Label label10;
        private ComboBox comboBox_Hotkey1;
        private ComboBox comboBox_Hotkey2;
        private ComboBox comboBox_Hotkey3;
        private ComboBox comboBox_Hotkey4;
        private Button buttonBindKey3And4;        

		public HotkeyVolumeChanger()
		{
			InitializeComponent();

            ListBoundKeys = new List<BoundKeys>();  // All bound keys
            vol = new List<float>();
            pidkey = new List<int>();
            key = new List<Keys>();

            AllKeys = GetAllKeys();                    // All keys            
            AvailableKeys = GetAllKeys();              // All non-bound keys   
            listBox_availableHotkeys.DataSource = AvailableKeys;
            comboBox_Hotkey1.DataSource = AvailableKeys;
            comboBox_Hotkey2.DataSource = AvailableKeys;
            comboBox_Hotkey3.DataSource = AvailableKeys;
            comboBox_Hotkey4.DataSource = AvailableKeys;
            foreach (Keys K in AvailableKeys)
            {
                if (Keys.F1 == K)
                    comboBox_Hotkey1.SelectedItem = K;
                else if (Keys.F2 == K)
                    comboBox_Hotkey2.SelectedItem = K;
                else if (Keys.F3 == K)
                    comboBox_Hotkey3.SelectedItem = K;
                else if (Keys.F4 == K)
                    comboBox_Hotkey4.SelectedItem = K;
            }
            listBox_boundHotkeys.DataSource = ListBoundKeys;

            base.Resize += Form1_Resize;
			notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
			createDataTable();
		}

        private List<Keys> GetAllKeys()
        {
            List<Keys> temp = new List<Keys>();
            for (int i = 8; i < 10;i++)
                temp.Add((Keys)i);
            for (int i = 16; i < 21; i++)
                temp.Add((Keys)i);
            for (int i = 32; i < 58; i++)
                temp.Add((Keys)i);
            for (int i = 65; i < 91; i++)
                temp.Add((Keys)i);
            for (int i = 95; i < 124; i++)
                temp.Add((Keys)i);
            for (int i = 144; i < 146; i++)
                temp.Add((Keys)i);
            for (int i = 160; i < 164; i++)
                temp.Add((Keys)i);
            return temp;
        }

        private void processInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if(listBox_processes.SelectedValue != null)
                {
                    var P = Process.GetProcessById(int.Parse(listBox_processes.SelectedValue.ToString()));
                    MessageBox.Show(
                        "Id: " + P.Id + 
                        "\nName: " + P.MainWindowTitle +
                        "\nProcessName: " + P.ProcessName + 
                        "\nMainModule: " + P.MainModule + 
                        "\nPath: " + P.MainModule.FileName);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
		{
			if (base.WindowState == FormWindowState.Minimized)
			{
				Hide();
				notifyIcon1.Visible = true;
			}
		}

		private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			Show();
			base.WindowState = FormWindowState.Normal;
			notifyIcon1.Visible = false;
		}

		public static Keys get_key(int nr)
		{
            return key[nr];
		}

		private void createDataTable()
		{
			dt = new DataTable();
			dt.Columns.Add("ProcessName");
			dt.Columns.Add("ProcessId");
			listBox_processes.DataSource = dt;
			listBox_processes.DisplayMember = "ProcessName";
			listBox_processes.ValueMember = "ProcessId";
			refreshDataTable();

		}

		private void refreshDataTable()
		{
			dt.Clear();
			Process[] processes = Process.GetProcesses(".");
			foreach (Process process in processes)
			{
				try
				{
					if (process.MainWindowTitle.Length > 0)
					{
						dt.Rows.Add();
						string mainWindowTitle = process.MainWindowTitle;
						string value = process.Id.ToString();
                        dt.Rows[dt.Rows.Count - 1][0] = mainWindowTitle;
						dt.Rows[dt.Rows.Count - 1][1] = value;

                        if(dt.Rows[dt.Rows.Count - 1][0].ToString() == "PLAYERUNKNOWN'S BATTLEGROUNDS ")
                            pubgPID = value;
                    }
				}
				catch
				{
				}
			}
            if (!string.IsNullOrEmpty(pubgPID))
                listBox_processes.SelectedValue = pubgPID;

        }

		private void buttonBindKeys_Click(object sender, EventArgs e)
		{
            key.Clear();
            key.Add((Keys)comboBox_Hotkey1.SelectedItem);
            key.Add((Keys)comboBox_Hotkey2.SelectedItem);
            key.Add((Keys)comboBox_Hotkey3.SelectedItem);
            key.Add((Keys)comboBox_Hotkey4.SelectedItem);
            List<string> temp = new List<string>();
            temp.Add(textBoxVol1.Text);
            temp.Add(textBoxVol2.Text);
            temp.Add(textBoxVol3.Text);
            temp.Add(textBoxVol4.Text);

            for(int i = 0; i < 4; i++)
            {
                foreach (BoundKeys BK in ListBoundKeys)
                {
                    try
                    {
                        if (key[i] == BK.Key)
                        {
                            ListBoundKeys.Remove(BK);
                            AvailableKeys.Add(BK.Key);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Make sure the hotkeys are correctly typed");
                    }
                }
                CheckInput(key[i], temp[i]);
            }
            if (hook == null)
                    hook = new KeyboardHook();            
        }

        private void buttonBindKey1And2_Click(object sender, EventArgs e)
        {
            key.Clear();
            key.Add((Keys)comboBox_Hotkey1.SelectedItem);
            key.Add((Keys)comboBox_Hotkey2.SelectedItem);
            List<string> temp = new List<string>();
            temp.Add(textBoxVol1.Text);
            temp.Add(textBoxVol2.Text);

            for (int i = 0; i < 4; i++)
            {
                foreach (BoundKeys BK in ListBoundKeys)
                {
                    try
                    {
                        if (key[i] == BK.Key)
                        {
                            ListBoundKeys.Remove(BK);
                            AvailableKeys.Add(BK.Key);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Make sure the hotkeys are correctly typed");
                    }
                }
                CheckInput(key[i], temp[i]);
            }
            if (hook == null)
                hook = new KeyboardHook();            
        }

        private void buttonBindKey3And4_Click(object sender, EventArgs e)
        {
            key.Clear();
            key.Add((Keys)comboBox_Hotkey3.SelectedItem);
            key.Add((Keys)comboBox_Hotkey4.SelectedItem);
            List<string> temp = new List<string>();
            temp.Add(textBoxVol3.Text);
            temp.Add(textBoxVol4.Text);

            for (int i = 0; i < 4; i++)
            {
                foreach (BoundKeys BK in ListBoundKeys)
                {
                    try
                    {
                        if (key[i] == BK.Key)
                        {
                            ListBoundKeys.Remove(BK);
                            AvailableKeys.Add(BK.Key);
                        }
                    }
                    catch(Exception)
                    {
                        MessageBox.Show("Make sure the hotkeys are correctly typed");
                    }
                }
                CheckInput(key[i], temp[i]);
            }
            if (hook == null)
                hook = new KeyboardHook();            
        }

        private void CheckInput(Keys K, string S)
        {
            float vol;
            string tempkey = string.Empty;
            int pid;
            if (!float.TryParse(S, out vol))
            {
                MessageBox.Show("Make sure volume is between 0-100");
            }
            else if (vol < 0f || vol > 100f)
            {
                MessageBox.Show("Make sure volume is between 0-100");
            }
            if (!int.TryParse(listBox_processes.SelectedValue.ToString(), out pid))
            {
                MessageBox.Show("Make sure you have selected an application");
            }

            try
            {
                hook.RegisterHotKey(K);
                AvailableKeys.Remove(K);

                BoundKeys temp = new BoundKeys();
                temp.Vol = vol;
                temp.PID = pid;
                temp.Key = K;
                ListBoundKeys.Add(temp);
            }
            catch
            {
                MessageBox.Show("Make sure the hotkeys are correctly typed");
            }
        }

        public static int GetPID(int hotkey)
		{
            return pidkey[hotkey];
        }

		private void buttonRefreshProcesses_Click(object sender, EventArgs e)
		{
			refreshDataTable();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void infoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Select the correct application in the list.\nMake sure you are using valid hotkeys, e.g. F1, F2, A, B etc.\nVolume must be between 0 and 100.\nIf you want to control multiple applications volume, you need to start more of this application\nHotkey1 will change the volume to Vol1's value, Hotkey2 will change the volume to Vol2's value, etc.\n\nThe program uses windows built-in Volume Mixer to change the volume, with other words nothing sketchy what so ever.\n\nPetroshek #2445 on discord if you want something added or changed\n");
		}        

        protected override void Dispose(bool disposing)
		{
            if (disposing && components != null)
			{
                hook.Dispose();
                components.Dispose();
			}
            base.Dispose(disposing);
		}

        private void button_unbindHotkey_Click(object sender, EventArgs e)
        {

        }

        private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HotkeyVolumeChanger));
            this.buttonBindKeys = new System.Windows.Forms.Button();
            this.madeByLabel = new System.Windows.Forms.Label();
            this.textBoxVol1 = new System.Windows.Forms.TextBox();
            this.textBoxVol2 = new System.Windows.Forms.TextBox();
            this.textBoxVol3 = new System.Windows.Forms.TextBox();
            this.textBoxVol4 = new System.Windows.Forms.TextBox();
            this.listBox_processes = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonRefreshProcesses = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonBindKey1And2 = new System.Windows.Forms.Button();
            this.buttonBindKey3And4 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.listBox_boundHotkeys = new System.Windows.Forms.ListBox();
            this.button_unbindHotkey = new System.Windows.Forms.Button();
            this.listBox_availableHotkeys = new System.Windows.Forms.ListBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBox_Hotkey1 = new System.Windows.Forms.ComboBox();
            this.comboBox_Hotkey2 = new System.Windows.Forms.ComboBox();
            this.comboBox_Hotkey3 = new System.Windows.Forms.ComboBox();
            this.comboBox_Hotkey4 = new System.Windows.Forms.ComboBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonBindKeys
            // 
            this.buttonBindKeys.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBindKeys.Location = new System.Drawing.Point(13, 421);
            this.buttonBindKeys.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBindKeys.Name = "buttonBindKeys";
            this.buttonBindKeys.Size = new System.Drawing.Size(107, 30);
            this.buttonBindKeys.TabIndex = 1;
            this.buttonBindKeys.Text = "Bind All Keys";
            this.buttonBindKeys.UseVisualStyleBackColor = true;
            this.buttonBindKeys.Click += new System.EventHandler(this.buttonBindKeys_Click);
            // 
            // madeByLabel
            // 
            this.madeByLabel.AutoSize = true;
            this.madeByLabel.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.madeByLabel.Location = new System.Drawing.Point(588, 438);
            this.madeByLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.madeByLabel.Name = "madeByLabel";
            this.madeByLabel.Size = new System.Drawing.Size(132, 17);
            this.madeByLabel.TabIndex = 6;
            this.madeByLabel.Text = "Made by Petroshek";
            // 
            // textBoxVol1
            // 
            this.textBoxVol1.AccessibleName = "Vol1";
            this.textBoxVol1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVol1.Location = new System.Drawing.Point(245, 325);
            this.textBoxVol1.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxVol1.MaxLength = 3;
            this.textBoxVol1.Name = "textBoxVol1";
            this.textBoxVol1.Size = new System.Drawing.Size(50, 25);
            this.textBoxVol1.TabIndex = 7;
            this.textBoxVol1.Text = "100";
            this.textBoxVol1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxVol2
            // 
            this.textBoxVol2.AccessibleName = "Vol2";
            this.textBoxVol2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVol2.Location = new System.Drawing.Point(245, 373);
            this.textBoxVol2.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxVol2.MaxLength = 3;
            this.textBoxVol2.Name = "textBoxVol2";
            this.textBoxVol2.Size = new System.Drawing.Size(50, 25);
            this.textBoxVol2.TabIndex = 8;
            this.textBoxVol2.Text = "75";
            this.textBoxVol2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxVol3
            // 
            this.textBoxVol3.AccessibleName = "Vol3";
            this.textBoxVol3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVol3.Location = new System.Drawing.Point(246, 421);
            this.textBoxVol3.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxVol3.MaxLength = 3;
            this.textBoxVol3.Name = "textBoxVol3";
            this.textBoxVol3.Size = new System.Drawing.Size(50, 25);
            this.textBoxVol3.TabIndex = 15;
            this.textBoxVol3.Text = "50";
            this.textBoxVol3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxVol4
            // 
            this.textBoxVol4.AccessibleName = "Vol4";
            this.textBoxVol4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVol4.Location = new System.Drawing.Point(246, 468);
            this.textBoxVol4.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxVol4.MaxLength = 3;
            this.textBoxVol4.Name = "textBoxVol4";
            this.textBoxVol4.Size = new System.Drawing.Size(50, 25);
            this.textBoxVol4.TabIndex = 16;
            this.textBoxVol4.Text = "25";
            this.textBoxVol4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // listBox_processes
            // 
            this.listBox_processes.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox_processes.FormattingEnabled = true;
            this.listBox_processes.ItemHeight = 17;
            this.listBox_processes.Location = new System.Drawing.Point(13, 66);
            this.listBox_processes.Margin = new System.Windows.Forms.Padding(4);
            this.listBox_processes.Name = "listBox_processes";
            this.listBox_processes.Size = new System.Drawing.Size(389, 225);
            this.listBox_processes.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(252, 305);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Vol1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(252, 354);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "Vol2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(312, 305);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 17);
            this.label4.TabIndex = 11;
            this.label4.Text = "Hotkey1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(312, 353);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "Hotkey2";
            // 
            // buttonRefreshProcesses
            // 
            this.buttonRefreshProcesses.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRefreshProcesses.Location = new System.Drawing.Point(13, 325);
            this.buttonRefreshProcesses.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRefreshProcesses.Name = "buttonRefreshProcesses";
            this.buttonRefreshProcesses.Size = new System.Drawing.Size(107, 85);
            this.buttonRefreshProcesses.TabIndex = 13;
            this.buttonRefreshProcesses.Text = "Refresh Processes";
            this.buttonRefreshProcesses.UseVisualStyleBackColor = true;
            this.buttonRefreshProcesses.Click += new System.EventHandler(this.buttonRefreshProcesses_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "HotkeyVolumeChanger";
            this.notifyIcon1.BalloonTipTitle = "HVC";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "HVC";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(730, 27);
            this.menuStrip1.TabIndex = 14;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoToolStripMenuItem,
            this.processInfoToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuToolStripMenuItem.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(55, 21);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.infoToolStripMenuItem.Text = "Info";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.infoToolStripMenuItem_Click);
            // 
            // processInfoToolStripMenuItem
            // 
            this.processInfoToolStripMenuItem.Name = "processInfoToolStripMenuItem";
            this.processInfoToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.processInfoToolStripMenuItem.Text = "Process Info";
            this.processInfoToolStripMenuItem.Click += new System.EventHandler(this.processInfoToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(252, 447);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 17);
            this.label1.TabIndex = 18;
            this.label1.Text = "Vol4";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(252, 401);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 17);
            this.label6.TabIndex = 17;
            this.label6.Text = "Vol3";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(312, 449);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 17);
            this.label7.TabIndex = 22;
            this.label7.Text = "Hotkey4";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(312, 401);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 17);
            this.label8.TabIndex = 21;
            this.label8.Text = "Hotkey3";
            // 
            // buttonBindKey1And2
            // 
            this.buttonBindKey1And2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBindKey1And2.Location = new System.Drawing.Point(127, 325);
            this.buttonBindKey1And2.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBindKey1And2.Name = "buttonBindKey1And2";
            this.buttonBindKey1And2.Size = new System.Drawing.Size(110, 30);
            this.buttonBindKey1And2.TabIndex = 25;
            this.buttonBindKey1And2.Text = "Bind Key 1+2";
            this.buttonBindKey1And2.UseVisualStyleBackColor = true;
            this.buttonBindKey1And2.Click += new System.EventHandler(this.buttonBindKey1And2_Click);
            // 
            // buttonBindKey3And4
            // 
            this.buttonBindKey3And4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBindKey3And4.Location = new System.Drawing.Point(127, 421);
            this.buttonBindKey3And4.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBindKey3And4.Name = "buttonBindKey3And4";
            this.buttonBindKey3And4.Size = new System.Drawing.Size(110, 30);
            this.buttonBindKey3And4.TabIndex = 26;
            this.buttonBindKey3And4.Text = "Bind Key 3+4";
            this.buttonBindKey3And4.UseVisualStyleBackColor = true;
            this.buttonBindKey3And4.Click += new System.EventHandler(this.buttonBindKey3And4_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(113, 35);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(160, 29);
            this.label9.TabIndex = 27;
            this.label9.Text = "Pick process";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // listBox_boundHotkeys
            // 
            this.listBox_boundHotkeys.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox_boundHotkeys.FormattingEnabled = true;
            this.listBox_boundHotkeys.ItemHeight = 17;
            this.listBox_boundHotkeys.Location = new System.Drawing.Point(407, 66);
            this.listBox_boundHotkeys.Margin = new System.Windows.Forms.Padding(4);
            this.listBox_boundHotkeys.Name = "listBox_boundHotkeys";
            this.listBox_boundHotkeys.Size = new System.Drawing.Size(155, 344);
            this.listBox_boundHotkeys.TabIndex = 28;
            // 
            // button_unbindHotkey
            // 
            this.button_unbindHotkey.Location = new System.Drawing.Point(407, 425);
            this.button_unbindHotkey.Name = "button_unbindHotkey";
            this.button_unbindHotkey.Size = new System.Drawing.Size(155, 30);
            this.button_unbindHotkey.TabIndex = 29;
            this.button_unbindHotkey.Text = "Unbind Key";
            this.button_unbindHotkey.UseVisualStyleBackColor = true;
            this.button_unbindHotkey.Click += new System.EventHandler(this.button_unbindHotkey_Click);
            // 
            // listBox_availableHotkeys
            // 
            this.listBox_availableHotkeys.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox_availableHotkeys.FormattingEnabled = true;
            this.listBox_availableHotkeys.ItemHeight = 17;
            this.listBox_availableHotkeys.Location = new System.Drawing.Point(570, 66);
            this.listBox_availableHotkeys.Margin = new System.Windows.Forms.Padding(4);
            this.listBox_availableHotkeys.Name = "listBox_availableHotkeys";
            this.listBox_availableHotkeys.Size = new System.Drawing.Size(155, 344);
            this.listBox_availableHotkeys.TabIndex = 30;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(406, 33);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(153, 29);
            this.label11.TabIndex = 32;
            this.label11.Text = "Bound Keys";
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(591, 33);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(117, 29);
            this.label10.TabIndex = 33;
            this.label10.Text = "Available";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // comboBox_Hotkey1
            // 
            this.comboBox_Hotkey1.FormattingEnabled = true;
            this.comboBox_Hotkey1.Location = new System.Drawing.Point(302, 325);
            this.comboBox_Hotkey1.Name = "comboBox_Hotkey1";
            this.comboBox_Hotkey1.Size = new System.Drawing.Size(100, 25);
            this.comboBox_Hotkey1.TabIndex = 34;
            // 
            // comboBox_Hotkey2
            // 
            this.comboBox_Hotkey2.FormattingEnabled = true;
            this.comboBox_Hotkey2.Location = new System.Drawing.Point(302, 373);
            this.comboBox_Hotkey2.Name = "comboBox_Hotkey2";
            this.comboBox_Hotkey2.Size = new System.Drawing.Size(100, 25);
            this.comboBox_Hotkey2.TabIndex = 35;
            // 
            // comboBox_Hotkey3
            // 
            this.comboBox_Hotkey3.FormattingEnabled = true;
            this.comboBox_Hotkey3.Location = new System.Drawing.Point(303, 421);
            this.comboBox_Hotkey3.Name = "comboBox_Hotkey3";
            this.comboBox_Hotkey3.Size = new System.Drawing.Size(100, 25);
            this.comboBox_Hotkey3.TabIndex = 36;
            // 
            // comboBox_Hotkey4
            // 
            this.comboBox_Hotkey4.FormattingEnabled = true;
            this.comboBox_Hotkey4.Location = new System.Drawing.Point(303, 469);
            this.comboBox_Hotkey4.Name = "comboBox_Hotkey4";
            this.comboBox_Hotkey4.Size = new System.Drawing.Size(100, 25);
            this.comboBox_Hotkey4.TabIndex = 37;
            // 
            // HotkeyVolumeChanger
            // 
            this.AccessibleDescription = "Changes applications volume with hotkeys";
            this.AccessibleName = "HotkeyVolumeChanger";
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(730, 502);
            this.Controls.Add(this.comboBox_Hotkey4);
            this.Controls.Add(this.comboBox_Hotkey3);
            this.Controls.Add(this.comboBox_Hotkey2);
            this.Controls.Add(this.comboBox_Hotkey1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.listBox_availableHotkeys);
            this.Controls.Add(this.button_unbindHotkey);
            this.Controls.Add(this.listBox_boundHotkeys);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.buttonBindKey3And4);
            this.Controls.Add(this.buttonBindKey1And2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxVol4);
            this.Controls.Add(this.textBoxVol3);
            this.Controls.Add(this.buttonRefreshProcesses);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxVol2);
            this.Controls.Add(this.textBoxVol1);
            this.Controls.Add(this.madeByLabel);
            this.Controls.Add(this.buttonBindKeys);
            this.Controls.Add(this.listBox_processes);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "HotkeyVolumeChanger";
            this.Text = "Hotkey Volume Changer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
    }

    public class BoundKeys
    {
        public Keys Key { get; set; }
        public float Vol { get; set; }
        public int PID { get; set; }
        public bool ToggleVol { get; set; }
        public float Vol1 { get; set; }
        public float Vol2 { get; set; }
    }
}
