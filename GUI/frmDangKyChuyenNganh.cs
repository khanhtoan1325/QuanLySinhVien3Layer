using BLL;
using DAL.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmDangKyChuyenNganh : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();

        public frmDangKyChuyenNganh()
        {
            InitializeComponent();
        }

        private void frmDangKyChuyenNganh_Load(object sender, EventArgs e)
        {
            try
            {
                
                var listFacultys = facultyService.GetAll();
                FillfacultyCombobox(listFacultys);

            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillfacultyCombobox(List<Faculty> listFacultys)
        {
            this.cmbKhoa.DataSource = listFacultys;
            this.cmbKhoa.DisplayMember = "FacultyName";
            this.cmbKhoa.ValueMember = "FacultyID";
        }
        private void FillMajor(List<Major> listMajors)
        {
            this.cmbChuyenNganh.DataSource = listMajors;
            this.cmbChuyenNganh.DisplayMember = "MajorName";
            this.cmbChuyenNganh.ValueMember = "MajorID";
        }
        private void cmbKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty SelectedFaculty = cmbKhoa.SelectedItem as Faculty;
            if ( SelectedFaculty != null)
            {
                var lisMajpor = majorService.GetAllByFaculty(SelectedFaculty.FacultyID);
                FillMajor(lisMajpor);
                var listStudents = studentService.GetAllHasNoMajor(SelectedFaculty.FacultyID);
                BindGrid(listStudents);


            }
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[1].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[2].Value = item.FullName;
                if (item.Faculty != null)
                    dgvStudent.Rows[index].Cells[3].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[4].Value = item.AverageScore.ToString();
                dgvStudent.Rows[index].Cells[5].Value = item.Major?.Name ?? "Chưa có chuyên ngành";
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvStudent.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để đăng ký chuyên ngành.");
                    return;
                }

                // Lấy sinh viên được chọn
                var selectedRow = dgvStudent.SelectedRows[0];
                int studentID = Convert.ToInt32(selectedRow.Cells[1].Value);

                // Lấy chuyên ngành được chọn
                Major selectedMajor = cmbChuyenNganh.SelectedItem as Major;
                if (selectedMajor == null)
                {
                    MessageBox.Show("Vui lòng chọn chuyên ngành.");
                    return;
                }

                // Gọi service để cập nhật chuyên ngành cho sinh viên
                bool result = studentService.RegisterMajor(studentID, selectedMajor.MajorID);

                if (result)
                {
                    MessageBox.Show("Đăng ký chuyên ngành thành công!");
                    // Cập nhật lại danh sách sinh viên
                    Faculty selectedFaculty = cmbKhoa.SelectedItem as Faculty;
                    var listStudents = studentService.GetAllHasNoMajor(selectedFaculty.FacultyID);
                    BindGrid(listStudents);
                }
                else
                {
                    MessageBox.Show("Đăng ký chuyên ngành không thành công.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        
    }
}
