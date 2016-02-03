﻿using NarlonLib.Control;

namespace TaleofMonsters.Forms
{
    sealed partial class NpcShopForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.bitmapButtonClose2 = new NarlonLib.Control.BitmapButton();

            this.SuspendLayout();
            // 
            // bitmapButtonClose2
            // 
            this.bitmapButtonClose2.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose2.Image = null;
            this.bitmapButtonClose2.ImageNormal = null;
            this.bitmapButtonClose2.Location = new System.Drawing.Point(265, 45);
            this.bitmapButtonClose2.Name = "bitmapButtonClose2";
            this.bitmapButtonClose2.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose2.TabIndex = 23;
            this.bitmapButtonClose2.Text = "";
            this.bitmapButtonClose2.UseVisualStyleBackColor = true;
            this.bitmapButtonClose2.Click += new System.EventHandler(this.pictureBoxCancel_Click);
            // 
            // NpcShopForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.bitmapButtonClose2);
            this.DoubleBuffered = true;
            this.Name = "NpcShopForm";
            this.Size = new System.Drawing.Size(301, 278);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ShopWindow_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose2;
    }
}