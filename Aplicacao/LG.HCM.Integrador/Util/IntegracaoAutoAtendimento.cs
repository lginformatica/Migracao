using LG.HCM.Integrador.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using W3.Library.DataSql;

namespace LG.HCM.Integrador.Util {

    public class IntegracaoAutoAtendimento {
        public static string Decrypt(string text) {
            var inputByteArray = Convert.FromBase64String(text);

            using (RijndaelManaged rm = new RijndaelManaged()) {
                rm.BlockSize = 256;
                rm.IV = Encoding.UTF8.GetBytes("d5Gh,PSr-!]aR(@yv535i9UMgu-J:q9#");
                rm.KeySize = 256;
                rm.Key = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes("c83501a9-985b-4d5f-9964-a15670344e18"));

                using (ICryptoTransform decryptor = rm.CreateDecryptor()) {
                    using (MemoryStream msDecrypt = new MemoryStream(inputByteArray)) {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}