using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_3___BMCSDL
{
    public partial class UcDashboard : UserControl
    {
        public UcDashboard()
        {
            InitializeComponent();
            InitializeContent();
        }

        private void InitializeContent()
        {
            // Tạo panel trung tâm chứa thông tin thành viên (dùng FlowLayoutPanel để sắp xếp dọc)
            FlowLayoutPanel centerPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Padding = new Padding(10),
                Anchor = AnchorStyles.Top, // Căn trên (và giữa ngang)
                Margin = new Padding(0),
                Dock = DockStyle.None
            };

            // Tiêu đề
            Label title = new Label
            {
                Text = "👨‍💻 GIỚI THIỆU NHÓM",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.Black,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 20)
            };
            centerPanel.Controls.Add(title);

            // Danh sách thành viên
            string[] thanhVien = {
        "MSSV 22120074 - Đỗ Nhật Duy",
        "MSSV 221200** - Đỗ Văn Hải",
        "MSSV 22120084 - Phạm Hoàng Giang"
    };

            foreach (var tv in thanhVien)
            {
                Label lbl = new Label
                {
                    Text = tv,
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point),
                    ForeColor = Color.Black,
                    AutoSize = true,
                    Margin = new Padding(0, 8, 0, 8),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                centerPanel.Controls.Add(lbl);
            }

            // TableLayoutPanel với 3 hàng: [trên - auto], [giữa - auto], [dưới - fill]
            TableLayoutPanel wrapper = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3,
                BackColor = Color.White
            };

            // Cột giữa chiếm 100%, 2 cột ngoài giữ để căn giữa ngang
            wrapper.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            wrapper.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            wrapper.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Hàng trên cùng chiếm ít, hàng giữa chứa nội dung, hàng dưới chiếm phần còn lại
            wrapper.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // Khoảng cách từ trên xuống
            wrapper.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            wrapper.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // phần còn lại

            // Thêm panel vào giữa (hàng 1, cột 1)
            wrapper.Controls.Add(centerPanel, 1, 1);

            // Thêm wrapper vào control
            this.Controls.Add(wrapper);
        }
    }
}
