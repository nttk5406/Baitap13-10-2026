using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bai4_8_AsyncProgress
{
    public class FormProduct : Form
    {
        private readonly BindingList<Product> products = new BindingList<Product>();
        private readonly DataGridView grid = new DataGridView();
        private readonly Button btnLoad = new Button();
        private readonly ProgressBar progressBar = new ProgressBar();
        private readonly Label lblProgress = new Label();

        public FormProduct()
        {
            Text = "Quản lý Sản phẩm - Async/Await";
            Size = new Size(850, 520);
            StartPosition = FormStartPosition.CenterScreen;
            BuildUi();
        }

        private void BuildUi()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 4 };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));

            var top = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            btnLoad.Text = "Nạp dữ liệu sản phẩm";
            btnLoad.AutoSize = true;
            btnLoad.Click += async (s, e) => await LoadProductsAsync();
            top.Controls.Add(btnLoad);

            progressBar.Dock = DockStyle.Fill;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            lblProgress.Dock = DockStyle.Fill;
            lblProgress.Text = "Tiến trình: 0%";
            lblProgress.TextAlign = ContentAlignment.MiddleLeft;

            var progressPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            progressPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 78));
            progressPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
            progressPanel.Controls.Add(progressBar, 0, 0);
            progressPanel.Controls.Add(lblProgress, 1, 0);

            grid.Dock = DockStyle.Fill;
            grid.AutoGenerateColumns = true;
            grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.DataSource = products;

            var context = new ContextMenuStrip();
            context.Items.Add("Xóa dòng", null, (s, e) => DeleteSelected());
            context.Items.Add("Làm mới dữ liệu", null, async (s, e) => await LoadProductsAsync());
            context.Items.Add("Xem chi tiết", null, (s, e) => ShowDetails());
            grid.ContextMenuStrip = context;

            layout.Controls.Add(top, 0, 0);
            layout.Controls.Add(progressPanel, 0, 1);
            layout.Controls.Add(grid, 0, 2);
            layout.Controls.Add(new Label { Text = "Dữ liệu sản phẩm mẫu được nạp bất đồng bộ.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
            Controls.Add(layout);
        }

        private async Task<List<Product>> FetchProductsAsync(IProgress<int> progress)
        {
            return await Task.Run(async () =>
            {
                var result = new List<Product>();
                const int total = 100;
                for (int i = 1; i <= total; i++)
                {
                    await Task.Delay(35);
                    result.Add(new Product
                    {
                        MaSP = "SP" + i.ToString("000"),
                        TenSP = "Sản phẩm " + i,
                        Gia = i * 12500m
                    });
                    progress.Report(i);
                }
                return result;
            });
        }

        private async Task LoadProductsAsync()
        {
            if (!btnLoad.Enabled) return;
            btnLoad.Enabled = false;
            progressBar.Value = 0;
            lblProgress.Text = "Tiến trình: 0%";
            NotifyParent("Đang nạp dữ liệu sản phẩm: 0%");

            var progress = new Progress<int>(value =>
            {
                int percent = Math.Max(0, Math.Min(100, value));
                progressBar.Value = percent;
                lblProgress.Text = "Tiến trình: " + percent + "%";
                NotifyParent("Đang nạp dữ liệu sản phẩm: " + percent + "%");
            });

            try
            {
                var data = await FetchProductsAsync(progress);
                products.Clear();
                foreach (var item in data) products.Add(item);
                progressBar.Value = 100;
                lblProgress.Text = "Tiến trình: 100%";
                NotifyParent("Nạp dữ liệu sản phẩm hoàn tất (" + data.Count + " bản ghi).");
            }
            catch (Exception ex)
            {
                NotifyParent("Lỗi nạp dữ liệu sản phẩm.");
                MessageBox.Show("Không thể nạp dữ liệu.\nChi tiết: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLoad.Enabled = true;
            }
        }

        private void DeleteSelected()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.DataBoundItem == null) return;
            var item = grid.CurrentRow.DataBoundItem as Product;
            if (item == null) return;
            products.Remove(item);
            NotifyParent("Đã xóa " + item.MaSP + ".");
        }

        private void ShowDetails()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm.", "Thông báo");
                return;
            }
            var item = (Product)grid.CurrentRow.DataBoundItem;
            MessageBox.Show("Mã SP: " + item.MaSP + "\nTên SP: " + item.TenSP + "\nGiá: " + item.Gia.ToString("N0"),
                "Chi tiết sản phẩm");
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
