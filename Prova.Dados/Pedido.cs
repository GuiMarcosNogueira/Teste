using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prova.Dados
{
    public class Pedido
    {
        public Guid Id { get; set; }
        public DateTime Data { get; set; }
        public IEnumerable<ItemPedido> Itens { get; set; }
    }
}
