using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prova.Dados
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public DateTime DataEntregaPedido { get; set; }
        public decimal ValorTotalPedido { get; set; }
        public int IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public int IdProduto { get; set; }

        public IEnumerable<Cliente> Clientes { get; set; }

    }
}
