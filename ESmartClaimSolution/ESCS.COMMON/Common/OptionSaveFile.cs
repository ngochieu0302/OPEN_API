using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Common
{
    public class OptionSaveFile
    {
        /// <summary>
        /// Loại file
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Mã đối tác
        /// </summary>
        public string ma_doi_tac { get; set; }
        /// <summary>
        /// Số ID
        /// </summary>
        public string so_id { get; set; }
        /// <summary>
        /// Có dupplicate file đó ra 1 file mini khác hay không
        /// </summary>
        public bool is_duplicate_mini_file { get; set; }
        /// <summary>
        /// Chiều rộng tối đa
        /// </summary>
        public int? max_width { get; set; }
        /// <summary>
        /// Kích thước tối đa của file
        /// </summary>
        public int? max_length { get; set; }
        /// <summary>
        /// Cấu hình file mini
        /// </summary>
        public ConfigDuplicateMiniFile config_mini_file { get; set; }

    }
    public class ConfigDuplicateMiniFile
    {
        public int? width { get; set; }
        public int? max_length { get; set; }
        public string prefix { get; set; }
    }
}
