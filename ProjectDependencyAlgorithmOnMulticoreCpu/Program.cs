using System;
using System.Collections.Generic;

namespace ProjectDependencyAlgorithmOnMulticoreCpu
{
    class Program
    {
        public class Project
        {
            public int NoOfDependencies;
            public int TimeToCompile;
            public int Id;
            public bool IsCompiled;
            public List<Project> DependentProjects;

            public Project(int id)
            {
                Id = id;
                NoOfDependencies = 0;
                IsCompiled = false;
                DependentProjects = new List<Project>();
            }
        }

        public class Graph
        {
            public Project[] AdjMat;

            public Graph(int n)
            {
                AdjMat = new Project[n];
                for(int i = 0; i < n; i++)
                {
                    AdjMat[i] = new Project(i);
                }
            }

            public void AddDirectedRelation(int dependency , int dependent)
            {
                AdjMat[dependency].DependentProjects.Add(AdjMat[dependent]);
                AdjMat[dependent].NoOfDependencies++;
            }

            public void AddVisitedNode(int project)
            {
                AdjMat[project].IsCompiled = true;
            }

            public void AddTimeToCompile(int projectNum, int timeToCompile)
            {
                AdjMat[projectNum].TimeToCompile = timeToCompile;
            }
        }



        static void Main(string[] args)
        {
            Graph graph = new Graph(5);
            graph.AddDirectedRelation(2, 0);
            graph.AddDirectedRelation(3, 2);
            graph.AddDirectedRelation(4, 2);
            graph.AddDirectedRelation(4, 3);

            graph.AddTimeToCompile(0, 1);
            graph.AddTimeToCompile(1, 3);
            graph.AddTimeToCompile(2, 4);
            graph.AddTimeToCompile(3, 2);
            graph.AddTimeToCompile(4, 1);


            int numberOfProcessor = 5;

            PerformTopologicalSorting(graph, numberOfProcessor);
        }

        private static void PerformTopologicalSorting(Graph graph, int numberOfProcessor)
        {
            int t = 1;
            int availableProcessors = numberOfProcessor;
            Queue<Project> queue = new Queue<Project>();
            // Add logic to detect cyclic graph
            bool isAcyclic = true;
            if (isAcyclic)
            {
                AddNodesWithIndegreesZero(graph, queue);
            }
            else
            {
                Console.WriteLine("Cyclic graph. Dont waste my time");
            }

            int projectsToCompileAtSameTime = queue.Count;

            while(queue.Count != 0)
            {
                if (projectsToCompileAtSameTime == 0 || availableProcessors == 0)
                {
                    t++;
                    availableProcessors = numberOfProcessor;
                    projectsToCompileAtSameTime = queue.Count;
                }

                Project node = queue.Dequeue();
                node.TimeToCompile--;
                availableProcessors--;
                projectsToCompileAtSameTime--;
                if (node.TimeToCompile == 0)
                {
                    node.IsCompiled = true;
                    foreach(var adjNode in node.DependentProjects)
                    {
                        adjNode.NoOfDependencies--;
                        if (adjNode.NoOfDependencies == 0) queue.Enqueue(adjNode);
                    }

                    Console.WriteLine("Compiled Project: " + node.Id + "at time: " + t);
                }
                else
                {
                    queue.Enqueue(node);
                }
            }
        }

        private static void AddNodesWithIndegreesZero(Graph graph, Queue<Project> queue)
        {
            foreach(var node in graph.AdjMat)
            {
                if (node.NoOfDependencies == 0) queue.Enqueue(node);
            }
        }
    }
}
