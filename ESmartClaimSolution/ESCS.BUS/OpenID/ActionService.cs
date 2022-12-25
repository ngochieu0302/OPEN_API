using ESCS.COMMON.Common;
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
    public interface IActionService
    {
        Task<PaginationGenneric<openid_sys_action>> GetPaging(openid_sys_action search);
        Task<int> Save(openid_sys_action model);
        Task<int> SaveNew(openid_sys_action model);
        Task<int> CheckExistAction(openid_action_check model);
        Task<openid_sys_action_detail> GetDetail(sys_action action);
        Task<IEnumerable<openid_sys_action>> GenCode();
    }
    public class ActionService : IActionService
    {
        private readonly IActionRepository _actionRepository;
        public ActionService(IActionRepository actionRepository)
        {
            _actionRepository = actionRepository;
        }
        public async Task<int> CheckExistAction(openid_action_check model)
        {
            return await _actionRepository.CheckExistAction(model);
        }
        public async Task<PaginationGenneric<openid_sys_action>> GetPaging(openid_sys_action search)
        {
            return await _actionRepository.GetPaging(search);
        }
        public async Task<IEnumerable<openid_sys_action>> GenCode()
        {
            return await _actionRepository.GenCode();
        }
        public async Task<int> Save(openid_sys_action model)
        {
            var openid_action_check = new openid_action_check()
            {
                actioncode = model.exc_actioncode,
                action_type = model.ac_action_type,
                exc_package_name = model.exc_package_name,
                exc_storedprocedure = model.exc_storedprocedure,
                exc_schema_id = model.exc_schema_id
            };
            int count = await _actionRepository.CheckExistAction(openid_action_check);
            if (count > 0)
            {
                throw new Exception("Thủ tục đã tồn tại trong OpenId");
            }
            return await _actionRepository.Save(model);
        }
        public async Task<int> SaveNew(openid_sys_action model)
        {
            if (!string.IsNullOrEmpty(model.file_user_remote))
            {
                model.file_user_remote = Utilities.Encrypt(model.file_user_remote);
            }
            if (!string.IsNullOrEmpty(model.file_pas_remote))
            {
                model.file_pas_remote = Utilities.Encrypt(model.file_pas_remote);
            }

            if (!string.IsNullOrEmpty(model.mail_user_remote))
            {
                model.mail_user_remote = Utilities.Encrypt(model.mail_user_remote);
            }
            if (!string.IsNullOrEmpty(model.mail_pas_remote))
            {
                model.mail_pas_remote = Utilities.Encrypt(model.mail_pas_remote);
            }
            return await _actionRepository.SaveNew(model);
        }
        public async Task<openid_sys_action_detail> GetDetail(sys_action action)
        {
            return await _actionRepository.GetDetail(action);
        }
    }
}
