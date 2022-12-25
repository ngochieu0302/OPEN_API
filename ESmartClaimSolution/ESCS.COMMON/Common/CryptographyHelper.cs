using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ESCS.COMMON.Common
{
    public static class CryptographyHelper
    {
        public const int KEYSIZE512 = 512;
        public const int KEYSIZE768 = 768;
        public const int KEYSIZE1024 = 1024;
        private static bool _optimalAsymmetricEncryptionPadding = false;
        private readonly static string PublicKey = "NDA5NiE8UlNBS2V5VmFsdWU+PE1vZHVsdXM+eVNQREYvY3dqUTduUnZjWmdWODI3cTcyZzNuMVN3RkFSc0VJbFpTcXczVUN0YWpaOE8wSUlicHVnZk9xSkZQREFJdmtKSlc5SVpTTVMzVkZTU1l2RmRTZHEzWTk3V1JyNFprTWdLZUlqbHJDN2crNk8vbmtWdWkwaUpnT0dHa3kvVTREQjlWOFFHNGthV2sydm45Q01XM2tNUUpyMGt1R3BRS2EzOUtnS0ROU2prN3JtbUhWd29qMUZTTk5Ed2RtbkdhQVNyb1ZtRU5SWkxHOHZFUXRKRG5WV1BCSHFtYkFDcUhYa1p5ZW9IbHRXM2lCdy9JUE5lRTBGSFdLNXcrYnRsbktwLyt1dXh6cW5rYkdTY1pkdVZHbk83SkJOYXU0UWhBdnJ1S0NCcVVDa3VtaUtmOFgzMTlpdTlKTXR1Ky9ZR3RwNGpZRHVhRTAwL2NTQ29FaWJVL09qOGxMVEZqVHE5ZW1ZMXplanNhdnl5R01CNVhNb0M5UVpyQlYvQXZCZzdFL2RJRE9wSVFSNVJJTzRkZDlJaWpJdkgyU0hSbDZCb09BVEZyS2R0WHFiSEhZMUUrSWpaVVZONWxxQjFJb2VUbjR1K0hLQTA0VTFONUdZWlJ6WHZwWGUySXBrcGJXeWRJbG11R0JJSmxXenZJV2FzZ1NjVzk1c3ZNclEzeXdYd3FBNS9lSVdrZU5lN09YUkQ4YzdpMHRCaDlOQlU1YlNCSnZJakxSUU8vblBidGY4ZHowODVBeCtKQ0RScmVqL2gwQlErazR4bWVmbld4bDZRT0M3V2hZL0E5L1JhalRhbWRGMjQyNnJHUW84cERyYWxRR0tUL3V6OGJnVG1sMW1uZWpWeGVWYTdseWpMRFJScVZMRXcxbXBBR0NVaERsekxMckZyQ25sMms9PC9Nb2R1bHVzPjxFeHBvbmVudD5BUUFCPC9FeHBvbmVudD48L1JTQUtleVZhbHVlPg==";
        private readonly static string PrivateKey = "NDA5NiE8UlNBS2V5VmFsdWU+PE1vZHVsdXM+eVNQREYvY3dqUTduUnZjWmdWODI3cTcyZzNuMVN3RkFSc0VJbFpTcXczVUN0YWpaOE8wSUlicHVnZk9xSkZQREFJdmtKSlc5SVpTTVMzVkZTU1l2RmRTZHEzWTk3V1JyNFprTWdLZUlqbHJDN2crNk8vbmtWdWkwaUpnT0dHa3kvVTREQjlWOFFHNGthV2sydm45Q01XM2tNUUpyMGt1R3BRS2EzOUtnS0ROU2prN3JtbUhWd29qMUZTTk5Ed2RtbkdhQVNyb1ZtRU5SWkxHOHZFUXRKRG5WV1BCSHFtYkFDcUhYa1p5ZW9IbHRXM2lCdy9JUE5lRTBGSFdLNXcrYnRsbktwLyt1dXh6cW5rYkdTY1pkdVZHbk83SkJOYXU0UWhBdnJ1S0NCcVVDa3VtaUtmOFgzMTlpdTlKTXR1Ky9ZR3RwNGpZRHVhRTAwL2NTQ29FaWJVL09qOGxMVEZqVHE5ZW1ZMXplanNhdnl5R01CNVhNb0M5UVpyQlYvQXZCZzdFL2RJRE9wSVFSNVJJTzRkZDlJaWpJdkgyU0hSbDZCb09BVEZyS2R0WHFiSEhZMUUrSWpaVVZONWxxQjFJb2VUbjR1K0hLQTA0VTFONUdZWlJ6WHZwWGUySXBrcGJXeWRJbG11R0JJSmxXenZJV2FzZ1NjVzk1c3ZNclEzeXdYd3FBNS9lSVdrZU5lN09YUkQ4YzdpMHRCaDlOQlU1YlNCSnZJakxSUU8vblBidGY4ZHowODVBeCtKQ0RScmVqL2gwQlErazR4bWVmbld4bDZRT0M3V2hZL0E5L1JhalRhbWRGMjQyNnJHUW84cERyYWxRR0tUL3V6OGJnVG1sMW1uZWpWeGVWYTdseWpMRFJScVZMRXcxbXBBR0NVaERsekxMckZyQ25sMms9PC9Nb2R1bHVzPjxFeHBvbmVudD5BUUFCPC9FeHBvbmVudD48UD56UU1xTlF2TzMxbHR2eUo4ZHRWZmtPT2JrNU52NmRWTDBCcEdQK0J4YmRPR0dVWWxFUkJHdHN5TVBmZ1hrZmx2aThNRlNoV1M2eVppZ3FaTitQcm5hYzA3ODlyVTNmODRmT0IyWHZ2MU9wRFdBVHdZdnIvMnFvUjJPQ0ZRaFFFVFI2RVVPTFdKRWFaYXJZZFRjYi85V0lxV2FLcnZ5WnZFK3pPRzJvODB6WHplR0U3SktGcFhHcWVUWjRieXlVeXAvUmVXZWJhYmNNRlYxQWtHQ0gvM2tNSVdzeUx6N3M3WmFtYzU5LzJMbnFyZTNSdE4yRS83N1Y0eHlyRGxGcVkzbXd5dFBaRmN2OWFDZ29KOWNxZHFEdzBMVm1ONlYvSHdoYXlmL2xMa2p4MWtEMHhvSUFkU1pkbXZaSjE1NzJLNnhid0FzV2w0NU45VGNOeUl4WnBuY3c9PTwvUD48UT4reW9IdXp4OTBVK0JkbHZKdlJOMGhDZ0F4bmIrVkgyUVptL0NBZ1pwSklrWWJ2Vkd5aDVIRUR0eTlWaCs4Q0RpL2xvQWMwNC9TNk5SMW9YaE55b2xwY1dNcGpXcHFsNExwbEZPTmxDUnZMUDdJUDdaYllQcmNLaFpSekE5ei9VdlAwcGtNTTBWbzJORVFCZlZlSzg4VmVLdW1CRTA3Q3o2NnZUUThXRXNIK3o5ZC9oa0hCYXRGMC9UdXl5bm9jd2dZUDBVUlBPU3RFekhyNy9DOHE2UnU2VGlPb3dxWUJtcyswNTFKdmFJazV5eEdjbGx6ci9OcGtZd0NXNENjSU5RL1NHRmNqQ2RQWXNDQWMrUnlrQUdpY3lXQ1A2U0NLUTFzaEdTNTlMdWUzVjFydVpxR2NyQWFMR1U5WXViNWhqYVZXYXljWkxNTzMrK0NoLzhidVUyc3c9PTwvUT48RFA+QlhvN0szV29UbEZlWEVmNG1WZzBHSFBzV1RJTVFHd2hmV3JtUzVpY2p6bWlrSlJPTXptREdpazI2Z1R4NUVXTTFHZ2VnUTg0Z0szaTNlakJscEJsbEc4V1ZmcVh5MktoU0x6YWpNa2drVE1EZEw3OVRrTmt2TU9UUVFhcTF4TGpSL3VteGFrY0EydTFhb04rR0VEdjdPaXZBaGJuRnpUaE8zRHgvbTdyRXo1dDFLcHVOM1RLZFB6Yk5GM25xTGN5cDM5MDFzV3BPNGIrMnc1dXFkVjVjTld1czVTUWZlWHE2eXZjNGFKVms2U3ZGYXRBRmpKMDRhK3RsdVBKQ3lUZndYWENQeTNlRTRZUXNKbjU3aEZKQlNQdEZKN055d0c4MG1WOVZOYzBJaHEvN0tTMjkzd2FtSHdMY3AyK2YwWGRCYUZ6UjBveEg3UlR6MlVsQmxxTXZ3PT08L0RQPjxEUT5GTEhwNFN1aTBsUlN2b1hob1lxdzF4TFV2WVdnZnlXNWVEZE5MUTJLTEZCL2l1VDY2RHlHYm5mM2lPKzV4ZDl2MUxWbHhCcDJiSzBRV1RTKzBlVFBKVkdadlRnK3VxTDFJc2NnUXFsdzJMT2J4YVE2RmlRQWlrL1V5MkRXTndSazJEb3c5elJ1eS8rcDUvNE93TnJ2aGNRZnpyZmNQSG1ZVXpQSTQ1cTJJQXRPOTB1b05BaEtTdGx4QU1NMUxNZTlBMERYSXJkUEp6d01XQmJjNFZJdy85MGVKdVRWdVFWa2szQnJoc0paanBBaXA1Vklwamgzb1hPM0VZL1c4NTVlVmYwbEdidVh1VEp5LytNeWlBK3Vlanc1UUpSNVErMEQzNkJERW9iMno0Vm1KUHNuVXVpNTRlWi85NXVCUlB4WFFTSXRJZDZPcFhocEdEeDZlRWk2c1E9PTwvRFE+PEludmVyc2VRPmsyMzRxWVZyN2ttY2Zpc3IvRm9za1gwbjJibzlsWmludkxmZ0Y2L1RaWEhraU1vY1NtZU1NcEMzUWk4bUtHRUZKQXNVTERMQVZ0ZFpXZSt6MlZ3QWs2SlpZNU5tVTN0Z3hpVmdCdy9HbEl5UnNoTnQyRU9MdHNuT3BGVmZ1Y2orbTN5SWYzdTNuVkJwZlNQZXJEbWFaUDNMZzROaVpYM3lWT25YUGFHMmJoQ3FZVDBhMHRDMGE3ZVpzcm9RUExPWkhDYTVEM2s1TTBib2dGaitrWlc1OUZPMXVvamZuUHRIY0VGMU8zUkN3NjhqMVd3MlFoMzc0OEkyUElxdzVXalFjZm83Sk1mY3VJZVF3cWFkY3hwU2piT1VWS1Z0ay81c1I0aCtCTUpvNlJvR2xJYWF2TGV3VHA2WGovM0tXQ1d0OWREa3UwQXp6aXM1UWFJTDZqMGg3QT09PC9JbnZlcnNlUT48RD5rYXpGK3BCNzZGMjM0V3pMV3lpRGw0QytaMkNuNkNmQ3pydTFwT2drZVNWQXppSzVuY1YrVXpjOEhBQkc1TUtKZzliTmFEWE80Z2c5UjlWcVZkMUMwdFBIVWN4MGMzOThwc3A3Qko3Unh4QXpCOG42UHIvd3dZVU9ZN09FMmVjdkY4Z3RaN29Wb0hCbERMYzMvK1laSWNXT05IU2xkSXhndXhmVTVYQWRzSi9qRmRCUTBPSTB1QTBMTzhEeWFZVVpxd0tOdmhGaDNJSTg2K25TWUtRYjhaVzFVQkhPUmJyZk1aUWZsWVZTV2JVNEg3eDZkMWpUN1ZqcXFEcXRYRVd3cC9RR1U0QlpvNGtBMzVzd3QzUGtvM2hnMzFwdDRobzVmQmRQQWFGcFBMaTcwWDFHaHBZTFN5dEJUZmkwdVBXUXJKdCs5MmZaY05NeGVLY0R1UG5uM090QlVyLytiUHoxM3VLdW5OZnlpTUZwd0x1YndGeGJnelNsTHZWYXBXclBTUzFZajQ2dEVHcmZSVTM2a29NSDZvam5vcGN6citiWWZ0bkxTOGg4UHN4NDR1QkFadWUyRVI1Yjlnd3Y5YXRlSUhyNWFLS0F2NENvREdQR2UwMExKdFR0a0lWWUhXQVhnUTQ3MVVSMUFjSjNYQUdpT1FlN3VjcFRVdXBwdW5hOFFqTCtwcHBZdDI4dzREcER1TmNtSEcwOEhjb3loaFNTbjlUcTQ4am9uNG5hTjNxbHE4TEFlenYvQWc0dUFSQVhibXRKMnEwZ0ZHVFBHMDFoZDlFQXlZUCt5UlArL0ZQeVRPWC9YcVhYSndKTHN1bG1yTlhIOVZvMEl4cWIxUWhXdi9CT2J0eFNSQTcxYlMzZDZHUkp5ZFR4b2Z6NEoxejRwNVdOd202aVlPVT08L0Q+PC9SU0FLZXlWYWx1ZT4=";
        public static string Encrypt(string plainText)
        {
            int keySize = 0;
            string publicKeyXml = "";

            GetKeyFromEncryptionString(PublicKey, out keySize, out publicKeyXml);

            var encrypted = Encrypt(Encoding.UTF8.GetBytes(plainText), keySize, publicKeyXml);

            return Convert.ToBase64String(encrypted);
        }
        private static byte[] Encrypt(byte[] data, int keySize, string publicKeyXml)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
            int maxLength = GetMaxDataLength(keySize);
            if (data.Length > maxLength) throw new ArgumentException(String.Format("Maximum data length is {0}", maxLength), "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicKeyXml)) throw new ArgumentException("Key is null or empty", "publicKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicKeyXml);
                return provider.Encrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }
        public static string Decrypt(string encryptedText)
        {
            int keySize = 0;
            string publicAndPrivateKeyXml = "";
            GetKeyFromEncryptionString(PrivateKey, out keySize, out publicAndPrivateKeyXml);
            var decrypted = Decrypt(Convert.FromBase64String(encryptedText), keySize, publicAndPrivateKeyXml);
            return Encoding.UTF8.GetString(decrypted);
        }
        private static byte[] Decrypt(byte[] data, int keySize, string publicAndPrivateKeyXml)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicAndPrivateKeyXml)) throw new ArgumentException("Key is null or empty", "publicAndPrivateKeyXml");
            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicAndPrivateKeyXml);
                return provider.Decrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }
        public static int GetMaxDataLength(int keySize)
        {
            if (_optimalAsymmetricEncryptionPadding)
            {
                return ((keySize - 384) / 8) + 7;
            }
            return ((keySize - 384) / 8) + 37;
        }
        private static bool IsKeySizeValid(int keySize)
        {
            return keySize >= 384 && keySize <= 16384 && keySize % 8 == 0;
        }
        private static void GetKeyFromEncryptionString(string rawkey, out int keySize, out string xmlKey)
        {
            keySize = 0;
            xmlKey = "";

            if (rawkey != null && rawkey.Length > 0)
            {
                byte[] keyBytes = Convert.FromBase64String(rawkey);
                var stringKey = Encoding.UTF8.GetString(keyBytes);

                if (stringKey.Contains("!"))
                {
                    var splittedValues = stringKey.Split(new char[] { '!' }, 2);

                    try
                    {
                        keySize = int.Parse(splittedValues[0]);
                        xmlKey = splittedValues[1];
                    }
                    catch (Exception e) { }
                }
            }
        }
    }
}
