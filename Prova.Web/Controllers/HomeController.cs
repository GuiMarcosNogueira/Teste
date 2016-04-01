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
            Pedido pedido = new Pedido();
            ItemPedido itemPedido = new ItemPedido();
            pedido.Clientes = repositorio.RetornaClientes();
            return View(pedido);
        }

        [HttpPost]
        public ActionResult NovoPedido(Pedido pedido)
        {
            // Adiciona o novo pedido preenchido automaticamente com o método do repositório
            repositorio.AdicionaPedido(pedido);
            return RedirectToAction("Index");
        }

        public ActionResult Itens(int id)
        {
            ItensViewModel ivm = new ItensViewModel();

            ivm.Pedido = repositorio.RetornaPedido(id);
            ivm.Items = repositorio.RetornaItems(id);
            return View(ivm);
        }

        public ActionResult NovoItem(int id)
        {
            ItemPedido itemPedido = new ItemPedido();
            
            itemPedido.IdPedido = repositorio.RetornaPedido(id).IdPedido;
            itemPedido.Produtos = repositorio.RetornaProdutos();
            return View(itemPedido);
        }

        [HttpPost]
        public ActionResult NovoItem(int id, ItemPedido itemPedido, int quantidade)
        {
            decimal ValorTotalPedido;
            // Adicionar pedido preenchendo o restante das informações que não estão em tela
            itemPedido.IdPedido = id;
            Produto prod = new Produto();
            prod = repositorio.RetornaProduto(itemPedido.IdProduto);
            itemPedido.QuantidadeItensPedido = quantidade;
            itemPedido.ValorTotalItemPedido = prod.ValorProduto * itemPedido.QuantidadeItensPedido;
            itemPedido.Valor = prod.ValorProduto;

            repositorio.AdicionaItemPedido(itemPedido);
            ValorTotalPedido = repositorio.GetTotalValueByPedido(itemPedido.IdPedido);

            repositorio.UpdatePedidoValue(itemPedido.IdPedido, ValorTotalPedido);

            return RedirectToAction("Itens", new { id = id });
        }
                
        public ActionResult Excluir(string id, int idPedido)
        {
            // Adicionar pedido preenchendo o restante das informações que não estão em tela

            decimal totalValue;

            repositorio.RemoveItemPedido(id);
            totalValue = repositorio.GetTotalValueByPedido(idPedido);
            repositorio.UpdatePedidoValue(idPedido, totalValue);


            return RedirectToAction("Itens", new { id = idPedido });
        }

        public ActionResult EditarItem(int id, int IdPedido)
        {
            ItemPedido itemPedido = new ItemPedido();
            itemPedido = repositorio.RetornaItem(id, IdPedido);
            itemPedido.Produtos = repositorio.RetornaProdutos();
            return View(itemPedido);
        }

        [HttpPost]
        public ActionResult EditarItem(string id, int idPedido, ItemPedido itemPedido, int quantidade)
        {
            decimal totalValue;
            itemPedido.QuantidadeItensPedido = quantidade;
            Produto prod = new Produto();
            prod = repositorio.RetornaProduto(itemPedido.IdProduto);
            itemPedido.ValorTotalItemPedido = prod.ValorProduto * itemPedido.QuantidadeItensPedido;
            itemPedido.Valor = prod.ValorProduto;
            repositorio.EditarItemPedido(itemPedido);
            totalValue = repositorio.GetTotalValueByPedido(itemPedido.IdPedido);
            repositorio.UpdatePedidoValue(itemPedido.IdPedido, totalValue);

            return RedirectToAction("Itens", new { id = idPedido });
        }
    }
}
