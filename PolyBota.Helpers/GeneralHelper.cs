using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyBota.Helpers
{
    public static class GeneralHelper
    {


        public static string KodHazirla(string onEk, int id)
        {
            var result = onEk;



            if (id > 100000)
            {
                result += "";
            }
            else
            {
                if (id > 10000)
                {
                    result += "0";
                }
                else
                {
                    if (id > 1000)
                    {
                        result += "00";
                    }
                    else
                    {
                        if (id > 100)
                        {
                            result += "000";
                        }
                        else
                        {
                            if (id > 10)
                            {
                                result += "0000";
                            }
                            else
                            {
                                result += "00000";
                            }
                        }
                    }
                }
            }

            result += id;
            return result;
        }

        public static List<string> StrArrayeCevir(this string str, string ayrac)
        {
            var liste = new List<string>();

            if (!string.IsNullOrEmpty(str))
            {
                liste = str.Split(new string[] { ayrac }, StringSplitOptions.None).ToList();
            }

            return liste;
        }

        public static decimal SaatiDecimaleCevir(this string str)
        {
            var result = 0;

            if (!string.IsNullOrWhiteSpace(str))
            {
                var strArray = str.Replace(" ", "").Split(':');

                if (strArray.Length > 0)
                {
                    var intDeger = Convert.ToInt32(strArray[0]);
                    result += (intDeger * 60);
                }
                if (strArray.Length > 1)
                {
                    var intDeger = Convert.ToInt32(strArray[1]);
                    result += intDeger;
                }
            }

            return result;
        }

        public static string DecimaliSaateCevir(this decimal dec)
        {
            var kusurat = Convert.ToInt32(dec % 60);

            dec -= kusurat;

            int tamkisim = Convert.ToInt32(dec / 60);

            string result = tamkisim < 10 ? "0" + tamkisim : tamkisim.ToString();
            result = result + ":" + (kusurat < 10 ? "0" + kusurat : kusurat.ToString(""));

            return result;
        }

    }
}
