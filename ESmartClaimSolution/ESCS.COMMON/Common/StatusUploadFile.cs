using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Common
{
    public class StatusUploadFileConstant
    {
        public const string SUCCESS = "SUCCESS";
        public const string ERROR = "ERROR";
    }
    public class StatusUploadFile
    {
        public string base_url { get; set; }
        public string path_file { get; set; }
        public string file_name { get; set; }
        public string file_name_new { get; set; }
        public string extension_file { get; set; }
        public string status_upload { get; set; }
        public string error_message { get; set; }
        public string thumnail_base64 { get; set; }
        //public IFormFile file { get; set; }
        public byte[] file { get; set; }
        public string file_base64 { get; set; }
        public byte[] file_thumnail { get; set; }


        public string ma_doi_tac { get; set; }
        public string so_id { get; set; }
        public string bt { get; set; }
        public string nhom_anh { get; set; }
        public decimal x { get; set; }
        public decimal y { get; set; }
        public string gid { get; set; }
        public decimal? stt { get; set; }

        public string loai { get; set; }
        public string lh_nv { get; set; }
        public string hang_muc { get; set; }
        public string muc_do { get; set; }
        public string thay_the_sc { get; set; }
        public string chinh_hang { get; set; }
        public string thu_hoi { get; set; }
        public decimal? vu_tt { get; set; }
        public decimal? tien_tu_dong { get; set; }
        public decimal? tien_gd { get; set; }
        public string ghi_chu { get; set; }
        public decimal? stt_hang_muc { get; set; }

        public StatusUploadFile(string base_url, string path_file,string file_name, string file_name_new, string nhom_anh, string extension_file, decimal? stt, string status_upload = StatusUploadFileConstant.SUCCESS, string error_message = "", byte[] file = null, decimal x =0, decimal y = 0)
        {
            this.file = file;
            this.stt = stt;
            this.path_file = path_file;
            this.file_name = file_name;
            this.extension_file = extension_file;
            this.status_upload = status_upload;
            this.error_message = error_message;
            this.file_name_new = file_name_new;
            this.nhom_anh = nhom_anh;
            this.x = x;
            this.y = y;
            this.gid = Guid.NewGuid().ToString("N");
        }
        public void SetPhanLoai(FileInfoData fileInfo)
        {
            this.loai = fileInfo==null?"": fileInfo.loai;
            this.lh_nv = fileInfo == null ? "" : fileInfo.lh_nv;
            this.hang_muc = fileInfo == null ? "" : fileInfo.hang_muc;
            this.muc_do = fileInfo == null ? "" : fileInfo.muc_do;
            this.thay_the_sc = fileInfo == null ? "" : fileInfo.thay_the_sc;
            this.chinh_hang = fileInfo == null ? "" : fileInfo.chinh_hang;
            this.thu_hoi = fileInfo == null ? "" : fileInfo.thu_hoi;
            this.vu_tt = fileInfo == null ? 0 : fileInfo.vu_tt;
            this.tien_tu_dong = fileInfo == null ? 0 : fileInfo.tien_tu_dong;
            this.tien_gd = fileInfo == null ? 0 : fileInfo.tien_gd;
            this.ghi_chu = fileInfo == null ? "" : fileInfo.ghi_chu;
            this.x = fileInfo == null ? 0 : fileInfo.x;
            this.y = fileInfo == null ? 0 : fileInfo.y;
            this.stt = fileInfo == null ? 0 : (fileInfo.stt??0);
            this.stt_hang_muc = fileInfo == null ? 0 : (fileInfo.stt_hang_muc??0);
        }
        public StatusUploadFile()
        {

        }

        
    }
    public class FileInfoData
    {
        public string key_file { get; set; }
        public string nhom { get; set; }
        public decimal x { get; set; }
        public decimal y { get; set; }
        public decimal? stt { get; set; }

        public string loai { get; set; }
        public string lh_nv { get; set; }
        public string hang_muc { get; set; }
        public string muc_do { get; set; }
        public string thay_the_sc { get; set; }
        public string chinh_hang { get; set; }
        public string thu_hoi { get; set; }
        public decimal? vu_tt { get; set; }
        public decimal? tien_tu_dong { get; set; }
        public decimal? tien_gd { get; set; }
        public string ghi_chu { get; set; }
        public decimal? stt_hang_muc { get; set; }
    }
}
