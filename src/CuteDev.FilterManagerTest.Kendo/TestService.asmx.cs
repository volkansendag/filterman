using CuteDev.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace CuteDev.FilterManagerTest.Kendo
{
    /// <summary>
    /// Summary description for TestService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class TestService : System.Web.Services.WebService
    {

        [WebMethod]
        public rValue<MyEntity> Add(MyEntity prms)
        {
            try
            {
                var db = new ModelTest();
                var ent = new MyEntity() { Name = prms.Name };
                db.MyEntities.Add(ent);
                db.SaveChanges();
                return new rValue<MyEntity>(ent);
            }
            catch (Exception ex)
            {
                return new rValue<MyEntity>(ex);
            }
        }

        [WebMethod]
        public rValue<MyEntity> Update(MyEntity prms)
        {
            try
            {
                var db = new ModelTest();
                var ent = db.MyEntities.Single(p => p.Id == prms.Id);
                ent.Name = prms.Name;
                db.SaveChanges();
                return new rValue<MyEntity>(ent);
            }
            catch (Exception ex)
            {
                return new rValue<MyEntity>(ex);
            }
        }

        [WebMethod]
        public rValue<int> Delete(MyEntity prms)
        {
            try
            {
                var db = new ModelTest();
                var ent = db.MyEntities.Single(p => p.Id == prms.Id);
                db.MyEntities.Remove(ent);
                db.SaveChanges();
                return new rValue<int>(prms.Id);
            }
            catch (Exception ex)
            {
                return new rValue<int>(ex);
            }
        }

        [WebMethod]
        public rList<MyEntity> List(pList prms)
        {
            try
            {
                var db = new ModelTest();
                return db.MyEntities.OrderBy(p => p.Id).Filter(prms);
            }
            catch (Exception ex)
            {
                return new rList<MyEntity>(ex);
            }
        }
    }
}
