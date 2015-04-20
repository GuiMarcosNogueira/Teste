using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prova.Dados
{
    public class ItemPedido
    {
        public Guid Id { get; set; }
        public Guid IdPedido { get; set; }
        public Guid IdTipoItem { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public Pedido Pedido { get; set; }
        public List<TipoItem> TipoItem { get; set; }
    }
}
