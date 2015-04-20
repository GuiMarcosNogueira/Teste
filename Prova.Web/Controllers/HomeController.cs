using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prova.Dados;
using Prova.Web.Models;

namespace Prova.Web.Controllers
{
    public class HomeController : Controller
    {
        private Repositorio repositorio = new Repositorio();

        public ActionResult Index()
        {
            List<Pedido> pedidos = new List<Pedido>();
            pedidos = repositorio.RetornaPedidos();

            return View(pedidos);
        }

        public ActionResult NovoPedido()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NovoPedido(Pedido pedido)
        {
            // Adiciona o novo pedido preenchido automaticamente com o método do repositório
            pedido.Id = Guid.NewGuid();
            repositorio.AdicionaPedido(pedido);
            return RedirectToAction("Index");
        }

        public ActionResult Itens(string id)
        {
            ItensViewModel ivm = new ItensViewModel();

            ivm.Pedido = repositorio.RetornaPedido(id);
            ivm.Items = repositorio.RetornaItems(id);
            ivm.TiposItem = repositorio.RetornaTipoItem();


            return View(ivm);
        }

        public ActionResult NovoItem(string id)
        {
            ItemPedido itemPedido = new ItemPedido();
            List<TipoItem> tiposItem = new List<TipoItem>();
            itemPedido.IdPedido = repositorio.RetornaPedido(id).Id;
            itemPedido.TipoItem = repositorio.RetornaTipoItem();
            return View(itemPedido);
        }

        [HttpPost]
        public ActionResult NovoItem(string id, ItemPedido itemPedido)
        {
            // Adicionar pedido preenchendo o restante das informações que não estão em tela
            itemPedido.Id = Guid.NewGuid();
            itemPedido.IdPedido = new Guid(id);
            itemPedido.IdTipoItem = itemPedido.IdTipoItem;

            repositorio.AdicionaItemPedido(itemPedido);

            return RedirectToAction("Itens", new { id = id });
        }
                
        public ActionResult Excluir(string id, string idPedido)
        {
            // Adicionar pedido preenchendo o restante das informações que não estão em tela

            repositorio.RemoveItemPedido(id);

            return RedirectToAction("Itens", new { id = idPedido });
        }

        public ActionResult EditarItem(string id, string IdPedido)
        {
            ItemPedido itemPedido = new ItemPedido();
            itemPedido = repositorio.RetornaItem(id, IdPedido);
            return View(itemPedido);
        }

        [HttpPost]
        public ActionResult EditarItem(string id, string idPedido, ItemPedido itemPedido)
        {
            // Adicionar pedido preenchendo o restante das informações que não estão em tela
            repositorio.EditarItemPedido(itemPedido);

            return RedirectToAction("Itens", new { id = idPedido });
        }
    }
}
