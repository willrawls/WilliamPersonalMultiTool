﻿
namespace WilliamPersonalMultiTool
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.KeySequenceList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.EditButton = new System.Windows.Forms.Button();
            this.ReloadButton = new System.Windows.Forms.Button();
            this.ToggleOnOffButton = new System.Windows.Forms.Button();
            this.HideStaticSequencesButton = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.Button2 = new System.Windows.Forms.Button();
            this.Button3 = new System.Windows.Forms.Button();
            this.Button4 = new System.Windows.Forms.Button();
            this.Button5 = new System.Windows.Forms.Button();
            this.Button6 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // KeySequenceList
            // 
            this.KeySequenceList.AllowDrop = true;
            this.KeySequenceList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.KeySequenceList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.KeySequenceList.FullRowSelect = true;
            this.KeySequenceList.HideSelection = false;
            this.KeySequenceList.Location = new System.Drawing.Point(0, 57);
            this.KeySequenceList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.KeySequenceList.Name = "KeySequenceList";
            this.KeySequenceList.Size = new System.Drawing.Size(368, 112);
            this.KeySequenceList.TabIndex = 3;
            this.KeySequenceList.UseCompatibleStateImageBehavior = false;
            this.KeySequenceList.View = System.Windows.Forms.View.Details;
            this.KeySequenceList.DragDrop += new System.Windows.Forms.DragEventHandler(this.KeySequenceList_DragDrop);
            this.KeySequenceList.DragEnter += new System.Windows.Forms.DragEventHandler(this.KeySequenceList_DragEnter);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Key Sequence";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Expansion";
            this.columnHeader2.Width = 200;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.MidnightBlue;
            this.flowLayoutPanel1.Controls.Add(this.EditButton);
            this.flowLayoutPanel1.Controls.Add(this.ReloadButton);
            this.flowLayoutPanel1.Controls.Add(this.ToggleOnOffButton);
            this.flowLayoutPanel1.Controls.Add(this.HideStaticSequencesButton);
            this.flowLayoutPanel1.Controls.Add(this.Button1);
            this.flowLayoutPanel1.Controls.Add(this.Button2);
            this.flowLayoutPanel1.Controls.Add(this.Button3);
            this.flowLayoutPanel1.Controls.Add(this.Button4);
            this.flowLayoutPanel1.Controls.Add(this.Button5);
            this.flowLayoutPanel1.Controls.Add(this.Button6);
            this.flowLayoutPanel1.Controls.Add(this.textBox1);
            this.flowLayoutPanel1.Controls.Add(this.comboBox1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(368, 57);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // EditButton
            // 
            this.EditButton.BackColor = System.Drawing.Color.SteelBlue;
            this.EditButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.EditButton.ForeColor = System.Drawing.Color.White;
            this.EditButton.Location = new System.Drawing.Point(3, 2);
            this.EditButton.Margin = new System.Windows.Forms.Padding(0);
            this.EditButton.Name = "EditButton";
            this.EditButton.Size = new System.Drawing.Size(44, 28);
            this.EditButton.TabIndex = 0;
            this.EditButton.Text = "&Edit";
            this.EditButton.UseVisualStyleBackColor = false;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // ReloadButton
            // 
            this.ReloadButton.BackColor = System.Drawing.Color.SteelBlue;
            this.ReloadButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ReloadButton.ForeColor = System.Drawing.Color.White;
            this.ReloadButton.Location = new System.Drawing.Point(47, 2);
            this.ReloadButton.Margin = new System.Windows.Forms.Padding(0);
            this.ReloadButton.Name = "ReloadButton";
            this.ReloadButton.Size = new System.Drawing.Size(50, 28);
            this.ReloadButton.TabIndex = 1;
            this.ReloadButton.Text = "&Load";
            this.ReloadButton.UseVisualStyleBackColor = false;
            this.ReloadButton.Click += new System.EventHandler(this.ReloadButton_Click);
            // 
            // ToggleOnOffButton
            // 
            this.ToggleOnOffButton.BackColor = System.Drawing.Color.SteelBlue;
            this.ToggleOnOffButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ToggleOnOffButton.ForeColor = System.Drawing.Color.White;
            this.ToggleOnOffButton.Location = new System.Drawing.Point(97, 2);
            this.ToggleOnOffButton.Margin = new System.Windows.Forms.Padding(0);
            this.ToggleOnOffButton.Name = "ToggleOnOffButton";
            this.ToggleOnOffButton.Size = new System.Drawing.Size(38, 28);
            this.ToggleOnOffButton.TabIndex = 2;
            this.ToggleOnOffButton.Text = "&Off";
            this.ToggleOnOffButton.UseVisualStyleBackColor = false;
            this.ToggleOnOffButton.Click += new System.EventHandler(this.ToggleOnOffButton_Click);
            // 
            // HideStaticSequencesButton
            // 
            this.HideStaticSequencesButton.BackColor = System.Drawing.Color.SteelBlue;
            this.HideStaticSequencesButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.HideStaticSequencesButton.ForeColor = System.Drawing.Color.White;
            this.HideStaticSequencesButton.Location = new System.Drawing.Point(135, 2);
            this.HideStaticSequencesButton.Margin = new System.Windows.Forms.Padding(0);
            this.HideStaticSequencesButton.Name = "HideStaticSequencesButton";
            this.HideStaticSequencesButton.Size = new System.Drawing.Size(58, 28);
            this.HideStaticSequencesButton.TabIndex = 2;
            this.HideStaticSequencesButton.Text = "&Mode";
            this.HideStaticSequencesButton.UseVisualStyleBackColor = false;
            this.HideStaticSequencesButton.Click += new System.EventHandler(this.HideStaticSequencesButton_Click);
            // 
            // Button1
            // 
            this.Button1.BackColor = System.Drawing.Color.SteelBlue;
            this.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Button1.ForeColor = System.Drawing.Color.White;
            this.Button1.Location = new System.Drawing.Point(193, 2);
            this.Button1.Margin = new System.Windows.Forms.Padding(0);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(28, 28);
            this.Button1.TabIndex = 2;
            this.Button1.Text = "&1";
            this.Button1.UseVisualStyleBackColor = false;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Button2
            // 
            this.Button2.BackColor = System.Drawing.Color.SteelBlue;
            this.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Button2.ForeColor = System.Drawing.Color.White;
            this.Button2.Location = new System.Drawing.Point(221, 2);
            this.Button2.Margin = new System.Windows.Forms.Padding(0);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(28, 28);
            this.Button2.TabIndex = 2;
            this.Button2.Text = "&2";
            this.Button2.UseVisualStyleBackColor = false;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Button3
            // 
            this.Button3.BackColor = System.Drawing.Color.SteelBlue;
            this.Button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Button3.ForeColor = System.Drawing.Color.White;
            this.Button3.Location = new System.Drawing.Point(249, 2);
            this.Button3.Margin = new System.Windows.Forms.Padding(0);
            this.Button3.Name = "Button3";
            this.Button3.Size = new System.Drawing.Size(28, 28);
            this.Button3.TabIndex = 2;
            this.Button3.Text = "&3";
            this.Button3.UseVisualStyleBackColor = false;
            this.Button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // Button4
            // 
            this.Button4.BackColor = System.Drawing.Color.SteelBlue;
            this.Button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Button4.ForeColor = System.Drawing.Color.White;
            this.Button4.Location = new System.Drawing.Point(277, 2);
            this.Button4.Margin = new System.Windows.Forms.Padding(0);
            this.Button4.Name = "Button4";
            this.Button4.Size = new System.Drawing.Size(28, 28);
            this.Button4.TabIndex = 2;
            this.Button4.Text = "&4";
            this.Button4.UseVisualStyleBackColor = false;
            this.Button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // Button5
            // 
            this.Button5.BackColor = System.Drawing.Color.SteelBlue;
            this.Button5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Button5.ForeColor = System.Drawing.Color.White;
            this.Button5.Location = new System.Drawing.Point(305, 2);
            this.Button5.Margin = new System.Windows.Forms.Padding(0);
            this.Button5.Name = "Button5";
            this.Button5.Size = new System.Drawing.Size(28, 28);
            this.Button5.TabIndex = 2;
            this.Button5.Text = "&5";
            this.Button5.UseVisualStyleBackColor = false;
            this.Button5.Click += new System.EventHandler(this.Button5_Click);
            // 
            // Button6
            // 
            this.Button6.BackColor = System.Drawing.Color.SteelBlue;
            this.Button6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Button6.ForeColor = System.Drawing.Color.White;
            this.Button6.Location = new System.Drawing.Point(333, 2);
            this.Button6.Margin = new System.Windows.Forms.Padding(0);
            this.Button6.Name = "Button6";
            this.Button6.Size = new System.Drawing.Size(28, 28);
            this.Button6.TabIndex = 2;
            this.Button6.Text = "&T";
            this.Button6.UseVisualStyleBackColor = false;
            this.Button6.Click += new System.EventHandler(this.Button6_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 34);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(165, 23);
            this.textBox1.TabIndex = 3;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(179, 32);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(182, 23);
            this.comboBox1.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 169);
            this.Controls.Add(this.KeySequenceList);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "William\'s Personal Multitool";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView KeySequenceList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button EditButton;
        private System.Windows.Forms.Button ReloadButton;
        private System.Windows.Forms.Button ToggleOnOffButton;
        private System.Windows.Forms.Button HideStaticSequencesButton;
        private System.Windows.Forms.Button Button1;
        private System.Windows.Forms.Button Button2;
        private System.Windows.Forms.Button Button3;
        private System.Windows.Forms.Button Button4;
        private System.Windows.Forms.Button Button5;
        private System.Windows.Forms.Button Button6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}



