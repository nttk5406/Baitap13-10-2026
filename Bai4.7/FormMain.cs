using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Bai4_7_MDI
{
    public class FormMain : Form
    {
        private readonly ToolStripStatusLabel userStatus = new ToolStripStatusLabel();
        private readonly ToolStripStatusLabel messageStatus = new ToolStripStatusLabel();
        private readonly ToolStripStatusLabel timeStatus = new ToolStripStatusLabel();
        private readonly Timer clockTimer = new Timer();

        public FormMain()
        {
            Text = "Bài 4.7 - Quản lý đa cửa sổ MDI";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            IsMdiContainer = true;
            MinimumSize = new Size(900, 600);

            BuildMenu();
            BuildToolStrip();
            BuildStatusStrip();

            clockTimer.Interval = 1000;
            clockTimer.Tick += (s, e) => timeStatus.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            clockTimer.Start();

            Shown += (s, e) => SetStatus("Hệ thống sẵn sàng.");
        }

        private void BuildMenu()
        {
            var menu = new MenuStrip();

            var systemMenu = new ToolStripMenuItem("Hệ thống");
            systemMenu.DropDownItems.Add("Đăng xuất", null, (s, e) =>
            {
                var result = MessageBox.Show("Bạn có muốn đăng xuất khỏi phiên làm việc hiện tại?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Close();
                }
            });
            systemMenu.DropDownItems.Add(new ToolStripSeparator());
            systemMenu.DropDownItems.Add("Thoát", null, (s, e) => Close());

            var categoryMenu = new ToolStripMenuItem("Danh mục");
            categoryMenu.DropDownItems.Add("Quản lý Sinh viên", null, (s, e) => OpenChild<FormStudent>());
            categoryMenu.DropDownItems.Add("Quản lý Sản phẩm", null, (s, e) => OpenChild<FormProduct>());

            var windowMenu = new ToolStripMenuItem("Cửa sổ");
            windowMenu.DropDownItems.Add("Xếp tầng (Cascade)", null, (s, e) => LayoutMdi(MdiLayout.Cascade));
            windowMenu.DropDownItems.Add("Xếp ngang (Tile Horizontal)", null, (s, e) => LayoutMdi(MdiLayout.TileHorizontal));
            windowMenu.DropDownItems.Add("Xếp dọc (Tile Vertical)", null, (s, e) => LayoutMdi(MdiLayout.TileVertical));
            windowMenu.DropDownItems.Add(new ToolStripSeparator());
            windowMenu.DropDownItems.Add("Đóng cửa sổ con hiện tại", null, (s, e) => ActiveMdiChild?.Close());

            menu.Items.AddRange(new ToolStripItem[] { systemMenu, categoryMenu, windowMenu });
            MainMenuStrip = menu;
            Controls.Add(menu);
        }

        private void BuildToolStrip()
        {
            var tools = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden };
            tools.Items.Add(new ToolStripButton("Sinh viên", null, (s, e) => OpenChild<FormStudent>())
            {
                ToolTipText = "Mở quản lý sinh viên"
            });
            tools.Items.Add(new ToolStripButton("Sản phẩm", null, (s, e) => OpenChild<FormProduct>())
            {
                ToolTipText = "Mở quản lý sản phẩm"
            });
            tools.Items.Add(new ToolStripSeparator());
            tools.Items.Add(new ToolStripButton("Đóng cửa sổ", null, (s, e) => ActiveMdiChild?.Close()));
            Controls.Add(tools);
        }

        private void BuildStatusStrip()
        {
            var status = new StatusStrip();
            userStatus.Text = "Người dùng: Admin";
            messageStatus.Text = "Sẵn sàng";
            messageStatus.Spring = true;
            messageStatus.TextAlign = ContentAlignment.MiddleLeft;
            timeStatus.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            status.Items.Add(userStatus);
            status.Items.Add(new ToolStripStatusLabel("|"));
            status.Items.Add(messageStatus);
            status.Items.Add(timeStatus);
            Controls.Add(status);
        }

        public void SetStatus(string message)
        {
            messageStatus.Text = message;
        }

        private void OpenChild<T>() where T : Form, new()
        {
            // Không mở trùng một loại Form; nếu đã có thì kích hoạt cửa sổ đó.
            var child = MdiChildren.FirstOrDefault(f => f is T);
            if (child == null)
            {
                child = new T
                {
                    MdiParent = this,
                    StartPosition = FormStartPosition.CenterScreen
                };
                child.FormClosed += (s, e) => SetStatus("Đã đóng cửa sổ: " + child.Text);
                child.Show();
            }
            else
            {
                if (child.WindowState == FormWindowState.Minimized)
                    child.WindowState = FormWindowState.Normal;
                child.Activate();
            }

            SetStatus("Đang làm việc: " + child.Text);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            clockTimer.Stop();
            base.OnFormClosed(e);
        }
    }
}
