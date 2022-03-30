using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbobsCOM;
namespace JEC_SAP.Helpers
{
    class SBOGetRecord
    {
        SAPbobsCOM.Recordset oRecSet { get; set; }
        SAPbobsCOM.Company company { get; set; }
        public string GetSingleValue(string StrQuery, SAPbobsCOM.Company SAPCompany)
        {
            try
            {
                oRecSet = (Recordset)Helpers.GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRecSet.DoQuery(StrQuery);
                return Convert.ToString(oRecSet.Fields.Item(0).Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
