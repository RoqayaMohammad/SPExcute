using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;
using test.Data;
using test.Models;

namespace test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly ContextClass _context;
        public APIController(ContextClass context)
        {
            _context = context;

        }


        ///-----------------------

        //[HttpPost]
        //public IActionResult ExecuteStoredProcedure([FromBody] StoredProcedureParams parameters)
        //{
        //    try
        //    {
        //        if (parameters == null || string.IsNullOrWhiteSpace(parameters.ProcedureName))
        //        {
        //            return BadRequest("Invalid input parameters. ProcedureName is required.");
        //        }

        //        var inputParams = parameters.InputParameters?
        //            .Select(p => ParseInputParameter(p))
        //            .ToArray();

        //        var outputParams = parameters.OutputParameters?
        //            .Select(p => new SqlParameter { ParameterName = p, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Output })
        //            .ToArray();

        //        var sqlCommand = $"EXEC {parameters.ProcedureName} {string.Join(", ", parameters.InputParameters.Select(p => $"@{ParseParameterName(p)}"))}";

        //        _context.Database.ExecuteSqlRaw(sqlCommand, inputParams);

        //        var result = new Dictionary<string, object>();

        //        foreach (var outputParam in outputParams)
        //        {
        //            result[outputParam.ParameterName.TrimStart('@')] = outputParam.Value;
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error executing stored procedure: {ex.Message}");
        //    }
        //}

        //private SqlParameter ParseInputParameter(string inputParam)
        //{
        //    //  format is "@Name='John Doe'"
        //    var parts = inputParam.Split('=');

        //    if (parts.Length == 2)
        //    {
        //        return new SqlParameter(parts[0], SqlDbType.VarChar, 255) { Value = parts[1].Trim('\'') };
        //    }


        //    return new SqlParameter();
        //}

        //private string ParseParameterName(string inputParam)
        //{

        //    var parts = inputParam.Split('=');

        //    if (parts.Length == 2)
        //    {
        //        return parts[0];
        //    }


        //    return inputParam;
        //}

        [HttpPost]
        public IActionResult ExecuteStoredProcedure([FromBody] StoredProcedureParams parameters)
        {
            try
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.ProcedureName))
                {
                    return BadRequest("Invalid input parameters. ProcedureName is required.");
                }

                var inputParams = parameters.InputParameters?
                    .Select(p => ParseInputParameter(p))
                    .ToArray();

                var outputParams = parameters.OutputParameters?
                    .Select(p => new SqlParameter { ParameterName = p, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Output, Size = 255 })
                    .ToArray();
                var sqlCommand = "";
                if (outputParams == null )
                {
                    sqlCommand = $"EXEC {parameters.ProcedureName} {string.Join(", ", parameters.InputParameters.Select(p => $"@{ParseParameterName(p)}"))}";
                }
                else
                {
                    var Declaring = "";
                    var Selecting = "";
                        
                    for (int i = 0; i < outputParams.Length; i++)
                    {
                        Declaring = string.Join(", ", $"@x{i}" + outputParams[i].SqlDbType);
                        Selecting = string.Join(", ", $"@{outputParams[i].ParameterName} AS x{i}");

                    }    
                      var  declarecommand = $"DECLARE "+Declaring;
                       var SelectCommand=$"SELECT "+ Selecting;


                    sqlCommand = declarecommand + $"EXEC {parameters.ProcedureName} {string.Join(", ", parameters.InputParameters.Select(p => $"@{ParseParameterName(p)}"))}, {}";
                }



                _context.Database.ExecuteSqlRaw(sqlCommand, inputParams);


                var result = new Dictionary<string, object>();

                foreach (var outputParam in outputParams)
                {
                    result[outputParam.ParameterName.TrimStart('@')] = outputParam.Value;
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error executing stored procedure: {ex.Message}");
            }
        }

        //[HttpPost]
        //public IActionResult ExecuteStoredProcedure([FromBody] StoredProcedureParams parameters)
        //{
        //    try
        //    {
        //        if (parameters == null || string.IsNullOrWhiteSpace(parameters.ProcedureName))
        //        {
        //            return BadRequest("Invalid input parameters. ProcedureName is required.");
        //        }

        //        var outputParams = parameters.OutputParameters?
        //            .Select(p => new SqlParameter { ParameterName = p, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output })
        //            .ToArray();

        //        var outputCommand = outputParams != null
        //            ? $"DECLARE {string.Join(", ", outputParams.Select(p => $"@{p.ParameterName} {p.SqlDbType} OUTPUT"))};"
        //            : "";

        //        var sqlCommand = $"{outputCommand} EXEC {parameters.ProcedureName} {string.Join(", ", parameters.InputParameters.Select(p => $"@{ParseParameterName(p)}"))}";

        //        _context.Database.ExecuteSqlRaw(sqlCommand, outputParams);

        //        var result = new Dictionary<string, object>();

        //        foreach (var outputParam in outputParams)
        //        {
        //            result[outputParam.ParameterName.TrimStart('@')] = outputParam.Value;
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error executing stored procedure: {ex.Message}");
        //    }
        //}

        private string GetSqlTypeDeclaration(SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.VarChar:
                    return "VARCHAR(255)"; 
                case SqlDbType.Int:
                    return "INT";
                
                default:
                    return "";
            }
        }


        private SqlParameter ParseInputParameter(string inputParam)
        {
            //  format is "@Name='John Doe'"
            var parts = inputParam.Split('=');

            if (parts.Length == 2)
            {
                return new SqlParameter(parts[0], SqlDbType.VarChar, 255) { Value = parts[1].Trim('\'') };
            }

            return new SqlParameter();
        }

        private string ParseParameterName(string inputParam)
        {
            var parts = inputParam.Split('=');

            if (parts.Length == 2)
            {
                return parts[0];
            }

            return inputParam;
        }


    }


}
