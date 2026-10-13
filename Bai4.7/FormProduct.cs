using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Bai4_7_MDI
{
    public class FormProduct : Form
    {
        private readonly BindingList<Product> products = new BindingList<Product>();
        private readonly DataGridView grid = new DataGridView();
        private readonly TextBox txtCode = new TextBox();
        private readonly TextBox txtName = new TextBox();
        private readonly NumericUpDown numPrice = new NumericUpDown();

        public FormProduct()
        {
            Text = "Quản lý Sản phẩm";
            Size = new Size(850, 500);
            BuildUi();
        }

        private void BuildUi()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10) };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 95));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            var input = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 2 };
            input.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            input.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            input.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            input.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            input.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 65));
            input.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            input.Controls.Add(new Label { Text = "Mã SP:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            input.Controls.Add(txtCode, 1, 0);
            input.Controls.Add(new Label { Text = "Tên SP:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 2, 0);
            input.Controls.Add(txtName, 3, 0);
            input.Controls.Add(new Label { Text = "Giá:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 4, 0);
            numPrice.Maximum = 1000000000;
            numPrice.ThousandsSeparator = true;
            numPrice.Dock = DockStyle.Fill;
            input.Controls.Add(numPrice, 5, 0);

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill };
            var add = new Button { Text = "Thêm sản phẩm", AutoSize = true };
            add.Click += (s, e) => AddProduct();
            var refresh = new Button { Text = "Làm mới", AutoSize = true };
            refresh.Click += (s, e) => { grid.Refresh(); NotifyParent("Đã làm mới danh sách sản phẩm."); };
            var details = new Button { Text = "Xem chi tiết", AutoSize = true };
            details.Click += (s, e) => ShowDetails();
            buttons.Controls.AddRange(new Control[] { add, refresh, details });
            input.Controls.Add(buttons, 0, 1);
            input.SetColumnSpan(buttons, 6);

            grid.Dock = DockStyle.Fill;
            grid.DataSource = products;
            grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;

            var context = new ContextMenuStrip();
            context.Items.Add("Xóa dòng", null, (s, e) => DeleteSelected());
            context.Items.Add("Làm mới dữ liệu", null, (s, e) => { grid.Refresh(); NotifyParent("Đã làm mới danh sách sản phẩm."); });
            context.Items.Add("Xem chi tiết", null, (s, e) => ShowDetails());
            grid.ContextMenuStrip = context;

            layout.Controls.Add(input, 0, 0);
            layout.Controls.Add(grid, 0, 1);
            layout.Controls.Add(new Label { Text = "Nhấp chuột phải vào bảng để sử dụng menu ngữ cảnh.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
            Controls.Add(layout);
        }

        private void AddProduct()
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã sản phẩm và Tên sản phẩm.", "Thiếu dữ liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            foreach (var p in products)
            {
                if (string.Equals(p.MaSP, txtCode.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Mã sản phẩm đã tồn tại.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            products.Add(new Product { MaSP = txtCode.Text.Trim(), TenSP = txtName.Text.Trim(), Gia = numPrice.Value });
            txtCode.Clear(); txtName.Clear(); numPrice.Value = 0;
            NotifyParent("Thêm sản phẩm thành công.");
        }

        private void DeleteSelected()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.DataBoundItem == null) return;
            var product = grid.CurrentRow.DataBoundItem as Product;
            if (product == null) return;
            if (MessageBox.Show("Xóa sản phẩm " + product.TenSP + "?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            products.Remove(product);
            NotifyParent("Xóa sản phẩm thành công.");
        }

        private void ShowDetails()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var p = (Product)grid.CurrentRow.DataBoundItem;
            MessageBox.Show("Mã SP: " + p.MaSP + "\nTên SP: " + p.TenSP + "\nGiá: " + p.Gia.ToString("N0"),
                "Chi tiết sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void NotifyParent(string message)
        {
            if (MdiParent is FormMain main) main.SetStatus(message);
        }

        private class Product
        {
            public string MaSP { get; set; }
            public string TenSP { get; set; }
            public decimal Gia { get; set; }
        }
    }
}
