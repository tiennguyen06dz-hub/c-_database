
using System;
using System.Windows.Forms;
using Bai6._1.DAL;

namespace Bai6._1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            try
            {
                using (var conn =
                    DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    MessageBox.Show(
                        "Kết nối SalesDB thành công!",
                        "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi kết nối: " + ex.Message);
            }
        }
    }
}
