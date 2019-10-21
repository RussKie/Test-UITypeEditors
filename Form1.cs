using System.Windows.Forms;

namespace NetFxApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            foreach (Control control in flowLayoutPanel1.Controls)
            {
                control.MouseDown += (s, e) =>
                {
                    lblSelectedObject.Text = s.GetType().FullName;
                    propertyGrid1.SelectedObject = s;
                };
            }
        }

        private void btnImageList_Click(object sender, System.EventArgs e)
        {
            lblSelectedObject.Text = imageList1.GetType().FullName;
            propertyGrid1.SelectedObject = imageList1;
        }
    }
}
