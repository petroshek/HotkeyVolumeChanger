using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace HotkeyVolumeChanger
{
	public class Form1 : Form
	{
		private KeyboardHook hook = new KeyboardHook();
		private TypeConverter converter = TypeDescriptor.GetConverter(typeof(Keys));
		private static string key1;
		private static string key2;
        private static string key3;
        private static string key4;
        private static float vol1;
		private static float vol2;
        private static float vol3;
        private static float vol4;
        private static bool bound;
        private ListBox listBox1;
		private DataTable dt;
		private static int pid;
		private IContainer components;
		private Button buttonBindKeys;
		private TextBox textBox1;
		private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
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
        private ComboBox comboBox1;
        private Label label9;
        private ToolStripMenuItem exitToolStripMenuItem;
        private List<string> numberOfHotkeys;
        private int nrOfHotkeys;
        private string pubgPID;

		public Form1()
		{
			InitializeComponent();
            numberOfHotkeys = new List<string>();
            numberOfHotkeys.Add("1");
            numberOfHotkeys.Add("2");
            numberOfHotkeys.Add("3");
            numberOfHotkeys.Add("4");
            comboBox1.DataSource = numberOfHotkeys;
            comboBox1.SelectedIndex = 3;
            base.Resize += Form1_Resize;
			notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
			createDataTable();
		}

        private void processInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if(listBox1.SelectedValue != null)
                {
                    var P = Process.GetProcessById(int.Parse(listBox1.SelectedValue.ToString()));
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

		public static string get_key(int nr)
		{
            if (nr == 1)
                return key1;
            else if (nr == 2)
                return key2;
            else if (nr == 3)
                return key3;
            else
                return key4;
		}

		private void createDataTable()
		{
			dt = new DataTable();
			dt.Columns.Add("ProcessName");
			dt.Columns.Add("ProcessId");
			listBox1.DataSource = dt;
			listBox1.DisplayMember = "ProcessName";
			listBox1.ValueMember = "ProcessId";
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
                listBox1.SelectedValue = pubgPID;

        }

		private void buttonBindKeys_Click(object sender, EventArgs e)
		{
			if (bound)
			{
				hook.Dispose();
				hook = new KeyboardHook();
			}
            if(comboBox1.SelectedValue.ToString() == "1")
            {
                nrOfHotkeys = 1;
                key1 = textBox1.Text;
            }
            else if (comboBox1.SelectedValue.ToString() == "2")
            {
                nrOfHotkeys = 2;
                key1 = textBox1.Text;
                key2 = textBox2.Text;
            }
            else if (comboBox1.SelectedValue.ToString() == "3")
            {
                nrOfHotkeys = 3;
                key1 = textBox1.Text;
                key2 = textBox2.Text;
                key3 = textBox3.Text;
            }
            else if (comboBox1.SelectedValue.ToString() == "4")
            {
                nrOfHotkeys = 4;
                key1 = textBox1.Text;
                key2 = textBox2.Text;
                key3 = textBox3.Text;
                key4 = textBox4.Text;
            }
            CheckInput();
		}

        private void CheckInput()
        {
            if (!float.TryParse(textBoxVol1.Text, out vol1) || !float.TryParse(textBoxVol2.Text, out vol2) || !float.TryParse(textBoxVol3.Text, out vol3) || !float.TryParse(textBoxVol4.Text, out vol4))
            {
                MessageBox.Show("Make sure volume is between 0-100");
            }
            else if (vol1 < 0f || vol1 > 100f || vol2 < 0f || vol2 > 100f || vol3 < 0f || vol3 > 100f || vol4 < 0f || vol4 > 100f)
            {
                MessageBox.Show("Make sure volume is between 0-100");
            }
            if (!int.TryParse(listBox1.SelectedValue.ToString(), out pid))
            {
                MessageBox.Show("Make sure you have selected an application");
            }
            try
            {
                hook.RegisterHotKey((Keys)converter.ConvertFromString(key1));
                if (nrOfHotkeys > 1)
                    hook.RegisterHotKey((Keys)converter.ConvertFromString(key2));
                if(nrOfHotkeys > 2)
                    hook.RegisterHotKey((Keys)converter.ConvertFromString(key3));
                if (nrOfHotkeys > 3)
                    hook.RegisterHotKey((Keys)converter.ConvertFromString(key4));
            }
            catch
            {
                MessageBox.Show("Make sure the hotkeys are correctly typed");
            }
            bound = true;
        }

        public static int getPID()
		{
			return pid;
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

		public static float getVol1()
		{
			return vol1;
		}
		public static float getVol2()
		{
			return vol2;
		}
        public static float getVol3()
        {
            return vol3;
        }
        public static float getVol4()
        {
            return vol4;
        }


        protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.buttonBindKeys = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.madeByLabel = new System.Windows.Forms.Label();
            this.textBoxVol1 = new System.Windows.Forms.TextBox();
            this.textBoxVol2 = new System.Windows.Forms.TextBox();
            this.textBoxVol3 = new System.Windows.Forms.TextBox();
            this.textBoxVol4 = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonBindKeys
            // 
            this.buttonBindKeys.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBindKeys.Location = new System.Drawing.Point(136, 418);
            this.buttonBindKeys.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBindKeys.Name = "buttonBindKeys";
            this.buttonBindKeys.Size = new System.Drawing.Size(100, 30);
            this.buttonBindKeys.TabIndex = 1;
            this.buttonBindKeys.Text = "Bind Keys";
            this.buttonBindKeys.UseVisualStyleBackColor = true;
            this.buttonBindKeys.Click += new System.EventHandler(this.buttonBindKeys_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(136, 325);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.MaxLength = 2;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(50, 25);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "F1";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(196, 325);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4);
            this.textBox2.MaxLength = 2;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(50, 25);
            this.textBox2.TabIndex = 5;
            this.textBox2.Text = "F2";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(256, 325);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4);
            this.textBox3.MaxLength = 2;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(50, 25);
            this.textBox3.TabIndex = 19;
            this.textBox3.Text = "F3";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox4
            // 
            this.textBox4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.Location = new System.Drawing.Point(316, 325);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4);
            this.textBox4.MaxLength = 2;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(50, 25);
            this.textBox4.TabIndex = 20;
            this.textBox4.Text = "F4";
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // madeByLabel
            // 
            this.madeByLabel.AutoSize = true;
            this.madeByLabel.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.madeByLabel.Location = new System.Drawing.Point(256, 425);
            this.madeByLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.madeByLabel.Name = "madeByLabel";
            this.madeByLabel.Size = new System.Drawing.Size(132, 17);
            this.madeByLabel.TabIndex = 6;
            this.madeByLabel.Text = "Made by Petroshek";
            // 
            // textBoxVol1
            // 
            this.textBoxVol1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVol1.Location = new System.Drawing.Point(136, 385);
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
            this.textBoxVol2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVol2.Location = new System.Drawing.Point(196, 385);
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
            this.textBoxVol3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVol3.Location = new System.Drawing.Point(256, 385);
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
            this.textBoxVol4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVol4.Location = new System.Drawing.Point(316, 385);
            this.textBoxVol4.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxVol4.MaxLength = 3;
            this.textBoxVol4.Name = "textBoxVol4";
            this.textBoxVol4.Size = new System.Drawing.Size(50, 25);
            this.textBoxVol4.TabIndex = 16;
            this.textBoxVol4.Text = "25";
            this.textBoxVol4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 17;
            this.listBox1.Location = new System.Drawing.Point(13, 71);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(380, 225);
            this.listBox1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(139, 364);
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
            this.label3.Location = new System.Drawing.Point(200, 364);
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
            this.label4.Location = new System.Drawing.Point(133, 300);
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
            this.label5.Location = new System.Drawing.Point(193, 300);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "Hotkey2";
            // 
            // buttonRefreshProcesses
            // 
            this.buttonRefreshProcesses.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRefreshProcesses.Location = new System.Drawing.Point(13, 357);
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
            this.notifyIcon1.Text = "HVC";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(404, 27);
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
            this.label1.Location = new System.Drawing.Point(322, 364);
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
            this.label6.Location = new System.Drawing.Point(262, 364);
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
            this.label7.Location = new System.Drawing.Point(313, 300);
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
            this.label8.Location = new System.Drawing.Point(253, 300);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 17);
            this.label8.TabIndex = 21;
            this.label8.Text = "Hotkey3";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.comboBox1.Location = new System.Drawing.Point(51, 325);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(47, 25);
            this.comboBox1.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(48, 300);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 17);
            this.label9.TabIndex = 24;
            this.label9.Text = "# Hotkeys";
            // 
            // Form1
            // 
            this.AccessibleDescription = "Changes applications volume with hotkeys";
            this.AccessibleName = "HotkeyVolumeChanger";
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.ClientSize = new System.Drawing.Size(404, 461);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
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
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonBindKeys);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Hotkey Volume Changer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
    }
}
