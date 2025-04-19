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
                Anchor = AnchorStyles.None
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

            // TableLayoutPanel để căn giữa trong toàn bộ contentPanel
            TableLayoutPanel wrapper = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                BackColor = Color.White
            };
            wrapper.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            wrapper.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            wrapper.Controls.Add(centerPanel, 0, 0);

            // Căn giữa
            wrapper.Controls[0].Anchor = AnchorStyles.None;

            this.Controls.Add(wrapper);
        }
    }
}
