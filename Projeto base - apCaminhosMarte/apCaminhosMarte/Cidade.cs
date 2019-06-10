using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apCaminhosMarte
{
    class Cidade : IComparable<Cidade>
    {
        int idCidade;
        const int tamanhoIdCidade = 3;
        string nomeCidade;
        const int tamanhoNomeCidade = 15;
        int coordenadaX;
        const int tamanhoCoordenadaX = 5;
        int coordenadaY;
        const int tamanhoCoordenadaY = 5;

        public int CompareTo(Cidade other)
        {
            throw new NotImplementedException();
        }
    }
}
