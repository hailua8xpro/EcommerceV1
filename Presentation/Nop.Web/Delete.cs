using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web
{
    public class State
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string SolrID { get; set; }
        public string STT { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public int TotalDoanhNghiep { get; set; }
    }
    public class DisTrictDTO
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int TinhThanhID { get; set; }
        public string TinhThanhTitle { get; set; }
        public string TinhThanhTitleAscii { get; set; }
        public string Type { get; set; }
        public string SolrID { get; set; }
        public string STT { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public int TotalDoanhNghiep { get; set; }
    }
    public class WardDTO
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int TinhThanhID { get; set; }
        public int QuanHuyenID { get; set; }
        public string QuanHuyenTitle { get; set; }
        public string QuanHuyenTitleAscii { get; set; }
        public string TinhThanhTitle { get; set; }
        public string TinhThanhTitleAscii { get; set; }
        public string Type { get; set; }
        public string SolrID { get; set; }
        public string STT { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public int TotalDoanhNghiep { get; set; }
    }
    public  class Fuck
    {
        public IEnumerable<State> LtsItem { get; set; }

    }
}
