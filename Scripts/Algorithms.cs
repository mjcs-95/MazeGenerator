using System; //ienumerable
using System.Collections.Generic;
using System.Linq; //enumerable.repeat

namespace Algorithms {
    public static class Prim<T> where T : IComparable<T>
    {
        public static MazeGraph<T> Execute(MazeGraph<T> inputGraph)
        {
            if (inputGraph == null)
                throw new ArgumentNullException(nameof(inputGraph));

            int totalVertices = inputGraph.NumVert;
            bool[] added = new bool[totalVertices];
            MazeGraph<T> resultGraph = new MazeGraph<T>(totalVertices, inputGraph.Rows, inputGraph.Cols);
            PartialOrderedTree<Edge<T>> queue = new PartialOrderedTree<Edge<T>>(totalVertices * 4); // In a general graph (N * ((N - 1) / 2) - N + 2)

            added[0] = true;
            EnqueueAdjacentEdgesNonAlloc(inputGraph, 0, added, queue);

            for (int i = 1; i < totalVertices; i++)
            {
                Edge<T> minimumEdge = new Edge<T>();
                do
                {
                    if (queue.Count == 0)
                        break;

                    minimumEdge = queue.Pop();
                }
                while (added[minimumEdge.Destination]);

                if (added[minimumEdge.Destination])
                    continue;

                resultGraph.AddEdge(minimumEdge.Origin, minimumEdge.Destination, minimumEdge.Cost);
                resultGraph.AddEdge(minimumEdge.Destination, minimumEdge.Origin, minimumEdge.Cost);

                int newNode = minimumEdge.Destination;
                added[newNode] = true;
                EnqueueAdjacentEdgesNonAlloc(inputGraph, newNode, added, queue);
            }
            return resultGraph;
        }
        private static void EnqueueAdjacentEdgesNonAlloc(MazeGraph<T> inputGraph, int vertexIndex, bool[] added, PartialOrderedTree<Edge<T>> queue)
        {
            IReadOnlyList<AdjListGraph<T>.VertexCost> adjacents = inputGraph.GetAdjacents(vertexIndex);

            for (int j = 0; j < adjacents.Count; ++j)
            {
                int targetVertex = adjacents[j].Vertex;
                if (!added[targetVertex])
                {
                    queue.Insert(new Edge<T>(vertexIndex, targetVertex, adjacents[j].Cost));
                }
            }
        }
    }

    static public class Kruskall<T> where T : IComparable<T>
    {
        public static MazeGraph<T> Execute(MazeGraph<T> inputGraph)
        {
            if (inputGraph == null)
                throw new ArgumentNullException(nameof(inputGraph));

            int totalVertices = inputGraph.NumVert;
            MazeGraph<T> resultGraph = new MazeGraph<T>(totalVertices, inputGraph.Rows, inputGraph.Cols);
            Partition P = new Partition(totalVertices);
            PartialOrderedTree<Edge<T>> queue = new PartialOrderedTree<Edge<T>>(totalVertices * 4);// totalVertices * totalVertices

            for (int i = 0; i < totalVertices; ++i)
            {
                IReadOnlyList<AdjListGraph<T>.VertexCost> adjacents = inputGraph.GetAdjacents(i);
                for (int j = 0; j < adjacents.Count; ++j)
                {
                    if (i < adjacents[j].Vertex)
                    {
                        queue.Insert(new Edge<T>(i, adjacents[j].Vertex, adjacents[j].Cost));
                    }
                }
            }

            int edgesAdded = 0;
            int targetEdges = totalVertices - 1;

            while (edgesAdded < targetEdges && queue.Count > 0)
            {
                Edge<T> edge = queue.Pop();
                int leader1 = P.find(edge.Origin);
                int leader2 = P.find(edge.Destination);
                if (leader1 != leader2)
                {
                    P.join(leader1, leader2);
                    resultGraph.AddEdge(edge.Origin, edge.Destination, edge.Cost);
                    resultGraph.AddEdge(edge.Destination, edge.Origin, edge.Cost);
                    ++edgesAdded;
                }
            }
            return resultGraph;
        }
    }

