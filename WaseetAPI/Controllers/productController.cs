using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WaseetAPI.Application;
using WaseetAPI.Application.HupProcedure;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;

namespace WaseetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class productController : Controller
    {
        private readonly IHubContext<ProceduresHub> _hubContext;
        GlobalProcedures global;
        public productController(IHubContext<ProceduresHub> hubContext)
        {
            global = new GlobalProcedures();
            _hubContext = hubContext;
        }
        [HttpGet("get_product_receive_statement")]
        public async Task<ReturnProductsObjectListResponse> get_product_receive_statement()
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ProductProcedures(connectionStr).GetReceiveProducts(user_id, "052");
        }
        [Route("get_warehouse_products/{id}")]
        [HttpGet("get_warehouse_products")]
        public async Task<ProductsResponse> get_warehouse_products(string id)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ProductProcedures(connectionStr).GetWarehousProducts(user_id, is_own_database, null, id);
        }
        [HttpGet("get_products/{page}")]
        public async Task<ProductsResponse> get_products(string id, int page)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ProductProcedures(connectionStr).get_products(user_id, is_own_database, page, 100);
        }
        //return all products
        [HttpGet("Webget_warehouse_products")]
        public async Task<ProductsResponse> Webget_warehouse_products(string id)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ProductProcedures(connectionStr).GetWarehousProducts(user_id, is_own_database, null, id);
        }

        [HttpGet("Webget_sales_products")]
        public async Task<ProductsResponse> Webget_sales_products(string id)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ProductProcedures(connectionStr).GetSalesProducts(user_id, is_own_database, null, id);
        }
        [Route("get_product_history/{id}")]
        [HttpGet("get_product_history")]
        public async Task<HistoryProductsResponse> Get(string id)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ProductProcedures(connectionStr).GetHistoryProducts(user_id, id);
        }
        [HttpPost("accept_product_receive_statement")]
        public async Task<MessageResponse> accept_product_receive_statement(Invoices invoice)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ProductProcedures(connectionStr).AcceptReceiveProducts(invoice.id ?? 0, 1);
        }

        [HttpPost("cancel_product_receive_statement")]
        public async Task<MessageResponse> cancel_product_receive_statement(Invoices invoice)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ProductProcedures(connectionStr).AcceptReceiveProducts(invoice.id ?? 0, 0);
        }
        [HttpPost("product_return_statement")]
        public async Task<ReturnProductsObjectResponse> product_return_statement(List<InvoicesProducts> listOfProducts)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ProductProcedures(connectionStr).CreateProductsReturn(user_id, listOfProducts);
        }

        [HttpPost("create")]
        public async Task<ProductsObjectResponse> create(Products product)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ProductProcedures(connectionStr).CreateProduct(user_id, product);

        }
        [HttpPost("update")]
        public async Task<ProductsObjectResponse> update(Products product)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ProductProcedures(connectionStr).UpdateProduct(user_id, product);
        }
        [Route("delete/{id}")]
        [HttpPost("delete")]
        public async Task<MessageResponse> delete(string id)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ProductProcedures(connectionStr).DeleteProduct(user_id, id);
        }
    }
}
