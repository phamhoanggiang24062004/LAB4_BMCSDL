using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;

namespace Lab_3___BMCSDL
{
    public partial class Form1 : Form
    {
        private RoundedPanel panelLogin;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;

        public Form1()
        {
            InitializeComponent();
            InitializeLoginForm();

            // Đăng ký sự kiện Resize cho Form
            this.Resize += MainForm_Resize;
        }

        private void InitializeLoginForm()
        {
            //--- Thiết lập Form ---
            this.Text = "Đăng nhập";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(800, 600);
            // Background sẽ thêm sau, ví dụ:
            this.BackgroundImage = Image.FromFile("C:\\Users\\PC\\Downloads\\microsoft-windows-3840x2160-12507.jpg");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            //--- Tạo Panel bo góc chứa phần login ---
            panelLogin = new RoundedPanel
            {
                CornerRadius = 20,
                BorderColor = Color.LightBlue,
                BorderThickness = 2,
                BackColor = Color.FromArgb(240, 248, 255), // màu nền nhạt
                Size = new Size(400, 230),
            };

            // Canh giữa panelLogin
            panelLogin.Location = new Point(
                (this.ClientSize.Width - panelLogin.Width) / 2,
                (this.ClientSize.Height - panelLogin.Height) / 2
            );

            this.Controls.Add(panelLogin);

            //--- Label và TextBox Tên đăng nhập ---
            Label lblUser = new Label
            {
                Text = "Tên đăng nhập:",
                AutoSize = true,
                Location = new Point(20, 20)
            };
            panelLogin.Controls.Add(lblUser);

            txtUsername = new TextBox
            {
                Size = new Size(360, 25),
                Location = new Point(20, 45)
                // Nếu bạn dùng .NET 6+, có thể thêm: PlaceholderText = "Nhập tên đăng nhập"
            };
            panelLogin.Controls.Add(txtUsername);

            //--- Label và TextBox Mật khẩu ---
            Label lblPass = new Label
            {
                Text = "Mật khẩu:",
                AutoSize = true,
                Location = new Point(20, 80)
            };
            panelLogin.Controls.Add(lblPass);

            txtPassword = new TextBox
            {
                Size = new Size(360, 25),
                Location = new Point(20, 105),
                UseSystemPasswordChar = true
                // Nếu .NET cũ không hỗ trợ PlaceholderText, bạn có thể tự custom watermark
            };
            panelLogin.Controls.Add(txtPassword);

            //--- Nút Đăng nhập ---
            btnLogin = new Button
            {
                Text = "Đăng nhập",
                Size = new Size(100, 30),
                Location = new Point((panelLogin.Width - 100) / 2, 160)
            };
            btnLogin.Click += BtnLogin_Click;
            panelLogin.Controls.Add(btnLogin);
        }

        // Xác thực đăng nhập 
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text;

            // Mã hóa mật khẩu người dùng nhập bằng SHA1
            byte[] hashedPassword = HashPasswordWithSHA1(pass);

            // Chuỗi kết nối đến cơ sở dữ liệu
            string connectionString = @"Server=LAPTOP-RBM16H2U\MSSQLSER2022;Database=QLSVNhom;Trusted_Connection=True;";

            // Truy vấn kiểm tra thông tin đăng nhập
            string query = "SELECT MANV FROM NHANVIEN WHERE TENDN = @Username AND MATKHAU = @Password";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Thêm tham số để tránh SQL Injection
                        command.Parameters.AddWithValue("@Username", user);
                        command.Parameters.AddWithValue("@Password", hashedPassword);

                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            // Đăng nhập thành công bằng tài khoản người dùng
                            string manv = result.ToString();
                            Dashboard dashboard = new Dashboard(manv);
                            this.Hide(); // Ẩn Form1
                            dashboard.ShowDialog();
                            this.Close(); // Đóng Form1 sau khi Dashboard đóng
                        }
                        else if(user == "admin" && pass == "123")
                        {
                            // Đăng nhập thành công bằng Admin 
                            Dashboard dashboard = new Dashboard("NV01");
                            this.Hide(); // Ẩn Form1
                            dashboard.ShowDialog();
                            this.Close(); // Đóng Form1 sau khi Dashboard đóng
                        }
                        else
                        {
                            // Sai tên đăng nhập hoặc mật khẩu
                            MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        // Canh giữa cho Round Panel - nơi chứa chỗ đăng nhập 
        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Tính tọa độ mới để panelLogin luôn ở giữa Form
            int newX = (this.ClientSize.Width - panelLogin.Width) / 2;
            int newY = (this.ClientSize.Height - panelLogin.Height) / 2;

            panelLogin.Location = new Point(newX, newY);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private byte[] HashPasswordWithSHA1(string password)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                return sha1.ComputeHash(passwordBytes);
            }
        }
    }
}