    public static class AldousBroder<T> where T : IComparable<T>
    {
        public static MazeGraph<T> Execute(MazeGraph<T> inputGraph)
        {
            if (inputGraph == null)
                throw new ArgumentNullException(nameof(inputGraph));
            int totalVertices = inputGraph.NumVert;
            if (totalVertices <= 0)
                return new MazeGraph<T>(0, 0, 0);

            MazeGraph<T> resultGraph = new MazeGraph<T>(totalVertices, inputGraph.Rows, inputGraph.Cols);
            bool[] visited = new bool[totalVertices];
            int unvisitedCount = totalVertices;
            Random rand = new Random();
            int currentVertex = rand.Next(0, totalVertices);
            visited[currentVertex] = true;
            unvisitedCount--;
            while (unvisitedCount > 0)
            {
                IReadOnlyList<AdjListGraph<T>.VertexCost> adjacents = inputGraph.GetAdjacents(currentVertex);
                if (adjacents.Count == 0)
                {
                    currentVertex = rand.Next(0, totalVertices);
                    continue;
                }
                var randomEdge = adjacents[rand.Next(0, adjacents.Count)];
                int neighbor = randomEdge.Vertex;
                if (!visited[neighbor])
                {
                    resultGraph.AddEdge(currentVertex, neighbor, randomEdge.Cost);
                    resultGraph.AddEdge(neighbor, currentVertex, randomEdge.Cost);
                    visited[neighbor] = true;
                    unvisitedCount--;
                }
                currentVertex = neighbor;
            }
            return resultGraph;
        }
    }

    static public class BinaryTree<T> where T : IComparable<T>
    {
        public static MazeGraph<T> Execute(MazeGraph<T> inputGraph)
        {
            if (inputGraph == null)
                throw new ArgumentNullException(nameof(inputGraph));

            int totalVertices = inputGraph.NumVert;
            int cols = inputGraph.Cols;

            MazeGraph<T> resultGraph = new MazeGraph<T>(totalVertices, inputGraph.Rows, inputGraph.Cols);
            Random rand = new Random();

            for (int i = 0; i < totalVertices; ++i)
            {
                int r = i / cols;
                int c = i % cols;

                int southNeighbor = inputGraph.GetSouth(r, c);
                int eastNeighbor = inputGraph.GetEast(r, c);

                bool hasSouth = southNeighbor != -1;
                bool hasEast = eastNeighbor != -1;

                int chosenNeighbor = -1;

                if (hasSouth && hasEast)
                    chosenNeighbor = (rand.Next(0, 2) == 0) ? southNeighbor : eastNeighbor;
                else if (hasSouth)
                    chosenNeighbor = southNeighbor;
                else if (hasEast)
                    chosenNeighbor = eastNeighbor;

                if (chosenNeighbor != -1)
                {
                    resultGraph.AddEdge(i, chosenNeighbor, default(T));
                    resultGraph.AddEdge(chosenNeighbor, i, default(T));
                }
            }
            return resultGraph;
        }
    }

    static public class Sidewinder<T> where T : IComparable<T>
    {
        public static MazeGraph<T> Execute(MazeGraph<T> inputGraph)
        {
            if (inputGraph == null)
                throw new ArgumentNullException(nameof(inputGraph));

            int rows = inputGraph.Rows;
            int cols = inputGraph.Cols;
            int totalVertices = inputGraph.NumVert;

            MazeGraph<T> resultGraph = new MazeGraph<T>(totalVertices, rows, cols);
            Random rand = new Random();
            for (int i = 0; i < rows; ++i)
            {
                int groupStartCol = 0;
                for (int j = 0; j < cols; ++j)
                {
                    int currentNode = inputGraph.GetNode(i, j);
                    bool isLastCol = (j == cols - 1);
                    bool isFirstRow = (i == 0);
                    bool shouldCloseGroup = isLastCol || (!isFirstRow && rand.Next(0, 2) == 0);

                    if (shouldCloseGroup)
                    {
                        if (isFirstRow)
                        {
                            if (!isLastCol)
                            {
                                int east = inputGraph.GetEast(i, j);
                                resultGraph.AddEdge(currentNode, east, default(T));
                                resultGraph.AddEdge(east, currentNode, default(T));
                            }
                        }
                        else
                        {
                            int randomCol = rand.Next(groupStartCol, j + 1);

                            int sourceNode = inputGraph.GetNode(i, randomCol);
                            int northNode = inputGraph.GetNorth(i, randomCol);

                            resultGraph.AddEdge(sourceNode, northNode, default(T));
                            resultGraph.AddEdge(northNode, sourceNode, default(T));
                        }
                        groupStartCol = j + 1;
                    }
                    else
                    {
                        int east = inputGraph.GetEast(i, j);
                        resultGraph.AddEdge(currentNode, east, default(T));
                        resultGraph.AddEdge(east, currentNode, default(T));
                    }
                }
            }
            return resultGraph;
        }
    }
    
