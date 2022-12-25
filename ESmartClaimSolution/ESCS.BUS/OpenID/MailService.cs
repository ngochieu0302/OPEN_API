using ESCS.COMMON.Response;
using ESCS.DAL.OpenID;
using ESCS.MODEL.OpenID;
using ESCS.MODEL.OpenID.ModelView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.BUS.OpenID
{
    public interface IMailService
    {
        Task<IEnumerable<sys_email_template>> GetAll();
    }
    public class MailService : IMailService
    {
        private readonly IMailRepository _mailRepository;
        public MailService(IMailRepository mailRepository)
        {
            _mailRepository = mailRepository;
        }
        public async Task<IEnumerable<sys_email_template>> GetAll()
        {
            return await _mailRepository.GetAll();
        }
    }
}
