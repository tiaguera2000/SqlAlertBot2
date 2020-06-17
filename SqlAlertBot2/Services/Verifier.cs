using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;

namespace SqlAlertBot2.Services
{
    public static class Verifier
    {
        private readonly static string Path = @"C:\Users\tiagoreis\Documents\teste"; 

        public static string msg;

        public static string Verify()
        {
            
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(Path);

                    var files = dir.GetFiles("*.txt");

                    if (files.Length <= 0 || files is null)
                    {
                        msg = $"⚠️ O backup não foi gerado: Arquivo não existente!";
                        {
                            return msg;
                        }
                    }
                    else
                    {
                        
                        foreach (FileInfo file in files)
                        {
                            string text = System.IO.File.ReadAllText(file.ToString());

                            if (text.Contains("Status: Succeeded"))
                            {
                                return $"✅ O arquivo {file.ToString()} apresenta status SUCCESS!";
                            }
                            else if (text == null || text.Contains("Status: Warning"))
                            {
                                var inicio = text.LastIndexOf("Details:");
                                var texto = text.Substring(inicio);
                                var fim = texto.IndexOf("Command:");

                                texto = texto.Substring(0, fim);

                                return $"🆘 O arquivo {file.ToString()} apresenta status WARNING! \n\n {texto}";
                            }

                        }

                    }

                }
                catch (Exception e)
                {
                    return e.Message;
                }
            return "";
        }
    }
}
