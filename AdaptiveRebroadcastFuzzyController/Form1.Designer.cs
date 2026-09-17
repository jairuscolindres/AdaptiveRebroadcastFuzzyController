namespace AdaptiveRebroadcastFuzzyController
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
            this.btnInitialize = new System.Windows.Forms.Button();
            this.txtNodes = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPackets = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRadius = new System.Windows.Forms.RichTextBox();
            this.canvasPanel = new System.Windows.Forms.PictureBox();
            this.btnRun = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.canvasPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // btnInitialize
            // 
            this.btnInitialize.Location = new System.Drawing.Point(33, 317);
            this.btnInitialize.Name = "btnInitialize";
            this.btnInitialize.Size = new System.Drawing.Size(79, 23);
            this.btnInitialize.TabIndex = 0;
            this.btnInitialize.Text = "Initialize";
            this.btnInitialize.UseVisualStyleBackColor = true;
            this.btnInitialize.Click += new System.EventHandler(this.btnInitialize_Click);
            // 
            // txtNodes
            // 
            this.txtNodes.Location = new System.Drawing.Point(194, 317);
            this.txtNodes.Name = "txtNodes";
            this.txtNodes.Size = new System.Drawing.Size(100, 23);
            this.txtNodes.TabIndex = 1;
            this.txtNodes.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(153, 322);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Nodes";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(332, 324);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Packets";
            // 
            // txtPackets
            // 
            this.txtPackets.Location = new System.Drawing.Point(377, 319);
            this.txtPackets.Name = "txtPackets";
            this.txtPackets.Size = new System.Drawing.Size(100, 23);
            this.txtPackets.TabIndex = 3;
            this.txtPackets.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(517, 322);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Radius";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtRadius
            // 
            this.txtRadius.Location = new System.Drawing.Point(562, 317);
            this.txtRadius.Name = "txtRadius";
            this.txtRadius.Size = new System.Drawing.Size(100, 23);
            this.txtRadius.TabIndex = 5;
            this.txtRadius.Text = "";
            // 
            // canvasPanel
            // 
            this.canvasPanel.Location = new System.Drawing.Point(92, 33);
            this.canvasPanel.Name = "canvasPanel";
            this.canvasPanel.Size = new System.Drawing.Size(524, 258);
            this.canvasPanel.TabIndex = 7;
            this.canvasPanel.TabStop = false;
            this.canvasPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.canvasPanel_Paint);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(33, 346);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(79, 23);
            this.btnRun.TabIndex = 8;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.canvasPanel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtRadius);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPackets);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtNodes);
            this.Controls.Add(this.btnInitialize);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.canvasPanel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInitialize;
        private System.Windows.Forms.RichTextBox txtNodes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox txtPackets;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox txtRadius;
        private System.Windows.Forms.PictureBox canvasPanel;
        private System.Windows.Forms.Button btnRun;
    }
}

