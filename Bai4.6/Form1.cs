using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Bai4_6_QuanLySinhVien
{
public partial class Form1 : Form
{
    private BindingList<Student> students = new BindingList<Student>();
    private BindingSource studentSource = new BindingSource();

    public Form1()
    {
        InitializeComponent();
        studentSource.DataSource = students;
        dgvStudents.AutoGenerateColumns = true;
        dgvStudents.DataSource = studentSource;

        cboGender.Items.Add("Nam");
        cboGender.Items.Add("Nữ");
        cboGender.SelectedIndex = 0;

        txtStudentId.DataBindings.Add("Text", studentSource, "StudentId", true, DataSourceUpdateMode.OnPropertyChanged);
        txtFullName.DataBindings.Add("Text", studentSource, "FullName", true, DataSourceUpdateMode.OnPropertyChanged);
        txtAge.DataBindings.Add("Text", studentSource, "Age", true, DataSourceUpdateMode.OnPropertyChanged);
        cboGender.DataBindings.Add("Text", studentSource, "Gender", true, DataSourceUpdateMode.OnPropertyChanged);

        studentSource.CurrentChanged += StudentSource_CurrentChanged;
        ShowAvatar();
    }

    private void StudentSource_CurrentChanged(object sender, EventArgs e)
    {
        ShowAvatar();
    }

    private Student CurrentStudent()
    {
        return studentSource.Current as Student;
    }

    private void ShowAvatar()
    {
        Student student = CurrentStudent();

        if (student == null || string.IsNullOrWhiteSpace(student.AvatarPath) || !File.Exists(student.AvatarPath))
        {
            picAvatar.Image = null;
            return;
        }

        try
        {
            using (Image image = Image.FromFile(student.AvatarPath))
            {
                picAvatar.Image = new Bitmap(image);
            }
        }
        catch
        {
            picAvatar.Image = null;
        }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        int age;
        if (string.IsNullOrWhiteSpace(txtStudentId.Text) ||
            string.IsNullOrWhiteSpace(txtFullName.Text) ||
            !int.TryParse(txtAge.Text, out age) || age < 1 || age > 120)
        {
            MessageBox.Show("Hãy nhập mã sinh viên, họ tên và tuổi hợp lệ.");
            return;
        }

        foreach (Student s in students)
        {
            if (s.StudentId == txtStudentId.Text.Trim())
            {
                MessageBox.Show("Mã sinh viên đã tồn tại.");
                return;
            }
        }

        Student student = new Student();
        student.StudentId = txtStudentId.Text.Trim();
        student.FullName = txtFullName.Text.Trim();
        student.Age = age;
        student.Gender = cboGender.Text;
        student.AvatarPath = "";

        students.Add(student);
        studentSource.Position = students.Count - 1;
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        Student student = CurrentStudent();
        if (student == null)
        {
            MessageBox.Show("Hãy chọn sinh viên cần xóa.");
            return;
        }

        if (MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            students.Remove(student);
            ShowAvatar();
        }
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        Student student = CurrentStudent();
        int age;

        if (student == null)
        {
            MessageBox.Show("Hãy chọn sinh viên cần cập nhật.");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtStudentId.Text) ||
            string.IsNullOrWhiteSpace(txtFullName.Text) ||
            !int.TryParse(txtAge.Text, out age) || age < 1 || age > 120)
        {
            MessageBox.Show("Hãy nhập mã sinh viên, họ tên và tuổi hợp lệ.");
            return;
        }

        foreach (Student s in students)
        {
            if (s != student && s.StudentId == txtStudentId.Text.Trim())
            {
                MessageBox.Show("Mã sinh viên đã được sử dụng.");
                return;
            }
        }

        student.StudentId = txtStudentId.Text.Trim();
        student.FullName = txtFullName.Text.Trim();
        student.Age = age;
        student.Gender = cboGender.Text;

        studentSource.ResetCurrentItem();
        dgvStudents.Refresh();
    }

    private void btnChooseAvatar_Click(object sender, EventArgs e)
    {
        Student student = CurrentStudent();
        if (student == null)
        {
            MessageBox.Show("Hãy thêm hoặc chọn một sinh viên trước.");
            return;
        }

        using (OpenFileDialog dialog = new OpenFileDialog())
        {
            dialog.Title = "Chọn ảnh đại diện";
            dialog.Filter = "Tệp ảnh (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                student.AvatarPath = dialog.FileName;
                ShowAvatar();
                studentSource.ResetCurrentItem();
            }
        }
    }

    private void btnExportCsv_Click(object sender, EventArgs e)
    {
        using (SaveFileDialog dialog = new SaveFileDialog())
        {
            dialog.Title = "Xuất danh sách sinh viên";
            dialog.Filter = "CSV (*.csv)|*.csv";
            dialog.FileName = "DanhSachSinhVien.csv";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(dialog.FileName, false, new UTF8Encoding(true)))
                    {
                        writer.WriteLine("MaSV,HoTen,Tuoi,GioiTinh,AvatarPath");

                        foreach (Student s in students)
                        {
                            writer.WriteLine(
                                CsvEscape(s.StudentId) + "," +
                                CsvEscape(s.FullName) + "," +
                                s.Age + "," +
                                CsvEscape(s.Gender) + "," +
                                CsvEscape(s.AvatarPath));
                        }
                    }

                    MessageBox.Show("Xuất CSV thành công.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xuất file: " + ex.Message);
                }
            }
        }
    }

    private string CsvEscape(string value)
    {
        if (value == null)
            value = "";

        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }

}
}
