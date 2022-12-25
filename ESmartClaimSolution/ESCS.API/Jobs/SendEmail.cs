using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESCS.API.Jobs
{
    public class SendEmail : Job
    {
        public override void DoJob()
        {
            try
            {
               
            }
            catch (Exception Ex)
            {
                
            }
        }

        public override string GetName()
        {
            return this.GetType().Name;
        }

        public override int GetRepetitionIntervalTime()
        {
            return 60000;
        }

        public override bool IsRepeatable()
        {
            throw new NotImplementedException();
        }
    }
}
