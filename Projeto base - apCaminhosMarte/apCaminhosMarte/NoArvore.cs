using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NoArvore<Dado> where Dado : IComparable<Dado>
{
    private NoArvore<Dado> esq;
    private NoArvore<Dado> dir;
    private Dado info;

    public NoArvore(NoArvore<Dado> esq, NoArvore<Dado> dir)
    {
        Esq = esq;
        Dir = dir;
        Info = info;
    }

    public NoArvore<Dado> Esq { get => esq; set => esq = value; }
    public NoArvore<Dado> Dir { get => dir; set => dir = value; }
    public Dado Info { get => info; set => info = value; }
}
