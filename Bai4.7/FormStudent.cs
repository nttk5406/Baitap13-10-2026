using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Bai4_7_MDI
{
    public class FormStudent : Form
    {
        private readonly DataGridView grid = new DataGridView();
        private readonly BindingList<Student> students = new BindingList<Student>();
        private readonly TextBox txtCode = new TextBox();
        private readonly TextBox txtName = new TextBox();
        private readonly TextBox txtClass = new TextBox();
        private readonly FormMain parent;

        public FormStudent()
        {
            Text = "Quản lý Sinh viên";
            Size = new Size(850, 500);
            MinimumSize = new Size(700, 400);
            parent = null;
            BuildUi();
        }

        private void BuildUi()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 95));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

            var input = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 2 };
            input.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            input.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            input.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            input.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            input.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            input.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            input.Controls.Add(new Label { Text = "Mã SV:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
            input.Controls.Add(txtCode, 1, 0);
            input.Controls.Add(new Label { Text = "Họ tên:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 2, 0);
            input.Controls.Add(txtName, 3, 0);
            input.Controls.Add(new Label { Text = "Lớp:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 4, 0);
            input.Controls.Add(txtClass, 5, 0);

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
            var add = new Button { Text = "Thêm sinh viên", AutoSize = true };
            add.Click += (s, e) => AddStudent();
            var refresh = new Button { Text = "Làm mới", AutoSize = true };
            refresh.Click += (s, e) => { grid.Refresh(); NotifyParent("Đã làm mới danh sách sinh viên."); };
            var details = new Button { Text = "Xem chi tiết", AutoSize = true };
            details.Click += (s, e) => ShowDetails();
            buttons.Controls.AddRange(new Control[] { add, refresh, details });
            input.Controls.Add(buttons, 0, 1);
            input.SetColumnSpan(buttons, 6);

            grid.Dock = DockStyle.Fill;
            grid.AutoGenerateColumns = true;
            grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.DataSource = students;

            var context = new ContextMenuStrip();
            context.Items.Add("Xóa dòng", null, (s, e) => DeleteSelected());
            context.Items.Add("Làm mới dữ liệu", null, (s, e) => { grid.Refresh(); NotifyParent("Đã làm mới danh sách sinh viên."); });
            context.Items.Add("Xem chi tiết", null, (s, e) => ShowDetails());
            grid.ContextMenuStrip = context;

            layout.Controls.Add(input, 0, 0);
            layout.Controls.Add(grid, 0, 1);
            layout.Controls.Add(new Label { Text = "Mẹo: Nhấp chuột phải vào bảng để mở menu ngữ cảnh.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
            Controls.Add(layout);
        }

        private void AddStudent()
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã sinh viên và Họ tên.", "Thiếu dữ liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var student in students)
            {
                if (string.Equals(student.MaSV, txtCode.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            students.Add(new Student { MaSV = txtCode.Text.Trim(), HoTen = txtName.Text.Trim(), Lop = txtClass.Text.Trim() });
            txtCode.Clear(); txtName.Clear(); txtClass.Clear();
            NotifyParent("Thêm sinh viên thành công.");
        }

        private void DeleteSelected()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.DataBoundItem == null) return;
            var student = grid.CurrentRow.DataBoundItem as Student;
            if (student == null) return;
            if (MessageBox.Show("Xóa sinh viên " + student.HoTen + "?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            students.Remove(student);
            NotifyParent("Xóa sinh viên thành công.");
        }

        private void ShowDetails()
        {
            if (grid.CurrentRow == null || grid.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Vui lòng chọn một sinh viên.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var st = (Student)grid.CurrentRow.DataBoundItem;
            MessageBox.Show("Mã SV: " + st.MaSV + "\nHọ tên: " + st.HoTen + "\nLớp: " + st.Lop,
                "Chi tiết sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void NotifyParent(string message)
        {
            if (MdiParent is FormMain main) main.SetStatus(message);
        }

        private class Student
        {
            public string MaSV { get; set; }
            public string HoTen { get; set; }
            public string Lop { get; set; }
        }
    }
}
