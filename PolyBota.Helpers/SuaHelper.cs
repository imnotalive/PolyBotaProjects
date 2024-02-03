using PolyBota.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PolyBota.BLL;

namespace PolyBota.Helpers
{
    public static class SuaHelper
    {
        public static string FirsToUpper(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
           
            }
            else
            {
                var result = "";

                var splited = str.Split(' ').ToList();

                foreach (var i in splited)
                {
                    var iStr = string.IsNullOrEmpty(i) ? i : i[0].ToString().ToUpper() + i.Substring(1).ToLower();
                   
                    result += iStr + " ";
                }
                return result;
            }
        }
        public static DatabaseConnectionRecord defaultSql()
        {
            return new DatabaseConnectionRecord()
            {
                Id = "1",
                IpAdress = @"10.10.1.1",
                DatabaseName = "POTA_BOTA",
                UserName = "sa",
                Password = ""
            };
        }

        public static decimal decimaleCevir(this string str)
        {

            if (str == null)
            {
                return 0;
            }
            var lst = str.ToCharArray();

            int noktaSay = 0;
            int virgulSay = 0;

            foreach (var c in lst)
            {
                if (c.ToString() == ".")
                {
                    noktaSay++;
                }
                if (c.ToString() == ",")
                {
                    virgulSay++;
                }
            }

            if (virgulSay == 0 && noktaSay == 0)
            {
                //integer
                return !string.IsNullOrEmpty(str) ? 0 : Convert.ToDecimal(str);
            }
            else
            {
                decimal TamKisim = 0;
                decimal Kusurat = 0;

                if (noktaSay == 0)
                {
                    // nokta yok

                    if (virgulSay == 1)
                    {
                        // tek virgülle ayrılmış

                        var splitLst = str.Split(',');
                        TamKisim = Convert.ToDecimal(splitLst[0]);
                        Kusurat = Convert.ToDecimal(splitLst[1]);
                        //if (Kusurat < 10)
                        //{
                        //    Kusurat /= 10;
                        //}
                        //else
                        //{
                        //    Kusurat /= 100;
                        //}
                    }
                    else
                    {
                        // virgül sayısı 1'den fazla

                        TamKisim = Convert.ToDecimal(str.Replace(",", ""));
                    }
                }
                else
                {
                    // nokta var

                    if (virgulSay == 0)
                    {
                        //SADECE nokta var

                        if (noktaSay == 1)
                        {
                            // tek nokta ile ayrılmış

                            var splitLst = str.Split('.');
                            TamKisim = Convert.ToDecimal(splitLst[0]);
                            Kusurat = Convert.ToDecimal(splitLst[1]);

                        }
                        else
                        {
                            // nokta sayısı 1'den fazla

                            TamKisim = Convert.ToDecimal(str.Replace(".", ""));
                        }
                    }
                    else
                    {
                        // hem nokta hem virgül var

                        if (noktaSay == 1)
                        {
                            // ondalık kısım nokta da
                            var splitLst = str.Replace(".", "").Split(',');
                            TamKisim = Convert.ToDecimal(splitLst[0]);
                            Kusurat = Convert.ToDecimal(splitLst[1]);
                        }
                        else
                        {
                            if (virgulSay == 1)
                            {
                                // ondalık kısım virgülde da
                                var splitLst = str.Replace(".", "").Split(',');
                                TamKisim = Convert.ToDecimal(splitLst[0]);
                                Kusurat = Convert.ToDecimal(splitLst[1]);
                            }
                        }
                    }
                }

                //if (Kusurat < 10)
                //{
                //    Kusurat /= 10;
                //}
                //else
                //{
                //    Kusurat /= 100;
                //}
                Kusurat /= 100;
                return TamKisim + Kusurat;
            }
        }

        public static string stringeCevir(this decimal dec)
        {
            int deger = Convert.ToInt32(dec * 100);

            int kusurat = deger % 100;

            int tamKisim = (deger - kusurat) / 100;

            string kusStr = kusurat.ToString();
            if (kusurat < 10)
            {
                kusStr += "0";
            }

            return string.Format("{0},{1}", tamKisim, kusStr);
        }

        public static string GgGetMD5Hash(string apikey, string secretkey, string timeestamp)
        {
            if (apikey == null) return null;

            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(apikey + secretkey + timeestamp);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public static string objEmpty(this object obj)
        {
            string result = "";
            try
            {
                result = obj.ToString();
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public static string KullaniciIsimGetir(this int id)
        {
            var result = "";
            var squery = string.Format("SELECT Name FROM dbo.[User] WHERE (Id = {0})", id);

            var liste = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var itt in liste)
            {
                result = itt.Values.ToList()[0]?.ToString();

            }

            return result;
        }
    }
}
