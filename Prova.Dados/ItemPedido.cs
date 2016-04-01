using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prova.Dados
{
    public class ItemPedido
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public IEnumerable<Produto> Produtos { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorTotalItemPedido { get; set; }
        public string DescricaoProduto { get; set; }
        public int IdProduto { get; set; }
        public int QuantidadeItensPedido { get; set; }
    }
}
