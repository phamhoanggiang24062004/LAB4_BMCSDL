using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_3___BMCSDL
{
    public partial class UcThemHocPhan : UserControl
    {
        string connectionString = @"Server=DESKTOP-P0BQAJD;Database=QLSVNhom;Trusted_Connection=True;";

        private Label lblMahp, lblTenhp, lblSotc;
        private TextBox txtMahp, txtTenhp, txtSotc;
        private Button btnSave;

        public UcThemHocPhan()
        {
            InitializeComponent();
            InitializeThemHocPhanUI();
        }

        private void InitializeThemHocPhanUI()
        {
            this.BackColor = Color.White;
            this.Size = new Size(600, 400); 

            int labelWidth = 100;
            int inputWidth = 250;
            int height = 30;
            int spacing = 30;
            int marginLeft = 30;
            int marginTop = 90;

            Font labelFont = new Font("Segoe UI", 10, FontStyle.Bold);
            Font inputFont = new Font("Segoe UI", 10, FontStyle.Regular);
            Font titleFont = new Font("Segoe UI", 14, FontStyle.Bold);

            // Tiêu đề
            Label lblTitle = new Label
            {
                Text = "THÔNG TIN HỌC PHẦN",
                Location = new Point(0, 10),
                Size = new Size(this.Width, 40),
                Font = titleFont,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Mã học phần
            lblMahp = new Label
            {
                Text = "Mã HP:",
                Location = new Point(marginLeft, marginTop),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtMahp = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };


            // Tên học phần
            lblTenhp = new Label
            {
                Text = "Ten HP:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 1),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtTenhp = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 1),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Số tín chỉ
            lblSotc = new Label
            {
                Text = "Số TC:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 2),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtSotc = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 2),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Nút Lưu
            btnSave = new Button
            {
                Text = "Lưu",
                Location = new Point(marginLeft + (labelWidth + inputWidth - 100) / 2, marginTop + (height + spacing) * 3),
                Size = new Size(100, 30),
                Font = inputFont
            };
            btnSave.Click += BtnSave_Click;

            // Thêm vào Control
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblMahp);
            this.Controls.Add(txtMahp);
            this.Controls.Add(lblTenhp);
            this.Controls.Add(txtTenhp);
            this.Controls.Add(lblSotc);
            this.Controls.Add(txtSotc);
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string mahp = txtMahp.Text.Trim();
            string tenhp = txtTenhp.Text.Trim();
            string sotcText = txtSotc.Text.Trim();

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(mahp) || string.IsNullOrEmpty(tenhp) || string.IsNullOrEmpty(sotcText))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin học phần!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra định dạng số tín chỉ
            if (!int.TryParse(sotcText, out int sotc) || sotc <= 0)
            {
                MessageBox.Show("Số tín chỉ phải là số nguyên dương!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_INS_HOCPHAN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MAHP", mahp);
                        cmd.Parameters.AddWithValue("@TENHP", tenhp);
                        cmd.Parameters.AddWithValue("@SOTC", sotc);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Thêm học phần thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Xóa dữ liệu sau khi lưu
                txtMahp.Clear();
                txtTenhp.Clear();
                txtSotc.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm học phần: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
