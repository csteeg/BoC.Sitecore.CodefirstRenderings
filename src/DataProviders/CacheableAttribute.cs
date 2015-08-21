using System.Collections;
using System.Collections.Generic;
using Sitecore.Data;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class CacheableAttribute : ActionAttribute
    {

        static ID _cachefield = new ID("{3D08DB46-2267-41B0-BC52-BE69FD618633}");
        static ID _clearonindexupdatefield = new ID("{F3E7E552-D7C8-469B-A150-69E4E14AB35C}");
        static ID _varybydatafield = new ID("{8B6D532B-6128-4486-A044-CA06D90948BA}");
        static ID _varybyloginfield = new ID("{8D9232B0-613F-440B-A2FA-DCDD80FBD33E}");
        static ID _varybydevicefield = new ID("{C98CF969-BA71-42DA-833D-B3FC1368BA27}");
        static ID _varybyparmfield = new ID("{3AD2506A-DC39-4B1E-959F-9D524ADDBF50}");
        static ID _varybyquerystringfield = new ID("{1084D3D2-0457-456A-ABBC-EB4CC0966072}");
        static ID _varybyuserfield = new ID("{0E54A8DC-72AD-4372-A7C7-BB4773FAD44D}");

        public CacheableAttribute()
        {
        }

        public bool VaryByData { get; set; }
        public bool VaryByDevice { get; set; }
        public bool VaryByLogin { get; set; }
        public bool VaryByParm { get; set; }
        public bool VaryByQueryString { get; set; }
        public bool VaryByUser { get; set; }
        public bool ClearOnIndexUpdate { get; set; }
        public override IEnumerable<KeyValuePair<ID, string>> GetFields()
        {
            return new Dictionary<ID, string>
            {
                {_cachefield, "1"},
                {_varybydatafield, VaryByData ? "1" : "0"},
                {_varybydevicefield, VaryByDevice ? "1" : "0"},
                {_varybyloginfield, VaryByLogin ? "1" : "0"},
                {_varybyparmfield, VaryByParm ? "1" : "0"},
                {_varybyquerystringfield, VaryByQueryString ? "1" : "0"},
                {_varybyuserfield, VaryByUser ? "1" : "0"},
                {_clearonindexupdatefield, ClearOnIndexUpdate? "1" : "0"}
            };
        }
    }
}
