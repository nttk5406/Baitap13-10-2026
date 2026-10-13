using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Bai4_8_AsyncProgress
{
    public class FormMain : Form
    {
        private readonly ToolStripStatusLabel userStatus = new ToolStripStatusLabel("Người dùng: Admin");
        private readonly ToolStripStatusLabel messageStatus = new ToolStripStatusLabel("Sẵn sàng");
        private readonly ToolStripStatusLabel timeStatus = new ToolStripStatusLabel();
        private readonly Timer timer = new Timer();

        public FormMain()
        {
            Text = "Bài 4.8 - Async/Await & ProgressBar";
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(900, 600);

            BuildMenu();
            BuildToolbar();
            BuildStatusStrip();

            timer.Interval = 1000;
            timer.Tick += (s, e) => timeStatus.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            timer.Start();
        }

        private void BuildMenu()
        {
            var menu = new MenuStrip();
            var system = new ToolStripMenuItem("Hệ thống");
            system.DropDownItems.Add("Thoát", null, (s, e) => Close());

            var catalog = new ToolStripMenuItem("Danh mục");
            catalog.DropDownItems.Add("Quản lý Sinh viên", null, (s, e) => OpenChild<FormStudent>());
            catalog.DropDownItems.Add("Quản lý Sản phẩm", null, (s, e) => OpenChild<FormProduct>());

            var window = new ToolStripMenuItem("Cửa sổ");
            window.DropDownItems.Add("Xếp tầng (Cascade)", null, (s, e) => LayoutMdi(MdiLayout.Cascade));
            window.DropDownItems.Add("Xếp ngang", null, (s, e) => LayoutMdi(MdiLayout.TileHorizontal));
            window.DropDownItems.Add("Xếp dọc", null, (s, e) => LayoutMdi(MdiLayout.TileVertical));

            menu.Items.AddRange(new ToolStripItem[] { system, catalog, window });
            MainMenuStrip = menu;
            Controls.Add(menu);
        }

        private void BuildToolbar()
        {
            var strip = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden };
            strip.Items.Add(new ToolStripButton("Sinh viên", null, (s, e) => OpenChild<FormStudent>()));
            strip.Items.Add(new ToolStripButton("Sản phẩm", null, (s, e) => OpenChild<FormProduct>()));
            Controls.Add(strip);
        }

        private void BuildStatusStrip()
        {
            var status = new StatusStrip();
            messageStatus.Spring = true;
            messageStatus.TextAlign = ContentAlignment.MiddleLeft;
            timeStatus.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            status.Items.Add(userStatus);
            status.Items.Add(new ToolStripStatusLabel("|"));
            status.Items.Add(messageStatus);
            status.Items.Add(timeStatus);
            Controls.Add(status);
        }

        public void SetStatus(string message) => messageStatus.Text = message;

        private void OpenChild<T>() where T : Form, new()
        {
            var child = MdiChildren.FirstOrDefault(f => f is T);
            if (child == null)
            {
                child = new T { MdiParent = this };
                child.FormClosed += (s, e) => SetStatus("Đã đóng cửa sổ: " + child.Text);
                child.Show();
            }
            else
            {
                if (child.WindowState == FormWindowState.Minimized) child.WindowState = FormWindowState.Normal;
                child.Activate();
            }
            SetStatus("Đang làm việc: " + child.Text);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            timer.Stop();
            base.OnFormClosed(e);
        }
    }
}
