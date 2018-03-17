using System;
using System.Windows.Forms;


namespace RailwayGIS
{
    public partial class MaxForm : Form
    {
        public FormState formState = new FormState();
        public GISForm gf = null;

        public MaxForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            gf.setMainContainer();           
            Hide();
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    formState.Restore(this);
        //}
    }
}