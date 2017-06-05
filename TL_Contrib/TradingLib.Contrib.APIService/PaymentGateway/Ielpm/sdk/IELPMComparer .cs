using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ielpm_merchant_code_demo.com.ielpm.merchant.code.sdk
{
    public class IELPMComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {

            char[] v1 = x.ToCharArray();
            char[] v2 = y.ToCharArray();
            int len1 = v1.Length;
            int len2 = v2.Length;
            int lim = len1 > len2 ? len2 : len1;
            char[] vc1 = v1;
            char[] vc2 = v2;

            int k = 0;
            while (k < lim)
            {
                char c1 = v1[k];
                char c2 = v2[k];
                if (c1 != c2)
                {
                    return c1 - c2;
                }
                k++;
            }
            return len1 - len2;
 
        }

    }
}