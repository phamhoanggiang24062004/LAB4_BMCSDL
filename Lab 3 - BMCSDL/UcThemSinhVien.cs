using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_3___BMCSDL
{
    public partial class UcThemSinhVien : UserControl
    {
        string connectionString = @"Server=DESKTOP-P0BQAJD;Database=QLSVNhom;Trusted_Connection=True;";

        private Label lblMasv, lblHoten, lblNgaysinh, lblDiachi, lblMalop, lblTendn, lblMatkhau;
        private TextBox txtMasv, txtHoten, txtNgaysinh, txtDiachi, txtMalop, txtTendn, txtMatkhau;
        private Button btnSave;
       

        public UcThemSinhVien()
        {
            InitializeComponent();
            InitializeThemSinhVienUI();
        }

        private void InitializeThemSinhVienUI()
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
                Text = "THÔNG TIN SINH VIÊN",
                Location = new Point(0, 10),
                Size = new Size(this.Width, 40),
                Font = titleFont,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Mã sinh viên
            lblMasv = new Label
            {
                Text = "Mã SV:",
                Location = new Point(marginLeft, marginTop),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtMasv = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };


            // Họ tên
            lblHoten = new Label
            {
                Text = "Họ tên:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 1),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtHoten = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 1),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Ngày sinh
            lblNgaysinh = new Label
            {
                Text = "Ngày sinh:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 2),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtNgaysinh = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 2),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Địa chỉ
            lblDiachi = new Label
            {
                Text = "Địa chỉ:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 3),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtDiachi = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 3),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Mã lớp
            lblMalop = new Label
            {
                Text = "Mã lớp:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 4),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtMalop = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 4),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Tên đăng nhập
            lblTendn = new Label
            {
                Text = "Tên ĐN:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 5),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtTendn = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 5),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Mật khẩu
            lblMatkhau = new Label
            {
                Text = "Mật khẩu:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 6),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtMatkhau = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 6),
                Size = new Size(inputWidth, height),
                Font = inputFont

            };


            // Nút Lưu
            btnSave = new Button
            {
                Text = "Lưu",
                Location = new Point(marginLeft + (labelWidth + inputWidth - 100) / 2, marginTop + (height + spacing) * 7),
                Size = new Size(100, 30),
                Font = inputFont
            };
            btnSave.Click += BtnSave_Click;

            // Thêm vào Control
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblMasv);
            this.Controls.Add(txtMasv);
            this.Controls.Add(lblHoten);
            this.Controls.Add(txtHoten);
            this.Controls.Add(lblNgaysinh);
            this.Controls.Add(txtNgaysinh);
            this.Controls.Add(lblDiachi);
            this.Controls.Add(txtDiachi);
            this.Controls.Add(lblMalop);
            this.Controls.Add(txtMalop);
            this.Controls.Add(lblTendn);
            this.Controls.Add(txtTendn);
            this.Controls.Add(lblMatkhau);
            this.Controls.Add(txtMatkhau);
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string masv = txtMasv.Text.Trim();
            string hoten = txtHoten.Text.Trim();
            string ngaysinhText = txtNgaysinh.Text.Trim();
            string diachi = txtDiachi.Text.Trim();
            string malop = txtMalop.Text.Trim();
            string tendn = txtTendn.Text.Trim();
            string matkhau = txtMatkhau.Text.Trim();

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(masv) || string.IsNullOrEmpty(hoten) || string.IsNullOrEmpty(ngaysinhText)
                || string.IsNullOrEmpty(diachi) || string.IsNullOrEmpty(malop)
                || string.IsNullOrEmpty(tendn) || string.IsNullOrEmpty(matkhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra định dạng ngày sinh
            if (!DateTime.TryParse(ngaysinhText, out DateTime ngaysinh))
            {
                MessageBox.Show("Ngày sinh không hợp lệ! Định dạng: dd/MM/yyyy", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_INS_SINHVIEN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MASV", masv);
                        cmd.Parameters.AddWithValue("@HOTEN", hoten);
                        cmd.Parameters.AddWithValue("@NGAYSINH", ngaysinh);
                        cmd.Parameters.AddWithValue("@DIACHI", diachi);
                        cmd.Parameters.AddWithValue("@MALOP", malop);
                        cmd.Parameters.AddWithValue("@TENDN", tendn);

                        // Mã hóa mật khẩu
                        byte[] hashedPassword = null;
                        using (SHA1 sha1 = SHA1.Create())
                        {
                            byte[] passwordBytes = Encoding.UTF8.GetBytes(matkhau);
                            hashedPassword = sha1.ComputeHash(passwordBytes);
                        }

                        cmd.Parameters.AddWithValue("@MATKHAU", hashedPassword); 

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Thêm sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Xóa dữ liệu trong các ô nhập
                txtMasv.Clear();
                txtHoten.Clear();
                txtNgaysinh.Clear();
                txtDiachi.Clear();
                txtMalop.Clear();
                txtTendn.Clear();
                txtMatkhau.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
