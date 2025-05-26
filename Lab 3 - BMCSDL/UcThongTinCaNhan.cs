using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Windows.Forms;

namespace Lab_3___BMCSDL
{
    public partial class UcThongTinCaNhan : UserControl
    {
        string connectionString = @"Server=DESKTOP-P0BQAJD;Database=QLSVNhom;Trusted_Connection=True;";
        private readonly string _manv; 
        private readonly string _pass;
        private readonly string _tendn;
        private byte[] _encryptedLuong;
        private readonly EmployeeKeyGenerator _keyGenerator;

        private Label lblManv, lblHoten, lblEmail, lblLuong;
        private Label valManv, valHoten, valEmail, valLuong;
        private Button btnGiaiMaLuong;

        public UcThongTinCaNhan(string manv, string pass, string tendn)
        {
            InitializeComponent();
            _manv = manv;
            _pass = pass;
            _tendn = tendn;
            _keyGenerator = new EmployeeKeyGenerator();
            InitializeThongTinNhanVienUI();
            LoadNhanVienData(); // Gọi hàm load dữ liệu ngay sau khi khởi tạo giao diện
           
        }

        private void InitializeThongTinNhanVienUI()
        {
            this.BackColor = Color.White;
            this.Size = new Size(600, 300);

            int labelWidth = 100;
            int valueWidth = 250;
            int height = 30;
            int spacing = 30;
            int marginTop = 90;
            int marginLeft = 30;

            Font labelFont = new Font("Segoe UI", 10, FontStyle.Bold);
            Font valueFont = new Font("Segoe UI", 10, FontStyle.Regular);
            Font titleFont = new Font("Segoe UI", 14, FontStyle.Bold);

            // Tiêu đề "THÔNG TIN CÁ NHÂN"
            Label lblTitle = new Label
            {
                Text = "THÔNG TIN CÁ NHÂN",
                Location = new Point(0, 10),
                Size = new Size(this.Width, 40),
                Font = titleFont,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Mã NV
            lblManv = new Label
            {
                Text = "Mã NV:",
                Location = new Point(marginLeft, marginTop),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            valManv = new Label
            {
                Text = "", // Không gán giá trị cố định
                Location = new Point(marginLeft + labelWidth, marginTop),
                Size = new Size(valueWidth, height),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Font = valueFont,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Họ tên
            lblHoten = new Label
            {
                Text = "Họ tên:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 1),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            valHoten = new Label
            {
                Text = "", // Không gán giá trị cố định
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 1),
                Size = new Size(valueWidth, height),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Font = valueFont,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Email
            lblEmail = new Label
            {
                Text = "Email:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 2),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            valEmail = new Label
            {
                Text = "", // Không gán giá trị cố định
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 2),
                Size = new Size(valueWidth, height),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Font = valueFont,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Lương
            lblLuong = new Label
            {
                Text = "Lương:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 3),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            valLuong = new Label
            {
                Text = "",
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 3),
                Size = new Size(valueWidth + 600, height + 120),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Font = valueFont,
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnGiaiMaLuong = new Button
            {
                Text = "Giải mã lương",
                Location = new Point(marginLeft + labelWidth + 50, marginTop + (height + spacing) * 6 + 10),
                Size = new Size(100, 30),
                
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                
            };
            btnGiaiMaLuong.Click += BtnGiaiMaLuong_Click;

            

            // Thêm vào Control
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblManv);
            this.Controls.Add(valManv);
            this.Controls.Add(lblHoten);
            this.Controls.Add(valHoten);
            this.Controls.Add(lblEmail);
            this.Controls.Add(valEmail);
            this.Controls.Add(lblLuong);
            this.Controls.Add(valLuong);
            this.Controls.Add(btnGiaiMaLuong);
        }

        private void BtnGiaiMaLuong_Click(object sender, EventArgs e)
        {
            if (_encryptedLuong != null)
            {
                GiaiMaLuong(_encryptedLuong);
            }
            else
            {
                MessageBox.Show("Không có dữ liệu lương để giải mã.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void GiaiMaLuong(byte[] encryptedData)
        {
            try
            {
                using (RSA rsa = _keyGenerator.LoadPrivateKey(_manv, _pass))
                {
                    byte[] decryptedData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
                    int salary = BitConverter.ToInt32(decryptedData, 0);
                    valLuong.Text = salary.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Giải mã lương thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadNhanVienData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Gọi Stored Procedure SP_SEL_PUBLIC_NHANVIEN
                    using (SqlCommand cmd = new SqlCommand("SP_SEL_PUBLIC_NHANVIEN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Thêm tham số cho stored procedure
                        cmd.Parameters.AddWithValue("@TENDN", _tendn);
                        cmd.Parameters.AddWithValue("@MK", _pass);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Cập nhật dữ liệu vào các Label
                                valManv.Text = reader["MANV"].ToString();
                                valHoten.Text = reader["HOTEN"].ToString();
                                valEmail.Text = reader["EMAIL"].ToString();

                                // Giải mã lương bằng private key
                                if (reader["LUONG"] != DBNull.Value)
                                {
                                    _encryptedLuong = (byte[])reader["LUONG"];
                                    valLuong.Text = "0x" + BitConverter.ToString(_encryptedLuong).Replace("-", "");
                                }
                                else
                                {
                                    _encryptedLuong = null;
                                    valLuong.Text = "NULL";
                                }
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy thông tin nhân viên.", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}