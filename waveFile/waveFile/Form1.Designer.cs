﻿namespace waveFile
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.openWavButton = new System.Windows.Forms.Button();
            this.pauseButton = new System.Windows.Forms.Button();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.volumeBar = new System.Windows.Forms.VScrollBar();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // openWavButton
            // 
            this.openWavButton.Location = new System.Drawing.Point(68, 12);
            this.openWavButton.Name = "openWavButton";
            this.openWavButton.Size = new System.Drawing.Size(159, 45);
            this.openWavButton.TabIndex = 0;
            this.openWavButton.Text = "OpenMp3File";
            this.openWavButton.UseVisualStyleBackColor = true;
            this.openWavButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // pauseButton
            // 
            this.pauseButton.Location = new System.Drawing.Point(68, 87);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(159, 44);
            this.pauseButton.TabIndex = 1;
            this.pauseButton.Text = "Play/Pause Wav";
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // chart1
            // 
            chartArea3.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea3);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Right;
            this.chart1.Location = new System.Drawing.Point(246, 0);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(723, 321);
            this.chart1.TabIndex = 2;
            this.chart1.Text = "chart1";
            // 
            // volumeBar
            // 
            this.volumeBar.Location = new System.Drawing.Point(20, 87);
            this.volumeBar.Name = "volumeBar";
            this.volumeBar.Size = new System.Drawing.Size(17, 191);
            this.volumeBar.TabIndex = 3;
            this.volumeBar.Value = 50;
            this.volumeBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.volumeBar1_Scroll);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 321);
            this.Controls.Add(this.volumeBar);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.pauseButton);
            this.Controls.Add(this.openWavButton);
            this.Name = "Form1";
            this.Text = "WavPlayer";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button openWavButton;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.VScrollBar volumeBar;
    }
}