    static public class Wilson<T> where T : IComparable<T>
    {
        public static MazeGraph<T> Execute(MazeGraph<T> inputGraph)
        {
            if (inputGraph == null)
                throw new ArgumentNullException(nameof(inputGraph));

            var numVert = inputGraph.NumVert;
            var g = new MazeGraph<T>(numVert, inputGraph.Rows, inputGraph.Cols);
            var rand = new Random();
            T defVal = default;

            bool[] inMaze = new bool[numVert];
            int[] nextStep = new int[numVert];

            int[] unvisited = new int[numVert];
            for (int i = 0; i < numVert; ++i)
                unvisited[i] = i;
            int unvisitedCount = numVert;

            int firstIdx = rand.Next(0, unvisitedCount);
            int firstNode = unvisited[firstIdx];
            inMaze[firstNode] = true;

            unvisited[firstIdx] = unvisited[unvisitedCount - 1];
            unvisitedCount--;

            int[] neighbors = new int[4];

            while (unvisitedCount > 0)
            {
                int startNode = unvisited[rand.Next(0, unvisitedCount)];
                int current = startNode;

                while (!inMaze[current])
                {
                    int row = current / inputGraph.Cols;
                    int col = current % inputGraph.Cols;
                    int neighborCount = 0;

                    if (row > 0)
                        neighbors[neighborCount++] = current - inputGraph.Cols;
                    if (row < inputGraph.Rows - 1)
                        neighbors[neighborCount++] = current + inputGraph.Cols;
                    if (col > 0)
                        neighbors[neighborCount++] = current - 1;
                    if (col < inputGraph.Cols - 1)
                        neighbors[neighborCount++] = current + 1;

                    int nextNode = neighbors[rand.Next(0, neighborCount)];

                    nextStep[current] = nextNode;
                    current = nextNode;
                }

                current = startNode;
                while (!inMaze[current])
                {
                    inMaze[current] = true;
                    int next = nextStep[current];

                    g.AddEdge(current, next, defVal);
                    g.AddEdge(next, current, defVal);

                    current = next;
                }

                for (int i = 0; i < unvisitedCount;)
                {
                    int node = unvisited[i];
                    if (inMaze[node])
                    {
                        unvisited[i] = unvisited[unvisitedCount - 1];
                        unvisitedCount--;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            return g;
        }
    }

    static public class RecursiveDivision<T> where T : IComparable<T>
    {
        public static MazeGraph<T> Execute(MazeGraph<T> inputGraph)
        {
            if (inputGraph == null)
                throw new ArgumentNullException(nameof(inputGraph));

            MazeGraph<T> resultGraph = new MazeGraph<T>(inputGraph);
            Random rand = new Random();

            Divide(0, 0, resultGraph.Rows, resultGraph.Cols, resultGraph, rand);
            return resultGraph;
        }

        private static void Divide(int row, int col, int height, int width, MazeGraph<T> graph, Random rand)
        {
            if (height <= 1 || width <= 1)
                return;

            if (height > width)
                DivideHorizontally(row, col, height, width, graph, rand);
            else
                DivideVertically(row, col, height, width, graph, rand);
        }

        private static void DivideHorizontally(int row, int col, int height, int width, MazeGraph<T> graph, Random rand)
        {
            int divideRowOffset = rand.Next(height - 1);
            int passageColOffset = rand.Next(width);
            int targetRow = row + divideRowOffset;

            for (int i = 0; i < width; ++i)
            {
                if (i != passageColOffset)
                {
                    int currentCol = col + i;

                    // Usamos los métodos nativos de tu grafo para obtener los índices reales y seguros
                    int currentNode = graph.GetNode(targetRow, currentCol);
                    int southNode = graph.GetSouth(targetRow, currentCol);

                    // Solo eliminamos la arista si el vecino del sur existe legalmente en el grafo
                    if (currentNode != -1 && southNode != -1)
                    {
                        graph.RemoveEdge(currentNode, southNode);
                        graph.RemoveEdge(southNode, currentNode);
                    }
                }
            }

            int nextHeight = divideRowOffset + 1;
            Divide(row, col, nextHeight, width, graph, rand);
            Divide(row + nextHeight, col, height - nextHeight, width, graph, rand);
        }

        private static void DivideVertically(int row, int col, int height, int width, MazeGraph<T> graph, Random rand)
        {
            int divideColOffset = rand.Next(width - 1);
            int passageRowOffset = rand.Next(height);
            int targetCol = col + divideColOffset;

            for (int i = 0; i < height; ++i)
            {
                if (i != passageRowOffset)
                {
                    int currentRow = row + i;

                    // Usamos los métodos nativos de tu grafo para obtener los índices reales y seguros
                    int currentNode = graph.GetNode(currentRow, targetCol);
                    int eastNode = graph.GetEast(currentRow, targetCol);

                    // Solo eliminamos la arista si el vecino del este existe legalmente en el grafo
                    if (currentNode != -1 && eastNode != -1)
                    {
                        graph.RemoveEdge(currentNode, eastNode);
                        graph.RemoveEdge(eastNode, currentNode);
                    }
                }
            }

            int nextWidth = divideColOffset + 1;
            Divide(row, col, height, nextWidth, graph, rand);
            Divide(row, col + nextWidth, height, width - nextWidth, graph, rand);
        }
    }
    
    static public class HuntAndKill<T> where T : IComparable<T>
    {
        public static MazeGraph<T> Execute(MazeGraph<T> G)
        {
            if (G == null) throw new ArgumentNullException(nameof(G));

            int numVert = G.NumVert;
            int cols = G.Cols;
            int rows = G.Rows;
            MazeGraph<T> g = new MazeGraph<T>(numVert, rows, cols);
            bool[] visited = new bool[numVert];
            Random rand = new Random();
            T defVal = default(T);

            int current = rand.Next(numVert);
            visited[current] = true;

            int[] neighbors = new int[4];

            int firstUnvisited = 0;

            while (current != -1)
            {
                bool deadend = false;
                while (!deadend)
                {
                    int r = current / cols;
                    int c = current % cols;
                    int neighborCount = 0;

                    if (r > 0 && !visited[current - cols])
                        neighbors[neighborCount++] = current - cols;
                    if (r < rows - 1 && !visited[current + cols])
                        neighbors[neighborCount++] = current + cols;
                    if (c > 0 && !visited[current - 1])
                        neighbors[neighborCount++] = current - 1;
                    if (c < cols - 1 && !visited[current + 1])
                        neighbors[neighborCount++] = current + 1;

                    if (neighborCount > 0)
                    {
                        int nextNode = neighbors[rand.Next(neighborCount)];

                        g.AddEdge(current, nextNode, defVal);
                        g.AddEdge(nextNode, current, defVal);

                        current = nextNode;
                        visited[current] = true;
                    }
                    else
                    {
                        deadend = true;
                    }
                }

                current = -1;

                while (firstUnvisited < numVert && visited[firstUnvisited])
                {
                    firstUnvisited++;
                }

                for (int hunt = firstUnvisited; hunt < numVert; ++hunt)
                {
                    if (!visited[hunt])
                    {
                        int r = hunt / cols;
                        int c = hunt % cols;
                        int connectedNeighbor = -1;

                        if (r > 0 && visited[hunt - cols])
                            connectedNeighbor = hunt - cols;
                        else if (r < rows - 1 && visited[hunt + cols])
                            connectedNeighbor = hunt + cols;
                        else if (c > 0 && visited[hunt - 1])
                            connectedNeighbor = hunt - 1;
                        else if (c < cols - 1 && visited[hunt + 1])
                            connectedNeighbor = hunt + 1;

                        if (connectedNeighbor != -1)
                        {
                            g.AddEdge(hunt, connectedNeighbor, defVal);
                            g.AddEdge(connectedNeighbor, hunt, defVal);

                            visited[hunt] = true;
                            current = hunt;
                            break;
                        }
                    }
                }
            }
            return g;
        }
    }

    static public class Ellers<T> where T : IComparable<T>
    {
        public static MazeGraph<T> Execute(MazeGraph<T> G)
        {
            MazeGraph<T> g = new MazeGraph<T>(G.NumVert, G.Rows, G.Cols);
            Random rand = new Random();

            int[] sets = new int[g.Cols];
            for (int j = 0; j < sets.Length; ++j)
                sets[j] = j;

            for (int i = 0; i < g.Rows; ++i)
            {
                for (int j = 0; j < g.Cols - 1; ++j)
                {
                    if ((i == g.Rows - 1 || rand.Next(2) == 1) && !g.HasEdge(g.GetNode(i, j), g.GetNode(i, j + 1)))
                    {
                        sets[j + 1] = sets[j];
                        g.AddEdge(g.GetNode(i, j), g.GetNode(i, j + 1), default(T));
                        g.AddEdge(g.GetNode(i, j + 1), g.GetNode(i, j), default(T));
                    }
                }
                if (i < g.Rows - 1)
                {
                    int[] siguientesconjuntos = Enumerable.Range((i + 1) * g.Cols, g.Cols).ToArray<int>();

                    HashSet<int> todoslosconjuntos = new HashSet<int>(sets);
                    HashSet<int> conjuntosmovidos = new HashSet<int>();
                    while (!todoslosconjuntos.SetEquals(conjuntosmovidos))
                    {
                        for (int j = 0; j < g.Cols; ++j)
                        {
                            if (rand.Next(2) == 1 && !conjuntosmovidos.Contains(sets[j]))
                            {
                                conjuntosmovidos.Add(sets[j]);
                                siguientesconjuntos[j] = sets[j];
                                g.AddEdge(g.GetNode(i, j), g.GetNode(i + 1, j), default(T));
                                g.AddEdge(g.GetNode(i + 1, j), g.GetNode(i, j), default(T));
                            }
                        }
                    }
                    sets = siguientesconjuntos;
                }
            }
            return g;
        }
    }

    public class Partition
    {
        private readonly int[] _parent;

        public Partition(int size)
        {
            _parent = new int[size];
            Array.Fill(_parent, -1); // Inicialización con Array.Fill para mayor eficiencia
        }

        public void join(int root1, int root2)
        {
            if (_parent[root2] < _parent[root1])
            {
                _parent[root1] = root2;
            }
            else
            {
                if (_parent[root1] == _parent[root2])
                {
                    --_parent[root1];
                }
                _parent[root2] = root1;
            }
        }

        public int find(int vertex)
        {
            int leader = vertex;
            while (_parent[leader] > -1)
            {
                leader = _parent[leader];
            }

            while (_parent[vertex] > -1)
            {
                int next = _parent[vertex];
                _parent[vertex] = leader;
                vertex = next;
            }
            return leader;
        }
    }

    static public class Shuffle<T>
    {
        private static readonly Random _rand = new Random();
        static public List<T> FisherYates(List<T> originalList)
        {
            if (originalList == null)
                throw new ArgumentNullException(nameof(originalList));

            List<T> newList = new List<T>(originalList);
            for (int i = originalList.Count - 1; i >= 0; --i)
            {
                int j = _rand.Next(i + 1);
                T temp = newList[j];
                newList[j] = newList[i];
                newList[i] = temp;
            }
            return newList;
        }
    }

}