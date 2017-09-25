using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Resources
{
    public class Resource
    {
        public static string MunicipalDOKConfirmedInfo(string confirmed)
        {
            return string.Format(DataSet.MunicipalDOKConfirmedInfoPlaceholder, confirmed);
        }
    }
}