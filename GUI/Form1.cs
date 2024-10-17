using BLL;
using DAL.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class Form1 : Form
    {
        public readonly StudentService studentService = new StudentService();
        public readonly FacultyService facultyService = new FacultyService();
        public Form1()
        { 
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dataGridView1);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFalcultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            listFacultys.Insert(0, new Faculty());
            this.cmbKhoa.DataSource = listFacultys;
            this.cmbKhoa.DisplayMember = "FacultyName";
            this.cmbKhoa.ValueMember = "FacultyID";
        }
        private void BindGrid(List<Student> listStudent)
        {
            dataGridView1.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = item.StudentID;
                dataGridView1.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)
                    dataGridView1.Rows[index].Cells[2].Value =
                    item.Faculty.FacultyName;
                dataGridView1.Rows[index].Cells[3].Value = item.AverageScore + "";
                if (item.MajorID != null)
                    dataGridView1.Rows[index].Cells[4].Value = item.Major.Name + "";
                ShowAvatar(item.Avatar);
            }
        }
        private void ShowAvatar(string ImageName)
        {
            if (string.IsNullOrEmpty(ImageName))
            {
                pictureBox1.Image = null;
            }
            else
            {
                string parentDirectory =
                Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagePath = Path.Combine(parentDirectory, "Images",
                ImageName);
                pictureBox1.Image = Image.FromFile(imagePath);
                pictureBox1.Refresh();
            }
        }
        public void setGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle =
            DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void chkUnregisterMajor_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.chkUnregisterMajor.Checked)
                listStudents = studentService.GetAllHasNoMajor();
            else
                listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo một đối tượng sinh viên mới
                var student = new Student
                {
                    StudentID = txtMa.Text,
                    FullName = txtTen.Text,
                    FacultyID = (int)cmbKhoa.SelectedValue,
                    AverageScore = double.Parse(txtDiem.Text),
                    MajorID = null // Nếu có ngành học, bạn có thể lấy giá trị từ combobox khác nếu có
                };

                // Gọi service để thêm sinh viên
                studentService.InsertUpdate(student);

                // Cập nhật lại danh sách sinh viên trên DataGridView
                BindGrid(studentService.GetAll());

                // Thông báo thành công
                MessageBox.Show("Thêm sinh viên thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnUpDate_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu có dòng được chọn trong DataGridView
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Lấy sinh viên hiện tại từ DataGridView
                    var selectedRow = dataGridView1.SelectedRows[0];
                    var studentID = selectedRow.Cells[0].Value.ToString();

                    // Tìm sinh viên theo StudentID
                    var student = studentService.FindById(studentID);

                    if (student != null)
                    {
                        // Cập nhật các thông tin sinh viên từ giao diện
                        student.FullName = txtTen.Text;             // Cập nhật tên
                        student.AverageScore = double.Parse(txtDiem.Text);  // Cập nhật điểm trung bình
                        student.FacultyID = (int)cmbKhoa.SelectedValue;  // Cập nhật khoa từ ComboBox
                        student.MajorID = null; // Bạn có thể lấy MajorID từ một ComboBox khác nếu có

                        // Gọi phương thức InsertUpdate() để cập nhật thông tin sinh viên
                        studentService.InsertUpdate(student);

                        // Cập nhật lại danh sách sinh viên trên DataGridView
                        BindGrid(studentService.GetAll());

                        // Thông báo thành công
                        MessageBox.Show("Cập nhật sinh viên thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên để sửa.");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để sửa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    var selectedRow = dataGridView1.SelectedRows[0];
                    var studentID = selectedRow.Cells[0].Value.ToString();

                    // Tìm sinh viên theo StudentID
                    var student = studentService.FindById(studentID);

                    if (student != null)
                    {
                        var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes)
                        {
                            // Gọi phương thức Remove từ đối tượng studentService
                            studentService.Remove(student); // Đảm bảo gọi từ đối tượng đã khởi tạo
                            BindGrid(studentService.GetAll());
                            MessageBox.Show("Xóa sinh viên thành công!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên để xóa.");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để xóa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnLayAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                pictureBox1.Tag = openFileDialog.FileName; // Lưu đường dẫn ảnh trong Tag
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0) 
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtMa.Text = row.Cells[0].Value.ToString();
                txtTen.Text = row.Cells[1].Value.ToString();
                cmbKhoa.Text = row.Cells[2].Value.ToString();
                txtDiem.Text = row.Cells[3].Value.ToString();
                
            }
        }
    }
}
