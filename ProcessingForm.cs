using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;


namespace ProcessingEmulator
{
    public partial class ProcessingForm : Form
    {
        static ProcessingForm processing;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            processing = new ProcessingForm();
            Application.Run(processing);
        }

        public ProcessingForm()
        {
            InitializeComponent();
            draw(Processing.setup);
        }


        bool manual = false;
        delegate void DrawDelegate(Graphics g);
        DrawDelegate func;
        private void draw(DrawDelegate func)
        {
            this.func = func;
            manual = true;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (manual)
            {
                this.func(e.Graphics);
                manual = false;
            }
            base.OnPaint(e);
        }

        private void ProcessingForm_Resize(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case FormWindowState.Maximized:
                    this.FormBorderStyle = FormBorderStyle.None;
                    break;
                default:
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    break;
            }
        }

        private void ProcessingForm_DoubleClick(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case FormWindowState.Maximized:
                    this.WindowState = FormWindowState.Normal;
                    break;
                case FormWindowState.Normal:
                    this.WindowState = FormWindowState.Maximized;
                    break;
            }
        }




    }
}
