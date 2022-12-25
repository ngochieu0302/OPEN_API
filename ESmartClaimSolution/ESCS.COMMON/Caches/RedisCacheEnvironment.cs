using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Caches
{
    public class RedisCacheEnvironment
    {
        public static string ServerName
        {
            get
            {
                return "Test";
            }
            set
            {

            }
        }

        public static string Endpoint
        {
            get
            {
                return "localhost";
            }
            set
            {

            }
        }

        public static int Database
        {
            get
            {
                return 2;
            }
            set
            {

            }
        }
    }
}
