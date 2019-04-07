namespace CS_Swarm
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
            this.floorPlan = new System.Windows.Forms.PictureBox();
            this.exitButton = new System.Windows.Forms.Button();
            this.mapButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.floorPlan)).BeginInit();
            this.SuspendLayout();
            // 
            // floorPlan
            // 
            this.floorPlan.BackColor = System.Drawing.Color.White;
            this.floorPlan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.floorPlan.Location = new System.Drawing.Point(0, 0);
            this.floorPlan.Name = "floorPlan";
            this.floorPlan.Size = new System.Drawing.Size(1008, 873);
            this.floorPlan.TabIndex = 0;
            this.floorPlan.TabStop = false;
            // 
            // exitButton
            // 
            this.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.exitButton.Location = new System.Drawing.Point(767, 800);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(229, 45);
            this.exitButton.TabIndex = 1;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // mapButton
            // 
            this.mapButton.Location = new System.Drawing.Point(532, 800);
            this.mapButton.Name = "mapButton";
            this.mapButton.Size = new System.Drawing.Size(229, 45);
            this.mapButton.TabIndex = 2;
            this.mapButton.Text = "Map";
            this.mapButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.exitButton;
            this.ClientSize = new System.Drawing.Size(1008, 873);
            this.Controls.Add(this.mapButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.floorPlan);
            this.Name = "Form1";
            this.Text = "Floor Plan";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.floorPlan)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox floorPlan;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button mapButton;
    }
}

