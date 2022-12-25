using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.AI
{
    public class AI_CONSTANT
    {
        public const string SUCCESS = "200";
        public const string ERRORCODE = "0";
        public const string ERRORMESSAGE = "SUCCESS";
    }
    public class cvs_response
    {
        public List<cvs_damage> data { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string image { get; set; }
    }
    public class cvs_damage
    {
        public List<string> damage_box { get; set; }
        public string damage_score { get; set; }
        public string damage_type { get; set; }
    }

    public class ai_response
    {
        public string code { get; set; }
        public string message { get; set; }
        public string photo_link { get; set; }
        public List<ai_damage> damage { get; set; }
        public string base64 { get; set; }
    }
    public class ai_damage
    {
        public string damage_type { get; set; }
        public string parts { get; set; }
        public string damage_box { get; set; }
        public string damage_score { get; set; }
    }
    public class ai_address_entities
    {
        public string province { get; set; }
        public string district { get; set; }
        public string ward { get; set; }
        public string street { get; set; }
    }
    public class ai_crop_image<T>
    {
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public List<T> data { get; set; }
    }
    public class ai_cmt_mat_truoc
    {
        public string id { get; set; }
        public string id_prob { get; set; }
        public string name { get; set; }
        public string name_prob { get; set; }
        public string dob { get; set; }
        public string dob_prob { get; set; }
        public string sex { get; set; }
        public string sex_prob { get; set; }
        public string nationality { get; set; }
        public string nationality_prob { get; set; }
        public string type_new { get; set; }
        public string doe { get; set; }
        public string doe_prob { get; set; }
        public string home { get; set; }
        public string home_prob { get; set; }
        public string address { get; set; }
        public string address_prob { get; set; }
        public ai_address_entities address_entities { get; set; }
        public string overall_score { get; set; }
        public string type { get; set; }
        public string face { get; set; }
        public string face_base64 { get; set; }
        public string cropped_idcard { get; set; }
        public string cropped_idcard_base64 { get; set; }
    }
}
