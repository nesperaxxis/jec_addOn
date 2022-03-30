using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using Newtonsoft.Json;

namespace JEC_SAP.Forms
{
    [FormAttribute("150", "Forms/frmItemMaster.b1f")]
    class frmItemMaster : SystemFormBase
    {
        private SAPbouiCOM.Button cmdTrans;
        private SAPbouiCOM.EditText edItem;

        public frmItemMaster()
        {
            Helpers.GlobalVar.GetIntegrationSetup();
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.cmdTrans = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.edItem = ((SAPbouiCOM.EditText)(this.GetItem("5").Specific));
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.DataAddAfter += new DataAddAfterHandler(this.Form_DataAddAfter);
            this.DataUpdateAfter += new DataUpdateAfterHandler(this.Form_DataUpdateAfter);
        }

        private void Form_DataAddAfter(ref SAPbouiCOM.BusinessObjectInfo pVal)
        {
            try
            {
                if (this.cmdTrans.Caption == "Add" && pVal.ActionSuccess == true)
                {
                    Helpers.GlobalVar.GetIntegrationSetup();

                    this.edItem = ((SAPbouiCOM.EditText)(this.GetItem("5").Specific));
                    Helpers.GlobalVar.listItem = Helpers.GlobalVar.ItemMasterData(Helpers.GlobalVar.pricelistcode, Helpers.GlobalVar.myCompany.GetDBServerDate().ToString());

                    string strQuery = "select \"id\" from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " where and \"module\" = 'Product' and \"uniqueId\" = '" + edItem.Value + "' and \"companyDB\" = '" + Helpers.GlobalVar.myCompany.CompanyDB + "' and \"lastTimeStamp\" = '" + Helpers.GlobalVar.myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "'";
                    Helpers.GlobalVar.oRS.DoQuery(strQuery);

                    if (Helpers.GlobalVar.oRS.RecordCount > 0)
                    {
                        string Id = Helpers.GlobalVar.oRS.Fields.Item(0).Value.ToString();
                        string strJSON = JsonConvert.SerializeObject(Helpers.GlobalVar.listItem);
                        if (Helpers.GlobalVar.SBOPostFinanceItem(Id, edItem.Value, strJSON) == false)
                        {
                            Application.SBO_Application.SetStatusBarMessage("Successfully create/update item in TAIDII Portal.", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.ToString());
            }
        }

        private void Form_DataUpdateAfter(ref SAPbouiCOM.BusinessObjectInfo pVal)
        {
            try
            {
                if (this.cmdTrans.Caption == "Update" && pVal.ActionSuccess == true)
                {
                    Helpers.GlobalVar.GetIntegrationSetup();

                    this.edItem = ((SAPbouiCOM.EditText)(this.GetItem("5").Specific));
                    Helpers.GlobalVar.listItem = Helpers.GlobalVar.ItemMasterData(Helpers.GlobalVar.pricelistcode, edItem.Value);

                    string strQuery = "select \"id\" from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " where \"module\" = 'Product' and \"uniqueId\" = '" + edItem.Value + "' and \"companyDB\" = '" + Helpers.GlobalVar.myCompany.CompanyDB + "'";
                    Helpers.GlobalVar.oRS.DoQuery(strQuery);
                    if (Helpers.GlobalVar.oRS.RecordCount > 0)
                    {
                        string Id = Helpers.GlobalVar.oRS.Fields.Item(0).Value.ToString();
                        string strJSON = JsonConvert.SerializeObject(Helpers.GlobalVar.listItem);
                        if (Helpers.GlobalVar.SBOPostFinanceItem(Id, edItem.Value, strJSON) == false)
                        {
                            Application.SBO_Application.SetStatusBarMessage("Successfully create/update item in TAIDII Portal.", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.ToString());
            }
        }
    }
}
