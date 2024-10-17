using DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class MajorService
    {
        public List<Major> GetAllByFaculty(int facultyId)
        {
            ModelSinhVienDB modelSinhVienDB = new ModelSinhVienDB();
            return modelSinhVienDB.Majors.Where(p => p.FacultyID == facultyId).ToList();
        }
    }
}
