using Engine.Areas.ImportExport.Service;
using Engine.Service.AbstractControllers;
using Newtonsoft.Json;
using WebAppIDEEngine.Models.ICore;

namespace Engine.Areas.ImportExport.ServiceTests
{
    public class ExcelStructreTableNode:IModel,IRemovableNode
    {
        public string ColumnName { get; set; }
        public int NumberInExcel { get; set; }
        public bool IsRequired { get; set; }
        public string ColumnTranslate { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        
        public bool IsRemoved { get; set; }
        
        [JsonIgnore]
        public virtual ExcelStructreTable StructureTable { get; set; }
        public virtual long StructureTableId { get; set; }
        
    }
}