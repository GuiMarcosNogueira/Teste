using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prova.Dados;

namespace Prova.Web.Models
{
    public class ItensViewModel
    {
        public Pedido Pedido { get; set; }
        public List<ItemPedido> Items { get; set; }
        public List<TipoItem> TiposItem { get; set; }
    }
}