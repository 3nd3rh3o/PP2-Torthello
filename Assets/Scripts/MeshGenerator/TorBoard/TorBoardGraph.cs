using System.Collections.Generic;
using UnityEngine;

namespace Tortello{
public class TorBoardGraph : IGraph
{
    private Graph graph;
    private TorBoardSettings settings;

    private int previousSize;
    public TorBoardGraph(TorBoardSettings settings)
    {
        this.settings = settings;
    }

    //initialisation du Graph
    public void InitGraph()
    {
        graph = new Graph
        {
            sommets = new Sommets[settings.BoardSize * settings.BoardSize]
        };

        for (int v = 0; v < settings.BoardSize; v++)
        {
            for (int u = 0; u < settings.BoardSize; u++)
            {
                graph.sommets[v * settings.BoardSize + u] = new Sommets
                {
                    arretes = new Arretes[4],
                    couleur = Couleur.Vide
                };

                // Define edges for toroidal connectivity
                graph.sommets[v * settings.BoardSize + u].arretes[0] = new Arretes { d = v * settings.BoardSize + u, a = v * settings.BoardSize + (u + 1) % settings.BoardSize };
                graph.sommets[v * settings.BoardSize + u].arretes[1] = new Arretes { d = v * settings.BoardSize + u, a = v * settings.BoardSize + (u - 1 + settings.BoardSize) % settings.BoardSize };
                graph.sommets[v * settings.BoardSize + u].arretes[2] = new Arretes { d = v * settings.BoardSize + u, a = ((v + 1) % settings.BoardSize) * settings.BoardSize + u };
                graph.sommets[v * settings.BoardSize + u].arretes[3] = new Arretes { d = v * settings.BoardSize + u, a = ((v - 1 + settings.BoardSize) % settings.BoardSize) * settings.BoardSize + u };
            }
        }
    }
    
    //ajout d'un pion et retournement des pions adverses
    public bool AddPawn(int idSommets, Couleur couleur, List<List<int>> pionsARetournes)
    {
        // si un coup est valide, on ajoute un pion et on retourne les pions adverses
        if(CoupEstValide(idSommets, couleur,pionsARetournes)){
                graph.sommets[idSommets].couleur = couleur;
                pionsARetournes.ForEach(l => l.ForEach(p => graph.sommets[p].couleur = graph.sommets[p].couleur == Couleur.Noir ? Couleur.Blanc : Couleur.Noir));
                return true;
            }
            return false;
        return true;
    }

    public void SetPawn(int idSommets, Couleur couleur)
    {
        graph.sommets[idSommets].couleur = couleur;
    }

    public void DestroyGraph()
    {
        graph = null;
    }

    public void RemoveAllPawns()
    {
        foreach (var sommet in graph.sommets)
        {
            sommet.couleur = Couleur.Vide;
        }
    }

    public void UpdateGraph()
    {
        // Implementation for updating the graph
    }

    public bool CoupEstValide(int idSommets, Couleur couleur, List<List<int>> pionsARetournes)
    {
        // Implementation for checking if a move is valid
        return true;
    }
    public void StartGame()
    {
        int u = Mathf.FloorToInt(settings.BoardSize / 2f) - 1;
        int v = Mathf.FloorToInt(settings.BoardSize / 2f) - 1;
        SetPawn(u + v * settings.BoardSize, Couleur.Noir);
        SetPawn(u + 1 + v * settings.BoardSize, Couleur.Blanc);
        SetPawn(u + (v + 1) * settings.BoardSize, Couleur.Blanc);
        SetPawn(u + 1 + (v + 1) * settings.BoardSize, Couleur.Noir);
    }

    public List<int> GetScore()
    {
        // Implementation for getting the score
        return new List<int>();
    }

    public bool NoPlacementAvailable(Couleur couleur)
    {
        // Implementation for checking if a player can place a pawn
        return false;
}
}
}

