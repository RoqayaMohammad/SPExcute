using Microsoft.Data.SqlClient;

namespace test.Models
{
    public class StoredProcedureParams
    {
        public string ProcedureName { get; set; }
        public List<string> InputParameters { get; set; }
        public List<string> OutputParameters { get; set; }
    }
}
