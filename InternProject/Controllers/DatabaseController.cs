using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternProject.Abstract;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InternProject.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly IDatabaseService _databaseService;

        public DatabaseController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<IActionResult> Index()
        {
            bool isSecure = true;

            var databases = await _databaseService.GetDatabasesAsync(isSecure);

            ViewBag.databases = databases;

            return View(databases);
        }

        public IActionResult Details(string databaseName)
        {
            ViewBag.DatabaseName = databaseName; 
            return View();
        }

        public async Task<IActionResult> Tables(string databaseName)
        {
            bool isSecure = true; 

            var tables = await _databaseService.GetTablesAsync(databaseName, isSecure);

            ViewBag.DatabaseName = databaseName; 

            return View(tables);
        }

        public async Task<IActionResult> Views(string databaseName)
        {
            bool isSecure = true; 

            var views = await _databaseService.GetViewsAsync(databaseName, isSecure);

            ViewBag.DatabaseName = databaseName; 

            return View(views);
        }

        public async Task<IActionResult> Functions(string databaseName)
        {
            bool isSecure = true; 

            var functions = await _databaseService.GetFunctionsAsync(databaseName, isSecure);

            ViewBag.DatabaseName = databaseName; 

            return View(functions);
        }

        public async Task<IActionResult> Procedures(string databaseName)
        {
            bool isSecure = true; 

            var procedures = await _databaseService.GetStoredProceduresAsync(databaseName, isSecure);

            ViewBag.DatabaseName = databaseName; // Seçilen veritabanı adını ViewBag ile view'e aktar

            return View(procedures);
        }

        public async Task<IActionResult> TableDetails(string databaseName, string tableName)
        {
            bool isSecure = true; 

            var tableData = await _databaseService.GetTableDataAsync(databaseName, tableName, isSecure);

            ViewBag.DatabaseName = databaseName;
            ViewBag.TableName = tableName;

            return View(tableData);
        }

        public async Task<IActionResult> ViewDetails(string databaseName, string viewName)
        {
            bool isSecure = true; 

            var viewData = await _databaseService.GetViewDataAsync(databaseName, viewName, isSecure);

            ViewBag.DatabaseName = databaseName;
            ViewBag.ViewName = viewName;

            return View(viewData);
        }

        [HttpGet]
        public async Task<IActionResult> ProcedureDetails(string databaseName, string procedureName)
        {
            ViewBag.DatabaseName = databaseName;
            ViewBag.ProcedureName = procedureName;

            var parameters = await _databaseService.GetStoredProcedureParametersAsync(databaseName, procedureName, true);

            if (parameters == null || !parameters.Any())
            {
                // Eğer parametre yoksa doğrudan procedure sonuçlarını döndür
                var procedureData = await _databaseService.GetStoredProcedureDataAsync(databaseName, procedureName, true);
                ViewBag.DatabaseName = databaseName;
                ViewBag.ProcedureName = procedureName;
                return View("ProcedureResult", procedureData);
            }

            return View(parameters);
        }

        [HttpPost]
        public async Task<IActionResult> ProcedureDetails(string databaseName, string procedureName, Dictionary<string, string> parameters)
        {
            bool isSecure = true; 

            var paramDict = new Dictionary<string, object>();
            foreach (var param in parameters)
            {
                if (DateTime.TryParse(param.Value, out DateTime dateValue))
                {
                    paramDict[param.Key] = dateValue;
                }
                else
                {
                    paramDict[param.Key] = param.Value;
                }
            }

            var procedureData = await _databaseService.GetStoredProcedureDataAsync(databaseName, procedureName, isSecure, paramDict);

            ViewBag.DatabaseName = databaseName;
            ViewBag.ProcedureName = procedureName;

            return View("ProcedureResult", procedureData);
        }

        [HttpGet]
        public async Task<IActionResult> FunctionDetails(string databaseName, string functionName)
        {
            ViewBag.DatabaseName = databaseName;
            ViewBag.FunctionName = functionName;

            var parameters = await _databaseService.GetFunctionParametersAsync(databaseName, functionName, true);

            if (parameters == null || !parameters.Any())
            {
                
                var functionData = await _databaseService.GetFunctionDataAsync(databaseName, functionName, true);
                ViewBag.DatabaseName = databaseName;
                ViewBag.FunctionName = functionName;
                return View("FunctionResult", functionData);
            }

            return View(parameters);
        }

        [HttpPost]
        public async Task<IActionResult> FunctionDetails(string databaseName, string functionName, Dictionary<string, string> parameters)
        {
            bool isSecure = true; 

            var paramDict = new Dictionary<string, object>();
            foreach (var param in parameters)
            {
                if (DateTime.TryParse(param.Value, out DateTime dateValue))
                {
                    paramDict[param.Key] = dateValue;
                }
                else
                {
                    paramDict[param.Key] = param.Value;
                }
            }

            var functionData = await _databaseService.GetFunctionDataAsync(databaseName, functionName, isSecure, paramDict);

            ViewBag.DatabaseName = databaseName;
            ViewBag.FunctionName = functionName;

            return View("FunctionResult", functionData);
        }

        public async Task<IActionResult> Home() 
        {
            bool isSecure = true;

            var databases = await _databaseService.GetDatabasesAsync(isSecure);
            ViewBag.databases = databases;

            return View();
        }
    }
}
