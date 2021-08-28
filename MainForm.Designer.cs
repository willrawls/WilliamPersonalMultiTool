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
            this.KeySequenceList.Location = new System.Drawing.Point(0, 39);
            this.KeySequenceList.Name = "KeySequenceList";
            this.KeySequenceList.Size = new System.Drawing.Size(614, 316);
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
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(614, 39);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // EditButton
            // 
            this.EditButton.BackColor = System.Drawing.Color.Navy;
            this.EditButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.EditButton.ForeColor = System.Drawing.Color.Yellow;
            this.EditButton.Location = new System.Drawing.Point(3, 3);
            this.EditButton.Name = "EditButton";
            this.EditButton.Size = new System.Drawing.Size(80, 37);
            this.EditButton.TabIndex = 0;
            this.EditButton.Text = "&Edit";
            this.EditButton.UseVisualStyleBackColor = false;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // ReloadButton
            // 
            this.ReloadButton.BackColor = System.Drawing.Color.Navy;
            this.ReloadButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ReloadButton.ForeColor = System.Drawing.Color.Yellow;
            this.ReloadButton.Location = new System.Drawing.Point(89, 3);
            this.ReloadButton.Name = "ReloadButton";
            this.ReloadButton.Size = new System.Drawing.Size(80, 37);
            this.ReloadButton.TabIndex = 1;
            this.ReloadButton.Text = "&Reload";
            this.ReloadButton.UseVisualStyleBackColor = false;
            this.ReloadButton.Click += new System.EventHandler(this.ReloadButton_Click);
            // 
            // ToggleOnOffButton
            // 
            this.ToggleOnOffButton.BackColor = System.Drawing.Color.Navy;
            this.ToggleOnOffButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ToggleOnOffButton.ForeColor = System.Drawing.Color.Yellow;
            this.ToggleOnOffButton.Location = new System.Drawing.Point(175, 3);
            this.ToggleOnOffButton.Name = "ToggleOnOffButton";
            this.ToggleOnOffButton.Size = new System.Drawing.Size(80, 37);
            this.ToggleOnOffButton.TabIndex = 2;
            this.ToggleOnOffButton.Text = "Turn &Off";
            this.ToggleOnOffButton.UseVisualStyleBackColor = false;
            this.ToggleOnOffButton.Click += new System.EventHandler(this.ToggleOnOffButton_Click);
            // 
            // HideStaticSequencesButton
            // 
            this.HideStaticSequencesButton.BackColor = System.Drawing.Color.Navy;
            this.HideStaticSequencesButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.HideStaticSequencesButton.ForeColor = System.Drawing.Color.Yellow;
            this.HideStaticSequencesButton.Location = new System.Drawing.Point(261, 3);
            this.HideStaticSequencesButton.Name = "HideStaticSequencesButton";
            this.HideStaticSequencesButton.Size = new System.Drawing.Size(95, 37);
            this.HideStaticSequencesButton.TabIndex = 2;
            this.HideStaticSequencesButton.Text = "&Hide statics";
            this.HideStaticSequencesButton.UseVisualStyleBackColor = false;
            this.HideStaticSequencesButton.Click += new System.EventHandler(this.HideStaticSequencesButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 355);
            this.Controls.Add(this.KeySequenceList);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "William\'s Personal Multitool";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
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
    }
}



